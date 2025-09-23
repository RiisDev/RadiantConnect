using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints
{
	public class CurrentGameEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		public async Task<CurrentGamePlayer?> GetCurrentGamePlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<CurrentGamePlayer>(Url, $"/core-game/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);

		public async Task<string?> GetCurrentGameMatchIdAsync() => (await GetCurrentGamePlayerAsync().ConfigureAwait(false))?.MatchId;

		public async Task<CurrentGameMatch?> GetCurrentGameMatchAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);
			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<CurrentGameMatch>(Url,
					$"/core-game/v1/matches/{matchId}").ConfigureAwait(false);
		}

		public async Task<GameLoadout?> GetCurrentGameLoadoutsAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/core-game/v1/matches/{matchId}").ConfigureAwait(false);
		}

		public async Task QuitCurrentGameAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);

			if (!matchId.IsNullOrEmpty())
				await initiator.ExternalSystem.Net
					.PostAsync(Url, $"/core-game/v1/players/{initiator.Client.UserId}/disassociate/{matchId}")
					.ConfigureAwait(false);
		}


		public async Task<CurrentSession?> GetCurrentSession() => await initiator.ExternalSystem.Net.GetAsync<CurrentSession>(Url, $"/session/v1/sessions/{initiator.Client.UserId}").ConfigureAwait(false);


		[Obsolete("Ambiguous spelling, please use newer method 'GetCurrentGameLoadoutsAsync'")]
		public async Task<GameLoadout?> GetCurrentGameLoadoutAsync() => await GetCurrentGameLoadoutsAsync().ConfigureAwait(false);
	}
}