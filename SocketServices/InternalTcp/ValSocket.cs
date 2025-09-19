using System.Net.WebSockets;
using RadiantConnect.Network;

namespace RadiantConnect.SocketServices.InternalTcp
{
	public class ValSocket
	{
		public void ShutdownConnection() => ShutdownSocket.Cancel();
		public delegate void SocketFired(string value);

		public event SocketFired? OnNewMessage;

		internal CancellationTokenSource ShutdownSocket = new();
		internal ValorantNet.UserAuth? Authentication;
		internal Initiator Init;

		public ValSocket(Initiator init, bool create = true)
		{
			Authentication = ValorantNet.GetAuth();

			if (Authentication is null)
				throw new RadiantConnectException("Failed to grab current auth, is valorant running?");

			Init = init;

			// Disabled since it's either set in the initiator, or here, and is a hidden null.
			// ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
			if (create)
				init.TcpEvents ??= new TcpEvents(init, this);
		}

		internal async Task<IReadOnlyList<string>> GetEvents()
		{
			JsonElement? response = await Init.Endpoints.LocalEndpoints.GetHelpAsync();
			if (response is null) return [];

			JsonDocument jsonDocument = JsonDocument.Parse(response.ToString()!);
			JsonElement root = jsonDocument.RootElement;

			return root.TryGetProperty("events", out JsonElement eventsElement) &&
				   eventsElement.ValueKind == JsonValueKind.Object
				? eventsElement.EnumerateObject().Select(eventProperty => eventProperty.Name).ToList()
				: [];
		}

		internal async Task ReceiveMessageAsync(ClientWebSocket webSocket)
		{
			const int bufferSize = 1024;
			byte[] buffer = new byte[bufferSize];
			WebSocketReceiveResult result;
			StringBuilder messageBuilder = new();

			do
			{
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

				if (result.MessageType == WebSocketMessageType.Text)
					messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

			} while (!result.EndOfMessage);

			if (messageBuilder.Length > 0) 
				OnNewMessage?.Invoke(messageBuilder.ToString());
		}

		public void InitializeConnection() =>
			Task.Run(async () =>
			{
				try
				{
					IReadOnlyList<string> events = await GetEvents();

					while (events.Count == 0)
					{
						Debug.WriteLine("Waiting for ClientReady...");
						await Task.Delay(500);
					}

					Uri uri = new($"wss://riot:{Authentication?.OAuth}@127.0.0.1:{Authentication?.AuthorizationPort}");

					ClientWebSocket clientWebSocket = new();
					clientWebSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
					clientWebSocket.Options.SetRequestHeader("Authorization", $"Basic {$"riot:{Authentication?.OAuth}".ToBase64()}");
					await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

					foreach (string eventName in events) 
						await clientWebSocket.SendAsync(new ArraySegment<byte>([.. Encoding.UTF8.GetBytes($"[5, \"{eventName}\"]")]), WebSocketMessageType.Text, true, CancellationToken.None);

					while (!ShutdownSocket.IsCancellationRequested) 
						await ReceiveMessageAsync(clientWebSocket);

					await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
					await clientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);

					clientWebSocket.Dispose();
				}
				catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
			});
	}
}
