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
		
		public async Task<PreGameMatch?> FetchPreGameMatchAsync()
		{
			string? matchId = await FetchPreGameMatchId();
			if (matchId.IsNullOrEmpty())
				return null;

			return await initiator.ExternalSystem.Net.GetAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}");
		}

		public async Task<GameLoadout?> FetchPreGameLoadoutAsync()
		{
			string? matchId = await FetchPreGameMatchId();
			if (matchId.IsNullOrEmpty())
				return null;

			return await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/pregame/v1/matches/{matchId}/loadouts");
		}

		public async Task<PreGameMatch?> SelectCharacterAsync(ValorantTables.Agent agent)
		{
			string? matchId = await FetchPreGameMatchId();
			if (matchId.IsNullOrEmpty())
				return null;

			return await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}/select/{ValorantTables.AgentToId[agent]}");
		}

		public async Task<PreGameMatch?> LockCharacterAsync(ValorantTables.Agent agent)
		{
			string? matchId = await FetchPreGameMatchId();
			if (matchId.IsNullOrEmpty())
				return null;

			return await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}/lock/{ValorantTables.AgentToId[agent]}");
		}

		public async Task QuitGameAsync()
		{
			string? matchId = await FetchPreGameMatchId();
			if (matchId.IsNullOrEmpty())
				return;
			
			await initiator.ExternalSystem.Net.PostAsync(Url, $"/pregame/v1/matches/{matchId}/quit").ConfigureAwait(false);
		}

	}
}