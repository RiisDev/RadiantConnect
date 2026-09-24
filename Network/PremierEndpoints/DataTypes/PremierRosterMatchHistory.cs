namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record LeagueMatchHistoryEntry(
		[property: JsonPropertyName("matchId")] string MatchId,
		[property: JsonPropertyName("leaguePointsBefore")] int LeaguePointsBefore,
		[property: JsonPropertyName("leaguePointsAfter")] int LeaguePointsAfter,
		[property: JsonPropertyName("leaguePointsEarned")] int LeaguePointsEarned,
		[property: JsonPropertyName("startTime")] long StartTime
	);

	public record TournamentMatchData(
		[property: JsonPropertyName("points")] int Points,
		[property: JsonPropertyName("roundNumber")] int RoundNumber,
		[property: JsonPropertyName("totalRounds")] int TotalRounds,
		[property: JsonPropertyName("bracketType")] string BracketType
	);

	public record TournamentMatchHistoryEntry(
		[property: JsonPropertyName("tournamentId")] string TournamentId,
		[property: JsonPropertyName("finalPlacement")] int FinalPlacement,
		[property: JsonPropertyName("finalPlacementLeaguePointsBonus")] int FinalPlacementLeaguePointsBonus,
		[property: JsonPropertyName("leaguePointsBefore")] int LeaguePointsBefore,
		[property: JsonPropertyName("leaguePointsAfter")] int LeaguePointsAfter,
		[property: JsonPropertyName("leaguePointsEarned")] int LeaguePointsEarned,
		[property: JsonPropertyName("startTime")] long StartTime,
		[property: JsonPropertyName("matchEntries")] IReadOnlyDictionary<string, int>? MatchEntries,
		[property: JsonPropertyName("tournamentMatchData")] IReadOnlyDictionary<string, TournamentMatchData>? TournamentMatchDataById
	);

	public record PremierRosterMatchHistory(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("leagueMatchHistory")] IReadOnlyList<LeagueMatchHistoryEntry>? LeagueMatchHistory,
		[property: JsonPropertyName("tournamentMatchHistory")] IReadOnlyList<TournamentMatchHistoryEntry>? TournamentMatchHistory
	);
}
