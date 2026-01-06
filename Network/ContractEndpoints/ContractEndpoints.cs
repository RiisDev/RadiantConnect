
using RadiantConnect.Network.ContractEndpoints.DataTypes;

namespace RadiantConnect.Network.ContractEndpoints
{
	/// <summary>
	/// Provides access to contract and progression related service endpoints.
	/// </summary>
	/// <remarks>
	/// This endpoint group exposes operations for retrieving player contracts,
	/// item upgrades, and progression metadata.
	/// </remarks>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing
	/// authentication, networking, and client context.
	/// </param>
	public class ContractEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		/// <summary>
		/// Retrieves all available item upgrade definitions.
		/// </summary>
		/// <returns>
		/// An <see cref="ItemUpgrade"/> instance containing item upgrade metadata,
		/// or <c>null</c> if the request fails or no data is returned.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network errors.
		/// </exception>
		/// <remarks>
		/// This endpoint is typically used to resolve upgrade paths for contracts,
		/// battle passes, and other progression systems.
		/// </remarks>
		public async Task<ItemUpgrade?> GetItemUpgradesAsync() => await initiator.ExternalSystem.Net.GetAsync<ItemUpgrade>(Url, "/contract-definitions/v3/item-upgrades").ConfigureAwait(false);
		
		/// <summary>
		/// Retrieves the current player's active and completed contracts.
		/// </summary>
		/// <returns>
		/// A raw contracts payload returned by the service,
		/// or <c>null</c> if the request fails.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network issues.
		/// </exception>
		/// <remarks>
		/// This method intentionally returns an untyped object to allow
		/// forward compatibility with evolving contract schemas.
		/// </remarks>
		public async Task<object?> GetContractsAsync() => await initiator.ExternalSystem.Net.GetAsync<object?>(Url, $"/contracts/v1/contracts/{initiator.Client.UserId}").ConfigureAwait(false);
	}
}