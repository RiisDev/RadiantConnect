using System.Globalization;
using RadiantConnect.Methods;
using RadiantConnect.Network;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.RConnect.DataTypes;
using Player = RadiantConnect.RConnect.DataTypes.Player;
using Stats = RadiantConnect.RConnect.DataTypes.Stats;

namespace RadiantConnect.RConnect
{
	internal static class InternalMethods
	{
		// This is hell, I want to kill myself
		internal static long GetPlayerFirstBloods(string puuid, IReadOnlyList<RoundResult> rounds)
		{
			long firstBloods = 0;

			foreach (RoundResult round in rounds)
			{
				long earliestKillTime = long.MaxValue;
				string topPuuid = string.Empty;

				foreach (PlayerStat playerStat in round.PlayerStats)
				{
					if (playerStat.Kills.Count == 0) continue;

					foreach (Kill kill in playerStat.Kills)
					{
						if (!(earliestKillTime > kill.RoundTime)) continue;

						earliestKillTime = kill.RoundTime.Value;
						topPuuid = playerStat.Subject;
					}
				}

				if (topPuuid == puuid) firstBloods++;
			}

			return firstBloods;
		}

		internal static long GetPlayerEconomyRating(string puuid, IReadOnlyList<RoundResult> rounds)
		{
			double totalDamage = 0.0;
			double totalSpent = 0.0;

			foreach (RoundResult round in rounds)
			{
				PlayerStat playerStat = round.PlayerStats.First(stat => stat.Subject == puuid);

				totalSpent += playerStat.Economy.Spent ?? 0 + .0;
				totalDamage += playerStat.DamageInternal.Sum(damage => damage.DamageInternal ?? 0.0);
			}

			double calculation = Math.Floor(totalDamage / (totalSpent / 1000));
			long parsedResult = long.Parse(calculation.ToString(CultureInfo.InvariantCulture), StringExtensions.CultureInfo);
			return parsedResult;
		}

		internal static Player ParsePlayerData(Network.PVPEndpoints.DataTypes.Player player, IReadOnlyList<RoundResult> rounds, string winningTeam)
		{
			string puuid = player.Subject;
			string gameName = player.GameName;
			string tagLine = player.TagLine;
			string character = ValorantTables.AgentIdToAgent[player.CharacterId.ToLower(StringExtensions.CultureInfo)];
			bool wonGame = player.TeamId == winningTeam;
			int accountLevel = int.Parse(player.AccountLevel?.ToString(StringExtensions.CultureInfo) ?? "0", StringExtensions.CultureInfo);

			double averageCombatScore = Math.Floor((player.Stats.Score + .0 ?? 1.0) / (player.Stats.RoundsPlayed + .0 ?? 1.0));
			long acsParsed = long.Parse(averageCombatScore.ToString(StringExtensions.CultureInfo), StringExtensions.CultureInfo);

			Stats stats = new(
				player.Stats.RoundsPlayed ?? 0,
				player.Stats.Kills ?? 0,
				player.Stats.Assists ?? 0,
				player.Stats.Deaths ?? 0,
				acsParsed,
				rounds.Count(round => round.BombPlanter == puuid),
				rounds.Count(round => round.BombDefuser == puuid),
				GetPlayerEconomyRating(puuid, rounds),
				GetPlayerFirstBloods(puuid, rounds)
			);

			AbilityCasts casts = new(
				player.Stats.AbilityCasts.GrenadeCasts,
				player.Stats.AbilityCasts.Ability1Casts,
				player.Stats.AbilityCasts.Ability2Casts,
				player.Stats.AbilityCasts.UltimateCasts
			);

			return new Player(puuid, gameName, tagLine, player.TeamId, character, wonGame, accountLevel, stats, casts);
		}
	}

