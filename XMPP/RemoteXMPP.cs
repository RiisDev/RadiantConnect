using System.Collections.Specialized;
using System.Net.Security;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;

namespace RadiantConnect.XMPP
{
    public class RemoteXMPP
    {
        #region XMPPRegions
        internal readonly Dictionary<string, string> ChatUrls = new() {
            {"asia", "jp1.chat.si.riotgames.com"},
            {"br1", "br.chat.si.riotgames.com"},
            {"eu", "euw1.chat.si.riotgames.com"},
            {"eun1", "eun1.chat.si.riotgames.com"},
            {"euw1", "euw1.chat.si.riotgames.com"},
            {"jp1", "jp1.chat.si.riotgames.com"},
            {"kr1", "kr1.chat.si.riotgames.com"},
            {"la1", "la1.chat.si.riotgames.com"},
            {"la2", "la2.chat.si.riotgames.com"},
            {"na1", "na2.chat.si.riotgames.com"},
            {"oc1", "kr1.chat.si.riotgames.com"},
            {"pbe1", "pbe1.chat.si.riotgames.com"},
            {"ru1", "euw1.chat.si.riotgames.com"},
            {"sea1", "sa1.chat.si.riotgames.com"},
            {"sea2", "sa2.chat.si.riotgames.com"},
            {"sea3", "sa3.chat.si.riotgames.com"},
            {"tr1", "euw1.chat.si.riotgames.com"},
            {"us", "la1.chat.si.riotgames.com"},
            {"us-br1", "br.chat.si.riotgames.com"},
            {"us-la2", "la2.chat.si.riotgames.com"}
        };

        internal readonly Dictionary<string, string> ChatAffinity = new()
        {
            { "asia", "jp1" },
            { "br1", "br1" },
            { "eu", "eu1" },
            { "eun1", "eu2" },
            { "euw1", "eu1" },
            { "jp1", "jp1" },
            { "kr1", "kr1" },
            { "la1", "la1" },
            { "la2", "la2" },
            { "na1", "na1" },
            { "oc1", "kr1" },
            { "pbe1", "pb1" },
            { "ru1", "eu1" },
            { "sea1", "sa1" },
            { "sea2", "sa2" },
            { "sea3", "sa3" },
            { "tr1", "eu1" },
            { "us", "la1" },
            { "us-br1", "br1" },
            { "us-la2", "la2" }
        };
        #endregion
        public delegate void InternalMessage(string data);

        public event InternalMessage? OnClientMessage;
        public event InternalMessage? OnServerMessage;

        static async Task AsyncSocketWrite(SslStream sslStream, string message)
        {
            Console.WriteLine($"Write: {message}");
            byte[] buffer = Encoding.UTF8.GetBytes(message);
            await sslStream.WriteAsync(buffer, 0, buffer.Length);
            await sslStream.FlushAsync();
        }

        static async Task<string> AsyncSocketRead(SslStream sslStream)
        {
            int byteCount;
            byte[] bytes = new byte[1024];
            string content;

            do
            {
                byteCount = await sslStream.ReadAsync(bytes);
                content = Encoding.UTF8.GetString(bytes, 0, byteCount);
                Array.Clear(bytes);

                if (byteCount < bytes.Length)
                    break;

            } while (byteCount != 0);
            
            if (content.Length > 0)
                Console.WriteLine($"Read: {content}");

            return content;
        }

        public async Task InitiateRemoteXMPP(Authentication.Authentication.RSOAuth auth)
        {
            string affinityDomain = ChatAffinity[auth.ChatAffinity!];
            string? token = auth.AccessToken;
            string? pasToken = auth.PasToken;
            string? entitlement = auth.Entitlement;
            string chatHost = ChatUrls[auth.ChatAffinity!];

            Console.WriteLine($"Chat Affinity: {auth.ChatAffinity}\nServer Url: {chatHost}\nStream To: {affinityDomain}.pvp.net");

            using TcpClient tcpClient = new(chatHost, 5223);
            SslStream sslStream = new(tcpClient.GetStream(), true, (_, _, _, _) => true);
            await sslStream.AuthenticateAsClientAsync(chatHost);
            
            Console.WriteLine($"Connected to {chatHost}, authenticating...");

            // Stage 1
            await AsyncSocketWrite(sslStream, $"<?xml version=\"1.0\"?><stream:stream to=\"{affinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\">");
            string incomingData;
            do
            {
                incomingData = await AsyncSocketRead(sslStream);
            } while (!incomingData.Contains("X-Riot-RSO-PAS"));

            // Stage 2
            await AsyncSocketWrite(sslStream, $"<auth mechanism=\"X-Riot-RSO-PAS\" xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\"><rso_token>{token}</rso_token><pas_token>{pasToken}</pas_token></auth>");
            string socketData = await AsyncSocketRead(sslStream);
            if (socketData.Contains("affinity-invalid-token")) return;

            // Stage 3
            await AsyncSocketWrite(sslStream, $"<?xml version=\"1.0\"?><stream:stream to=\"{affinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\">");
            do
            {
                incomingData = await AsyncSocketRead(sslStream);
            } while (!incomingData.Contains("stream:features"));

            // Stage 4
            await AsyncSocketWrite(sslStream, "<iq id=\"_xmpp_bind1\" type=\"set\"><bind xmlns=\"urn:ietf:params:xml:ns:xmpp-bind\"></bind></iq>");
            await AsyncSocketRead(sslStream);

            // Stage 5
            await AsyncSocketWrite(sslStream, "<iq id=\"_xmpp_session1\" type=\"set\"><session xmlns=\"urn:ietf:params:xml:ns:xmpp-session\"/></iq>");
            await AsyncSocketRead(sslStream);

            // Stage 6
            await AsyncSocketWrite(sslStream, $"<iq id=\"xmpp_entitlements_0\" type=\"set\"><entitlements xmlns=\"urn:riotgames:entitlements\"><token xmlns=\"\">{entitlement}</token></entitlements></iq>");
            await AsyncSocketRead(sslStream);

            Console.WriteLine("Connected and authenticated, now proxying data!");
        }

    }
}
