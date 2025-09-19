namespace RadiantConnect.ValorantApi
{
	public static class Maps
	{
		public static async Task<MapsData?> GetMapsAsync() => await ValorantApiClient.GetAsync<MapsData?>("https://valorant-api.com/v1", "maps");

		public static async Task<MapData?> GetMapAsync(string uuid) => await ValorantApiClient.GetAsync<MapData?>("https://valorant-api.com/v1", $"maps/{uuid}");

		public static async Task<MapData?> GetMapByUuidAsync(string uuid) => await GetMapAsync(uuid);

		public record Callout(
			[property: JsonPropertyName("regionName")] string RegionName,
			[property: JsonPropertyName("superRegionName")] string SuperRegionName,
			[property: JsonPropertyName("location")] Location Location
		);

		public record MapDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("narrativeDescription")] object NarrativeDescription,
			[property: JsonPropertyName("tacticalDescription")] string TacticalDescription,
			[property: JsonPropertyName("coordinates")] string Coordinates,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("listViewIcon")] string ListViewIcon,
			[property: JsonPropertyName("listViewIconTall")] string ListViewIconTall,
			[property: JsonPropertyName("splash")] string Splash,
			[property: JsonPropertyName("stylizedBackgroundImage")] string StylizedBackgroundImage,
			[property: JsonPropertyName("premierBackgroundImage")] string PremierBackgroundImage,
			[property: JsonPropertyName("assetPath")] string AssetPath,
			[property: JsonPropertyName("mapUrl")] string MapUrl,
			[property: JsonPropertyName("xMultiplier")] double? XMultiplier,
			[property: JsonPropertyName("yMultiplier")] double? YMultiplier,
			[property: JsonPropertyName("xScalarToAdd")] double? XScalarToAdd,
			[property: JsonPropertyName("yScalarToAdd")] double? YScalarToAdd,
			[property: JsonPropertyName("callouts")] IReadOnlyList<Callout> Callouts
		);

		public record Location(
			[property: JsonPropertyName("x")] double? X,
			[property: JsonPropertyName("y")] double? Y
		);

		public record MapsData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<MapDatum> Data
		);

		public record MapData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] MapDatum Data
		);
	}  
}
