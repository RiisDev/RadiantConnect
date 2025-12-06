using System.Net.WebSockets;
using RadiantConnect.Network;
#pragma warning disable CA5359 

namespace RadiantConnect.SocketServices.InternalTcp
{
	/// <summary>
	/// Handles WebSocket communication with the local Riot Client for Valorant.
	/// Listens for game events, presence updates, currency changes, and other local socket messages.
	/// </summary>
	public class ValSocket : IDisposable
	{
		/// <summary>
		/// Cancels the WebSocket connection.
		/// </summary>
		public void ShutdownConnection() => ShutdownSocket.Cancel();
		
		/// <summary>
		/// Delegate fired when a new message is received from the WebSocket.
		/// </summary>
		public delegate void SocketFired(string value);

		/// <summary>
		/// Event fired when a new incoming message is detected from the socket.
		/// </summary>
		public event SocketFired? OnNewMessage;

		internal CancellationTokenSource ShutdownSocket = new();
		internal ValorantNet.UserAuth? Authentication;
		internal Initiator Init;

		/// <summary>
		/// Initializes a new instance of the ValSocket class and optionally attaches TcpEvents.
		/// </summary>
		/// <param name="init">The initiator instance providing client context.</param>
		/// <param name="create">Whether to create TcpEvents if not already attached.</param>
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
			JsonElement? response = await Init.Endpoints.LocalEndpoints.GetHelpAsync().ConfigureAwait(false);
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
				result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), ShutdownSocket.Token).ConfigureAwait(false);

				if (result.MessageType == WebSocketMessageType.Text)
					messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

			} while (!result.EndOfMessage);

			if (messageBuilder.Length > 0) 
				OnNewMessage?.Invoke(messageBuilder.ToString());
		}

		/// <summary>
		/// Initializes a WebSocket connection to the local Riot Client and subscribes to all available event streams.
		/// </summary>
		public void InitializeConnection() =>
			Task.Run(async () =>
			{
				try
				{
					IReadOnlyList<string> events = await GetEvents().ConfigureAwait(false);

					while (events.Count == 0)
					{
						Debug.WriteLine("Waiting for ClientReady...");
						await Task.Delay(500).ConfigureAwait(false);
					}

					Uri uri = new($"wss://riot:{Authentication?.OAuth}@127.0.0.1:{Authentication?.AuthorizationPort}");

					ClientWebSocket clientWebSocket = new();
					clientWebSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
					clientWebSocket.Options.SetRequestHeader("Authorization", $"Basic {$"riot:{Authentication?.OAuth}".ToBase64()}");
					await clientWebSocket.ConnectAsync(uri, CancellationToken.None).ConfigureAwait(false);

					foreach (string eventName in events)
						await clientWebSocket.SendAsync(new ArraySegment<byte>([.. Encoding.UTF8.GetBytes($"[5, \"{eventName}\"]")]), WebSocketMessageType.Text, true, ShutdownSocket.Token).ConfigureAwait(false);

					while (!ShutdownSocket.IsCancellationRequested) 
						await ReceiveMessageAsync(clientWebSocket).ConfigureAwait(false);

					await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).ConfigureAwait(false);
					await clientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None).ConfigureAwait(false);

					clientWebSocket.Dispose();
				}
				catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
			});

		/// <summary>
		/// Releases all resources used by the <see cref="ValSocket"/> instance.
		/// </summary>
		public void Dispose()
		{
			if (!ShutdownSocket.IsCancellationRequested)
				ShutdownSocket.Cancel();
			ShutdownSocket.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}
