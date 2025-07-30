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
            Contract? contract = await Initiator.Endpoints.ContractEndpoints.GetContractsAsync(Initiator.Client.UserId);

            Assert.NotNull(contract);
            Assert.NotEmpty(contract.Contracts);
            Assert.NotEmpty(contract.Contracts[0].ContractDefinitionID);
        }
    }
}
