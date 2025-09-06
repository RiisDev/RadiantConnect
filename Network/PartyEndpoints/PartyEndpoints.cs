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

		public async Task<Party?> FetchPartyAsync() => await initiator.ExternalSystem.Net.GetAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}");

		public async Task<CustomGameConfig?> FetchCustomGameConfigAsync() => await initiator.ExternalSystem.Net.GetAsync<CustomGameConfig>(Url, "parties/v1/parties/customgameconfigs");

		public async Task<PartyChatToken?> FetchPartyChatTokenAsync() => await initiator.ExternalSystem.Net.GetAsync<PartyChatToken>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/muctoken");

		public async Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync() => await initiator.ExternalSystem.Net.GetAsync<PartyVoiceToken>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/voicetoken");

		public async Task<PartySetReady?> SetPartyReadyAsync(bool ready)
		{
			JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "ready", ready.ToString() } });
			return await initiator.ExternalSystem.Net.PostAsync<PartySetReady>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/members/{initiator.Client.UserId}/setReady", jsonContent);
		}

		public async Task<Party?> RefreshCompetitveTierAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/members/{initiator.Client.UserId}/refreshCompetitiveTier");

		public async Task<Party?> RefreshPlayerIdentityAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/members/{initiator.Client.UserId}/refreshPlayerIdentity");

		public async Task<Party?> RefreshPingsAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/members/{initiator.Client.UserId}/refreshPings");

		public async Task<Party?> ChangeQueueAsync(ValorantTables.QueueId queueId)
		{
			JsonContent jsonContent = JsonContent.Create(new { queueID = queueId.ToString() });
			return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/queue",  jsonContent);
		}

		public async Task<Party?> StartCustomGameAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/startcustomgame");

		public async Task<Party?> EnterQueueAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/matchmaking/join");

		public async Task<Party?> LeaveQueueAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/matchmaking/leave");

		public async Task<Party?> SetPartyOpenStatusAsync(ValorantTables.PartyState state)
		{
			JsonContent jsonContent = JsonContent.Create(new NameValueCollection() { { "accessibility", state.ToString() } });
			return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/accessibility",  jsonContent);
		}

		public async Task<Party?> SetCustomGameSettingsAsync(CustomGameSettings gameSettings)
		{
			JsonContent jsonContent = JsonContent.Create(gameSettings);
			return await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/customgamesettings", jsonContent);
		}

		public async Task EnterCustomGameQueue() => await initiator.ExternalSystem.Net.PostAsync(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/makecustomgame");
		public async Task EnterCustomGame() => await EnterCustomGameQueue();
		public async Task SwitchToCustomGame() => await EnterCustomGameQueue();

		public async Task<Party?> InvitePlayerAsync(string name, string tagLine) => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/invites/name/{name}/tag/{tagLine}");

		public async Task KickFromPartyAsync(string userId) => await initiator.ExternalSystem.Net.DeleteAsync(Url, $"parties/v1/players/{userId}");

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> RequestPartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request");

		// TODO WORK ON REQUEST PARTY AND DECLINE PARTY
		internal async Task<Party?> DeclinePartyAsync() => await initiator.ExternalSystem.Net.PostAsync<Party>(Url, $"parties/v1/parties/{await FetchPartyIdAsync()}/request");
	}
}