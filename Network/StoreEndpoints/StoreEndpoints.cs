using RadiantConnect.Methods;
using RadiantConnect.Network.StoreEndpoints.DataTypes;
namespace RadiantConnect.Network.StoreEndpoints
{
	public class StoreEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;
    
		public async Task<Storefront?> FetchStorefrontAsync() => await initiator.ExternalSystem.Net.PostAsync<Storefront>(Url, $"/store/v3/storefront/{initiator.Client.UserId}", new StringContent("{}"));

		public async Task<BalancesMain?> FetchBalancesAsync() => await initiator.ExternalSystem.Net.GetAsync<BalancesMain>(Url, $"/store/v1/wallet/{initiator.Client.UserId}");

		public async Task<OwnedItem?> FetchOwnedItemByTypeAsync(ValorantTables.ItemType type) => await initiator.ExternalSystem.Net.GetAsync<OwnedItem>(Url, $"/store/v1/entitlements/{initiator.Client.UserId}/{ValorantTables.ItemTypeToId[type]}");
	}
}