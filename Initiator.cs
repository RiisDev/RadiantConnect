using RadiantConnect.LogManager;
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

namespace RadiantConnect
{
    public record InternalSystem(
        ValorantService ValorantClient,
        ValorantNet Net,
        LogParser LogManager, 
        ClientData ClientData
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
        public InternalSystem ExternalSystem { get; }

        public Endpoints Endpoints { get; }

        public Initiator()
        {
            ValorantService client = new();
            LogParser logParser = new();
            ValorantNet net = new(client);
            ClientData cData = LogParser.GetClientData();

            ExternalSystem = new InternalSystem(
                client,
                net,
                logParser,
                cData
            );

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
        }
    }
}
