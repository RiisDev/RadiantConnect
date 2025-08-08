using RadiantConnect.Authentication.DriverRiotAuth.Records;
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
    public class RmsClient(RSOAuth authData, RmsRegion region)
#else
    internal class RmsClient(RSOAuth authData, RmsRegion region)
#endif
    {
#if DEBUG
        public async Task StartClient()
#else
        internal async Task StartClient()
#endif
        {
            string rmsData = $"{{{Match(authData.ClientConfig?.ToString()!, "(\"rms\\.affinities\"\\s*:\\s*{[^}]*})").Value}}}";
            RmsData? rmsAffinitiesData = JsonSerializer.Deserialize<RmsData>(rmsData);

            if (rmsAffinitiesData == null) throw new RadiantConnectXMPPException("Failed to find affinity url");
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

            if (clientId != "riot-client")
            {
                throw new RadiantConnectXMPPException("Access Token, must originate from riot-client, or QR");
            }

            using var clientWebSocket = new ClientWebSocket();
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
                                   "id": "{{Guid.NewGuid().ToString()}}",
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
            var bytes = Encoding.UTF8.GetBytes(message);
            var segment = new ArraySegment<byte>(bytes);
            await ws.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("Sent: " + message);
        }

        private async Task ReadMessages(ClientWebSocket ws)
        {
            var buffer = new byte[8192];
            while (ws.State == WebSocketState.Open)
            {
                var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine("WebSocket closed.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                Console.WriteLine("Received: " + message);
            }
        }
    }
}
