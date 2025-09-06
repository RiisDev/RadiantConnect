using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network.ContractEndpoints.DataTypes;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.Network.StoreEndpoints.DataTypes;
using RadiantConnect.Services;
using static Xunit.Assert;

namespace RadiantConnect.Tests
{
    public class EndpointsTest
    {
        public static bool MachineEligible(bool valorantRequired = true) => Client.MachineReady(valorantRequired);

        private readonly Lock _lock = new();
        private Task<Initiator>? _initTask;
        private Task<Initiator> GetInitiatorAsync()
        {
            lock (_lock)
            {
                return _initTask ??= InitAsync();
            }
        }
        private static async Task<Initiator> InitAsync()
        {
            Authentication.Authentication auth = new();
            RSOAuth? rso = await auth.AuthenticateWithQr(Authentication.Authentication.CountryCode.Na);
            if (rso is null)
                throw new Exception("Failed to authenticate with RSO.");
            return new Initiator(rso);
        }
        
        [Fact]
        public async Task TestPvpEndpoints()
        {
            if (!MachineEligible()) return;
            Initiator initiator = await GetInitiatorAsync();

            NotNull(initiator);
            NotNull(initiator.Endpoints.PvpEndpoints);

            AccountXP? accountXp = await initiator.Endpoints.PvpEndpoints.FetchAccountXPAsync();

            NotNull(accountXp);
            False(string.IsNullOrEmpty(accountXp.Subject));
        }

        [Fact]
        public async Task TestStoreEndpoints()
        {
            if (!MachineEligible()) return;
            Initiator initiator = await GetInitiatorAsync();
            NotNull(initiator);
            NotNull(initiator.Endpoints.StoreEndpoints);

            BalancesMain? walletData = await initiator.Endpoints.StoreEndpoints.FetchBalancesAsync();

            NotNull(walletData);
            NotNull(walletData.Balances);
            NotNull(walletData.Balances.Radianite);
        }

    }
}
