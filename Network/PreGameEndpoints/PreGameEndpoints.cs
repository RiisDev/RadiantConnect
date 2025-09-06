using RadiantConnect.Methods;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;
// ReSharper disable All

namespace RadiantConnect.Network.PreGameEndpoints
{
	public class PreGameEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		public async Task<PreGamePlayer?> FetchPreGamePlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PreGamePlayer>(Url, $"/pregame/v1/players/{initiator.Client.UserId}");

		public async Task<string?> FetchPreGameMatchId() => (await FetchPreGamePlayerAsync())?.MatchId;

		public async Task<PreGameMatch?> FetchPreGameMatchAsync() => await initiator.ExternalSystem.Net.GetAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{await FetchPreGameMatchId()}");

		public async Task<GameLoadout?> FetchPreGameLoadoutAsync() => await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/pregame/v1/matches/{await FetchPreGameMatchId()}/loadouts");

		public async Task<PreGameMatch?> SelectCharacterAsync(ValorantTables.Agent agent) => await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{await FetchPreGameMatchId()}/select/{ValorantTables.AgentToId[agent]}");

		public async Task<PreGameMatch?> LockCharacterAsync(ValorantTables.Agent agent) => await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{await FetchPreGameMatchId()}/lock/{ValorantTables.AgentToId[agent]}");

		public async Task QuitGameAsync() => await initiator.ExternalSystem.Net.PostAsync(Url, $"/pregame/v1/matches/{await FetchPreGameMatchId()}/quit").ConfigureAwait(false);
	}
}