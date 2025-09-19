namespace RadiantConnect.ValorantApi
{
	public static class PlayerTitles
	{
		public static async Task<PlayerTitlesData?> GetPlayerTitlesAsync() => await ValorantApiClient.GetAsync<PlayerTitlesData?>("https://valorant-api.com/v1", "playertitles");

		public static async Task<PlayerTitleData?> GetPlayerTitleAsync(string uuid) => await ValorantApiClient.GetAsync<PlayerTitleData?>("https://valorant-api.com/v1", $"playertitles/{uuid}");

		public static async Task<PlayerTitleData?> GetPlayerTitleByUuidAsync(string uuid) => await GetPlayerTitleAsync(uuid);

		public record PlayerTitleDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("titleText")] string TitleText,
			[property: JsonPropertyName("isHiddenIfNotOwned")] bool? IsHiddenIfNotOwned,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record PlayerTitlesData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<PlayerTitleDatum> Data
		);
		public record PlayerTitleData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] PlayerTitleDatum Data
		);
	}  
}
