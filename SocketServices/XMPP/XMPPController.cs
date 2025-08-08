using RadiantConnect.XMPP;

namespace RadiantConnect.SocketServices.XMPP
{
    public class XMPPController
    {
        public enum Status
        {
            Chat,
            Away,
        }

        public enum ChatRoom
        {
            Party,
            InGame,
            PreGame,
            PrivateMessage
        }

        public delegate void XMPPReceived(string xmlData);

        public event XMPPReceived? OnXMPPReceived;
        public event ValXMPP.PresenceUpdated? OnPresenceUpdated;
        public event ValXMPP.PlayerPresenceUpdated? OnPlayerPresenceUpdated;

        #region Controller Setup
        private readonly RemoteXMPP? _remoteClient;
        private readonly ValXMPP? _valClient;
        private readonly string? _affinity;
        public XMPPController(RemoteXMPP remoteClient)
        {
            _remoteClient = remoteClient;
            remoteClient.OnMessage += HandleXMPPData;

            if (remoteClient.AuthData is null || string.IsNullOrEmpty(remoteClient.AuthData.Affinity))
                throw new RadiantConnectXMPPException("Failed to find stream url");

            _affinity = remoteClient.ChatAffinity[remoteClient.AuthData.Affinity];
        }

        public XMPPController(ValXMPP valClient)
        {
            _valClient = valClient;
            _valClient.OnServerMessage += HandleXMPPData;

            if (string.IsNullOrEmpty(valClient.StreamUrl))
                throw new RadiantConnectXMPPException("Failed to find stream url");

            _affinity = valClient.StreamUrl;
        }

        #endregion
        #region Base Methods

        private string _lastData = "";
        private void HandleXMPPData(string data)
        {
            if (string.Equals(_lastData, data, StringComparison.OrdinalIgnoreCase)) return;
            _lastData = data;

            OnXMPPReceived?.Invoke(data);
            ValXMPP.HandlePlayerPresence(data, presence => OnPlayerPresenceUpdated?.Invoke(presence));
            ValXMPP.HandlePresenceObject(data, valorantPresence => OnPresenceUpdated?.Invoke(valorantPresence));
        }

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
                await _valClient.Handle.SendXmlMessageAsync(message);
            else if (_remoteClient is not null && _valClient is null)
                throw new RadiantConnectXMPPException("Cannot send internal XMPP to remote client.");
            else
                throw new RadiantConnectXMPPException("No client connected");
        }

        #endregion

        public async Task SendPresenceEvent() => await SendMessage("<presence/>");

        public async Task SendChatMessage([StringSyntax(StringSyntaxAttribute.GuidFormat)] string recipient, string message)
        {
            if (!SocketUtil.IsValidGuid(recipient))
                throw new RadiantConnectXMPPException("Invalid user Id provided.");

            await SendMessage($"""
                              <message id="{SocketUtil.GetUnixTimestamp()}:1" to="{recipient}@{_affinity}.pvp.net" type="chat">
                              	<body>{message}</body>
                              </message>
                              """);

            await GetChatMessages(recipient);
        }

        public async Task GetChatMessages([StringSyntax(StringSyntaxAttribute.GuidFormat)] string recipient)
        {
            if (!SocketUtil.IsValidGuid(recipient))
                throw new RadiantConnectXMPPException("Invalid user Id provided.");

            await SendMessage($"""
                               <iq type="get" id="get_archive_7">
                                 <query xmlns="jabber:iq:riotgames:archive">
                                   <with>{recipient}@{_affinity}.pvp.net</with>
                                 </query>
                               </iq>
                               """);
        }

        public record ChatMessage(string Timestamp, string Sender, string Recipient, string Text, string Type);
    }
}
/*
  <!-- Party Chat -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='fb27e146-a21c-4e55-a2b0-e49de2909e8d@ares-parties.na1.pvp.net/ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc'
   	stamp='2025-05-29 12: 50: 00.801' id='1748523002478: 1' type='groupchat'>
   	<x xmlns='http: //jabber.org/protocol/muc#user'>
   		<item jid='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' />
   	</x>
   
   	<body>eased</body>
   </message>
   
   <!-- Private Message -->
   <message from='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' type='chat' xmlns='jabber:client'>
   	<sent xmlns='urn:xmpp:carbons:2'>
   		<forwarded xmlns='urn:xmpp:forward:0'>
   			<message to='49507637-f4e9-5246-bda7-b92d40425c0f@la1.pvp.net' stamp='2025-05-29 12:53:10.655'
   				id='1748523192452:2' type='chat'>
   
   				<body>test</body>
   			</message>
   		</forwarded>
   	</sent>
   </message>
   
   <!-- Pregame -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='2d9c6a66-10aa-47a6-87dc-ed538f8579e1-2@ares-pregame.na1.pvp.net/ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc'
   	stamp='2025-05-29 12:54:49.082' id='1748523290734:3' type='groupchat'>
   	<x xmlns='http://jabber.org/protocol/muc#user'>
   		<item jid='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' />
   	</x>
   
   	<body>e</body>
   </message>
   
   <!-- Team Chat Red -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='2d9c6a66-10aa-47a6-87dc-ed538f8579e1-red@ares-coregame.na1.pvp.net/ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc'
   	stamp='2025-05-29 12:56:22.708' id='1748523384516:4' type='groupchat'>
   	<x xmlns='http://jabber.org/protocol/muc#user'>
   		<item jid='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' />
   	</x>
   
   	<body>yer</body>
   </message>
   
   <!-- Team Chat Blue -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='2d9c6a66-10aa-47a6-87dc-ed538f8579e1-blu@ares-coregame.na1.pvp.net/ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc'
   	stamp='2025-05-29 12:56:22.708' id='1748523384516:4' type='groupchat'>
   	<x xmlns='http://jabber.org/protocol/muc#user'>
   		<item jid='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' />
   	</x>
   
   	<body>yer</body>
   </message>
   
   <!-- ALl Chat  -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='2d9c6a66-10aa-47a6-87dc-ed538f8579e1-all@ares-coregame.na1.pvp.net/ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc'
   	stamp='2025-05-29 12:56:36.341' id='1748523398075:5' type='groupchat'>
   	<x xmlns='http://jabber.org/protocol/muc#user'>
   		<item jid='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net' />
   	</x>
   
   	<body>hola</body>
   </message>
   
   <!--  All Enemy -->
   <message to='ed47b7fa-f5aa-5d68-8c50-3cfa8aa2b9fc@la1.pvp.net/RC-292159530'
   	from='2d9c6a66-10aa-47a6-87dc-ed538f8579e1-all@ares-coregame.na1.pvp.net/2a217e2b-ae1e-5065-899d-da391e019408'
   	stamp='2025-05-29 12:59:33.149' id='1748523572981:3' type='groupchat'>
   	<x xmlns='http://jabber.org/protocol/muc#user'>
   		<item jid='2a217e2b-ae1e-5065-899d-da391e019408@br1.pvp.net' />
   	</x>
   
   	<body>no</body>
   </message>
*/