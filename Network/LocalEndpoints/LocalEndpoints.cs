using System.Collections.Specialized;
using System.Net.Http.Json;
using RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Network.LocalEndpoints;

public class LocalEndpoints(Initiator initiator)
{
    public async Task<dynamic?> GetHelp()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/help");
    }

    public async Task<dynamic?> GetLocalSessions()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/product-session/v1/external-sessions");
    }

    public async Task<dynamic?> GetRSOInfo()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/rso-auth/v1/authorization/userinfo");
    }

    public async Task<dynamic?> GetLocalSwaggerDocs()
    {
        return await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/swagger/v3/openapi.json");
    }

    public async Task<LocaleInternal?> GetLocaleInfo()
    {
        return await initiator.ExternalSystem.Net.GetAsync<LocaleInternal>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/riotclient/region-locale");
    }

    public async Task<AliasInfo?> GetAliasInfo()
    {
        return await initiator.ExternalSystem.Net.GetAsync<AliasInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/player-account/aliases/v1/active");
    }

    public async Task<EntitlementTokens?> GetEntitlementTokens()
    {
        return await initiator.ExternalSystem.Net.GetAsync<EntitlementTokens>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/entitlements/v1/token");
    }

    public async Task<LocalChatSession?> GetLocalChatSession()
    {
        return await initiator.ExternalSystem.Net.GetAsync<LocalChatSession>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v1/session");
    }

    public async Task<InternalFriends?> GetLocalFriends()
    {
        return await initiator.ExternalSystem.Net.GetAsync<InternalFriends>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friends");
    }

    public async Task<FriendPresences?> GetFriendsPresences()
    {
        return await initiator.ExternalSystem.Net.GetAsync<FriendPresences>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/presences");
    }

    public async Task<InternalRequests?> GetFriendRequests()
    {
        return await initiator.ExternalSystem.Net.GetAsync<InternalRequests>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/presences");
    }

    public async Task SendFriendRequest(string gameName, string tagLine)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Post,$"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(
            new NameValueCollection{
                {"game_name", gameName},
                {"game_tag", tagLine}
            }
        ));
    }

    public async Task RemoveFriendRequest(string userId)
    {
        await initiator.ExternalSystem.Net.CreateRequest(ValorantNet.HttpMethod.Delete, $"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new NameValueCollection{{"puuid", userId}}));
    }
}