	/// <summary>
	/// Provides high-level helper methods for interacting with Riot/Valorant services,
	/// including process checks and player/match information retrieval.
	/// </summary>
	public static class RConnectMethods
	{
		/// <summary>
		/// Determines whether the Valorant game process is currently running.
		/// </summary>
		/// <returns>True if the Valorant process is running; otherwise, false.</returns>
		public static bool IsValorantRunning() => InternalValorantMethods.IsValorantProcessRunning();

		/// <summary>
		/// Determines whether the Riot Client process is currently running.
		/// </summary>
		/// <returns>True if the Riot Client process is running; otherwise, false.</returns>
		public static bool IsRiotClientRunning() => InternalValorantMethods.IsRiotClientRunning();

		/// <summary>
		///	Provides extension to the initiator class.
		/// </summary>
		extension(Initiator initiator)
		{
			/// <summary>
			/// Retrieves the Riot ID (GameName#TagLine) for a given player UUID (puuid).
			/// </summary>
			/// <param name="puuid">The unique player identifier.</param>
			/// <returns>The Riot ID string or null if not found.</returns>
			public async Task<string?> GetRiotIdByPuuidAsync(string puuid)
			{
				List<NameService>? serviceResponse = await initiator.Endpoints.PvpEndpoints.FetchNameServiceReturn(puuid).ConfigureAwait(false);
		   
				if (serviceResponse == null) return null;
				if (serviceResponse.Count == 0) return null;

				NameService userId = serviceResponse[0];

				return $"{userId.GameName}#{userId.TagLine}";
			}

			/// <summary>
			/// Retrieves the puuid for a player given their in-game name and tag line.
			/// </summary>
			/// <param name="gameName">The player's in-game name.</param>
			/// <param name="tagLine">The player's tag line/discriminator.</param>
			/// <returns>The player's puuid or null if not found.</returns>
			/// <exception cref="InvalidOperationException">Thrown if local endpoints are unavailable.</exception>
			public async Task<string?> GetPuuidByNameAsync(string gameName, string tagLine)
			{
				if (initiator.Endpoints.LocalEndpoints is null)
					throw new InvalidOperationException("Endpoint is only available during local usage");

				string? aliasReturn = await initiator.Endpoints.LocalEndpoints.PerformLocalRequestAsync(ValorantNet.HttpMethod.Get, $"/player-account/aliases/v1/lookup?gameName={gameName}&tagLine={tagLine}").ConfigureAwait(false);

				string? parsedId = aliasReturn?[(aliasReturn.LastIndexOf(':') + 2)..^3];

				return parsedId;
			}

			/// <summary>
			/// Retrieves the player's current competitive rank in Valorant.
			/// </summary>
			/// <param name="puuid">The player's unique identifier.</param>
			/// <returns>The player's rank as a string, or "Unranked" if unavailable.</returns>
			public async Task<string?> GetValorantRankAsync(string puuid)
			{
				PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid).ConfigureAwait(false);
			
				string? seasonId = await FetchCurrentSeasonIdAsync(initiator).ConfigureAwait(false);
				if (playerMmr is null || seasonId is null or "" || playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID is null) return "Unranked";
		   
				return !playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID.TryGetValue(seasonId,
					out SeasonId? seasonData)
					? "Unranked"
					: ValorantTables.TierToRank[seasonData.CompetitiveTier ?? 0];
			}

			/// <summary>
			/// Retrieves the player's current rank rating points in competitive queue.
			/// </summary>
			/// <param name="puuid">The player's unique identifier.</param>
			/// <returns>The player's rank rating as an integer, or 0 if unavailable.</returns>
			public async Task<int?> GetCurrentRankRatingAsync(string puuid)
			{
				PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid).ConfigureAwait(false);

				string? seasonId = await FetchCurrentSeasonIdAsync(initiator).ConfigureAwait(false);

				if (playerMmr is null || seasonId is null or "" || playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID is null) return 0;
				try
				{
					if (playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID.TryGetValue(seasonId, out SeasonId? seasonData))
						return seasonData.RankedRating.HasValue ? int.Parse(seasonData.RankedRating.Value.ToString(StringExtensions.CultureInfo), StringExtensions.CultureInfo) : 0;
				}catch {/**/}

