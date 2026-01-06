using System.Collections.Specialized;
using RadiantConnect.Methods;
using RadiantConnect.Network.PartyEndpoints.DataTypes;
namespace RadiantConnect.Network.PartyEndpoints
{
	// ReSharper disable All
	/// <summary>
	/// Provides access to party-related endpoints in Valorant, including party management,
	/// matchmaking, custom game operations, and party tokens.
	/// </summary>
	/// <remarks>
	/// All endpoints operate on the currently authenticated player and their current party.
	/// </remarks>
	public class PartyEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		/// <summary>
		/// Fetches the current player's party player information.
		/// </summary>
		/// <returns>
		/// A <see cref="PartyPlayer"/> instance containing information about the player's party membership,
		/// or <c>null</c> if not in a party.
		/// </returns>
		public async Task<PartyPlayer?> FetchPartyPlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PartyPlayer>(Url, $"parties/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);

		private async Task<string?> FetchPartyIdAsync() => (await FetchPartyPlayerAsync().ConfigureAwait(false))?.CurrentPartyId;


		/// <summary>
		/// Fetches the current party of the authenticated player.
		/// </summary>
		/// <returns>
		/// A <see cref="Party"/> instance representing the party, or <c>null</c> if no party exists.
		/// </returns>
		public async Task<Party?> FetchPartyAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<Party>(Url, $"parties/v1/parties/{partyId}").ConfigureAwait(false);
		}


		/// <summary>
		/// Fetches the custom game configuration for the current party.
		/// </summary>
		public async Task<CustomGameConfig?> FetchCustomGameConfigAsync() => await initiator.ExternalSystem.Net.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs").ConfigureAwait(false);

		/// <summary>
		/// Fetches the chat token for the current party.
		/// </summary>
		public async Task<PartyChatToken?> FetchPartyChatTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{partyId}/muctoken").ConfigureAwait(false);
		}


		/// <summary>
		/// Fetches the voice token for the current party.
		/// </summary>
		public async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{partyId}/voicetoken").ConfigureAwait(false);
		}

		/// <summary>
		/// Sets the ready status of the current player in the party.
		/// </summary>
		/// <param name="ready">True to mark ready, false otherwise.</param>
		/// <returns>The updated party state or <c>null</c> if no party exists.</returns>
		public async Task<PartySetReady?> SetPartyReadyAsync(bool ready)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(
					new NameValueCollection() { { "ready", ready.ToString() } });

				return await initiator.ExternalSystem.Net
					.PostAsync<PartySetReady>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/setReady", jsonContent).ConfigureAwait(false);
			}

			return null;
		}

		/// <summary>
		/// Refreshes the competitive tier for the current player in the party.
		/// </summary>
		public async Task<Party?> RefreshCompetitveTierAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshCompetitiveTier").ConfigureAwait(false);
		}

		/// <summary>
		/// Refreshes the player's identity information in the party.
		/// </summary>
		public async Task<Party?> RefreshPlayerIdentityAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPlayerIdentity").ConfigureAwait(false);
		}

		/// <summary>
		/// Refreshes ping information for the current party members.
		/// </summary>
		public async Task<Party?> RefreshPingsAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPings").ConfigureAwait(false);
		}

		/// <summary>
		/// Changes the current party's queue ID.
		/// </summary>
		public async Task<Party?> ChangeQueueAsync(ValorantTables.QueueId queueId)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(new { queueID = queueId.ToString() });

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/queue", jsonContent).ConfigureAwait(false);
			}

			return null;
		}

		/// <summary>
		/// Starts a custom game for the current party.
		/// </summary>
		public async Task<Party?> StartCustomGameAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/startcustomgame").ConfigureAwait(false);
		}

		/// <summary>
		/// Enters the party into the matchmaking queue.
		/// </summary>
		public async Task<Party?> EnterQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/join").ConfigureAwait(false);
		}

		/// <summary>
		/// Leaves the matchmaking queue for the current party.
		/// </summary>
		public async Task<Party?> LeaveQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/leave").ConfigureAwait(false);
		}

		/// <summary>
		/// Sets the party's accessibility status (open/closed).
		/// </summary>
		public async Task<Party?> SetPartyOpenStatusAsync(ValorantTables.PartyState state)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(
					new NameValueCollection() { { "accessibility", state.ToString() } });

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/accessibility", jsonContent).ConfigureAwait(false);
			}
			
			return null;
		}

		/// <summary>
		/// Updates the current party's custom game settings.
		/// </summary>
		public async Task<Party?> SetCustomGameSettingsAsync(CustomGameSettings gameSettings)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(gameSettings);

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/customgamesettings", jsonContent).ConfigureAwait(false);
			}

			return null;
		}

		/// <summary>
		/// Starts the game queue.
		/// </summary>
		public async Task EnterCustomGameQueue()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
				await initiator.ExternalSystem.Net
					.PostAsync(Url, $"parties/v1/parties/{partyId}/makecustomgame").ConfigureAwait(false);
		}

		/// <summary>
		/// Enters custom game lobby.
		/// </summary>
		public async Task EnterCustomGame() => await EnterCustomGameQueue().ConfigureAwait(false);

		/// <summary>
		/// Enters custom game lobby.
		/// </summary>
		public async Task SwitchToCustomGame() => await EnterCustomGameQueue().ConfigureAwait(false);

		/// <summary>
		/// Invites player to the current party by their name and tag line.
		/// <param name="name">Player Name</param>
		/// <param name="tagLine">The tagline</param>
		/// </summary>
		public async Task<Party?> InvitePlayerAsync(string name, string tagLine)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/invites/name/{name}/tag/{tagLine}").ConfigureAwait(false);
		}

		/// <summary>
		/// Kicks player from the current party by their user ID.
		/// </summary>
		public async Task KickFromPartyAsync(string userId) => await initiator.ExternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}").ConfigureAwait(false);

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> RequestPartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync().ConfigureAwait(false)}/request").ConfigureAwait(false);

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> DeclinePartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync().ConfigureAwait(false)}/request").ConfigureAwait(false);
	}
}