namespace RadiantConnect.ValorantApi
{
	public static class CompetitiveTiers
	{
		public static async Task<CompetitiveTiersData?> GetCompetitiveTiersAsync() => await ValorantApiClient.GetAsync<CompetitiveTiersData>("https://valorant-api.com/v1", "competitivetiers").ConfigureAwait(false);

		public static async Task<CompetitiveTierData?> GetCompetitiveTierAsync(string uuid) => await ValorantApiClient.GetAsync<CompetitiveTierData>("https://valorant-api.com/v1", $"competitivetiers/{uuid}").ConfigureAwait(false);

		public static async Task<CompetitiveTierData?> GetCompetitiveTierByUuidAsync(string uuid) => await GetCompetitiveTierAsync(uuid).ConfigureAwait(false);

		public static async Task<CompetitiveTierData?> GetTierByUuidAsync(string uuid) => await GetCompetitiveTierAsync(uuid).ConfigureAwait(false);

		public record CompetitiveDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("assetObjectName")] string AssetObjectName,
			[property: JsonPropertyName("tiers")] IReadOnlyList<TierData> Tiers,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record CompetitiveTiersData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<CompetitiveDatum> Data
		);

		public record CompetitiveTierData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<CompetitiveDatum> Data
		);

		public record TierData(
			[property: JsonPropertyName("tier")] int? Tier,
			[property: JsonPropertyName("tierName")] string TierName,
			[property: JsonPropertyName("division")] string Division,
			[property: JsonPropertyName("divisionName")] string DivisionName,
			[property: JsonPropertyName("color")] string Color,
			[property: JsonPropertyName("backgroundColor")] string BackgroundColor,
			[property: JsonPropertyName("smallIcon")] string SmallIcon,
			[property: JsonPropertyName("largeIcon")] string LargeIcon,
			[property: JsonPropertyName("rankTriangleDownIcon")] string RankTriangleDownIcon,
			[property: JsonPropertyName("rankTriangleUpIcon")] string RankTriangleUpIcon
		);


	}
}
