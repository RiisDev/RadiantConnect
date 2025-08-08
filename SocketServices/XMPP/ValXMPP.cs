﻿using RadiantConnect.Services;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using RadiantConnect.SocketServices.XMPP.DataTypes;
using RadiantConnect.SocketServices.XMPP.XMPPManagement;
#pragma warning disable SYSLIB0057

// Credit to https://github.com/molenzwiebel/Deceive for guide
// ReSharper disable CheckNamespace

namespace RadiantConnect.XMPP
{
    public partial class ValXMPP
    {
        public event Action? OnReady;

        public bool Ready
        {
            get;
            private set
            {
                OnReady?.Invoke();
                field = value;
            }
        } = false;

        internal string? StreamUrl { get; set; }

        public delegate void InternalMessage(string data);
        public delegate void PresenceUpdated(ValorantPresence presence);
        public delegate void PlayerPresenceUpdated(PlayerPresence presence);

        public event InternalMessage? OnClientMessage;
        public event InternalMessage? OnServerMessage;
        public event PresenceUpdated? OnValorantPresenceUpdated;
        public event PlayerPresenceUpdated? OnPlayerPresenceUpdated;

        public event InternalMessage? OnOutboundMessage;
        public event InternalMessage? OnInboundMessage;

        internal delegate void SocketHandled(XMPPSocketHandle handle);
        internal event SocketHandled? OnSocketCreated;

        internal XMPPSocketHandle Handle { get; private set; } = null!;

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

        internal static ValorantPresence? HandlePresenceObject(string data, Action<ValorantPresence>? presenceAction = null)
        {
            // Pull the <presence> XML out of the stream.
            // I do regex in case an error with the stream includes extra data
            Match match = ValorantPresenceRegex().Match(data);
            if (!match.Success) return null;

            string valorant64Data = match.Groups[1].Value;
            ValorantPresence? presenceData = JsonSerializer.Deserialize<ValorantPresence>(valorant64Data.FromBase64());

            if (presenceData is not null)
                presenceAction?.Invoke(presenceData);

            return presenceData;
        }

        internal static void HandlePlayerPresence(string data, Action<PlayerPresence>? action = null)
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

                action?.Invoke(new PlayerPresence(
                    chatServer,
                    lobbyServer,
                    "riot",
                    riotId,
                    tagLine,
                    lobbyServer[(lobbyServer.IndexOf('/') + 1)..],
                    platforms,
                    HandlePresenceObject(newData)!
                ));
            }
        }

        internal void HandleValorantPresence(string data)
        {
            try
            {
                if (OnValorantPresenceUpdated is not null)
                    HandlePresenceObject(data, presenceData => OnValorantPresenceUpdated?.Invoke(presenceData));
                if (OnPlayerPresenceUpdated is not null)
                    HandlePlayerPresence(data, presenceData => OnPlayerPresenceUpdated?.Invoke(presenceData));
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
                        catch (Exception ex) { throw new RadiantConnectXMPPException($"Unable to communicate with chat client. {ex}"); }
                    }

                    SslStream outgoingStream = new(outgoingClient.GetStream());
                    await outgoingStream.AuthenticateAsClientAsync(chatHost);

                    XMPPSocketHandle handler = new(incomingStream, outgoingStream);
                    OnSocketCreated?.Invoke(handler);
                    Handle = handler;
                    handler.OnClientMessage += (data) => OnClientMessage?.Invoke(data);
                    handler.OnServerMessage += (data) =>
                    {
                        OnServerMessage?.Invoke(data);

                        if (!data.Contains("<valorant>")) return;

                        HandleValorantPresence(data);
                    };
                    handler.Initiate();
                    Ready = true;
                }
                catch (IOException ex)
                {
                    throw new RadiantConnectXMPPException($"Client closed. {ex}");
                }
                catch (Exception ex)
                {
                    throw new RadiantConnectXMPPException($"Failed to initiate communication. {ex}");
                }
            }
        }

        public Process InitializeConnection(string patchLine = "live")
        {
            string riotClientPath = RiotPathService.GetRiotClientPath();
            string valorantPath = RiotPathService.GetValorantPath();
            if (IsRiotRunning()) throw new RadiantConnectXMPPException("Riot/Valorant cannot be running.");
            if (!File.Exists(riotClientPath)) throw new RadiantConnectXMPPException($"Riot Client executable not found: {riotClientPath}");
            if (!File.Exists(valorantPath)) throw new RadiantConnectXMPPException($"Valorant executable not found: {valorantPath}");

            (TcpListener currentTcpListener, int currentPort) = NewTcpListener();

            InternalProxy proxyServer = new(currentPort);

            bool serverHooked = false;

            proxyServer.OnChatPatched += async (_, args) =>
            {
                if (serverHooked) return;
                serverHooked = true;
                StreamUrl = args.ChatAffinity;
                await HandleClients(currentTcpListener, args.ChatHost, args.ChatPort);
            };

            proxyServer.OnOutboundMessage += data => OnOutboundMessage?.Invoke(data);
            proxyServer.OnInboundMessage += data => OnInboundMessage?.Invoke(data);

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
