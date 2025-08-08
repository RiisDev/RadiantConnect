namespace RadiantConnect.ValorantApi
{
    public static class PlayerCards
    {
        public static async Task<PlayerCardsData?> GetPlayerCardsAsync() => await ValorantApiClient.GetAsync<PlayerCardsData?>("https://valorant-api.com/v1", "playercards");

        public static async Task<PlayerCardData?> GetPlayerCardAsync(string uuid) => await ValorantApiClient.GetAsync<PlayerCardData?>("https://valorant-api.com/v1", $"playercards/{uuid}");

        public static async Task<PlayerCardData?> GetPlayerCardByUuidAsync(string uuid) => await GetPlayerCardAsync(uuid);

        public record PlayerCardDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("isHiddenIfNotOwned")] bool? IsHiddenIfNotOwned,
            [property: JsonPropertyName("themeUuid")] object ThemeUuid,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("smallArt")] string SmallArt,
            [property: JsonPropertyName("wideArt")] string WideArt,
            [property: JsonPropertyName("largeArt")] string LargeArt,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record PlayerCardsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<PlayerCardDatum> Data
        );

        public record PlayerCardData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] PlayerCardDatum Data
        );
    }  
}
