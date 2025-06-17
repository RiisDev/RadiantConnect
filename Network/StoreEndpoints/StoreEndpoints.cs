using RadiantConnect.Methods;
using RadiantConnect.Network.StoreEndpoints.DataTypes;
namespace RadiantConnect.Network.StoreEndpoints;

public class StoreEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.PdUrl;
    
    public async Task<Storefront?> FetchStorefrontAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Storefront>(Url, $"/store/v3/storefront/{userId}", new StringContent("{}"));
    }

    public async Task<BalancesMain?> FetchBalancesAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<BalancesMain>(Url, $"/store/v1/wallet/{userId}");
    }

    public async Task<OwnedItem?> FetchOwnedItemByTypeAsync(ValorantTables.ItemType type, string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<OwnedItem>(Url, $"/store/v1/entitlements/{userId}/{ValorantTables.ItemTypeToId[type]}");
    }
}