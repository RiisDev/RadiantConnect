namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PremierCrestSeason(
		[property: JsonPropertyName("seasonId")] string SeasonId,
		[property: JsonPropertyName("rosterId")] string RosterId,
		[property: JsonPropertyName("rosterName")] string RosterName,
		[property: JsonPropertyName("rosterTag")] string RosterTag,
		[property: JsonPropertyName("conference")] string Conference,
		[property: JsonPropertyName("division")] int Division,
		[property: JsonPropertyName("points")] int Points,
		[property: JsonPropertyName("crest")] string Crest,
		[property: JsonPropertyName("championshipPointRequirement")] int ChampionshipPointRequirement,
		[property: JsonPropertyName("gamesPlayedByEventType")] IReadOnlyDictionary<string, int>? GamesPlayedByEventType
	);

	public record PremierPlayerCrests(
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("contenderEligibilityExpiry")] string? ContenderEligibilityExpiry,
		[property: JsonPropertyName("seasons")] IReadOnlyDictionary<string, PremierCrestSeason>? Seasons
	);
}
