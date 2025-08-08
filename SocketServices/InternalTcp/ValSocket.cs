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
#if DEBUG
        public TcpEvents Events;
#endif
        public ValSocket(Initiator init)
        {
            Authentication = ValorantNet.GetAuth();

            if (Authentication is null)
                throw new RadiantConnectException("Failed to grab current auth, is valorant running?");

            Init = init;
#if DEBUG
            Events = new TcpEvents(this);
#endif
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
                {
                    messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                }

            } while (!result.EndOfMessage);

            if (messageBuilder.Length > 0)
            {
                OnNewMessage?.Invoke(messageBuilder.ToString());
            }
        }

        public void InitializeConnection() =>
	        Task.Run(async () =>
	        {
		        try
		        {
			        while (!await InternalValorantMethods.IsReady(Init.Endpoints.LocalEndpoints))
			        {
				        Debug.WriteLine("Waiting for ClientReady...");
				        await Task.Delay(500);
			        }

			        Uri uri = new($"wss://riot:{Authentication?.OAuth}@127.0.0.1:{Authentication?.AuthorizationPort}");

			        ClientWebSocket clientWebSocket = new();
			        clientWebSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
			        clientWebSocket.Options.SetRequestHeader("Authorization", $"Basic {$"riot:{Authentication?.OAuth}".ToBase64()}");
			        await clientWebSocket.ConnectAsync(uri, CancellationToken.None);

			        foreach (string eventName in await GetEvents()) 
				        await clientWebSocket.SendAsync(new ArraySegment<byte>([.. Encoding.UTF8.GetBytes($"[5, \"{eventName}\"]")]), WebSocketMessageType.Text, true, CancellationToken.None);

			        while (!ShutdownSocket.IsCancellationRequested)
			        {
				        await ReceiveMessageAsync(clientWebSocket);
			        }

			        await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
			        await clientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);

			        clientWebSocket.Dispose();
		        }
		        catch (Exception ex) { Debug.WriteLine(ex.ToString()); }
	        });
    }
}
