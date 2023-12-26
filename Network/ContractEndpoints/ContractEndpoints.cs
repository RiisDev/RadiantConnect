
using RadiantConnect.Network.ContractEndpoints.DataTypes;

namespace RadiantConnect.Network.ContractEndpoints;

public class ContractEndpoints
{
    internal static string Url = Initiator.InternalSystem.ClientData.PdUrl;

    public static async Task<ItemUpgrade?> GetItemUpgrades()
    {
        return await ValorantNet.GetAsync<ItemUpgrade>(Url, "/contract-definitions/v3/item-upgrades");
    }

    public static async Task<Contract?> GetContracts(string userId)
    {
        return await ValorantNet.GetAsync<Contract?>(Url, $"/contracts/v1/contracts/{userId}");
    }
}