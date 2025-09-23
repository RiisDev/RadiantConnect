namespace RadiantConnect.ValorantApi
{
	public static class Gears
	{
		public static async Task<GearsData?> GetGearsAsync() => await ValorantApiClient.GetAsync<GearsData?>("https://valorant-api.com/v1", "gear").ConfigureAwait(false);

		public static async Task<GearData?> GetGearAsync(string uuid) => await ValorantApiClient.GetAsync<GearData?>("https://valorant-api.com/v1", $"gear/{uuid}").ConfigureAwait(false);

		public static async Task<GearData?> GetGearByUuidAsync(string uuid) => await GetGearAsync(uuid).ConfigureAwait(false);

		public record GearDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("description")] string Description,
			[property: JsonPropertyName("descriptions")] IReadOnlyList<string> Descriptions,
			[property: JsonPropertyName("details")] IReadOnlyList<Detail> Details,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath,
			[property: JsonPropertyName("shopData")] ShopData ShopData
		);

		public record Detail(
			[property: JsonPropertyName("name")] string Name,
			[property: JsonPropertyName("value")] string Value
		);


		public record ShopData(
			[property: JsonPropertyName("cost")] int? Cost,
			[property: JsonPropertyName("category")] string Category,
			[property: JsonPropertyName("shopOrderPriority")] int? ShopOrderPriority,
			[property: JsonPropertyName("categoryText")] string CategoryText,
			[property: JsonPropertyName("gridPosition")] object GridPosition,
			[property: JsonPropertyName("canBeTrashed")] bool? CanBeTrashed,
			[property: JsonPropertyName("image")] object Image,
			[property: JsonPropertyName("newImage")] string NewImage,
			[property: JsonPropertyName("newImage2")] object NewImage2,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record GearsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<GearDatum> Data
		);
		public record GearData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] GearDatum Data
		);
	}  
}
