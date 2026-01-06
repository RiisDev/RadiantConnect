using RadiantConnect.Methods;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;
// ReSharper disable All

namespace RadiantConnect.Network.PreGameEndpoints
{
	/// <summary>
	/// Provides access to pre-game endpoints in Valorant, including fetching pre-game player info,
	/// match details, loadouts, and performing character selection/lock operations.
	/// </summary>
	public class PreGameEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		/// <summary>
		/// Fetches the pre-game player information for the current authenticated player.
		/// </summary>
		/// <returns>
		/// A <see cref="PreGamePlayer"/> instance containing pre-game data, or <c>null</c> if not in pre-game.
		/// </returns>
		public async Task<PreGamePlayer?> FetchPreGamePlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<PreGamePlayer>(Url, $"/pregame/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);

		/// <summary>
		/// Fetches the pre-game match ID associated with the current player.
		/// </summary>
		/// <returns>The pre-game match ID, or <c>null</c> if the player is not in pre-game.</returns>
		public async Task<string?> FetchPreGameMatchId() => (await FetchPreGamePlayerAsync().ConfigureAwait(false))?.MatchId;

		/// <summary>
		/// Fetches full pre-game match information.
		/// </summary>
		/// <returns>
		/// A <see cref="PreGameMatch"/> instance representing the match, or <c>null</c> if not in pre-game.
		/// </returns>
		public async Task<PreGameMatch?> FetchPreGameMatchAsync()
		{
			string? matchId = await FetchPreGameMatchId().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<PreGameMatch>(Url, $"/pregame/v1/matches/{matchId}").ConfigureAwait(false);
		}
		/// <summary>
		/// Fetches the pre-game loadout for the current match.
		/// </summary>
		/// <returns>
		/// A <see cref="GameLoadout"/> instance representing the match, or <c>null</c> if not in pre-game.
		/// </returns>
		public async Task<GameLoadout?> FetchPreGameLoadoutAsync()
		{
			string? matchId = await FetchPreGameMatchId().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url,
					$"/pregame/v1/matches/{matchId}/loadouts").ConfigureAwait(false);
		}

		/// <summary>
		/// Selects an agent for the current player in pre-game.
		/// </summary>
		/// <param name="agent">The <see cref="ValorantTables.Agent"/> to select.</param>
		/// <returns>The updated <see cref="PreGameMatch"/> after selection, or <c>null</c> if not in pre-game.</returns>
		public async Task<PreGameMatch?> SelectCharacterAsync(ValorantTables.Agent agent)
		{
			string? matchId = await FetchPreGameMatchId().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url,
					$"/pregame/v1/matches/{matchId}/select/{ValorantTables.AgentToId[agent]}").ConfigureAwait(false);
		}

		/// <summary>
		/// Locks the selected agent for the current player in pre-game.
		/// </summary>
		/// <param name="agent">The <see cref="ValorantTables.Agent"/> to lock.</param>
		/// <returns>The updated <see cref="PreGameMatch"/> after locking, or <c>null</c> if not in pre-game.</returns>
		public async Task<PreGameMatch?> LockCharacterAsync(ValorantTables.Agent agent)
		{
			string? matchId = await FetchPreGameMatchId().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.PostAsync<PreGameMatch>(Url,
					$"/pregame/v1/matches/{matchId}/lock/{ValorantTables.AgentToId[agent]}").ConfigureAwait(false);
		}

		/// <summary>
		/// Quits the current pre-game session for the player.
		/// </summary>
		public async Task QuitGameAsync()
		{
			string? matchId = await FetchPreGameMatchId().ConfigureAwait(false);
			if (matchId.IsNullOrEmpty())
				return;
			
			await initiator.ExternalSystem.Net.PostAsync(Url, $"/pregame/v1/matches/{matchId}/quit").ConfigureAwait(false);
		}

	}
}