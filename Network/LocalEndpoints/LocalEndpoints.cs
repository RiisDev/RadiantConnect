using System.Collections.Specialized;
using System.Net.Http.Json;
using RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Network.LocalEndpoints;

public class LocalEndpoints
{
    public static async Task<dynamic?> GetHelp()
    {
        return await ValorantNet.GetAsync<dynamic>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/help");
    }

    public static async Task<dynamic?> GetLocalSessions()
    {
        return await ValorantNet.GetAsync<dynamic>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/product-session/v1/external-sessions");
    }

    public static async Task<dynamic?> GetRSOInfo()
    {
        return await ValorantNet.GetAsync<dynamic>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/rso-auth/v1/authorization/userinfo");
    }

    public static async Task<dynamic?> GetLocalSwaggerDocs()
    {
        return await ValorantNet.GetAsync<dynamic>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/swagger/v3/openapi.json");
    }

    public static async Task<LocaleInternal?> GetLocaleInfo()
    {
        return await ValorantNet.GetAsync<LocaleInternal>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/riotclient/region-locale");
    }

    public static async Task<AliasInfo?> GetAliasInfo()
    {
        return await ValorantNet.GetAsync<AliasInfo>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/player-account/aliases/v1/active");
    }

    public static async Task<EntitlementTokens?> GetEntitlementTokens()
    {
        return await ValorantNet.GetAsync<EntitlementTokens>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/entitlements/v1/token");
    }

    public static async Task<LocalChatSession?> GetLocalChatSession()
    {
        return await ValorantNet.GetAsync<LocalChatSession>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v1/session");
    }

    public static async Task<InternalFriends?> GetLocalFriends()
    {
        return await ValorantNet.GetAsync<InternalFriends>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v4/friends");
    }

    public static async Task<FriendPresences?> GetFriendsPresences()
    {
        return await ValorantNet.GetAsync<FriendPresences>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v4/presences");
    }

    public static async Task<InternalRequests?> GetFriendRequests()
    {
        return await ValorantNet.GetAsync<InternalRequests>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v4/presences");
    }

    public static async Task SendFriendRequest(string gameName, string tagLine)
    {
        await Initiator.InternalSystem.Net.PostAsync($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(
            new NameValueCollection{
                {"game_name", gameName},
                {"game_tag", tagLine}
            }
        ));
    }

    public static async Task RemoveFriendRequest(string userId)
    {
        await Initiator.InternalSystem.Net.DeleteAsJsonAsync($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new NameValueCollection{{"puuid", userId}}));
    }
}