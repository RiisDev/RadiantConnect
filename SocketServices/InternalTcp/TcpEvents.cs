namespace RadiantConnect.SocketServices.InternalTcp
{
    public class TcpEvents
    {
		public delegate void QueueEvent(string value);

		public TcpEvents(ValSocket socket, bool initiateSocket = false)
        {
	        socket.OnNewMessage += TcpMessage;

			if (initiateSocket) socket.InitializeConnection();
		}

        private void TcpMessage(string value) => Debug.WriteLine(value);
    }

}