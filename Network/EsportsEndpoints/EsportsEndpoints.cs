using RadiantConnect.Network.EsportsEndpoints.DataTypes;

namespace RadiantConnect.Network.EsportsEndpoints
{
	/// <summary>
	/// Provides access to esports-related service endpoints, such as league schedules and match results.
	/// </summary>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class EsportsEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		/// <summary>
		/// Fetches upcoming esports matches for the given leagues.
		/// </summary>
		/// <param name="leagueIds">A comma-separated list of league IDs.</param>
		/// <param name="locale">An optional locale (e.g. "en-US").</param>
		/// <param name="sport">The sport identifier, always "val" for Valorant.</param>
		/// <returns>
		/// An <see cref="UpcomingMatches"/> instance containing leagues, tournaments, and teams,
		/// or <c>null</c> if the request fails.
		/// </returns>
		public async Task<UpcomingMatches?> FetchUpcomingMatchesAsync(string leagueIds, string? locale = null, string sport = "val") =>
			await initiator.ExternalSystem.Net.GetAsync<UpcomingMatches>(Url, $"esports-service/v2/upcomingMatches?leagueID={leagueIds}&locale={locale}&sport={sport}").ConfigureAwait(false);

		/// <summary>
		/// Fetches esports match details for the given match IDs.
		/// </summary>
		/// <param name="matchIds">The match IDs to fetch.</param>
		/// <param name="locale">An optional locale (e.g. "en-US").</param>
		/// <param name="sport">The sport identifier, always "val" for Valorant.</param>
		/// <returns>
		/// An <see cref="EsportsMatches"/> instance containing match details, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<EsportsMatches?> FetchMatchesAsync(IReadOnlyList<string> matchIds, string? locale = null, string sport = "val") =>
			await initiator.ExternalSystem.Net.PostAsync<EsportsMatches>(Url, $"esports-service/v2/matches?locale={locale}&sport={sport}", JsonContent.Create(new { MATCHIDS = matchIds })).ConfigureAwait(false);
	}
}
