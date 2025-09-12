using RadiantConnect.Network.ContractEndpoints.DataTypes;

namespace RadiantConnect.Tests.NetworkBulk
{
    public class ContractEndpointsTests
    {
        private static readonly Initiator Initiator = new();

        [Fact]
        public async Task GetItemUpgrades()
        {
            if (!Client.MachineReady()) return;

            ItemUpgrade? itemUpgrade = await Initiator.Endpoints.ContractEndpoints.GetItemUpgradesAsync();

            Assert.NotNull(itemUpgrade);
            Assert.NotEmpty(itemUpgrade.Definitions);
        }

        [Fact]
        public async Task GetContracts()
        {
            if (!Client.MachineReady()) return;
            object? contract = await Initiator.Endpoints.ContractEndpoints.GetContractsAsync();

            Assert.NotNull(contract);
        }
    }
}
