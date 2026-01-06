using RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Network.LocalEndpoints
{
	/// <summary>
	/// Provides access to local client endpoints for retrieving session information,
	/// friend data, aliases, and other client-local resources.
	/// </summary>
	/// <remarks>
	/// This endpoint group exposes operations that interact with the local
	/// Valorant/Riot client, including session management, friend requests,
	/// presence information, and entitlement tokens.
	/// </remarks>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class LocalEndpoints(Initiator initiator)
	{ 
		/// <summary>
		/// Retrieves general help information from the local client.
		/// </summary>
		/// <returns>
		/// A dynamic object representing help data, or <c>null</c> if unavailable.
		/// </returns>
		public async Task<dynamic?> GetHelpAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/help").ConfigureAwait(false);

		/// <summary>
		/// Retrieves information about local sessions for the current player.
		/// </summary>
		/// <returns>
		/// A dynamic object representing session information, or <c>null</c> if unavailable.
		/// </returns>
		public async Task<dynamic?> GetLocalSessionsAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/product-session/v1/external-sessions").ConfigureAwait(false);

		/// <summary>
		/// Retrieves RSO (Riot Sign-On) information from the local client.
		/// </summary>
		public async Task<dynamic?> GetRSOInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/rso-auth/v1/authorization/userinfo").ConfigureAwait(false);

		/// <summary>
		/// Retrieves Swagger documentation for local endpoints.
		/// </summary>
		public async Task<dynamic?> GetLocalSwaggerDocsAsync() => await initiator.ExternalSystem.Net.GetAsync<dynamic>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/swagger/v3/openapi.json").ConfigureAwait(false);

		/// <summary>
		/// Retrieves locale-specific information for the local client.
		/// </summary>
		public async Task<LocaleInternal?> GetLocaleInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<LocaleInternal>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/riotclient/region-locale").ConfigureAwait(false);

		/// <summary>
		/// Retrieves alias information for the current player.
		/// </summary>
		public async Task<AliasInfo?> GetAliasInfoAsync() => await initiator.ExternalSystem.Net.GetAsync<AliasInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/player-account/aliases/v1/active").ConfigureAwait(false);

		/// <summary>
		/// Retrieves entitlement tokens for the authenticated player.
		/// </summary>
		public async Task<EntitlementTokens?> GetEntitlementTokensAsync() => await initiator.ExternalSystem.Net.GetAsync<EntitlementTokens>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/entitlements/v1/token").ConfigureAwait(false);

		/// <summary>
		/// Retrieves information about the local chat session.
		/// </summary>
		public async Task<LocalChatSession?> GetLocalChatSessionAsync() => await initiator.ExternalSystem.Net.GetAsync<LocalChatSession>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v1/session").ConfigureAwait(false);

		/// <summary>
		/// Retrieves information about the player's friends.
		/// </summary>
		public async Task<InternalFriends?> GetLocalFriendsAsync() => await initiator.ExternalSystem.Net.GetAsync<InternalFriends>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friends").ConfigureAwait(false);

		/// <summary>
		/// Retrieves friend presence information for all friends.
		/// </summary>
		public async Task<FriendPresences?> GetFriendsPresencesAsync() => await initiator.ExternalSystem.Net.GetAsync<FriendPresences>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/presences").ConfigureAwait(false);

		/// <summary>
		/// Retrieves outstanding friend requests for the player.
		/// </summary>
		public async Task<InternalRequests?> GetFriendRequestsAsync() => await initiator.ExternalSystem.Net.GetAsync<InternalRequests>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests").ConfigureAwait(false);

		/// <summary>
		/// Sends a friend request to the specified player.
		/// </summary>
		/// <param name="gameName">The in-game name of the target player.</param>
		/// <param name="tagLine">The tag line of the target player.</param>
		public async Task SendFriendRequestAsync(string gameName, string tagLine) =>
			await initiator.ExternalSystem.Net.PostAsync($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
			{
				{ "game_name", gameName },
				{ "game_tag", tagLine }
			})).ConfigureAwait(false);

		/// <summary>
		/// Removes a previously sent friend request or unfriends the specified player.
		/// </summary>
		/// <param name="userId">The user ID of the target player.</param>
		public async Task RemoveFriendRequestAsync(string userId) =>
			await initiator.ExternalSystem.Net.DeleteAsync($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v4/friendrequests", JsonContent.Create(new Dictionary<string, string>
			{
				{ "puuid", userId },
			})).ConfigureAwait(false);

		/// <summary>
		/// Performs a raw HTTP request against the local client.
		/// </summary>
		/// <param name="method">The HTTP method to use.</param>
		/// <param name="endpoint">The endpoint to query.</param>
		/// <param name="content">Optional HTTP content for methods like POST or PUT.</param>
		/// <returns>
		/// A dynamic object representing the response, or <c>null</c> if the request fails.
		/// </returns>
		public async Task<string?> PerformLocalRequestAsync(ValorantNet.HttpMethod method, string endpoint, HttpContent? content = null) => await initiator.ExternalSystem.Net.CreateRequest(method, $"https://127.0.0.1:{ValorantNet.GetAuthPort()}", endpoint, content).ConfigureAwait(false);
	}
}