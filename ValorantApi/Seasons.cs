namespace RadiantConnect.ValorantApi
{
	public static class Seasons
	{
		public static async Task<SeasonsData?> GetSeasonsAsync() =>
			await ValorantApiClient.GetAsync<SeasonsData?>("https://valorant-api.com/v1", "/seasons");

		public static async Task<CompetitiveSeasonsData?> GetCompetitiveSeasonsAsync() =>
			await ValorantApiClient.GetAsync<CompetitiveSeasonsData?>("https://valorant-api.com/v1", "/seasons/competitive");

		public static async Task<SeasonData?> GetSeasonByUuidAsync(string seasonUuid) =>
			await ValorantApiClient.GetAsync<SeasonData?>("https://valorant-api.com/v1", $"/seasons/{seasonUuid}");

		public static async Task<CompetitiveSeasonData?> GetCompetitiveSeasonByUuidAsync(string competitiveSeasonUuid) =>
			await ValorantApiClient.GetAsync<CompetitiveSeasonData?>("https://valorant-api.com/v1", $"/seasons/competitive/{competitiveSeasonUuid}");


		public record SeasonDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("title")] string Title,
			[property: JsonPropertyName("type")] string Type,
			[property: JsonPropertyName("startTime")] DateTime? StartTime,
			[property: JsonPropertyName("endTime")] DateTime? EndTime,
			[property: JsonPropertyName("parentUuid")] string ParentUuid,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record Border(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("level")] int? Level,
			[property: JsonPropertyName("winsRequired")] int? WinsRequired,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("smallIcon")] string SmallIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record CompetitiveSeasonDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("startTime")] DateTime? StartTime,
			[property: JsonPropertyName("endTime")] DateTime? EndTime,
			[property: JsonPropertyName("seasonUuid")] string SeasonUuid,
			[property: JsonPropertyName("competitiveTiersUuid")] string CompetitiveTiersUuid,
			[property: JsonPropertyName("borders")] IReadOnlyList<Border> Borders,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record CompetitiveSeasonsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<CompetitiveSeasonDatum> Data
		);

		public record CompetitiveSeasonData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] CompetitiveSeasonDatum Data
		);

		public record SeasonsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<SeasonDatum> Data
		);

		public record SeasonData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] SeasonDatum Data
		);
	}  
}
