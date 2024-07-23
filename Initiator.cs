using RadiantConnect.EventHandler;
using RadiantConnect.Methods;
using RadiantConnect.Services;
using RadiantConnect.Network;
using RadiantConnect.Network.ChatEndpoints;
using RadiantConnect.Network.ContractEndpoints;
using RadiantConnect.Network.CurrentGameEndpoints;
using RadiantConnect.Network.LocalEndpoints;
using RadiantConnect.Network.PartyEndpoints;
using RadiantConnect.Network.PreGameEndpoints;
using RadiantConnect.Network.PVPEndpoints;
using RadiantConnect.Network.StoreEndpoints;
using System.Diagnostics;

namespace RadiantConnect
{
    public record InternalSystem(
        ValorantService ValorantClient,
        ValorantNet Net,
        LogService LogService,
        LogService.ClientData ClientData
    );

    public record Endpoints(
        ChatEndpoints ChatEndpoints,
        ContractEndpoints ContractEndpoints,
        CurrentGameEndpoints CurrentGameEndpoints,
        LocalEndpoints LocalEndpoints,
        PartyEndpoints PartyEndpoints,
        PreGameEndpoints PreGameEndpoints,
        PVPEndpoints PvpEndpoints,
        StoreEndpoints StoreEndpoints
    );

    public class Initiator
    {
        internal static bool ClientIsReady() =>
            InternalValorantMethods.IsValorantProcessRunning() &&
            Directory.Exists(Path.GetDirectoryName(LogService.GetLogPath())) &&
            File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
                "Local", "Riot Games", "Riot Client", "Config", "lockfile")) &&
            File.Exists(LogService.GetLogPath()) &&
            !LogService.GetLogText().Split('\n').Last().Contains("Log file closed");

        internal static string IsVpnDetected() => string.Join('|', Process.GetProcesses().Where(process => process.ProcessName.Contains("vpn", StringComparison.CurrentCultureIgnoreCase)));

        public InternalSystem ExternalSystem { get; }
        public Endpoints Endpoints { get; }
        public GameEvents GameEvents { get; set; } = null!;
        public LogService.ClientData Client { get; }

        public Initiator(bool ignoreVpn = true, SuppliedAuth? suppliedAuth = null)
        {
            while (!ClientIsReady())
            {
                Debug.WriteLine("[INITIATOR] Waiting For Client...");
                Task.Delay(2000);
            }
            ValorantService client = new();
            LogService logService = new();
            LogService.ClientData cData = LogService.GetClientData();
            ValorantNet net = new(client, suppliedAuth);

            string vpnDetected = IsVpnDetected();

            if (!string.IsNullOrEmpty(vpnDetected) && !ignoreVpn)
                throw new RadiantConnectException($"Can not run with VPN running, found processes: {vpnDetected}. \n\nTo bypass this check launch Initiator with (true)");

            ExternalSystem = new InternalSystem(
                client,
                net,
                logService,
                cData
            );

            Client = cData;

            Endpoints = new Endpoints(
                new ChatEndpoints(this),
                new ContractEndpoints(this),
                new CurrentGameEndpoints(this),
                new LocalEndpoints(this),
                new PartyEndpoints(this),
                new PreGameEndpoints(this),
                new PVPEndpoints(this),
                new StoreEndpoints(this)
            );
            
            _ = LogService.InitiateEvents(this);
        }
    }
}
