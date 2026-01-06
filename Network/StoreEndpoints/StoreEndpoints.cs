using RadiantConnect.Methods;
using RadiantConnect.Network.StoreEndpoints.DataTypes;
namespace RadiantConnect.Network.StoreEndpoints
{
	/// <summary>
	/// Provides access to endpoints related to the in-game store, player balances,
	/// and owned items.
	/// </summary>
	/// <remarks>
	/// This endpoint group exposes operations for retrieving the storefront,
	/// the player's currency balances, and items owned by the player.
	/// </remarks>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class StoreEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;
		/// <summary>
		/// Fetches the current storefront for the authenticated player.
		/// </summary>
		/// <returns>
		/// A <see cref="Storefront"/> instance containing store offers and metadata,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<Storefront?> FetchStorefrontAsync() => await initiator.ExternalSystem.Net.PostAsync<Storefront>(Url, $"/store/v3/storefront/{initiator.Client.UserId}", new StringContent("{}")).ConfigureAwait(false);
		
		/// <summary>
		/// Retrieves the current currency balances for the authenticated player.
		/// </summary>
		/// <returns>
		/// A <see cref="BalancesMain"/> instance containing the player's current
		/// in-game currency balances, or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<BalancesMain?> FetchBalancesAsync() => await initiator.ExternalSystem.Net.GetAsync<BalancesMain>(Url, $"/store/v1/wallet/{initiator.Client.UserId}").ConfigureAwait(false);
		
		/// <summary>
		/// Fetches a specific owned item by type for the authenticated player.
		/// </summary>
		/// <param name="type">The <see cref="ValorantTables.ItemType"/> to retrieve.</param>
		/// <returns>
		/// An <see cref="OwnedItem"/> instance representing the owned item of the
		/// specified type, or <c>null</c> if not owned or the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<OwnedItem?> FetchOwnedItemByTypeAsync(ValorantTables.ItemType type) => await initiator.ExternalSystem.Net.GetAsync<OwnedItem>(Url, $"/store/v1/entitlements/{initiator.Client.UserId}/{ValorantTables.ItemTypeToId[type]}").ConfigureAwait(false);
	}
}