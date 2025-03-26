using RadiantConnect.Network.ChatEndpoints.DataTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using static RadiantConnect.SocketServices.XMPP.RemoteXMPP;
using static RadiantConnect.SocketServices.XMPP.XMPPController;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Text.RegularExpressions.Regex;
using System.Net.WebSockets;
using System.Security.Cryptography;

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

    internal enum RmsRegion
    {
        Asia,
        Eu,
        Sea,
        Us
    }

    //internal class RmsClient(RSOAuth authData, RmsRegion region)
    //{
    //    internal async Task AsyncSocketWrite(SslStream sslStream, string message)
    //    {
    //        byte[] buffer = Encoding.UTF8.GetBytes(message);
    //        await sslStream.WriteAsync(buffer, 0, buffer.Length);
    //        await sslStream.FlushAsync();
    //    }

    //    // Super hacky way of continuous reading from the stream,
    //    // but it works and I apologize for the hackyness.
    //    internal async Task<string> AsyncSocketRead(SslStream sslStream)
    //    {
    //        int byteCount;
    //        byte[] bytes = new byte[1024];
    //        StringBuilder contentBuilder = new();

    //        do
    //        {
    //            try { byteCount = await sslStream.ReadAsync(bytes, 0, bytes.Length); }
    //            catch (IOException) { break; } // Timeout Occurred, aka no data left to read

    //            if (byteCount > 0) contentBuilder.Append(Encoding.UTF8.GetString(bytes, 0, byteCount));

    //        } while (byteCount > 0);

    //        //Debug.WriteLine(contentBuilder.ToString());

    //        return contentBuilder.ToString();
    //    }

    //    internal static string GenerateNonce(int length = 16)
    //    {
    //        byte[] nonceBytes = new byte[length];
    //        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
    //        {
    //            rng.GetBytes(nonceBytes);
    //        }
    //        return Convert.ToBase64String(nonceBytes);
    //    }

    //    internal async Task StartClient()
    //    {
    //        string rmsData = $"{{{Match(authData?.ClientConfig?.ToString()!, "(\"rms\\.affinities\"\\s*:\\s*{[^}]*})").Value}}}";
    //        RmsData? rmsAffinitiesData = JsonSerializer.Deserialize<RmsData>(rmsData);

    //        if (rmsAffinitiesData == null) throw new RadiantConnectXMPPException("Failed to find affinity url");
    //        if (string.IsNullOrEmpty(authData?.RmsToken)) throw new RadiantConnectXMPPException("Failed to get RMSToken");

    //        string affinityUrl = region switch
    //        {
    //            RmsRegion.Asia => rmsAffinitiesData.RmsAffinities.Asia,
    //            RmsRegion.Eu => rmsAffinitiesData.RmsAffinities.Eu,
    //            RmsRegion.Sea => rmsAffinitiesData.RmsAffinities.Sea,
    //            RmsRegion.Us => rmsAffinitiesData.RmsAffinities.Us,
    //            _ => rmsAffinitiesData.RmsAffinities.Us // Just in case somehow they send null
    //        };
            
    //        using var clientWebSocket = new ClientWebSocket();
    //        // Set custom headers
    //        clientWebSocket.Options.SetRequestHeader("Upgrade", "websocket");
    //        clientWebSocket.Options.SetRequestHeader("Connection", "Upgrade");
    //        clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Key", GenerateNonce());
    //        clientWebSocket.Options.SetRequestHeader("Sec-WebSocket-Version", "13");
    //        clientWebSocket.Options.SetRequestHeader("User-Agent", "RiotGamesApi/24.10.4.4733 riot-messaging-service (Windows;10;;Professional, x64) valorant/10.04.00.3283852");
    //        clientWebSocket.Options.SetRequestHeader("X-Riot-Affinity", authData.RmsToken);

    //        Uri serverUri = new Uri(affinityUrl);
    //        await clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

    //        Console.WriteLine("Connected to the WebSocket server");

    //        // Step 2: Send a custom message after connection (like the `yeet` message you provided)
    //        string message = $$"""
    //                           {
    //                               "id": "{{Guid.NewGuid().ToString()}}",
    //                               "payload": {
    //                                   "enable": "true"
    //                               },
    //                               "subject": "rms:gzip",
    //                               "type": "request"
    //                           }
    //                           """;

    //        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
    //        await clientWebSocket.SendAsync(new ArraySegment<byte>(messageBytes), WebSocketMessageType.Text, true, CancellationToken.None);
    //        Console.WriteLine("Sent message: " + message);

    //        // Step 3: Read the WebSocket handshake response
    //        byte[] buffer = new byte[1024];
    //        WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
    //        string response = Encoding.ASCII.GetString(buffer, 0, result.Count);
    //        Console.WriteLine("Received WebSocket Handshake Response:");
    //        Console.WriteLine(response);

    //        // Step 4: Optionally, continue with WebSocket communication (send/receive more messages)
    //        // e.g., await SendMessagesAsync(clientWebSocket);
    //    }
    //}
}
