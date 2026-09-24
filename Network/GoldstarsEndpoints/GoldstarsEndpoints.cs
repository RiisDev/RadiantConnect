using RadiantConnect.Network.GoldstarsEndpoints.DataTypes;

namespace RadiantConnect.Network.GoldstarsEndpoints
{
	/// <summary>
	/// Provides access to goldstar (in-game commendation) definitions and player goldstar history.
	/// </summary>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class GoldstarsEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		/// <summary>
		/// Fetches all goldstar definitions.
		/// </summary>
		/// <returns>
		/// A list of <see cref="Goldstar"/> instances, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<IReadOnlyList<Goldstar>?> FetchGoldstarsAsync() =>
			await initiator.ExternalSystem.Net.GetAsync<IReadOnlyList<Goldstar>>(Url, "goldstars/v1/goldstars").ConfigureAwait(false);

		/// <summary>
		/// Fetches the authenticated player's goldstar history.
		/// </summary>
		/// <returns>
		/// A <see cref="PlayerGoldstars"/> instance, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<PlayerGoldstars?> FetchPlayerGoldstarsAsync() =>
			await initiator.ExternalSystem.Net.GetAsync<PlayerGoldstars>(Url, $"goldstars/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);
	}
}
