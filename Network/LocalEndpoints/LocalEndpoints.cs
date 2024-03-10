using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Json;
using RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Network.LocalEndpoints;

public class LocalEndpoints(Initiator initiator)
{
    public async Task<dynamic?> GetHelpAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/help");
    }

    public async Task<dynamic?> GetLocalSessionsAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/product-session/v1/external-sessions");
    }

    public async Task<dynamic?> GetRSOInfoAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/rso-auth/v1/authorization/userinfo");
    }

    public async Task<dynamic?> GetLocalSwaggerDocsAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/swagger/v3/openapi.json");
    }

    public async Task<LocaleInternal?> GetLocaleInfoAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<LocaleInternal>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/riotclient/region-locale");
    }

    public async Task<AliasInfo?> GetAliasInfoAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<AliasInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/player-account/aliases/v1/active");
    }

    public async Task<EntitlementTokens?> GetEntitlementTokensAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<EntitlementTokens>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/entitlements/v1/token");
    }

    public async Task<LocalChatSession?> GetLocalChatSessionAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<LocalChatSession>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v1/session");
    }

    public async Task<InternalFriends?> GetLocalFriendsAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<InternalFriends>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friends");
    }

    public async Task<FriendPresences?> GetFriendsPresencesAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<FriendPresences>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/presences");
    }

    public async Task<InternalRequests?> GetFriendRequestsAsync()
    {
        return await initiator.ExternalSystem.Net.GetAsync<InternalRequests>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests");
    }

    public async Task SendFriendRequestAsync(string gameName, string tagLine)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Post,$"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
        {
            { "game_name", gameName },
            { "game_tag", tagLine }
        }));
    }

    public async Task RemoveFriendRequestAsync(string userId)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Delete, $"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
        {
            { "puuid", userId },
        }));
    }

    public async Task<string?> PerformLocalRequestAsync(ValorantNet.HttpMethod method, string endpoint, HttpContent? content = null)
    {
        return await initiator.ExternalSystem.Net.CreateRequest(method, $"https://127.0.0.1:{ValorantNet.GetAuthPort()}", endpoint, content);
    }
}