namespace RadiantConnect.Network.EsportsEndpoints.DataTypes
{
	public record MatchDestination(
		[property: JsonPropertyName("StructuralID")] string StructuralId,
		[property: JsonPropertyName("Type")] string Type,
		[property: JsonPropertyName("Slot")] long Slot
	);

	public record MatchTeam(
		[property: JsonPropertyName("TeamID")] string TeamId,
		[property: JsonPropertyName("OriginStructuralID")] string OriginStructuralId,
		[property: JsonPropertyName("OriginType")] string OriginType,
		[property: JsonPropertyName("OriginSlot")] long OriginSlot,
		[property: JsonPropertyName("Result")] string Result,
		[property: JsonPropertyName("GameWins")] long GameWins
	);

	public record EsportsMatch(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("LeagueID")] string LeagueId,
		[property: JsonPropertyName("TournamentID")] string TournamentId,
		[property: JsonPropertyName("StageID")] string StageId,
		[property: JsonPropertyName("StageName")] string StageName,
		[property: JsonPropertyName("StructuralID")] string StructuralId,
		[property: JsonPropertyName("StartTime")] string StartTime,
		[property: JsonPropertyName("State")] string State,
		[property: JsonPropertyName("Destinations")] Dictionary<string, MatchDestination> Destinations,
		[property: JsonPropertyName("MatchTeams")] IReadOnlyList<MatchTeam> MatchTeams,
		[property: JsonPropertyName("Streams")] IReadOnlyList<object> Streams
	);

	public record EsportsMatches(
		[property: JsonPropertyName("Matches")] IReadOnlyList<EsportsMatch> Matches
	);
}
