using System.Collections.Specialized;
using System.Net.Http.Json;
using CyphersWatchfulEye.ValorantAPI.DataTypes;
using RadiantConnect.Network.PartyEndpoints.DataTypes;
namespace RadiantConnect.Network.PartyEndpoints;

// ReSharper disable All

public class PartyEndpoints
{
    internal static string Url = Initiator.InternalSystem.ClientData.SharedUrl;
    internal static ValorantNet Net = Initiator.InternalSystem.Net;

    public enum QueueId
    {
        unrated,
        competitive,
        swiftplay,
        spikerush,
        deathmatch,
        ggteam,
        hurm
    }

    public enum PartyState
    {
        OPEN,
        CLOSED
    }

    public static async Task<PartyPlayer?> FetchPartyPlayerAsync(string userId)
    {
        return await ValorantNet.GetAsync<PartyPlayer>(Url, $"parties/v1/players/{userId}");
    }

    public static async Task<Party?> FetchPartyAsync(string partyId)
    {
        return await ValorantNet.GetAsync<Party>(Url, $"parties/v1/parties/{partyId}");
    }

    public static async Task<CustomGameConfig?> FetchCustomGameConfigAsync()
    {
        return await ValorantNet.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs");
    }

    public static async Task<PartyChatToken?> FetchPartyChatTokenAsync(string partyId)
    {
        return await ValorantNet.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{partyId}/muctoken");
    }

    public static async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync(string partyId)
    {
        return await ValorantNet.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{partyId}/voicetoken");
    }

    public static async Task<PartySetReady?> SetPartyReadyAsync(string partyId, string userId, bool ready)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "ready", ready.ToString() } });
        return await ValorantNet.PostAsync<PartySetReady>(Url, $"parties/v1/parties/{partyId}/members/{userId}/setReady", jsonContent);
    }

    public static async Task<PartyInfo?> RefreshCompetitveTierAsync(string partyId, string userId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshCompetitiveTier");
    }

    public static async Task<PartyInfo?> RefreshPlayerIdentityAsync(string partyId, string userId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshPlayerIdentity");
    }

    public static async Task<PartyInfo?> RefreshPingsAsync(string partyId, string userId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshPings");
    }

    public static async Task<PartyInfo?> ChangeQueueAsync(string partyId, QueueId queueId)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "queueId", queueId.ToString() } });
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/queue", jsonContent);
    }

    public static async Task<PartyInfo?> StartCustomGameeAsync(string partyId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/startcustomgame");
    }

    public static async Task<PartyInfo?> EnterQueueAsync(string partyId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/matchmaking/join");
    }

    public static async Task<PartyInfo?> LeaveQueueAsync(string partyId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/matchmaking/leave");
    }

    public static async Task<PartyInfo?> SetPartyOpenStatusAsync(string partyId, PartyState state)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "accessibility", state.ToString() } });
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/accessibility", jsonContent);
    }

    public static async Task<PartyInfo?> SetCustomGameSettingsAsync(string partyId, CustomGameSettings gameSettings)
    {
        JsonContent jsonContent = JsonContent.Create(gameSettings);
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/customgamesettings", jsonContent);
    }

    public static async Task<PartyInfo?> InvitePlayerAsync(string partyId, string name, string tagLine)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/invites/name/{name}/tag/{tagLine}");
    }

    // TO DO WORK ON REQUEST PARTY AND DECLINE PARTY
    public static async Task<PartyInfo?> RequestPartyAsync(string partyId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/request");
    }
    // TO DO WORK ON REQUEST PARTY AND DECLINE PARTY
    public static async Task<PartyInfo?> DeclinePartyAsync(string partyId)
    {
        return await ValorantNet.PostAsync<PartyInfo>(Url, $"parties/v1/parties/{partyId}/request");
    }

    public static async Task KickFromPartyAsync(string userId)
    {
        await Initiator.InternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}");
    }
}