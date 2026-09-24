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

		/// <summary>
		/// Fetches every owned item across all item types for the authenticated player asynchronously.
		/// </summary>
		/// <returns>
		/// An <see cref="AllOwnedItems"/> instance containing all owned items grouped by type,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<AllOwnedItems?> FetchAllOwnedItemsAsync() => await initiator.ExternalSystem.Net.GetAsync<AllOwnedItems>(Url, $"/store/v1/entitlements/{initiator.Client.UserId}").ConfigureAwait(false);

		/// <summary>
		/// Places a store order for the specified offer asynchronously.
		/// </summary>
		/// <param name="offerId">The ID of the offer to purchase.</param>
		/// <returns>
		/// An <see cref="Order"/> instance describing the created order,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<Order?> CreateOrderAsync(string offerId) => await initiator.ExternalSystem.Net.PostAsync<Order>(Url, "/store/v1/order/", JsonContent.Create(new { XID = initiator.Client.UserId, OfferID = offerId })).ConfigureAwait(false);

		/// <summary>
		/// Fetches the status and rewards of a previously placed order asynchronously.
		/// </summary>
		/// <param name="orderId">The ID of the order returned by <see cref="CreateOrderAsync"/>.</param>
		/// <returns>
		/// An <see cref="Order"/> instance describing the order, or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<Order?> FetchOrderAsync(string orderId) => await initiator.ExternalSystem.Net.GetAsync<Order>(Url, $"/store/v1/order/{orderId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches whether the authenticated player is eligible to purchase gifts asynchronously.
		/// </summary>
		/// <returns>
		/// A <see cref="GiftEligibility"/> instance describing purchaser eligibility,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<GiftEligibility?> FetchGiftPurchaserEligibilityAsync() => await initiator.ExternalSystem.Net.GetAsync<GiftEligibility>(Url, $"/store/v1/gifts/{initiator.Client.UserId}/purchasereligibility").ConfigureAwait(false);

		/// <summary>
		/// Fetches whether a specific player is eligible to receive gifts from the authenticated player asynchronously.
		/// </summary>
		/// <param name="recipientUserId">The ID of the intended gift recipient.</param>
		/// <returns>
		/// A <see cref="GiftRecipientEligibility"/> instance describing recipient eligibility,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<GiftRecipientEligibility?> FetchGiftRecipientEligibilityAsync(string recipientUserId) => await initiator.ExternalSystem.Net.PostAsync<GiftRecipientEligibility>(Url, $"/store/v1/gifts/{initiator.Client.UserId}/eligibility/{recipientUserId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the current featured agent storefront asynchronously.
		/// </summary>
		/// <returns>
		/// An <see cref="AgentStorefront"/> instance describing the agent store offers,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		public async Task<AgentStorefront?> FetchAgentStorefrontAsync() => await initiator.ExternalSystem.Net.GetAsync<AgentStorefront>(Url, "/store/v1/storefronts/agent").ConfigureAwait(false);
	}
}