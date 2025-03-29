using RadiantConnect.XMPP;
using System.Diagnostics.CodeAnalysis;

namespace RadiantConnect.SocketServices.XMPP
{
    public class XMPPController
    {
        public enum Status
        {
            Chat,
            Away,
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
        private void HandleXMPPData(string data)
        {
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

        internal string GetUnixTimestamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

        internal bool IsValidGuid([StringSyntax(StringSyntaxAttribute.GuidFormat)] string guid) => Guid.TryParse(guid, out Guid _) && guid.Contains('-');

        #endregion

        public async Task SendPresenceEvent() => await SendMessage("<presence/>");

        public async Task SendChatMessage([StringSyntax(StringSyntaxAttribute.GuidFormat)] string recipient, string message)
        {
            if (!IsValidGuid(recipient))
                throw new RadiantConnectXMPPException("Invalid user Id provided.");

            await SendMessage($"""
                              <message id="{GetUnixTimestamp()}:1" to="{recipient}@{_affinity}.pvp.net" type="chat">
                              	<body>{message}</body>
                              </message>
                              """);

            await GetChatMessages(recipient);
        }

        public async Task GetChatMessages([StringSyntax(StringSyntaxAttribute.GuidFormat)] string recipient)
        {
            if (!IsValidGuid(recipient))
                throw new RadiantConnectXMPPException("Invalid user Id provided.");

            await SendMessage($"""
                               <iq type="get" id="get_archive_7">
                                 <query xmlns="jabber:iq:riotgames:archive">
                                   <with>{recipient}@{_affinity}.pvp.net</with>
                                 </query>
                               </iq>
                               """);
        }

        internal void SendUserStatus(Status status)
        {
            // Todo

            /*
             *{
                 "isValid": true,
                 "sessionLoopState": "MENUS",
                 "partyOwnerSessionLoopState": "MENUS",
                 "customGameName": "",
                 "customGameTeam": "",
                 "partyOwnerMatchMap": "",
                 "partyOwnerMatchCurrentTeam": "",
                 "partyOwnerMatchScoreAllyTeam": 0,
                 "partyOwnerMatchScoreEnemyTeam": 0,
                 "partyOwnerProvisioningFlow": "Invalid",
                 "provisioningFlow": "Invalid",
                 "matchMap": "",
                 "partyId": "f395adc8-3ba9-47a0-b0de-7633ecd94920",
                 "isPartyOwner": true,
                 "partyState": "DEFAULT",
                 "partyAccessibility": "CLOSED",
                 "maxPartySize": 5,
                 "queueId": "unrated",
                 "partyLFM": false,
                 "partyClientVersion": "release-09.10-shipping-16-2953141",
                 "partySize": 1,
                 "tournamentId": "",
                 "rosterId": "",
                 "partyVersion": 1733155231459,
                 "queueEntryTime": "0001.01.01-00.00.00",
                 "playerCardId": "2ee6d025-4aac-3a67-0f6e-dba827acc75f",
                 "playerTitleId": "171e2f90-41e0-48d0-bbf5-28a531c7eafb",
                 "preferredLevelBorderId": "",
                 "accountLevel": 336,
                 "competitiveTier": 0,
                 "leaderboardPosition": 0,
                 "isIdle": true, // This changes
                 "tempValueX": "",
                 "tempValueY": "",
                 "tempValueZ": false,
                 "tempValueW": false,
                 "tempValueV": 1
               }
               

             *<presence id="presence_12">
                 <show>chat</show> <---- this is aither away or chat
                 <status/>
                 <games>
                   <keystone>
                     <st>chat</st> <---- this is aither away or chat
                     <s.t>1733155224221</s.t>
                     <m/>
                     <s.p>keystone</s.p>
                     <pty/>
                   </keystone>
                   <valorant>
                     <s.r>PC</s.r>
                     <st>chat</st> <---- this is aither away or chat
                     <p>ew0KCSJpc1ZhbGlkIjogdHJ1ZSwNCgkic2Vzc2lvbkxvb3BTdGF0ZSI6ICJNRU5VUyIsDQoJInBhcnR5T3duZXJTZXNzaW9uTG9vcFN0YXRlIjogIk1FTlVTIiwNCgkiY3VzdG9tR2FtZU5hbWUiOiAiIiwNCgkiY3VzdG9tR2FtZVRlYW0iOiAiIiwNCgkicGFydHlPd25lck1hdGNoTWFwIjogIiIsDQoJInBhcnR5T3duZXJNYXRjaEN1cnJlbnRUZWFtIjogIiIsDQoJInBhcnR5T3duZXJNYXRjaFNjb3JlQWxseVRlYW0iOiAwLA0KCSJwYXJ0eU93bmVyTWF0Y2hTY29yZUVuZW15VGVhbSI6IDAsDQoJInBhcnR5T3duZXJQcm92aXNpb25pbmdGbG93IjogIkludmFsaWQiLA0KCSJwcm92aXNpb25pbmdGbG93IjogIkludmFsaWQiLA0KCSJtYXRjaE1hcCI6ICIiLA0KCSJwYXJ0eUlkIjogImYzOTVhZGM4LTNiYTktNDdhMC1iMGRlLTc2MzNlY2Q5NDkyMCIsDQoJImlzUGFydHlPd25lciI6IHRydWUsDQoJInBhcnR5U3RhdGUiOiAiREVGQVVMVCIsDQoJInBhcnR5QWNjZXNzaWJpbGl0eSI6ICJDTE9TRUQiLA0KCSJtYXhQYXJ0eVNpemUiOiA1LA0KCSJxdWV1ZUlkIjogInVucmF0ZWQiLA0KCSJwYXJ0eUxGTSI6IGZhbHNlLA0KCSJwYXJ0eUNsaWVudFZlcnNpb24iOiAicmVsZWFzZS0wOS4xMC1zaGlwcGluZy0xNi0yOTUzMTQxIiwNCgkicGFydHlTaXplIjogMSwNCgkidG91cm5hbWVudElkIjogIiIsDQoJInJvc3RlcklkIjogIiIsDQoJInBhcnR5VmVyc2lvbiI6IDE3MzMxNTUyMzE0NTksDQoJInF1ZXVlRW50cnlUaW1lIjogIjAwMDEuMDEuMDEtMDAuMDAuMDAiLA0KCSJwbGF5ZXJDYXJkSWQiOiAiMmVlNmQwMjUtNGFhYy0zYTY3LTBmNmUtZGJhODI3YWNjNzVmIiwNCgkicGxheWVyVGl0bGVJZCI6ICIxNzFlMmY5MC00MWUwLTQ4ZDAtYmJmNS0yOGE1MzFjN2VhZmIiLA0KCSJwcmVmZXJyZWRMZXZlbEJvcmRlcklkIjogIiIsDQoJImFjY291bnRMZXZlbCI6IDMzNiwNCgkiY29tcGV0aXRpdmVUaWVyIjogMCwNCgkibGVhZGVyYm9hcmRQb3NpdGlvbiI6IDAsDQoJImlzSWRsZSI6IGZhbHNlLA0KCSJ0ZW1wVmFsdWVYIjogIiIsDQoJInRlbXBWYWx1ZVkiOiAiIiwNCgkidGVtcFZhbHVlWiI6IGZhbHNlLA0KCSJ0ZW1wVmFsdWVXIjogZmFsc2UsDQoJInRlbXBWYWx1ZVYiOiAxDQp9</p>
                     <s.p>valorant</s.p>
                     <s.t>1733155716852</s.t>
                     <pty/>
                   </valorant>
                 </games>
               </presence>
               
             */
        }
    }
}
