using RadiantConnect.EventHandler;
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
using RadiantConnect.Authentication.DriverRiotAuth.Records;

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

    public record GeoAffinities(
        [property: JsonPropertyName("pbe")] string Pbe,
        [property: JsonPropertyName("live")] string Live
    );

    public record GeoRoot(
        [property: JsonPropertyName("token")] string Token,
        [property: JsonPropertyName("affinities")] GeoAffinities Affinities
    );

    public class Initiator : IDisposable
    {
        internal static bool IsDisposed { get; private set; }
        
        private static string IsVpnDetected() => string.Join('|',
            Process.GetProcesses().Where(process =>
                process.ProcessName.Contains("vpn", StringComparison.CurrentCultureIgnoreCase)));

        public InternalSystem ExternalSystem { get; private set; } = null!;
        public Endpoints Endpoints { get; private set; } = null!;
        public GameEvents GameEvents { get; internal set; } = null!;
        public LogService.ClientData Client { get; private set; } = null!;

        private async Task<LogService.ClientData> BuildClientData(ValorantNet net, RSOAuth rsoAuth)
        {
            if (net == null) throw new RadiantConnectException("Failed to build net data");

            GeoRoot? data = await net.PutAsync<GeoRoot>(
                "https://riot-geo.pas.si.riotgames.com",
                "/pas/v1/product/valorant",
                new StringContent($"{{\"id_token\": \"{rsoAuth.IdToken}\"}}")
            );

            Enum.TryParse(data?.Affinities.Live, true, out LogService.ClientData.ShardType shard);

            JsonWebToken token = new(data?.Token);

            return new LogService.ClientData(
                Shard: shard,
                UserId: token.Subject,
                PdUrl: $"https://pd.{shard}.a.pvp.net",
                GlzUrl: $"https://glz-{data?.Affinities.Live}-1.{shard}.a.pvp.net",
                SharedUrl: $"https://shared.{shard}.a.pvp.net"
            );
        }

        private void Initialize(ValorantNet net, RSOAuth rsoAuth)
        {
            Client = BuildClientData(net, rsoAuth).Result;

            ExternalSystem = new InternalSystem(
                null!,
                net,
                null!,
                Client
            );

            Endpoints = new Endpoints(
                new ChatEndpoints(this),
                new ContractEndpoints(this),
                new CurrentGameEndpoints(this),
                null!,
                new PartyEndpoints(this),
                new PreGameEndpoints(this),
                new PVPEndpoints(this),
                new StoreEndpoints(this)
            );
        }
        
        public Initiator(RSOAuth rsoAuth)
        {
            ValorantNet net = new(rsoAuth);
            Initialize(net, rsoAuth);
        }

        public Initiator(bool ignoreVpn = true)
        {
#if !DEBUG
            DateTime startTime = DateTime.Now;
            TimeSpan timeout = TimeSpan.FromMinutes(1);

            while (!InternalValorantMethods.ClientIsReady())
            {
                if (DateTime.Now - startTime > timeout)
                    throw new TimeoutException("Client did not become ready within 1 minute.");

                Task.Delay(2000);
            }
#endif
            ValorantService client = new();
            LogService logService = new();
            LogService.ClientData cData = LogService.GetClientData();
            ValorantNet net = new(client);

            if (!ignoreVpn)
            {
	            string vpnDetected = IsVpnDetected();

	            if (!vpnDetected.IsNullOrEmpty())
		            throw new RadiantConnectException($"Can not run with VPN running, found processes: {vpnDetected}. \n\nTo bypass this check launch Initiator with (true)");
			}

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

        public void Dispose() => IsDisposed = true;
    }
}
