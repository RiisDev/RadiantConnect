using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadiantConnect.XMPP
{
    public class XMPPController
    {
        private readonly RemoteXMPP? _remoteClient;
        private readonly ValXMPP? _valClient;

        public XMPPController(RemoteXMPP remoteClient) => _remoteClient = remoteClient;

        public XMPPController(ValXMPP valClient) => _valClient = valClient;

        public async Task SendMessage([StringSyntax(StringSyntaxAttribute.Xml)] string message)
        {
            if (_remoteClient is not null)
                await _remoteClient.SendMessage(message);
            else if (_valClient is not null)
                await _valClient.Handle.SendXmlToOutgoingStream(message);
            else
                throw new RadiantConnectXMPPException("No client connected");
        }

        public async Task SendInternalMessage([StringSyntax(StringSyntaxAttribute.Xml)] string message)
        {
            if (_valClient is not null)
                await _valClient.Handle.SendXmlToIncomingStream(message);
            else if (_remoteClient is not null && _valClient is null)
                throw new RadiantConnectXMPPException("Cannot send internal XMPP to remote client.");
            else
                throw new RadiantConnectXMPPException("No client connected");
        }

        public async Task SendPresenceEvent() => await SendMessage("<presence/>");
    }
}
