using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using RadiantConnect.Network;
using RadiantConnect.Services;

namespace RadiantConnect.Socket
{
    public class Socket(UserAuth auth)
    {
        public delegate void SocketFired(string value);

        public event SocketFired? OnNewMessage;

        internal CancellationTokenSource ShutdownSocket = new();
        internal readonly UserAuth Authentication = auth;

        internal async Task ReceiveMessageAsync(ClientWebSocket webSocket)
        {
            const int bufferSize = 1024;
            byte[] buffer = new byte[bufferSize];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Text) {
                OnNewMessage?.Invoke(Encoding.UTF8.GetString(buffer, 0, result.Count));
            }
        }

        public void ShutdownConnection()
        {
            ShutdownSocket.Cancel();
        }

        public void InitializeConnection()
        {
            Task.Run(async () =>
            {
                Uri uri = new($"wss://127.0.0.1:{Authentication.AuthorizationPort}"); // Replace with your WebSocket server address

                using ClientWebSocket clientWebSocket = new();
                clientWebSocket.Options.SetRequestHeader("Authorization", $"Bearer {Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{Authentication.OAuth}"))}");
                await clientWebSocket.ConnectAsync(uri, CancellationToken.None);
                
                while (!ShutdownSocket.IsCancellationRequested)
                {
                    await ReceiveMessageAsync(clientWebSocket);
                }

                await clientWebSocket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                await clientWebSocket.CloseAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            });
        }
    }
}
