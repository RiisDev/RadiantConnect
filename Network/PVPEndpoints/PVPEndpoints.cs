using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.Services;
using System.Net.Http.Headers;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints
{
	/// <summary>
	/// Provides PVP-related API endpoints and operations.
	/// </summary>
	/// <param name="initiator">The initiator object used for API calls.</param>
	public class PVPEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		/// <summary>
		/// Fetches general content information asynchronously.
		/// </summary>
		/// <returns>A <see cref="Content"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<Content?> FetchContentAsync() => await initiator.ExternalSystem.Net.GetAsync<Content>(initiator.ExternalSystem.ClientData.SharedUrl, $"content-service/v3/content").ConfigureAwait(false);

		/// <summary>
		/// Fetches the account XP information for the current user asynchronously.
		/// </summary>
		/// <returns>An <see cref="AccountXP"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<AccountXP?> FetchAccountXPAsync() => await initiator.ExternalSystem.Net.GetAsync<AccountXP>(Url, $"account-xp/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the player's loadout asynchronously.
		/// </summary>
		/// <returns>A <see cref="PlayerLoadout"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<PlayerLoadout?> FetchPlayerLoadoutAsync() => await initiator.ExternalSystem.Net.GetAsync<PlayerLoadout>(Url, $"personalization/v2/players/{initiator.Client.UserId}/playerloadout").ConfigureAwait(false);

		/// <summary>
		/// Fetches the player's MMR (Matchmaking Rating) asynchronously.
		/// </summary>
		/// <param name="userId">The ID of the user whose MMR is being fetched.</param>
		/// <returns>A <see cref="PlayerMMR"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<PlayerMMR?> FetchPlayerMMRAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<PlayerMMR>(Url, $"mmr/v1/players/{userId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the match history for a player asynchronously.
		/// </summary>
		/// <param name="userId">The ID of the user whose match history is being fetched.</param>
		/// <returns>A <see cref="MatchHistory"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the match history for a player filtered by a specific queue asynchronously.
		/// </summary>
		/// <param name="userId">The ID of the user whose match history is being fetched.</param>
		/// <param name="queueId">The queue ID to filter matches.</param>
		/// <returns>A <see cref="MatchHistory"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<MatchHistory?> FetchPlayerMatchHistoryByQueueIdAsync(string userId, string queueId) => await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}?startIndex=0&endIndex=20&queue={queueId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches detailed information for a specific match asynchronously.
		/// </summary>
		/// <param name="matchId">The ID of the match to fetch.</param>
		/// <returns>A <see cref="MatchInfo"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<MatchInfo?> FetchMatchInfoAsync(string matchId) => await initiator.ExternalSystem.Net.GetAsync<MatchInfo>(Url, $"match-details/v1/matches/{matchId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches competitive updates for a player asynchronously.
		/// </summary>
		/// <param name="userId">The ID of the user to fetch competitive updates for.</param>
		/// <returns>A <see cref="CompetitiveUpdate"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<CompetitiveUpdate?> FetchCompetitveUpdatesAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<CompetitiveUpdate>(Url, $"mmr/v1/players/{userId}/competitiveupdates?startIndex=0&endIndex=20&queue=competitive").ConfigureAwait(false);

		/// <summary>
		/// Fetches the leaderboard for a specific season asynchronously.
		/// </summary>
		/// <param name="seasonId">The ID of the season to fetch the leaderboard for.</param>
		/// <returns>A <see cref="Leaderboard"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<Leaderboard?> FetchLeaderboardAsync(string seasonId) => await initiator.ExternalSystem.Net.GetAsync<Leaderboard>(Url, $"mmr/v1/leaderboards/affinity/na/queue/competitive/season/{seasonId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches penalties asynchronously.
		/// </summary>
		/// <returns>A <see cref="Penalty"/> object if available; otherwise, <c>null</c>.</returns>
		public async Task<Penalty?> FetchPenaltiesAsync() => await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, "restrictions/v3/penalties").ConfigureAwait(false);

		/// <summary>
		/// Fetches the client configuration asynchronously.
		/// </summary>
		/// <param name="shard">The shard type to fetch client configuration for.</param>
		/// <returns>A <see cref="Penalty"/> object representing client configuration if available; otherwise, <c>null</c>.</returns>
		public async Task<Penalty?> FetchClientConfigAsync(LogService.ClientData.ShardType shard) => await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, $"v1/config/{shard}").ConfigureAwait(false);

		/// <summary>
		/// Updates the player's loadout asynchronously.
		/// </summary>
		/// <param name="newLoadout">The new loadout data to set for the player.</param>
		/// <returns>A <see cref="PlayerLoadout"/> object reflecting the updated loadout if successful; otherwise, <c>null</c>.</returns>
		public async Task<PlayerLoadout?> SetPlayerLoadoutAsync(SetPlayerLoadout newLoadout) => await initiator.ExternalSystem.Net.PutAsync<PlayerLoadout>(Url, $"personalization/v2/players/{initiator.Client.UserId}/playerloadout", JsonContent.Create(newLoadout)).ConfigureAwait(false);

		/// <summary>
		/// Fetches name service information for multiple users asynchronously.
		/// </summary>
		/// <param name="userIds">An array of user IDs to fetch name service information for.</param>
		/// <returns>A list of <see cref="NameService"/> objects if available; otherwise, <c>null</c>.</returns>
		public async Task<List<NameService>?> FetchNameServiceReturn(params string[] userIds)
		{
			StringContent jsonData = new (JsonSerializer.Serialize(userIds), MediaTypeHeaderValue.Parse("application/json"));
			List<NameService>? namesData = await initiator.ExternalSystem.Net.PutAsync<List<NameService>>(Url, "name-service/v2/players", jsonData).ConfigureAwait(false);
			return namesData;
		} 
	}
}