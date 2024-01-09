using RadiantConnect.EventHandler;
using RadiantConnect.Methods;
using RadiantConnect.Services;
using RadiantConnect.Network;
using RadiantConnect.Network.Authorization;
using RadiantConnect.Network.Authorization.DataTypes;
using RadiantConnect.Network.ContractEndpoints;
using RadiantConnect.Network.CurrentGameEndpoints;
using RadiantConnect.Network.LocalEndpoints;
using RadiantConnect.Network.PartyEndpoints;
using RadiantConnect.Network.PreGameEndpoints;
using RadiantConnect.Network.PVPEndpoints;
using RadiantConnect.Network.StoreEndpoints;

namespace RadiantConnect
{
    public record InternalSystem(
        ValorantService ValorantClient,
        ValorantNet Net,
        LogService LogService,
        LogService.ClientData ClientData
    );


    public record Endpoints(
        //ChatEndpoints ChatEndpoints,
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
        internal static bool ClientIsReady()
        {
            return InternalValorantMethods.IsValorantProcessRunning() &&
                   Directory.Exists(Path.GetDirectoryName(LogService.GetLogPath())) &&
                   File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
                       "Local", "Riot Games", "Riot Client", "Config", "lockfile")) &&
                   File.Exists(LogService.GetLogPath()) &&
                   !LogService.GetLogText().Split('\n').Last().Contains("Log file closed");
        }

        public InternalSystem ExternalSystem { get; }
        public Endpoints Endpoints { get; }
        public GameEvents GameEvents { get; set; } = null!;

        public Initiator(InternalAuth? internalAuth = null)
        {
            while (!ClientIsReady()) Task.Delay(500);
            ValorantNet? net;
            ValorantService client = new();
            LogService logService = new();
            LogService.ClientData cData = LogService.GetClientData();
            if (internalAuth is null)
                net = new(client, null);
            else
                net = new(client, null);

            ExternalSystem = new InternalSystem(
                client,
                net,
                logService,
                cData
            );

            Endpoints = new Endpoints(
               //new ChatEndpoints(this),
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
