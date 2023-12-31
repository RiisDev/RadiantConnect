﻿// ReSharper disable All

using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints;

public class CurrentGameEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.SharedUrl;

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

    public async Task QuitCurrentGameAsync(string userId, string matchId)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Post, Url, $"/core-game/v1/players/{userId}/disassociate/{matchId}");
    }
}