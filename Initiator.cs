using RadiantConnect.LogManager;
using RadiantConnect.Methods;
using RadiantConnect.Network;
using RadiantConnect.Services;

namespace RadiantConnect
{
    public record InternalSystem(ValorantService ValorantClient, LogParser LogManager, ValorantNet Net, ClientData ClientData);

    public class Initiator
    {
        private static InternalSystem _internalSystem = null!;

        public static InternalSystem InternalSystem
        {
            get
            {
                if (_internalSystem == null) throw new InvalidOperationException("InternalSystem is not initialized. Please invoke the Initiator() before accessing it.");
                return _internalSystem;
            }
        }

        public Initiator()
        {
            ValorantService client = new();
            LogParser logParser = new();
            ValorantNet net = new (client);
            ClientData cData = LogParser.GetClientData();

            _internalSystem = new InternalSystem(
                client,
                logParser,
                net,
                cData
            );
        }
    }
}
