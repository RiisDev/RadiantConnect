using System.Net.Http.Json;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.Services;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints;

public class PVPEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.PdUrl;

    public async Task<Content?> FetchContentAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<Content>(initiator.ExternalSystem.ClientData.SharedUrl, $"content-service/v3/content");
    }

    public async Task<AccountXP?> FetchAccountXPAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<AccountXP>(Url, $"account-xp/v1/players/{userId}");
    }

    public async Task<PlayerLoadout?> FetchPlayerLoadoutAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PlayerLoadout>(Url, $"personalization/v2/players/{userId}/playerloadout");
    }

    public async Task<PlayerMMR?> FetchPlayerMMRAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PlayerMMR>(Url, $"mmr/v1/players/{userId}");
    }

    public async Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}");
    }

    public async Task<MatchHistory?> FetchPlayerMatchHistoryByQueueIdAsync(string userId, string queueId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<MatchHistory>(Url, $"match-history/v1/history/{userId}?startIndex=0&endIndex=20&queue={queueId}");
    }

    public async Task<MatchInfo?> FetchMatchInfoAsync(string matchId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<MatchInfo>(Url, $"match-details/v1/matches/{matchId}");
    }

    public async Task<CompetitiveUpdate?> FetchCompetitveUpdatesAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<CompetitiveUpdate>(Url, $"mmr/v1/players/{userId}/competitiveupdates?startIndex=0&endIndex=20&queue=competitive");
    }

    public async Task<Leaderboard?> FetchLeaderboardAsync(string seasonId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<Leaderboard>(Url, $"mmr/v1/leaderboards/affinity/na/queue/competitive/season/{seasonId}");
    }

    public async Task<Penalty?> FetchPenaltiesAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, "restrictions/v3/penalties");
    }

    public async Task<Penalty?> FetchClientConfigAsync(LogService.ClientData.ShardType shard)
    {
        return await initiator.ExternalSystem.Net.GetAsync<Penalty>(Url, $"v1/config/{shard}");
    }

    public async Task<PlayerLoadout?> SetPlayerLoadoutAsync(string userId, SetPlayerLoadout newLoadout)
    {
        return await initiator.ExternalSystem.Net.PutAsync<PlayerLoadout>(Url, $"personalization/v2/players/{userId}/playerloadout", JsonContent.Create(newLoadout));
    }

    public async Task<NameService?> FetchNameServiceReturn(string userId)
    {
        List<NameService>? namesData = await initiator.ExternalSystem.Net.PutAsync<List<NameService>>(Url, "name-service/v2/players", new StringContent($"[\"{userId}\"]")); ;
        if (namesData == null || namesData.Count == 0) { return null; }
        return namesData[0];
    } 
}