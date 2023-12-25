using System.Net.Http.Json;
using RadiantConnect.Methods;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints;

public class PVPEndpoints
{
    internal static ValorantNet Net = Initiator.InternalSystem.Net;
    internal static string Url = Initiator.InternalSystem.ClientData.PdUrl;

    public static async Task<AccountXP?> FetchAccountXPAsync(string userId)
    {
        return await ValorantNet.GetAsync<AccountXP>(Url, $"/account-xp/v1/players/{userId}");
    }

    public static async Task<PlayerLoadout?> FetchPlayerLoadoutAsync(string userId)
    {
        return await ValorantNet.GetAsync<PlayerLoadout>(Url, $"/personalization/v2/players/{userId}/playerloadout");
    }

    public static async Task<PlayerMMR?> FetchPlayerMMRAsync(string userId)
    {
        return await ValorantNet.GetAsync<PlayerMMR>(Url, $"/mmr/v1/players/{userId}");
    }

    public static async Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId)
    {
        return await ValorantNet.GetAsync<MatchHistory>(Url, $"/mmr/v1/players/{userId}");
    }

    public static async Task<MatchHistory?> FetchPlayerMatchInfoAsync(string matchId)
    {
        return await ValorantNet.GetAsync<MatchHistory>(Url, $"/match-details/v1/matches/{matchId}");
    }

    public static async Task<CompetitiveUpdate?> FetchCompetitveUpdatesAsync(string userId)
    {
        return await ValorantNet.GetAsync<CompetitiveUpdate>(Url, $"/mmr/v1/players/{userId}/competitiveupdates");
    }

    public static async Task<Leaderboard?> FetchLeaderboardAsync(string seasonId)
    {
        return await ValorantNet.GetAsync<Leaderboard>(Url, $"/mmr/v1/leaderboards/affinity/na/queue/competitive/season/{seasonId}");
    }

    public static async Task<Penalty?> FetchPenaltiesAsync()
    {
        return await ValorantNet.GetAsync<Penalty>(Url, "/restrictions/v3/penalties");
    }

    public static async Task<Penalty?> FetchClientConfigAsync(ClientData.ShardType shard)
    {
        return await ValorantNet.GetAsync<Penalty>(Url, $"/v1/config/{shard}");
    }

    public static async Task<PlayerLoadout?> SetPlayerLoadoutAsync(string userId, SetPlayerLoadout newLoadout)
    {
        return await ValorantNet.PutAsync<PlayerLoadout>(Url, $"personalization/v2/players/{userId}/playerloadout", JsonContent.Create(newLoadout));
    }
}