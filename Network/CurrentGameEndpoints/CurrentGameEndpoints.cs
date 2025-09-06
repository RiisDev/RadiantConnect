using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints
{
	public class CurrentGameEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		public async Task<CurrentGamePlayer?> GetCurrentGamePlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<CurrentGamePlayer>(Url, $"/core-game/v1/players/{initiator.Client.UserId}");

		public async Task<string?> GetCurrentGameMatchIdAsync() => (await GetCurrentGamePlayerAsync())?.MatchId;

		public async Task<CurrentGameMatch?> GetCurrentGameMatchAsync() => await initiator.ExternalSystem.Net.GetAsync<CurrentGameMatch>(Url, $"/core-game/v1/matches/{await GetCurrentGameMatchIdAsync()}");

		public async Task<GameLoadout?> GetCurrentGameLoadoutsAsync() => await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/core-game/v1/matches/{await GetCurrentGameMatchIdAsync()}");

		[Obsolete("Ambiguous spelling, please use newer method 'GetCurrentGameLoadoutsAsync'")]
		public async Task<GameLoadout?> GetCurrentGameLoadoutAsync() => await GetCurrentGameLoadoutsAsync();

		// Need to get data return
		public async Task<CurrentSession?> GetCurrentSession() => await initiator.ExternalSystem.Net.GetAsync<CurrentSession>(Url, $"/session/v1/sessions/{initiator.Client.UserId}");

		public async Task QuitCurrentGameAsync() => await initiator.ExternalSystem.Net.PostAsync(Url, $"/core-game/v1/players/{initiator.Client.UserId}/disassociate/{await GetCurrentGameMatchIdAsync()}");
	}
}