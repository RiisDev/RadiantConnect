namespace RadiantConnect.ValorantApi
{
	public static class Ceremonies
	{
		public static async Task<CeremoniesData?> GetCeremoniesAsync() => await ValorantApiClient.GetAsync<CeremoniesData>("https://valorant-api.com/v1", "/ceremonies");

		public static async Task<CeremonyData?> GetCeremonyAsync(string uuid) => await ValorantApiClient.GetAsync<CeremonyData>("https://valorant-api.com/v1", $"/ceremonies/{uuid}");

		public static async Task<CeremonyData?> GetCeremonyByUuidAsync(string uuid) => await GetCeremonyAsync(uuid);

		public record CeremonyDatum(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record CeremoniesData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<CeremonyDatum> Data
		);

		public record CeremonyData(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] CeremonyDatum Data
		);
	}
}
