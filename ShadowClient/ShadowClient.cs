using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network;
using RadiantConnect.Services;
using RadiantConnect.XMPP;

namespace RadiantConnect.ShadowClient
{
    internal record GameTagLine(
        [property: JsonPropertyName("game_name")] string GameName,
        [property: JsonPropertyName("tag_line")] string TagLine
    );


    internal class ShadowClient(RSOAuth rsoAuth)
    {
        private bool _built;
        private RemoteXMPP _remoteXmpp = null!;
        private XMPPController _controller = null!;
        private ValorantNet _net = null!;
        private LogService.ClientData _clientData = null!;
        private string _region = null!;
        private string _xmppBind = null!;
        
        public async Task BuildClient(bool autorun = true)
        {
            if (_built)
                throw new RadiantConnectShadowClientException("Client has already been built");

            Initiator init = new(rsoAuth);
            _net = init.ExternalSystem.Net;
            _clientData = init.Client;
            _region = init.Client.Shard.ToString();

            _remoteXmpp = new RemoteXMPP();
            await _remoteXmpp.InitiateRemoteXMPP(rsoAuth);

            _xmppBind = _remoteXmpp.XmppBind;

            _controller = new XMPPController(_remoteXmpp);

            _built = true;
            
            if (autorun)
                await InitiateConnections();

            _remoteXmpp.OnMessage += data => Console.WriteLine($"XMPP: {data}");
        }

        public async Task InitiateConnections()
        {
            if (!_built) throw new RadiantConnectShadowClientException("Client has not been built");

            await _controller.SendMessage("<iq id='update_session_active_4' type='set'><query xmlns='jabber:iq:riotgames:session'><session mode='active'/></query></iq>");

            await _controller.SendMessage($"""
                                           <presence id='presence_5'>
                                           	<show>chat</show>
                                           	<status></status>
                                           	<games>
                                           		<keystone>
                                           			<st>chat</st>
                                           			<s.t>{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}</s.t>
                                           			<m></m>
                                           			<s.p>keystone</s.p>
                                           			<pty/>
                                           		</keystone>
                                           	</games>
                                           </presence>
                                           """);

            await _net.PostAsync<object>(_clientData.PdUrl, "/name-service/v3/players");
            await _net.PostAsync<object>(GetLoginKeystone(), $"/login-queue/v2/login/products/valorant/regions/{_region}");
            await _net.PostAsync<object>(_clientData.GlzUrl, $"/session/v2/sessions/{rsoAuth.Subject}/connect");

            GameTagLine tagLine = GetNameTagLine();

            await _controller.SendMessage($"""
                                           <iq type="result" id="_xmpp_session1">
                                             <session xmlns="urn:ietf:params:xml:ns:xmpp-session">
                                               <platform>windows</platform>
                                               <id name="{tagLine.GameName}" tagline="{tagLine.TagLine}"/>
                                               <platforms>
                                                 <riot name="{tagLine.GameName}" tagline="{tagLine.TagLine}"/>
                                               </platforms>
                                               <ts>{DateTimeOffset.UtcNow:yyyy-MM-dd HH:mm:ss.fff}</ts>
                                             </session>
                                           </iq>
                                           """);

            await _controller.SendPresenceEvent();
        }

        private string GetLoginKeystone()
        {
            string playerConfig = JsonSerializer.Serialize(rsoAuth.ClientConfig);
            
            Regex getKeystoneRegex = new ("voice_chat\\.voice_recording_upload_uri\":\"(https:\\/\\/.+?\\.pp\\.sgp\\.pvp\\.net)", RegexOptions.Compiled);
            Match found = getKeystoneRegex.Match(playerConfig);

            if (!found.Success)
            {
                throw new RadiantConnectShadowClientException("Failed to find login keystone");
            }

            return found.Groups[1].Value;
        }

        private GameTagLine GetNameTagLine()
        {
            JsonWebToken token = new (rsoAuth.IdToken);
            JsonElement acctToken = token.GetPayloadValue<JsonElement>("acct");

            return JsonSerializer.Deserialize<GameTagLine>(acctToken.GetRawText())!;
        }
    }
}
