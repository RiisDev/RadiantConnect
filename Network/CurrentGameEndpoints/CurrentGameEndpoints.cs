using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints;

public class CurrentGameEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

    public async Task<CurrentGamePlayer?> GetCurrentGamePlayerAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<CurrentGamePlayer>(Url, $"/core-game/v1/players/{userId}");
    }

    public async Task<CurrentGameMatch?> GetCurrentGameMatchAsync(string matchId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<CurrentGameMatch>(Url, $"/core-game/v1/matches/{matchId}");
    }

    public async Task<GameLoadout?> GetCurrentGameLoadoutAsync(string matchId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/core-game/v1/matches/{matchId}");
    }
    // Need to get data return
    public async Task<CurrentSession?> GetCurrentSession(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<CurrentSession>(Url, $"/session/v1/sessions/{userId}");
    }

    public async Task QuitCurrentGameAsync(string userId, string matchId)
    {
        await initiator.ExternalSystem.Net.PostAsync(Url, $"/core-game/v1/players/{userId}/disassociate/{matchId}");
    }
}