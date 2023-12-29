using System.Collections.Specialized;
using System.Net.Http.Json;
using RadiantConnect.Network.PartyEndpoints.DataTypes;
namespace RadiantConnect.Network.PartyEndpoints;

// ReSharper disable All

public class PartyEndpoints(Initiator initiator)
{
    internal string Url = initiator.ExternalSystem.ClientData.SharedUrl;

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

    public async Task<PartyPlayer?> FetchPartyPlayerAsync(string userId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PartyPlayer>(Url, $"parties/v1/players/{userId}");
    }

    public async Task<Party?> FetchPartyAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<Party>(Url, $"parties/v1/parties/{partyId}");
    }

    public async Task<CustomGameConfig?> FetchCustomGameConfigAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs");
    }

    public async Task<PartyChatToken?> FetchPartyChatTokenAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{partyId}/muctoken");
    }

    public async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{partyId}/voicetoken");
    }

    public async Task<PartySetReady?> SetPartyReadyAsync(string partyId, string userId, bool ready)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "ready", ready.ToString() } });
        return await initiator.ExternalSystem.Net.PostAsync<PartySetReady>(Url, $"parties/v1/parties/{partyId}/members/{userId}/setReady", jsonContent);
    }

    public async Task<Party?> RefreshCompetitveTierAsync(string partyId, string userId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshCompetitiveTier");
    }

    public async Task<Party?> RefreshPlayerIdentityAsync(string partyId, string userId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshPlayerIdentity");
    }

    public async Task<Party?> RefreshPingsAsync(string partyId, string userId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/members/{userId}/refreshPings");
    }

    public async Task<Party?> ChangeQueueAsync(string partyId, QueueId queueId)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "queueId", queueId.ToString() } });
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/queue", jsonContent);
    }

    public async Task<Party?> StartCustomGameeAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/startcustomgame");
    }

    public async Task<Party?> EnterQueueAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/join");
    }

    public async Task<Party?> LeaveQueueAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/leave");
    }

    public async Task<Party?> SetPartyOpenStatusAsync(string partyId, PartyState state)
    {
        JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "accessibility", state.ToString() } });
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/accessibility", jsonContent);
    }

    public async Task<Party?> SetCustomGameSettingsAsync(string partyId, CustomGameSettings gameSettings)
    {
        JsonContent jsonContent = JsonContent.Create(gameSettings);
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/customgamesettings", jsonContent);
    }

    public async Task<Party?> InvitePlayerAsync(string partyId, string name, string tagLine)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/invites/name/{name}/tag/{tagLine}");
    }

    // TO DO WORK ON REQUEST PARTY AND DECLINE PARTY
    public async Task<Party?> RequestPartyAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/request");
    }
    // TO DO WORK ON REQUEST PARTY AND DECLINE PARTY
    public async Task<Party?> DeclinePartyAsync(string partyId)
    {
        return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/request");
    }

    public async Task KickFromPartyAsync(string userId)
    {
        await initiator.ExternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}");
    }
}