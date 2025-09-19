using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.Services;
using System.Net.Http.Headers;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints
{
	public class PVPEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

		public async Task<Content?> FetchContentAsync() => await initiator.ExternalSystem.Net.GetAsync<Content>(initiator.ExternalSystem.ClientData.SharedUrl, $"content-service/v3/content");

		public async Task<AccountXP?> FetchAccountXPAsync() => await initiator.ExternalSystem.Net.GetAsync<AccountXP>(Url, $"account-xp/v1/players/{initiator.Client.UserId}");

		public async Task<PlayerLoadout?> FetchPlayerLoadoutAsync() => await initiator.ExternalSystem.Net.GetAsync<PlayerLoadout>(Url, $"personalization/v2/players/{initiator.Client.UserId}/playerloadout");

		public async Task<PlayerMMR?> FetchPlayerMMRAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<PlayerMMR>(Url, $"mmr/v1/players/{userId}");

		public async Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}");

		public async Task<MatchHistory?> FetchPlayerMatchHistoryByQueueIdAsync(string userId, string queueId) => await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}?startIndex=0&endIndex=20&queue={queueId}");

		public async Task<MatchInfo?> FetchMatchInfoAsync(string matchId) => await initiator.ExternalSystem.Net.GetAsync<MatchInfo>(Url, $"match-details/v1/matches/{matchId}");

		public async Task<CompetitiveUpdate?> FetchCompetitveUpdatesAsync(string userId) => await initiator.ExternalSystem.Net.GetAsync<CompetitiveUpdate>(Url, $"mmr/v1/players/{userId}/competitiveupdates?startIndex=0&endIndex=20&queue=competitive");

		public async Task<Leaderboard?> FetchLeaderboardAsync(string seasonId) => await initiator.ExternalSystem.Net.GetAsync<Leaderboard>(Url, $"mmr/v1/leaderboards/affinity/na/queue/competitive/season/{seasonId}");

		public async Task<Penalty?> FetchPenaltiesAsync() => await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, "restrictions/v3/penalties");

		public async Task<Penalty?> FetchClientConfigAsync(LogService.ClientData.ShardType shard) => await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, $"v1/config/{shard}");

		public async Task<PlayerLoadout?> SetPlayerLoadoutAsync(SetPlayerLoadout newLoadout) => await initiator.ExternalSystem.Net.PutAsync<PlayerLoadout>(Url, $"personalization/v2/players/{initiator.Client.UserId}/playerloadout", JsonContent.Create(newLoadout));

		public async Task<List<NameService>?> FetchNameServiceReturn(params string[] userIds)
		{
			StringContent jsonData = new StringContent(JsonSerializer.Serialize(userIds), MediaTypeHeaderValue.Parse("application/json"));
			List<NameService>? namesData = await initiator.ExternalSystem.Net.PutAsync<List<NameService>>(Url, "name-service/v2/players", jsonData);
			return namesData;
		} 
	}
}