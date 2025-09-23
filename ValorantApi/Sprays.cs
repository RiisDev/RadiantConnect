namespace RadiantConnect.ValorantApi
{
	public static class Sprays
	{
		public static async Task<SpraysData?> GetSpraysAsync() => await ValorantApiClient.GetAsync<SpraysData?>("https://valorant-api.com/v1", "/sprays").ConfigureAwait(false);

		public static async Task<LevelsData?> GetSprayLevelsAsync() => await ValorantApiClient.GetAsync<LevelsData?>("https://valorant-api.com/v1", "/sprays/levels").ConfigureAwait(false);

		public static async Task<SprayData?> GetSprayAsync(string uuid) => await ValorantApiClient.GetAsync<SprayData?>("https://valorant-api.com/v1", $"/sprays/{uuid}").ConfigureAwait(false);

		public static async Task<SprayData?> GetSprayByUuidAsync(string uuid) => await GetSprayAsync(uuid).ConfigureAwait(false);

		public static async Task<LevelData?> GetSprayLevelByLevelUuidAsync(string uuid) => await ValorantApiClient.GetAsync<LevelData?>("https://valorant-api.com/v1", $"/sprays/levels/{uuid}").ConfigureAwait(false);

		public record SprayDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("category")] string Category,
			[property: JsonPropertyName("themeUuid")] string ThemeUuid,
			[property: JsonPropertyName("isNullSpray")] bool? IsNullSpray,
			[property: JsonPropertyName("hideIfNotOwned")] bool? HideIfNotOwned,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("fullIcon")] string FullIcon,
			[property: JsonPropertyName("fullTransparentIcon")] string FullTransparentIcon,
			[property: JsonPropertyName("animationPng")] string AnimationPng,
			[property: JsonPropertyName("animationGif")] string AnimationGif,
			[property: JsonPropertyName("assetPath")] string AssetPath,
			[property: JsonPropertyName("levels")] IReadOnlyList<Level> Levels
		);

		public record Level(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("sprayLevel")] int? SprayLevel,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record SpraysData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<SprayDatum> Data
		);

		public record SprayData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] SprayDatum Data
		);

		public record LevelsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<Level> Data
		);

		public record LevelData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] Level Data
		);
	}  
}
