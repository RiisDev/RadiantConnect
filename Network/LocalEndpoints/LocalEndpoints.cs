using RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Network.LocalEndpoints
{
	public class LocalEndpoints(Initiator initiator)
	{
		public async Task<dynamic?> GetHelpAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/help").ConfigureAwait(false);

		public async Task<dynamic?> GetLocalSessionsAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/product-session/v1/external-sessions").ConfigureAwait(false);

		public async Task<dynamic?> GetRSOInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/rso-auth/v1/authorization/userinfo").ConfigureAwait(false);

		public async Task<dynamic?> GetLocalSwaggerDocsAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/swagger/v3/openapi.json").ConfigureAwait(false);

		public async Task<LocaleInternal?> GetLocaleInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<LocaleInternal>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/riotclient/region-locale").ConfigureAwait(false);

		public async Task<AliasInfo?> GetAliasInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<AliasInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/player-account/aliases/v1/active").ConfigureAwait(false);

		public async Task<EntitlementTokens?> GetEntitlementTokensAsync() => await initiator.ExternalSystem.Net.GetAsync<EntitlementTokens>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/entitlements/v1/token").ConfigureAwait(false);

		public async Task<LocalChatSession?> GetLocalChatSessionAsync() => await initiator.ExternalSystem.Net.GetAsync<LocalChatSession>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v1/session").ConfigureAwait(false);

		public async Task<InternalFriends?> GetLocalFriendsAsync() => await initiator.ExternalSystem.Net.GetAsync<InternalFriends>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friends").ConfigureAwait(false);

		public async Task<FriendPresences?> GetFriendsPresencesAsync() => await initiator.ExternalSystem.Net.GetAsync<FriendPresences>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/presences").ConfigureAwait(false);

		public async Task<InternalRequests?> GetFriendRequestsAsync() => await initiator.ExternalSystem.Net.GetAsync<InternalRequests>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests").ConfigureAwait(false);

		public async Task SendFriendRequestAsync(string gameName, string tagLine) =>
			await initiator.ExternalSystem.Net.PostAsync($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
			{
				{ "game_name", gameName },
				{ "game_tag", tagLine }
			})).ConfigureAwait(false);

		public async Task RemoveFriendRequestAsync(string userId) =>
			await initiator.ExternalSystem.Net.DeleteAsync($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
			{
				{ "puuid", userId },
			})).ConfigureAwait(false);

		public async Task<string?> PerformLocalRequestAsync(ValorantNet.HttpMethod method, string endpoint, HttpContent? content = null) => await initiator.ExternalSystem.Net.CreateRequest(method, $"https://127.0.0.1:{ValorantNet.GetAuthPort()}", endpoint, content).ConfigureAwait(false);
	}
}