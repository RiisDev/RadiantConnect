
using RadiantConnect.Network.ContractEndpoints.DataTypes;

namespace RadiantConnect.Network.ContractEndpoints
{
	public class ContractEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		public async Task<ItemUpgrade?> GetItemUpgradesAsync() => await initiator.ExternalSystem.Net.GetAsync<ItemUpgrade>(Url, "/contract-definitions/v3/item-upgrades");

		public async Task<object?> GetContractsAsync() => await initiator.ExternalSystem.Net.GetAsync<object?>(Url, $"/contracts/v1/contracts/{initiator.Client.UserId}");
	}
}