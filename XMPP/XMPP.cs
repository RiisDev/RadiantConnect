using RadiantConnect.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

// Credit to https://github.com/molenzwiebel/Deceive for guide

namespace RadiantConnect.XMPP
{
    public class XMPP
    {
        internal bool Connected = true;

        //public delegate void InternalMessage(string data);

        //public event InternalMessage? OnClientMessage;

        public static void KillRiot()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes.Where(proc => proc.ProcessName is "Riot Client" or "VALORANT" or "VALORANT-Win64-Shipping" or "RiotClientServices")) process.Kill();
        }

        internal static bool IsRiotRunning()
        {
            Process[] processes = Process.GetProcesses();
            return processes.Any(proc => proc.ProcessName is "Riot Client" or "VALORANT-Win64-Shipping" or "VALORANT" or "RiotClientServices");
        }

        internal static (TcpListener, int) NewTcpListener()
        {
            TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            return (listener, ((IPEndPoint)listener.LocalEndpoint).Port);
        }

        internal static X509Certificate2 GenerateCertificate()
        {
            ECDsa ecdsaValue = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            CertificateRequest certRequest = new("CN=RadiantConnect", ecdsaValue, HashAlgorithmName.SHA256);
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));
            X509Certificate2 generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10));
            return new X509Certificate2(generatedCert.Export(X509ContentType.Pfx));
        }
        
        private async Task WriteAsync(SslStream sslStream, string data)
        {
            Debug.WriteLine($"Writing to server: {data}");
            await sslStream.WriteAsync(Encoding.UTF8.GetBytes(data));
        }

        private async Task<string> ReadAsync(SslStream stream)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (bytesRead <= 0) return string.Empty;
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            return response;
        }

        internal readonly Dictionary<string, string> ChatToAffinity = new()
        {
            { "as2.chat.si.riotgames.com", "as2" },
            { "br.chat.si.riotgames.com", "br1" },
            { "eu3.chat.si.riotgames.com", "eu3" },
            { "eun1.chat.si.riotgames.com", "eun1" },
            { "euw1.chat.si.riotgames.com", "euw1" },
            { "jp1.chat.si.riotgames.com", "jp1" },
            { "kr1.chat.si.riotgames.com", "kr1" },
            { "la1.chat.si.riotgames.com", "la1" },
            { "la2.chat.si.riotgames.com", "la2" },
            { "na2.chat.si.riotgames.com", "na1" },
            { "oc1.chat.si.riotgames.com", "oc1" },
            { "pbe1.chat.si.riotgames.com", "pbe1" },
            { "ru1.chat.si.riotgames.com", "ru1" },
            { "sa1.chat.si.riotgames.com", "sea1" },
            { "sa2.chat.si.riotgames.com", "sea2" },
            { "sa3.chat.si.riotgames.com", "sea3" },
            { "sa4.chat.si.riotgames.com", "sea4" },
            { "tr1.chat.si.riotgames.com", "tr1" },
            { "us2.chat.si.riotgames.com", "us2" }
        };

        internal readonly Dictionary<string, string> InitialConnections = new()
        {
            {"<?xml version=\"1.0\" encoding=\"UTF-8\"?><stream:stream to=\"AFFINITY_REPLACE.pvp.net\" xml:lang=\"en\" version=\"1.0\" xmlns=\"jabber:client\" xmlns:stream=\"http://etherx.jabber.org/streams\">", "x-riot-rso-pas"},
            {"<auth mechanism=\"X-Riot-RSO-PAS\" xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\"><rso_token>PAUTHINSERT</rso_token><pas_token>JAUTHINSERT</pas_token></auth>\" xmlns:stream=\"http://etherx.jabber.org/streams\">", "<success xmlns="},
            {"<?xml version=\"1.1\" encoding=\"UTF-8\"?><stream:stream to=\"AFFINITY_REPLACE.pvp.net\" xml:lang=\"en\" version=\"1.0\" xmlns=\"jabber:client\" xmlns:stream=\"http://etherx.jabber.org/streams\">","<stream:features>"},
            {"<iq id=\"_xmpp_bind1\" type=\"set\"><bind xmlns=\"urn:ietf:params:xml:ns:xmpp-bind\"></bind></iq>", "<iq id='_xmpp_bind1' type='result'>"},
            {$"<iq type=\"set\" id=\"xmpp_entitlements_0\"><entitlements xmlns=\"urn:riotgames:entitlements\"><token>JAUTHINSERT</token></entitlements></iq><iq id=\"_xmpp_session1\" type=\"set\"><session xmlns=\"urn:ietf:params:xml:ns:xmpp-session\"><platform>riot</platform></session></iq>", "<iq type='result' id='xmpp_entitlements_0'>"}
        };

        internal async Task HandleClientServer(TcpListener server, string chatHost, int chatPort, string oAuth, string jAuth, string pAuth)
        {
            X509Certificate2 proxyCertificate = new(GenerateCertificate());
            TcpClient incomingClient = await server.AcceptTcpClientAsync();
            SslStream incomingStream = new(incomingClient.GetStream());

            await incomingStream.AuthenticateAsServerAsync(proxyCertificate);

            TcpClient outgoingClient;
            while (true)
            {
                try { outgoingClient = new TcpClient(chatHost, chatPort); break; }
                catch (Exception ex) { throw new Exception($"Unable to communicate with chat client. {ex}"); }
            }

            SslStream outgoingStream = new(outgoingClient.GetStream());
            await outgoingStream.AuthenticateAsClientAsync(chatHost);

            Debug.WriteLine("Connected to XMPP server, authenticating...");

            for (int connectionIndex = 0; connectionIndex < InitialConnections.Count; connectionIndex++)
            {
                KeyValuePair<string,string> connectionData = InitialConnections.ElementAt(connectionIndex);
                await WriteAsync(outgoingStream, connectionData.Key
                    .Replace("AFFINITY_REPLACE", ChatToAffinity[chatHost])
                    .Replace("JAUTHINSERT", jAuth)
                    .Replace("PAUTHINSERT", pAuth)
                );
                do
                {
                    await Task.Delay(1000);
                    string data = await ReadAsync(outgoingStream);
                    Debug.WriteLine(data);
                    if (data.Contains("temporary-auth-failure")) throw new Exception("failure: temporary-auth-failure");
                    if (data.Contains("not-well-formed")) throw new Exception("failure: not-well-formed");
                    if (data.Contains("failure")) throw new Exception($"Unknown exception: {data}");
                    if (!data.Contains(connectionData.Value, StringComparison.OrdinalIgnoreCase)) continue;
                    break;

                } while (true);
                Debug.WriteLine($"Finished Step: {connectionIndex+1}/{InitialConnections.Count}");
            }
            
            Debug.WriteLine("Connected and authenticated, now proxy data!");

            _ = new Timer(_ => { outgoingStream.Write(" "u8.ToArray()); }, null, 0, 150000);

        }

        public Process InitializeConnection()
        {
            string riotClientPath = ValorantService.GetRiotClientPath();
            string valorantPath = ValorantService.GetValorantPath();
            if (IsRiotRunning()) throw new Exception("Riot/Valorant cannot be running.");
            if (!File.Exists(riotClientPath)) throw new Exception($"Riot Client executable not found: {riotClientPath}");
            if (!File.Exists(valorantPath)) throw new Exception($"Valorant executable not found: {valorantPath}");

            (TcpListener currentTcpListener, int currentPort) = NewTcpListener();

            InternalProxy proxyServer = new(currentPort);

            bool serverHooked = false;

            proxyServer.OnChatPatched += (_, args) =>
            {
                if (serverHooked) return;

                serverHooked = true;

                Task.Run(async () =>
                {
                    Debug.WriteLine(args.RiotSecurePAuth);
                    await HandleClientServer(currentTcpListener, args.ChatHost!, args.ChatPort, args.RiotSecureOAuth!, args.RiotSecureJAuth!, args.RiotSecurePAuth!);
                });
            };

            ProcessStartInfo riotClientStartArgs = new()
            {
                FileName = riotClientPath,
                Arguments = $"--client-config-url=\"http://127.0.0.1:{proxyServer.ConfigPort}\" --launch-product=valorant --launch-patchline=live"
            };
            
            return Process.Start(riotClientStartArgs)!;
        }
    }
}
