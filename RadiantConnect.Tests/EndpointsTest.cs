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
            RSOAuth? rso = await auth.AuthenticateWithQr(Authentication.Authentication.CountryCode.NA);
            if (rso is null)
                throw new Exception("Failed to authenticate with RSO.");
            return new Initiator(rso);
        }

        public static bool MachineEligible(bool valorantRequired = true)
        {
            string valorantPath = RiotPathService.GetValorantPath();

            return (File.Exists(valorantPath) && valorantRequired)
                   || Environment.UserName == "irisd"; // I want to be able to run these tests on my machine always.
        }

        [Fact]
        public async Task TestPvpEndpoints()
        {
            if (!MachineEligible()) return;
            Initiator initiator = await GetInitiatorAsync();

            NotNull(initiator);
            NotNull(initiator.Endpoints.PvpEndpoints);

            AccountXP? accountXp = await initiator.Endpoints.PvpEndpoints.FetchAccountXPAsync(initiator.Client.UserId);

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

            BalancesMain? walletData = await initiator.Endpoints.StoreEndpoints.FetchBalancesAsync(initiator.Client.UserId);

            NotNull(walletData);
            NotNull(walletData.Balances);
            NotNull(walletData.Balances.Radianite);
        }

    }
}
