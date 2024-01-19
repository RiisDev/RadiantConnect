﻿using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RadiantConnect.XMPP
{
    public class SocketHandle(Stream incomingStream, Stream outgoingStream)
    {
        internal bool DoBreak;
        internal delegate void InternalMessage(string data);

        internal event InternalMessage? OnClientMessage;
        internal event InternalMessage? OnServerMessage;

        internal void Initiate()
        {
            Task.Run(IncomingHandler);
            Task.Run(OutgoingHandler);
        }

        internal async Task IncomingHandler()
        {
            try
            {
                int byteCount;
                byte[] bytes = new byte[8192];
                do
                {
                    byteCount = await incomingStream.ReadAsync(bytes);
                    string content = Encoding.UTF8.GetString(bytes, 0, byteCount);
                    await outgoingStream.WriteAsync(bytes.AsMemory(0, byteCount));
                    OnClientMessage?.Invoke(content);
                } while (byteCount != 0 && !DoBreak);
            }
            catch (IOException)
            {
                DoBreak = true;
            }
        }

        internal async Task OutgoingHandler()
        {
            try
            {
                int byteCount;
                byte[] bytes = new byte[8192];
                do
                {
                    byteCount = await outgoingStream.ReadAsync(bytes);
                    string content = Encoding.UTF8.GetString(bytes, 0, byteCount);
                    await incomingStream.WriteAsync(bytes.AsMemory(0, byteCount));
                    OnServerMessage?.Invoke(content);
                } while (byteCount != 0 && !DoBreak);
            }
            catch (IOException)
            {
                DoBreak = true;
            }
        }

        public async Task SendXmlMessageAsync([StringSyntax(StringSyntaxAttribute.Xml)] string data)
        {
            try
            {
                while (!incomingStream.CanWrite) { await Task.Delay(50); }
                byte[] bytes = Encoding.UTF8.GetBytes(data);
                await incomingStream.WriteAsync(bytes.AsMemory(0, bytes.Length));
            }
            catch (Exception ex) { Debug.WriteLine(ex); }
        }
    }
}
