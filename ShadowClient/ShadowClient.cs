using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network;
using RadiantConnect.Services;
using RadiantConnect.SocketServices.XMPP;
using static System.Text.RegularExpressions.Regex;

// ReSharper disable All <-- for now we'll ignore everything since this is unused temporarily

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

        internal async Task BuildClient(bool autorun = true)
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

        internal async Task InitiateConnections()
        {
            if (!_built) throw new RadiantConnectShadowClientException("Client has not been built");
            string xmppBindParsed = Match(_xmppBind, "(.*)@", RegexOptions.Compiled).Value[..^1];
            string xmppRcConnect = Match(_xmppBind, "\\/(.*)", RegexOptions.Compiled).Value[1..];
            string xmppUrl = Match(_xmppBind, "@(.*)\\/", RegexOptions.Compiled).Value[1..^1];

            await _controller.SendMessage("""
                                          <iq id="_xmpp_session1" type="set">
                                              <session xmlns="urn:ietf:params:xml:ns:xmpp-session">
                                                  <platform>riot</platform>
                                              </session>
                                          </iq>
                                          """);


            //RmsClient client = new(rsoAuth, RmsRegion.Us);

           // await client.StartClient();

            await _controller.SendMessage("""
                                          <iq id='update_session_active_4' type='set'>
                                              <query xmlns='jabber:iq:riotgames:session'>
                                                  <session mode='active'/>
                                              </query>
                                          </iq>
                                          """);

            await _controller.SendMessage($"<iq from='{xmppBindParsed}@{xmppUrl}' to='{_xmppBind}' id='update_session_active_4' type='result' />");

            await _controller.SendMessage($"""
                                          <presence from='{_xmppBind}'
                                                    to='{_xmppBind}' id='presence_5'>
                                              <games>
                                                  <keystone>
                                                      <st>chat</st>
                                                      <s.t>{DateTime.Now.ToFileTimeUtc()}</s.t>
                                                      <m />
                                                      <s.p>keystone</s.p>
                                                      <pty />
                                                  </keystone>
                                              </games>
                                              <platform>windows</platform>
                                              <show>chat</show>
                                              <status />
                                          </presence>
                                          """);

            await _net.GetAsync<object>(_clientData.GlzUrl, $"/sessions/{xmppBindParsed}/reconnect");
            

            await _controller.SendPresenceEvent();
        }

        internal string GetLoginKeystone()
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

        internal GameTagLine GetNameTagLine()
        {
            JsonWebToken token = new (rsoAuth.IdToken);
            JsonElement acctToken = token.GetPayloadValue<JsonElement>("acct");

            return JsonSerializer.Deserialize<GameTagLine>(acctToken.GetRawText())!;
        }
    }
}
