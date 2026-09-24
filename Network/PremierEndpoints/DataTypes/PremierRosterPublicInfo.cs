namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PointDeltas(
		[property: JsonPropertyName("before")] int Before,
		[property: JsonPropertyName("after")] int After,
		[property: JsonPropertyName("earned")] int Earned
	);

	public record PublicRosterMatchEntry(
		[property: JsonPropertyName("matchId")] string MatchId,
		[property: JsonPropertyName("seasonId")] string SeasonId,
		[property: JsonPropertyName("eventId")] string EventId,
		[property: JsonPropertyName("startTime")] long StartTime,
		[property: JsonPropertyName("pointDeltas")] PointDeltas? PointDeltas,
		[property: JsonPropertyName("outcome")] string Outcome,
		[property: JsonPropertyName("opponentRosterID")] string? OpponentRosterId
	);

	public record PublicRosterTournamentEntry(
		[property: JsonPropertyName("tournamentId")] string TournamentId,
		[property: JsonPropertyName("placement")] int Placement,
		[property: JsonPropertyName("startTime")] long StartTime,
		[property: JsonPropertyName("pointDeltas")] PointDeltas? PointDeltas,
		[property: JsonPropertyName("matches")] IReadOnlyDictionary<string, TournamentMatchData>? Matches
	);

	public record PublicRosterSeasonInfo(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("isEnrolled")] bool IsEnrolled,
		[property: JsonPropertyName("conference")] string Conference,
		[property: JsonPropertyName("division")] int Division,
		[property: JsonPropertyName("isProvisionalDivision")] bool IsProvisionalDivision,
		[property: JsonPropertyName("promotionApplied")] bool PromotionApplied,
		[property: JsonPropertyName("points")] int Points,
		[property: JsonPropertyName("wins")] int Wins,
		[property: JsonPropertyName("gamesPlayed")] int GamesPlayed,
		[property: JsonPropertyName("seasonMatchRoundWins")] int SeasonMatchRoundWins,
		[property: JsonPropertyName("seasonMatchRoundsPlayed")] int SeasonMatchRoundsPlayed,
		[property: JsonPropertyName("crest")] string? Crest,
		[property: JsonPropertyName("plating")] string? Plating,
		[property: JsonPropertyName("matches")] IReadOnlyDictionary<string, PublicRosterMatchEntry>? Matches,
		[property: JsonPropertyName("tournaments")] IReadOnlyDictionary<string, PublicRosterTournamentEntry>? Tournaments,
		[property: JsonPropertyName("hasEarnedPromotionForNextSeason")] bool HasEarnedPromotionForNextSeason,
		[property: JsonPropertyName("hasEarnedPrestige")] bool HasEarnedPrestige
	);

	public record PremierRosterPublicInfo(
		[property: JsonPropertyName("rosterId")] string RosterId,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("tag")] string Tag,
		[property: JsonPropertyName("customization")] RosterCustomization? Customization,
		[property: JsonPropertyName("members")] IReadOnlyList<RosterMember>? Members,
		[property: JsonPropertyName("season")] PublicRosterSeasonInfo? Season,
		[property: JsonPropertyName("version")] RosterVersion? Version,
		[property: JsonPropertyName("createdAt")] long CreatedAt,
		[property: JsonPropertyName("seasonalInfoBySeasonID")] IReadOnlyDictionary<string, PublicRosterSeasonInfo>? SeasonalInfoBySeasonId
	);
}
