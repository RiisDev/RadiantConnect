using System.Text;
using System.Xml;

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

        public async Task SendXmlMessageAsync(XmlDocument xml)
        {
            byte[] xmlBytes = Encoding.UTF8.GetBytes(xml.OuterXml);
            await incomingStream.WriteAsync(xmlBytes);
        }
    }
}
