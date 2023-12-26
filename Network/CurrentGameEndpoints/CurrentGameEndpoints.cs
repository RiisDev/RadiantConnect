// ReSharper disable All

using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints;

public class CurrentGameEndpoints
{
    internal static string Url = Initiator.InternalSystem.ClientData.SharedUrl;

    public static async Task<CurrentGamePlayer?> GetCurrentGamePlayer(string userId)
    {
        return await ValorantNet.GetAsync<CurrentGamePlayer>(Url, $"/core-game/v1/players/{userId}");
    }

    public static async Task<CurrentGameMatch?> GetCurrentGameMatch(string matchId)
    {
        return await ValorantNet.GetAsync<CurrentGameMatch>(Url, $"/core-game/v1/matches/{matchId}");
    }

    public static async Task<GameLoadout?> GetCurrentGameLoadout(string matchId)
    {
        return await ValorantNet.GetAsync<GameLoadout>(Url, $"/core-game/v1/matches/{matchId}");
    }

    public static async Task QuitCurrentGame(string userId, string matchId)
    {
        await Initiator.InternalSystem.Net.PostAsync(Url, $"/core-game/v1/players/{userId}/disassociate/{matchId}");
    }
}