using System.Collections.Specialized;
using RadiantConnect.Methods;
using RadiantConnect.Network.PartyEndpoints.DataTypes;
namespace RadiantConnect.Network.PartyEndpoints
{
	// ReSharper disable All

	public class PartyEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		public async Task<PartyPlayer?> FetchPartyPlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PartyPlayer>(Url, $"parties/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);

		private async Task<string?> FetchPartyIdAsync() => (await FetchPartyPlayerAsync().ConfigureAwait(false))?.CurrentPartyID;
		public async Task<Party?> FetchPartyAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<Party>(Url, $"parties/v1/parties/{partyId}").ConfigureAwait(false);
		}

		public async Task<CustomGameConfig?> FetchCustomGameConfigAsync() => await initiator.ExternalSystem.Net.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs").ConfigureAwait(false);

		public async Task<PartyChatToken?> FetchPartyChatTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{partyId}/muctoken").ConfigureAwait(false);
		}

		public async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{partyId}/voicetoken").ConfigureAwait(false);
		}

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

		public async Task<Party?> RefreshCompetitveTierAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshCompetitiveTier").ConfigureAwait(false);
		}

		public async Task<Party?> RefreshPlayerIdentityAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPlayerIdentity").ConfigureAwait(false);
		}

		public async Task<Party?> RefreshPingsAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPings").ConfigureAwait(false);
		}

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

		public async Task<Party?> StartCustomGameAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/startcustomgame").ConfigureAwait(false);
		}

		public async Task<Party?> EnterQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/join").ConfigureAwait(false);
		}

		public async Task<Party?> LeaveQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/leave").ConfigureAwait(false);
		}

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

		public async Task EnterCustomGameQueue()
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			if (!partyId.IsNullOrEmpty())
				await initiator.ExternalSystem.Net
					.PostAsync(Url, $"parties/v1/parties/{partyId}/makecustomgame").ConfigureAwait(false);
		}

		public async Task EnterCustomGame() => await EnterCustomGameQueue().ConfigureAwait(false);

		public async Task SwitchToCustomGame() => await EnterCustomGameQueue().ConfigureAwait(false);

		public async Task<Party?> InvitePlayerAsync(string name, string tagLine)
		{
			string? partyId = await FetchPartyIdAsync().ConfigureAwait(false);

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/invites/name/{name}/tag/{tagLine}").ConfigureAwait(false);
		}

		public async Task KickFromPartyAsync(string userId) => await initiator.ExternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}").ConfigureAwait(false);

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> RequestPartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request").ConfigureAwait(false);

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> DeclinePartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request").ConfigureAwait(false);
	}
}