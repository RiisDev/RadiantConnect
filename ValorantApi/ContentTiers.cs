namespace RadiantConnect.ValorantApi
{
	public static class ContentTiers
	{
		public static async Task<ContentTiersData?> GetContentTiersAsync() => await ValorantApiClient.GetAsync<ContentTiersData>("https://valorant-api.com/v1", "contenttiers");

		public static async Task<ContentTierData?> GetContentTierAsync(string uuid) => await ValorantApiClient.GetAsync<ContentTierData>("https://valorant-api.com/v1", $"contenttiers/{uuid}");

		public static async Task<ContentTierData?> GetContentTierByUuidAsync(string uuid) => await GetContentTierAsync(uuid);

		public record ContentDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("devName")] string DevName,
			[property: JsonPropertyName("rank")] int? Rank,
			[property: JsonPropertyName("juiceValue")] int? JuiceValue,
			[property: JsonPropertyName("juiceCost")] int? JuiceCost,
			[property: JsonPropertyName("highlightColor")] string HighlightColor,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record ContentTiersData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<ContentDatum> Data
		);

		public record ContentTierData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] ContentDatum Data
		);

	}
}
