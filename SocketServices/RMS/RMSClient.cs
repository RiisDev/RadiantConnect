using RadiantConnect;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.SocketServices.RMS;
using System.Net.WebSockets;

namespace RadiantConnect.SocketServices.RMS
{
    internal record RmsData(
        [property: JsonPropertyName("rms.affinities")]
        RmsAffinities RmsAffinities
    );

    internal record RmsAffinities(
        [property: JsonPropertyName("asia")]
        string Asia,
        [property: JsonPropertyName("eu")]
        string Eu,
        [property: JsonPropertyName("sea")]
        string Sea,
        [property: JsonPropertyName("us")]
        string Us
    );


#if DEBUG
	public enum RmsRegion
#else
    internal enum RmsRegion
#endif
    {
        Asia,
        Eu,
        Sea,
        Us
    }

#if DEBUG
    public partial class RmsClient(RSOAuth authData, RmsRegion region)
#else
    internal partial class RmsClient(RSOAuth authData, RmsRegion region)
#endif
	{

		[GeneratedRegex("(\"rms\\.affinities\"\\s*:\\s*{[^}]*})")]
	    private static partial Regex RmsData();

#if DEBUG
		public async Task StartClient()
#else
        internal async Task StartClient()
#endif
        {
            string rmsData = $"{{{RmsData().Match(authData.ClientConfig?.ToString() ?? "").Value}}}";
            RmsData rmsAffinitiesData = JsonSerializer.Deserialize<RmsData>(rmsData) ?? throw new RadiantConnectXMPPException("Failed to find affinity url");
			if (string.IsNullOrEmpty(authData.RmsToken)) throw new RadiantConnectXMPPException("Failed to get RMSToken");

            string affinityUrl = region switch
            {
                RmsRegion.Asia => rmsAffinitiesData.RmsAffinities.Asia,
                RmsRegion.Eu => rmsAffinitiesData.RmsAffinities.Eu,
                RmsRegion.Sea => rmsAffinitiesData.RmsAffinities.Sea,
                RmsRegion.Us => rmsAffinitiesData.RmsAffinities.Us,
                _ => rmsAffinitiesData.RmsAffinities.Us // Just in case somehow they send null
            };

            JsonWebToken jwt = new (authData.AccessToken);
            string clientId = jwt.GetClaim("cid");

            if (clientId != "riot-client") throw new RadiantConnectXMPPException("Access Token, must originate from riot-client, or QR");

            using ClientWebSocket clientWebSocket = new();
            clientWebSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
            clientWebSocket.Options.SetRequestHeader("Upgrade", "websocket");
            clientWebSocket.Options.SetRequestHeader("Connection", "Upgrade");
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Key", AuthUtil.GenerateNonce(22));
            clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Version", "13");
            clientWebSocket.Options.SetRequestHeader("User-Agent", "RiotGamesApi/25.2.1.5093 riot-messaging-service (Windows;10;;Professional, x64) riot-client/0");
            clientWebSocket.Options.SetRequestHeader("X-Riot-Affinity", authData.RmsToken);
            await clientWebSocket.ConnectAsync(new Uri(affinityUrl), CancellationToken.None);
            
            string message = $$"""
                               {
                                   "id": "{{Guid.NewGuid()}}",
                                   "payload": {
                                       "enable": "true"
                                   },
                                   "subject": "rms:gzip",
                                   "type": "request"
                               }
                               """;

            await SendMessage(clientWebSocket, message);

            Task readTask = ReadMessages(clientWebSocket);

            await readTask; 
        }

        private async Task SendMessage(ClientWebSocket ws, string message)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            ArraySegment<byte> segment = new(bytes);
            await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Sent: " + message);
        }

        private async Task ReadMessages(ClientWebSocket ws)
        {
            byte[] buffer = new byte[8192];
            while (ws.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket closed.");
                    break;
                }

                string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received: " + message);
            }
        }
    }
}
