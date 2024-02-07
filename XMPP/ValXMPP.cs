using RadiantConnect.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;
using RadiantConnect.Methods;

// Credit to https://github.com/molenzwiebel/Deceive for guide
// ReSharper disable CheckNamespace

namespace RadiantConnect.XMPP
{
    public partial class ValXMPP
    {
        public delegate void InternalMessage(string data);
        public delegate void PresenceUpdated(ValorantPresence presence);
        public delegate void PlayerPresenceUpdated(PlayerPresence presence);

        public event InternalMessage? OnClientMessage;
        public event InternalMessage? OnServerMessage;
        public event PresenceUpdated? OnValorantPresenceUpdated;
        public event PlayerPresenceUpdated? OnPlayerPresenceUpdated;

        public delegate void SocketHandled(XMPPSocketHandle handle);
        public event SocketHandled? OnSocketCreated;

        public static void KillRiot()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes.Where(proc => proc.ProcessName is "Riot Client" or "VALORANT-Win64-Shipping" or "RiotClientServices")) process.Kill();
        }

        public static bool IsRiotRunning()
        {
            Process[] processes = Process.GetProcesses();
            return processes.Any(proc => proc.ProcessName is "Riot Client" or "VALORANT-Win64-Shipping" or "RiotClientServices");
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

        internal ValorantPresence? HandlePresenceObject(string data, bool invoke = true)
        {
            // Pull the <presence> XML out of the stream.
            // I do regex in case an error with the stream includes extra data
            Match match = ValorantPresenceRegex().Match(data);
            if (!match.Success) return null;

            string valorant64Data = match.Groups[1].Value;
            ValorantPresence? presenceData = JsonSerializer.Deserialize<ValorantPresence>(valorant64Data.FromBase64());

            if (invoke)
                OnValorantPresenceUpdated?.Invoke(presenceData!);

            return presenceData;
        }

        internal void HandlePlayerPresence(string data)
        {
            if (!data.Contains("<item jid=")) return;

            string[] presences = data.Split("<presence to=");

            foreach (string presence in presences)
            {
                if (string.IsNullOrEmpty(presence)) continue;
                if (!presence.Contains("tagline=")) continue;

                Match presenceMatch = XmlPresenceUpdateRegex().Match($"<presence to={presence}");
                if (!presenceMatch.Success) return;
                string newData = presenceMatch.Value;

                Dictionary<string, string> platforms = [];
                (string chatServer, string lobbyServer, string riotId, string tagLine) = ("", "", "", "");

                Match riotData = RiotDataRegex().Match(newData);
                Match platformsData = PlatformsDataRegex().Match(newData);
                Match chatServerData = ChatServerDataRegex().Match(newData);
                Match lobbyServerData = LobbyServerDataRegex().Match(newData);


                if (chatServerData.Success)
                    chatServer = chatServerData.Groups[1].Value;
                if (lobbyServerData.Success)
                    lobbyServer = lobbyServerData.Groups[1].Value;

                if (riotData.Success)
                {
                    riotId = riotData.Groups[1].Value;
                    tagLine = riotData.Groups[2].Value;
                }

                while (platformsData.Success)
                {
                    platforms.Add(platformsData.Groups[1].Value, platformsData.Groups[2].Value);
                    platformsData = platformsData.NextMatch();
                }


                OnPlayerPresenceUpdated?.Invoke(new PlayerPresence(
                    chatServer,
                    lobbyServer,
                    "riot",
                    riotId,
                    tagLine,
                    lobbyServer[(lobbyServer.IndexOf('/')+1)..],
                    platforms,
                    HandlePresenceObject(newData, false)!
                ));
            }
        }

        internal void HandleValorantPresence(string data)
        {
            try
            {
                if (OnValorantPresenceUpdated is not null)
                    HandlePresenceObject(data);
                if (OnPlayerPresenceUpdated is not null)
                    HandlePlayerPresence(data);
            }
            catch{/**/}
        }
        
        internal async Task HandleClients(TcpListener server, string chatHost, int chatPort)
        {
            X509Certificate2 proxyCertificate = new(GenerateCertificate());

            while (true)
            {
                try
                {
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
                    
                    XMPPSocketHandle handler = new(incomingStream, outgoingStream);
                    OnSocketCreated?.Invoke(handler);
                    handler.OnClientMessage += (data) => OnClientMessage?.Invoke(data);
                    handler.OnServerMessage += (data) =>
                    {
                        OnServerMessage?.Invoke(data);

                        if (!data.Contains("<valorant>")) return;

                        HandleValorantPresence(data);
                    };
                    handler.Initiate();
                    
                }
                catch (IOException ex)
                {
                    throw new Exception($"Client closed. {ex}");
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to initiate communication. {ex}");
                }
            }
        }

        public Process InitializeConnection(string patchLine = "live")
        {
            string riotClientPath = ValorantService.GetRiotClientPath();
            string valorantPath = ValorantService.GetValorantPath();
            if (IsRiotRunning()) throw new Exception("Riot/Valorant cannot be running.");
            if (!File.Exists(riotClientPath)) throw new Exception($"Riot Client executable not found: {riotClientPath}");
            if (!File.Exists(valorantPath)) throw new Exception($"Valorant executable not found: {valorantPath}");

            (TcpListener currentTcpListener, int currentPort) = NewTcpListener();

            InternalProxy proxyServer = new(currentPort);

            bool serverHooked = false;

            proxyServer.OnChatPatched += async (_, args) =>
            {
                if (serverHooked) return;
                serverHooked = true;
                await HandleClients(currentTcpListener, args.ChatHost, args.ChatPort);
            };

            ProcessStartInfo riotClientStartArgs = new()
            {
                FileName = riotClientPath,
                Arguments = $"--client-config-url=\"http://127.0.0.1:{proxyServer.ConfigPort}\" --launch-product=valorant --launch-patchline={patchLine}"
            };
            
            return Process.Start(riotClientStartArgs)!;
        }

        [GeneratedRegex("<p>(.*?)<\\/p>")]
        private static partial Regex ValorantPresenceRegex();

        [GeneratedRegex("(<presence.*<\\/presence>)")]
        private static partial Regex XmlPresenceUpdateRegex();

        [GeneratedRegex("<id name='([^']*)' tagline='(.{0,6})'\\/><(p|l|\\/item)")]
        private static partial Regex RiotDataRegex();

        [GeneratedRegex("<riot name='([^']*)' tagline='(.{0,6})'\\/>")]
        private static partial Regex PlatformsDataRegex();

        [GeneratedRegex("to='(.*)' from")]
        private static partial Regex ChatServerDataRegex();

        [GeneratedRegex("from='(.*)' id")]
        private static partial Regex LobbyServerDataRegex();
    }
}
