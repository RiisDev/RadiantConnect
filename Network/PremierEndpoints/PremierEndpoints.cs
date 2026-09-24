using RadiantConnect.Network.PremierEndpoints.DataTypes;
using RadiantConnect.Services;

namespace RadiantConnect.Network.PremierEndpoints
{
	/// <summary>
	/// Provides access to Premier-related service endpoints, including roster management,
	/// season/conference metadata, and player eligibility.
	/// </summary>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class PremierEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		/// <summary>
		/// Fetches the Premier profile for the current authenticated player.
		/// </summary>
		/// <returns>A <see cref="PremierPlayer"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierPlayer?> FetchPremierPlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PremierPlayer>(Url, $"premier/v2/players/{initiator.Client.UserId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the Premier crests earned by the current player across seasons.
		/// </summary>
		/// <returns>A <see cref="PremierPlayerCrests"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierPlayerCrests?> FetchPremierPlayerCrestsAsync() => await initiator.ExternalSystem.Net.GetAsync<PremierPlayerCrests>(Url, $"premier/v2/players/{initiator.Client.UserId}/crests").ConfigureAwait(false);

		/// <summary>
		/// Fetches the current player's eligibility to participate in Premier.
		/// </summary>
		/// <returns>A <see cref="PremierEligibility"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierEligibility?> FetchPremierEligibilityAsync() => await initiator.ExternalSystem.Net.GetAsync<PremierEligibility>(Url, "premier/v1/player/eligibility").ConfigureAwait(false);

		/// <summary>
		/// Fetches all Premier seasons for the given affinity/region.
		/// </summary>
		/// <param name="affinity">The regional affinity to fetch seasons for.</param>
		/// <returns>A <see cref="PremierSeasons"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierSeasons?> FetchPremierSeasonsAsync(LogService.ClientData.ShardType affinity) => await initiator.ExternalSystem.Net.GetAsync<PremierSeasons>(Url, $"premier/v1/affinities/{affinity}/premier-seasons").ConfigureAwait(false);

		/// <summary>
		/// Fetches the currently active Premier season for the given affinity/region.
		/// </summary>
		/// <param name="affinity">The regional affinity to fetch the active season for.</param>
		/// <returns>A <see cref="PremierSeason"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierSeason?> FetchActivePremierSeasonAsync(LogService.ClientData.ShardType affinity) => await initiator.ExternalSystem.Net.GetAsync<PremierSeason>(Url, $"premier/v1/affinities/{affinity}/premier-seasons/active").ConfigureAwait(false);

		/// <summary>
		/// Fetches the Premier conferences available for the given affinity/region.
		/// </summary>
		/// <param name="affinity">The regional affinity to fetch conferences for.</param>
		/// <returns>A <see cref="PremierConferencesRoot"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierConferencesRoot?> FetchPremierConferencesAsync(LogService.ClientData.ShardType affinity) => await initiator.ExternalSystem.Net.GetAsync<PremierConferencesRoot>(Url, $"premier/v1/affinities/{affinity}/conferences").ConfigureAwait(false);

		/// <summary>
		/// Fetches a Premier roster by its ID.
		/// </summary>
		/// <param name="rosterId">The ID of the roster to fetch.</param>
		/// <returns>A <see cref="PremierRoster"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierRoster?> FetchRosterAsync(string rosterId) => await initiator.ExternalSystem.Net.GetAsync<PremierRoster>(Url, $"premier/v2/rosters/{rosterId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the match history for a Premier roster.
		/// </summary>
		/// <param name="rosterId">The ID of the roster to fetch match history for.</param>
		/// <returns>A <see cref="PremierRosterMatchHistory"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierRosterMatchHistory?> FetchRosterMatchHistoryAsync(string rosterId) => await initiator.ExternalSystem.Net.GetAsync<PremierRosterMatchHistory>(Url, $"premier/v1/rosters/{rosterId}/matchhistory").ConfigureAwait(false);

		/// <summary>
		/// Fetches the public-facing info for a Premier roster.
		/// </summary>
		/// <param name="rosterId">The ID of the roster to fetch public info for.</param>
		/// <returns>A <see cref="PremierRosterPublicInfo"/> instance if available; otherwise, <c>null</c>.</returns>
		public async Task<PremierRosterPublicInfo?> FetchRosterPublicInfoAsync(string rosterId) => await initiator.ExternalSystem.Net.GetAsync<PremierRosterPublicInfo>(Url, $"premier/v2/rosters/{rosterId}/publicInfo").ConfigureAwait(false);

		/// <summary>
		/// Updates the customization (icon and colors) for a Premier roster.
		/// </summary>
		/// <param name="rosterId">The ID of the roster to customize.</param>
		/// <param name="customization">The new customization data to apply.</param>
		/// <returns>
		/// The raw response payload, or <c>null</c> if the request fails.
		/// </returns>
		/// <remarks>
		/// This method intentionally returns an untyped object since Riot's response
		/// shape for this endpoint is undocumented/unstable.
		/// </remarks>
		public async Task<object?> SetRosterCustomizationAsync(string rosterId, RosterCustomization customization) => await initiator.ExternalSystem.Net.PutAsync<object>(Url, $"premier/v1/rosters/{rosterId}/customization", JsonContent.Create(customization)).ConfigureAwait(false);
	}
}
