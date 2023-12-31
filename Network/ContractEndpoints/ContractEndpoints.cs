
using RadiantConnect.Network.ContractEndpoints.DataTypes;

namespace RadiantConnect.Network.ContractEndpoints;

public class ContractEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

    public async Task<ItemUpgrade?> GetItemUpgradesAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<ItemUpgrade>(Url, "/contract-definitions/v3/item-upgrades");
    }

    public async Task<Contract?> GetContractsAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<Contract?>(Url, $"/contracts/v1/contracts/{userId}");
    }
}