				return 0;
			}

			/// <summary>
			/// Retrieves recent match statistics for a given player.
			/// </summary>
			/// <param name="puuid">The player's unique identifier.</param>
			/// <returns>A list of <see cref="MatchStats"/> objects for the player's recent matches.</returns>
			public async Task<List<MatchStats?>> GetRecentMatchStatsAsync(string puuid)
			{
				List<MatchStats?> stats = [];
				MatchHistory? matchHistory = await initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryAsync(puuid).ConfigureAwait(false);

				if (matchHistory == null || matchHistory.History.Count == 0) return stats;

				foreach (MatchHistoryInternal matchData in matchHistory.History)
					stats.Add(await GetMatchLeaderboardAsync(initiator, matchData.MatchId).ConfigureAwait(false));

				return stats;
			}

			/// <summary>
			/// Retrieves the leaderboard information for the last match played by the player.
			/// </summary>
			/// <param name="puuid">The player's unique identifier.</param>
			/// <param name="competitiveOnly">If true, only considers competitive matches.</param>
			/// <returns>The last match's <see cref="MatchStats"/>, or null if unavailable.</returns>
			public async Task<MatchStats?> GetLastMatchLeaderboardAsync(string puuid, bool competitiveOnly = false)
			{
				MatchHistory? matchHistory = await initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryAsync(puuid).ConfigureAwait(false);

				if (matchHistory == null) return null;
				if (matchHistory.History.Count == 0) return null;

				MatchHistoryInternal match = matchHistory.History[0];

				try
				{
					if (competitiveOnly)
						match = matchHistory.History.First(matchData => matchData.QueueId == "competitive");
				}
				catch (InvalidOperationException) { return null;}

				return await GetMatchLeaderboardAsync(initiator, match.MatchId).ConfigureAwait(false);
			}

			/// <summary>
			/// Retrieves the leaderboard information for a specific match by ID.
			/// </summary>
			/// <param name="matchId">The unique identifier of the match.</param>
			/// <returns>The <see cref="MatchStats"/> object representing the match leaderboard, or null if unavailable.</returns>
			public async Task<MatchStats?> GetMatchLeaderboardAsync(string matchId)
			{
				MatchInfo? matchInfo = await initiator.Endpoints.PvpEndpoints.FetchMatchInfoAsync(matchId).ConfigureAwait(false);
				if (matchInfo == null) return null;

				List<Player> playerList = [];
				MatchInfoInternal internalInfo = matchInfo.MatchInfoInternal;
				string mapId = internalInfo.MapId;
				string parsedMapName = mapId[(mapId.LastIndexOf('/')+1)..];
				string properMapName = ValorantTables.InternalMapNames[parsedMapName];
				string properQueueId = ValorantTables.InternalGameModeToGameMode[internalInfo.QueueId];
				string winningTeam = matchInfo.Teams.First(team => team.Won == true).TeamId;

				playerList.AddRange(matchInfo.Players.Select(player => InternalMethods.ParsePlayerData(player, matchInfo.RoundResults, winningTeam)));

				return new MatchStats(
					internalInfo.MatchId,
					properMapName,
					internalInfo.GamePodId,
					properQueueId,
					internalInfo.SeasonId,
					winningTeam,
					playerList
				);
			}

			/// <summary>
			/// Retrieves the current active Valorant season ID.
			/// </summary>
			/// <returns>The current season ID as a string, or empty string if unavailable.</returns>
			public async Task<string?> FetchCurrentSeasonIdAsync()
			{
				Content? content = await initiator.Endpoints.PvpEndpoints.FetchContentAsync().ConfigureAwait(false);
				string? seasonId;
				try {
					seasonId = content?.Seasons.First(x => x.IsActive.HasValue && x.IsActive.Value && x.Type == "act").Id;
				} catch {
					seasonId = string.Empty;
				}

				return seasonId;
			}
		}

	}
}
