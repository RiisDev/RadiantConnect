using RadiantConnect.XMPP;

namespace RadiantConnect.SocketServices.XMPP
{
	/// <summary>
	/// Provides a high-level controller for interacting with XMPP services,
	/// including presence updates, chat room context, and raw XMPP message handling.
	/// </summary>
	/// <remarks>
	/// This controller abstracts common XMPP functionality to simplify usage
	/// within the application.
	/// </remarks>
	public class XMPPController
	{
		/// <summary>
		/// Represents the current presence status of the user in XMPP.
		/// </summary>
		public enum Status
		{
			/// <summary>
			/// The user is available and actively participating in chat.
			/// </summary>
			Chat,

			/// <summary>
			/// The user is currently away or inactive.
			/// </summary>
			Away,
		}

		/// <summary>
		/// Defines the type of chat room or messaging context.
		/// </summary>
		public enum ChatRoom
		{
			/// <summary>
			/// A party-based chat room shared with a group.
			/// </summary>
			Party,

			/// <summary>
			/// Chat occurring while actively in a game.
			/// </summary>
			InGame,

			/// <summary>
			/// Chat occurring before a game has started.
			/// </summary>
			PreGame,

			/// <summary>
			/// A direct one-to-one private message channel.
			/// </summary>
			PrivateMessage
		}

		/// <summary>
		/// Delegate invoked when raw XMPP XML data is received.
		/// </summary>
		/// <param name="xmlData">
		/// The raw XML payload received from the XMPP stream.
		/// </param>
		public delegate void XMPPReceived(string xmlData);

		/// <summary>
		/// Raised when raw XMPP XML data is received.
		/// </summary>
		public event XMPPReceived? OnXMPPReceived;

		/// <summary>
		/// Raised when a general presence update is received from XMPP.
		/// </summary>
		public event ValXMPP.PresenceUpdated? OnPresenceUpdated;

		/// <summary>
		/// Raised when a specific player's presence information is updated.
		/// </summary>
		public event ValXMPP.PlayerPresenceUpdated? OnPlayerPresenceUpdated;

		#region Controller Setup
		private readonly RemoteXMPP? _remoteClient;
		private readonly ValXMPP? _valClient;
		private readonly string? _affinity;

		/// <summary>
		/// Initializes a new instance of the <see cref="XMPPController"/> class
		/// using a remote XMPP client connection.
		/// </summary>
		/// <param name="remoteClient">
		/// The remote XMPP client responsible for handling authentication
		/// and message transport.
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when the authentication data is missing or does not contain
		/// a valid affinity value required to determine the XMPP stream URL.
		/// </exception>
		public XMPPController(RemoteXMPP remoteClient)
		{
			_remoteClient = remoteClient;
			remoteClient.OnMessage += HandleXMPPData;

			if (remoteClient.AuthData is null || string.IsNullOrEmpty(remoteClient.AuthData.Affinity))
				throw new RadiantConnectXMPPException("Failed to find stream url");

			_affinity = remoteClient.ChatAffinity[remoteClient.AuthData.Affinity];
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XMPPController"/> class
		/// using a VAL-specific XMPP client connection.
		/// </summary>
		/// <param name="valClient">
		/// The VAL XMPP client used to receive server messages and stream data.
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when the VAL client does not provide a valid stream URL.
		/// </exception>
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

		/// <summary>
		/// Sends a raw XMPP XML message through the active client connection.
		/// </summary>
		/// <param name="message">
		/// The raw XML payload to send to the XMPP stream.
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when no XMPP client is currently connected.
		/// </exception>
		/// <remarks>
		/// This method automatically routes the message to either the remote
		/// or VAL XMPP client depending on which is active.
		/// </remarks>
		public async Task SendMessage([StringSyntax(StringSyntaxAttribute.Xml)] string message)
		{
			if (_remoteClient is not null)
				await _remoteClient.SendMessage(message).ConfigureAwait(false);
			else if (_valClient is not null)
				await _valClient.Handle.SendXmlToOutgoingStream(message).ConfigureAwait(false);
			else
				throw new RadiantConnectXMPPException("No client connected");
		}

		/// <summary>
		/// Sends an internal XMPP XML message intended only for VAL server communication.
		/// </summary>
		/// <param name="message">
		/// The raw internal XML payload to send.
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when no client is connected or when attempting to send an
		/// internal message through a remote XMPP client.
		/// </exception>
		/// <remarks>
		/// Internal messages are not supported by remote XMPP clients and
		/// will be rejected.
		/// </remarks>
		public async Task SendInternalMessage([StringSyntax(StringSyntaxAttribute.Xml)] string message)
		{
			if (_valClient is not null)
				await _valClient.Handle.SendXmlMessageAsync(message).ConfigureAwait(false);
			else if (_remoteClient is not null)
				throw new RadiantConnectXMPPException("Cannot send internal XMPP to remote client.");
			else
				throw new RadiantConnectXMPPException("No client connected");
		}

		#endregion

		/// <summary>
		/// Sends a basic XMPP presence stanza to notify the server of availability.
		/// </summary>
		public async Task SendPresenceEvent() => await SendMessage("<presence/>").ConfigureAwait(false);

		/// <summary>
		/// Sends a chat message to a specific user.
		/// </summary>
		/// <param name="recipient">
		/// The recipient's unique user identifier (GUID format).
		/// </param>
		/// <param name="message">
		/// The text content of the chat message.
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when the recipient identifier is invalid.
		/// </exception>
		/// <remarks>
		/// After sending the message, the chat archive is queried to retrieve
		/// the updated conversation history.
		/// </remarks>
		public async Task SendChatMessage([StringSyntax(StringSyntaxAttribute.GuidFormat)] string recipient, string message)
		{
			if (!SocketUtil.IsValidGuid(recipient))
				throw new RadiantConnectXMPPException("Invalid user Id provided.");

			await SendMessage($"""
							  <message id="{SocketUtil.GetUnixTimestamp()}:1" to="{recipient}@{_affinity}.pvp.net" type="chat">
							  	<body>{message}</body>
							  </message>
							  """).ConfigureAwait(false);

			await GetChatMessages(recipient).ConfigureAwait(false);
		}

		/// <summary>
		/// Requests archived chat messages for a specific user conversation.
		/// </summary>
		/// <param name="recipient">
		/// The recipient's unique user identifier (GUID format).
		/// </param>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown when the recipient identifier is invalid.
		/// </exception>
		/// <remarks>
		/// This method sends an IQ stanza requesting archived messages
		/// from the Riot Games XMPP archive namespace.
		/// </remarks>
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
								""").ConfigureAwait(false);
		}

		/// <summary>
		/// Represents a parsed XMPP chat message.
		/// </summary>
		/// <param name="Timestamp">
		/// The timestamp indicating when the message was sent.
		/// </param>
		/// <param name="Sender">
		/// The sender's identifier.
		/// </param>
		/// <param name="Recipient">
		/// The recipient's identifier.
		/// </param>
		/// <param name="Text">
		/// The textual content of the message.
		/// </param>
		/// <param name="Type">
		/// The message type (e.g., chat, system).
		/// </param>
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