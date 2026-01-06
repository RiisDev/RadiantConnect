using RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
using RadiantConnect.Network.PreGameEndpoints.DataTypes;

namespace RadiantConnect.Network.CurrentGameEndpoints
{
	/// <summary>
	/// Provides access to endpoints related to the player's current or active game.
	/// </summary>
	/// <remarks>
	/// This endpoint group exposes operations for retrieving current game state,
	/// match metadata, player loadouts, and session information, as well as
	/// disassociating the player from an active match.
	/// </remarks>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing authentication,
	/// networking, and client context.
	/// </param>
	public class CurrentGameEndpoints(Initiator initiator)
	{
		internal string Url = initiator.ExternalSystem.ClientData.GlzUrl;

		/// <summary>
		/// Retrieves the current game player state for the authenticated user.
		/// </summary>
		/// <returns>
		/// A <see cref="CurrentGamePlayer"/> instance containing player and match
		/// metadata, or <c>null</c> if the player is not currently in a game.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network issues.
		/// </exception>
		public async Task<CurrentGamePlayer?> GetCurrentGamePlayerAsync() => await initiator.ExternalSystem.Net.GetAsync<CurrentGamePlayer>(Url, $"/core-game/v1/players/{initiator.Client.UserId}").ConfigureAwait(false);
		
		/// <summary>
		/// Retrieves the match identifier for the player's current game.
		/// </summary>
		/// <returns>
		/// The match ID if the player is currently in a game; otherwise <c>null</c>.
		/// </returns>
		public async Task<string?> GetCurrentGameMatchIdAsync() => (await GetCurrentGamePlayerAsync().ConfigureAwait(false))?.MatchId;
		
		/// <summary>
		/// Retrieves detailed match information for the player's current game.
		/// </summary>
		/// <returns>
		/// A <see cref="CurrentGameMatch"/> instance containing match metadata,
		/// or <c>null</c> if the player is not currently in a match.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network issues.
		/// </exception>
		public async Task<CurrentGameMatch?> GetCurrentGameMatchAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);
			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<CurrentGameMatch>(Url,
					$"/core-game/v1/matches/{matchId}").ConfigureAwait(false);
		}

		/// <summary>
		/// Retrieves player loadouts for the current game.
		/// </summary>
		/// <returns>
		/// A <see cref="GameLoadout"/> instance describing the current match loadouts,
		/// or <c>null</c> if the player is not currently in a match.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network issues.
		/// </exception>
		public async Task<GameLoadout?> GetCurrentGameLoadoutsAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);

			return matchId.IsNullOrEmpty()
				? null
				: await initiator.ExternalSystem.Net.GetAsync<GameLoadout>(Url, $"/core-game/v1/matches/{matchId}").ConfigureAwait(false);
		}

		/// <summary>
		/// Forces the player to leave the currently active game.
		/// </summary>
		/// <remarks>
		/// This operation disassociates the player from the current match and should
		/// be used with caution, as it may result in penalties or unexpected behavior.
		/// </remarks>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network issues.
		/// </exception>
		public async Task QuitCurrentGameAsync()
		{
			string? matchId = await GetCurrentGameMatchIdAsync().ConfigureAwait(false);

			if (!matchId.IsNullOrEmpty())
				await initiator.ExternalSystem.Net
					.PostAsync(Url, $"/core-game/v1/players/{initiator.Client.UserId}/disassociate/{matchId}")
					.ConfigureAwait(false);
		}

		/// <summary>
		/// Retrieves the current session information for the authenticated player.
		/// </summary>
		/// <returns>
		/// A <see cref="CurrentSession"/> instance describing the active session,
		/// or <c>null</c> if no session is active.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request fails due to authentication or network issues.
		/// </exception>
		public async Task<CurrentSession?> GetCurrentSession() => await initiator.ExternalSystem.Net.GetAsync<CurrentSession>(Url, $"/session/v1/sessions/{initiator.Client.UserId}").ConfigureAwait(false);

		/// <summary>
		/// Retrieves player loadouts for the current game.
		/// </summary>
		/// <remarks>
		/// This method is obsolete due to ambiguous naming.  
		/// Use <see cref="GetCurrentGameLoadoutsAsync"/> instead.
		/// </remarks>
		[Obsolete("Ambiguous spelling, please use newer method 'GetCurrentGameLoadoutsAsync'")]
		public async Task<GameLoadout?> GetCurrentGameLoadoutAsync() => await GetCurrentGameLoadoutsAsync().ConfigureAwait(false);
	}
}