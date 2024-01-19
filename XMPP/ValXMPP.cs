using RadiantConnect.Services;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
// Credit to https://github.com/molenzwiebel/Deceive for guide

namespace RadiantConnect.XMPP
{
    public class ValXMPP
    {
        public delegate void InternalMessage(string data);
        public delegate void PresenceUpdated(ValorantPresence presence);

        public event InternalMessage? OnClientMessage;
        public event InternalMessage? OnServerMessage;
        public event PresenceUpdated? OnValorantPresenceUpdated;

        public delegate void SocketHandled(SocketHandle handle);
        public event SocketHandled? OnSocketCreated;

        public static void KillRiot()
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process process in processes.Where(proc => proc.ProcessName is "Riot Client" or "VALORANT-Win64-Shipping" or "RiotClientServices")) process.Kill();
        }

        internal static bool IsRiotRunning()
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
                    
                    SocketHandle handler = new(incomingStream, outgoingStream);
                    OnSocketCreated?.Invoke(handler);
                    handler.OnClientMessage += (data) => OnClientMessage?.Invoke(data);
                    handler.OnServerMessage += (data) =>
                    {
                        OnServerMessage?.Invoke(data);

                        if (!data.Contains("<valorant>")) return;
                        Match match = Regex.Match(data, "<p>(.*)<\\/p>");
                        if (!match.Success) return;
                        string valorant64Data = match.Groups[1].Value;
                        byte[] decodedValorantBytes = Convert.FromBase64String(valorant64Data);
                        string decodedValorantData = Encoding.ASCII.GetString(decodedValorantBytes);
                        OnValorantPresenceUpdated?.Invoke(JsonSerializer.Deserialize<ValorantPresence>(decodedValorantData)!);
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
    }
}
