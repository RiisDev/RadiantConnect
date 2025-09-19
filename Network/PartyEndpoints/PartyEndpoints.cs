using System.Collections.Specialized;
using RadiantConnect.Methods;
using RadiantConnect.Network.PartyEndpoints.DataTypes;
namespace RadiantConnect.Network.PartyEndpoints
{
	// ReSharper disable All

	public class PartyEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		public async Task<PartyPlayer?> FetchPartyPlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PartyPlayer>(Url, $"parties/v1/players/{initiator.Client.UserId}");

		private async Task<string?> FetchPartyIdAsync() => (await FetchPartyPlayerAsync())?.CurrentPartyID;
		public async Task<Party?> FetchPartyAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<Party>(Url, $"parties/v1/parties/{partyId}");
		}

		public async Task<CustomGameConfig?> FetchCustomGameConfigAsync() => await initiator.ExternalSystem.Net.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs");

		public async Task<PartyChatToken?> FetchPartyChatTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{partyId}/muctoken");
		}

		public async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{partyId}/voicetoken");
		}

		public async Task<PartySetReady?> SetPartyReadyAsync(bool ready)
		{
			string? partyId = await FetchPartyIdAsync();

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(
					new NameValueCollection() { { "ready", ready.ToString() } });

				return await initiator.ExternalSystem.Net
					.PostAsync<PartySetReady>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/setReady", jsonContent);
			}

			return null;
		}

		public async Task<Party?> RefreshCompetitveTierAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshCompetitiveTier");
		}

		public async Task<Party?> RefreshPlayerIdentityAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPlayerIdentity");
		}

		public async Task<Party?> RefreshPingsAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url,
						$"parties/v1/parties/{partyId}/members/{initiator.Client.UserId}/refreshPings");
		}

		public async Task<Party?> ChangeQueueAsync(ValorantTables.QueueId queueId)
		{
			string? partyId = await FetchPartyIdAsync();

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(new { queueID = queueId.ToString() });

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/queue", jsonContent);
			}

			return null;
		}

		public async Task<Party?> StartCustomGameAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/startcustomgame");
		}

		public async Task<Party?> EnterQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/join");
		}

		public async Task<Party?> LeaveQueueAsync()
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/matchmaking/leave");
		}

		public async Task<Party?> SetPartyOpenStatusAsync(ValorantTables.PartyState state)
		{
			string? partyId = await FetchPartyIdAsync();

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(
					new NameValueCollection() { { "accessibility", state.ToString() } });

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/accessibility", jsonContent);
			}
			
			return null;
		}

		public async Task<Party?> SetCustomGameSettingsAsync(CustomGameSettings gameSettings)
		{
			string? partyId = await FetchPartyIdAsync();

			if (!partyId.IsNullOrEmpty())
			{
				JsonContent jsonContent = JsonContent.Create(gameSettings);

				return await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/customgamesettings", jsonContent);
			}

			return null;
		}

		public async Task EnterCustomGameQueue()
		{
			string? partyId = await FetchPartyIdAsync();

			if (!partyId.IsNullOrEmpty())
				await initiator.ExternalSystem.Net
					.PostAsync(Url, $"parties/v1/parties/{partyId}/makecustomgame");
		}

		public async Task EnterCustomGame() => await EnterCustomGameQueue();

		public async Task SwitchToCustomGame() => await EnterCustomGameQueue();

		public async Task<Party?> InvitePlayerAsync(string name, string tagLine)
		{
			string? partyId = await FetchPartyIdAsync();

			return partyId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net
					.PostAsync<Party>(Url, $"parties/v1/parties/{partyId}/invites/name/{name}/tag/{tagLine}");
		}

		public async Task KickFromPartyAsync(string userId) => await initiator.ExternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}");

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> RequestPartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request");

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> DeclinePartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request");
	}
}