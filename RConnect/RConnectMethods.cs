using System.Globalization;
using RadiantConnect.Methods;
using RadiantConnect.Network;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.RConnect.DataTypes;
using Player = RadiantConnect.RConnect.DataTypes.Player;
using Stats = RadiantConnect.RConnect.DataTypes.Stats;

namespace RadiantConnect.RConnect
{
    public static class InternalMethods
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
            long parsedResult = long.Parse(calculation.ToString(CultureInfo.InvariantCulture));
            return parsedResult;
        }

        internal static Player ParsePlayerData(Network.PVPEndpoints.DataTypes.Player player, IReadOnlyList<RoundResult> rounds, string winningTeam)
        {
            string puuid = player.Subject;
            string gameName = player.GameName;
            string tagLine = player.TagLine;
            string character = ValorantTables.AgentIdToAgent[player.CharacterId.ToLower()];
            bool wonGame = player.TeamId == winningTeam;
            int accountLevel = int.Parse(player.AccountLevel?.ToString() ?? "0");

            double averageCombatScore = Math.Floor((player.Stats.Score + .0 ?? 1.0) / (player.Stats.RoundsPlayed + .0 ?? 1.0));
            long acsParsed = long.Parse(averageCombatScore.ToString(CultureInfo.InvariantCulture));

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

    public static class RConnectMethods
    {
        public static bool IsValorantRunning() => InternalValorantMethods.IsValorantProcessRunning();
        public static bool IsRiotClientRunning() => InternalValorantMethods.IsRiotClientRunning();

        public static async Task<string?> GetRiotIdByPuuidAsync(this Initiator initiator, string puuid)
        {
            NameService? serviceResponse = await initiator.Endpoints.PvpEndpoints.FetchNameServiceReturn(puuid);
           
            return $"{serviceResponse?.GameName}#{serviceResponse?.TagLine}";
        }

        public static async Task<string?> GetPuuidByNameAsync(this Initiator initiator, string gameName, string tagLine)
        {
            string? aliasReturn = await initiator.Endpoints.LocalEndpoints.PerformLocalRequestAsync(ValorantNet.HttpMethod.Get, $"/player-account/aliases/v1/lookup?gameName={gameName}&tagLine={tagLine}");

            string? parsedId = aliasReturn?[(aliasReturn.LastIndexOf(':') + 2)..^3];

            return parsedId;
        }

		#pragma warning disable IDE0046
		public static async Task<string?> GetValorantRankAsync(this Initiator initiator, string puuid)
        {
            PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid);
            
            string? seasonId = await FetchCurrentSeasonIdAsync(initiator);
            if (playerMmr is null || seasonId is null or "" || playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID is null) return "Unranked";
           
            return !playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID.TryGetValue(seasonId,
                out SeasonId? seasonData)
                ? "Unranked"
                : ValorantTables.TierToRank[seasonData.CompetitiveTier ?? 0];
        }

        public static async Task<int?> GetCurrentRankRatingAsync(this Initiator initiator, string puuid)
        {
            PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid);

            string? seasonId = await FetchCurrentSeasonIdAsync(initiator);

            if (playerMmr is null || seasonId is null or "" || playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID is null) return 0;
            try
            {
                if (playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID.TryGetValue(seasonId,
                        out SeasonId? seasonData))
                {
                    return seasonData.RankedRating.HasValue ? int.Parse(seasonData.RankedRating.Value.ToString()) : 0;
                }
            }catch {/**/}

            return 0;
        }

        public static async Task<List<MatchStats?>> GetRecentMatchStatsAsync(this Initiator initiator, string puuid)
        {
            List<MatchStats?> stats = [];
            MatchHistory? matchHistory = await initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryAsync(puuid);

            if (matchHistory == null) return stats;
            if (matchHistory.History.Count == 0) return stats;

            foreach (MatchHistoryInternal matchData in matchHistory.History)
                stats.Add(await GetMatchLeaderboardAsync(initiator, matchData.MatchID));

            return stats;
        }

        public static async Task<MatchStats?> GetLastMatchLeaderboardAsync(this Initiator initiator, string puuid, bool competitiveOnly = false)
        {
            MatchHistory? matchHistory = await initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryAsync(puuid);

            if (matchHistory == null) return null;
            if (matchHistory.History.Count == 0) return null;

            MatchHistoryInternal match = matchHistory.History[0];

            try
            {
                if (competitiveOnly)
                    match = matchHistory.History.First(matchData => matchData.QueueID == "competitive");
            }
            catch (InvalidOperationException) { return null;}

            return await GetMatchLeaderboardAsync(initiator, match.MatchID);
        }

        public static async Task<MatchStats?> GetMatchLeaderboardAsync(this Initiator initiator, string matchId)
        {
            MatchInfo? matchInfo = await initiator.Endpoints.PvpEndpoints.FetchMatchInfoAsync(matchId);
            if (matchInfo == null) return null;

            List<Player> playerList = [];
            MatchInfoInternal internalInfo = matchInfo.MatchInfoInternal;
            string mapId = internalInfo.MapId;
            string parsedMapName = mapId[(mapId.LastIndexOf('/')+1)..];
            string properMapName = ValorantTables.InternalMapNames[parsedMapName];
            string properQueueId = ValorantTables.InternalGameModeToGameMode[internalInfo.QueueID];
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

        public static async Task<string?> FetchCurrentSeasonIdAsync(this Initiator initiator)
        {
            Content? content = await initiator.Endpoints.PvpEndpoints.FetchContentAsync();
            string? seasonId;
            try {
                seasonId = content?.Seasons.First(x => (x.IsActive.HasValue && x.IsActive.Value) && x.Type == "act").ID;
            } catch {
                seasonId = string.Empty;
            }

            return seasonId;
            
        }
    }
}
