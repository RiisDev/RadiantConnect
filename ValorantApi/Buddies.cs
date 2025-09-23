namespace RadiantConnect.ValorantApi
{
	public static class Buddies
	{
		public static async Task<BuddiesData?> GetBuddiesAsync() => await ValorantApiClient.GetAsync<BuddiesData?>("https://valorant-api.com/v1", "/buddies").ConfigureAwait(false);

		public static async Task<BuddyLevel?> GetBuddyLevelsAsync() => await ValorantApiClient.GetAsync<BuddyLevel?>("https://valorant-api.com/v1", "/buddies/levels").ConfigureAwait(false);

		public static async Task<BuddyData?> GetBuddyAsync(string uuid) => await ValorantApiClient.GetAsync<BuddyData?>("https://valorant-api.com/v1", $"/buddies/{uuid}").ConfigureAwait(false);
		
		public static async Task<BuddyData?> GetBuddyByUuidAsync(string uuid) => await GetBuddyAsync(uuid).ConfigureAwait(false);

		public static async Task<BuddyLevelsData?> GetBuddyLevelByLevelUuidAsync(string uuid) => await ValorantApiClient.GetAsync<BuddyLevelsData?>("https://valorant-api.com/v1", $"/buddies/levels/{uuid}").ConfigureAwait(false);

		public record BuddyDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("isHiddenIfNotOwned")] bool? IsHiddenIfNotOwned,
			[property: JsonPropertyName("themeUuid")] string ThemeUuid,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath,
			[property: JsonPropertyName("levels")] IReadOnlyList<BuddyLevel> Levels
		);

		public record BuddyLevel(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("charmLevel")] int? CharmLevel,
			[property: JsonPropertyName("hideIfNotOwned")] bool? HideIfNotOwned,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record BuddiesData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<BuddyDatum> Data
		);

		public record BuddyData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] BuddyDatum Data
		);

		public record BuddyLevelsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] BuddyLevel Data
		);
	}
}
