﻿using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using RadiantConnect.Methods;
using RadiantConnect.Network;

namespace RadiantConnect.ValSocket
{
    public class ValSocket(UserAuth authentication, Initiator init)
    {
        public void ShutdownConnection() => ShutdownSocket.Cancel();
        public delegate void SocketFired(string value);

        public event SocketFired? OnNewMessage;

        internal CancellationTokenSource ShutdownSocket = new();

        internal async Task<IReadOnlyList<string>> GetEvents()
        {
            JsonElement? response = await init.Endpoints.LocalEndpoints.GetHelpAsync();
            if (response is null) return Array.Empty<string>();

            JsonDocument jsonDocument = JsonDocument.Parse(response.ToString()!);
            JsonElement root = jsonDocument.RootElement;

            if (root.TryGetProperty("events", out JsonElement eventsElement) && eventsElement.ValueKind == JsonValueKind.Object)
            {
                return eventsElement.EnumerateObject().Select(eventProperty => eventProperty.Name).ToList();
            }

            return Array.Empty<string>();
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
        
        public void InitializeConnection()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (!await InternalValorantMethods.IsReady(init.Endpoints.LocalEndpoints))
                    {
                        Debug.WriteLine("Waiting for ClientReady...");
                        await Task.Delay(500);
                    }

                    Uri uri = new($"wss://riot:{authentication.OAuth}@127.0.0.1:{authentication.AuthorizationPort}");

                    ClientWebSocket clientWebSocket = new();
                    clientWebSocket.Options.RemoteCertificateValidationCallback = (_, _, _, _) => true;
                    clientWebSocket.Options.SetRequestHeader("Authorization", $"Basic {$"riot:{authentication.OAuth}".ToBase64()}");
                    await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                    
                    foreach (string eventName in await GetEvents())
                    {
                        await clientWebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"[5, \"{eventName}\"]").ToArray()), WebSocketMessageType.Text, true, CancellationToken.None);
                    }

                    while (!ShutdownSocket.IsCancellationRequested)
                    {
                        await ReceiveMessageAsync(clientWebSocket);
                    }

                    await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                    await clientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);

                    clientWebSocket.Dispose();
                }
                catch(Exception ex) {Debug.WriteLine(ex.ToString());}
            });
        }
    }
}
