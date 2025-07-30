using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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