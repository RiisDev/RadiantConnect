#if DEBUG
namespace RadiantConnect.SocketServices.InternalTcp
{
    public class TcpEvents
    {
        public TcpEvents(ValSocket socket) => socket.OnNewMessage += TcpMessage;

        private void TcpMessage(string value)
        {
            Debug.WriteLine(value);
        }
    }

}
#endif