namespace RadiantConnect.Network.EsportsEndpoints.DataTypes
{
	public record League(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Name")] string Name,
		[property: JsonPropertyName("ImageURL")] string ImageUrl,
		[property: JsonPropertyName("TournamentIDs")] IReadOnlyList<string> TournamentIds,
		[property: JsonPropertyName("TeamIDs")] IReadOnlyList<string>? TeamIds
	);

	public record Tournament(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Name")] string Name,
		[property: JsonPropertyName("LeagueID")] string LeagueId,
		[property: JsonPropertyName("LeagueName")] string LeagueName,
		[property: JsonPropertyName("StartTime")] string StartTime,
		[property: JsonPropertyName("EndTime")] string EndTime,
		[property: JsonPropertyName("StageIDs")] IReadOnlyList<string> StageIds,
		[property: JsonPropertyName("TeamIDs")] IReadOnlyList<string> TeamIds
	);

	public record EsportsTeam(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Name")] string Name,
		[property: JsonPropertyName("Code")] string Code,
		[property: JsonPropertyName("BaseImageURL")] string BaseImageUrl,
		[property: JsonPropertyName("HighResImageURL")] string HighResImageUrl,
		[property: JsonPropertyName("LowResImageURL")] string LowResImageUrl,
		[property: JsonPropertyName("HomeLeagueID")] string HomeLeagueId,
		[property: JsonPropertyName("TeamMemberIDs")] IReadOnlyList<string>? TeamMemberIds,
		[property: JsonPropertyName("BundleID")] string BundleId,
		[property: JsonPropertyName("BundleDataAssetID")] string BundleDataAssetId,
		[property: JsonPropertyName("DataAssetID")] string DataAssetId
	);

	public record UpcomingMatches(
		[property: JsonPropertyName("Leagues")] IReadOnlyList<League> Leagues,
		[property: JsonPropertyName("Tournaments")] IReadOnlyList<Tournament> Tournaments,
		[property: JsonPropertyName("Teams")] IReadOnlyList<EsportsTeam> Teams
	);
}
