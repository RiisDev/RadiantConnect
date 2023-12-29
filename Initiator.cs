using RadiantConnect.EventHandler;
using RadiantConnect.Methods;
using RadiantConnect.Services;
using RadiantConnect.Network;
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
        internal static async void WaitTillReady()
        {
            while (true)
            {
                if (!InternalValorantMethods.IsValorantProcessOpened()) continue;
                if (!Directory.Exists(Path.GetFullPath(LogService.GetLogPath()))) continue;
                if (!File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "Riot Client", "Config", "lockfile"))) continue;
                if (!File.Exists(LogService.GetLogPath())) continue;
                if (!LogService.GetLogText().Split('\n').Reverse().Last().Contains("Log file closed")) break;
                await Task.Delay(500);
            }
        }

        public InternalSystem ExternalSystem { get; }
        public Endpoints Endpoints { get; }
        public GameEvents GameEvents { get; }

        public Initiator()
        {
            WaitTillReady();
            ValorantService client = new();
            LogService logService = new();
            ValorantNet net = new(client);
            LogService.ClientData cData = LogService.GetClientData();

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
            
            GameEvents = LogService.InitiateEvents(this).Result;
        }
    }
}
