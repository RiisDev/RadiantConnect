namespace RadiantConnect.ValorantApi
{
    public static class Currencies
    {
        public static async Task<CurrenciesData?> GetCurrenciesAsync() => await ValorantApiClient.GetAsync<CurrenciesData?>("https://valorant-api.com/v1", "currencies");

        public static async Task<CurrencyData?> GetCurrencyAsync(string uuid) => await ValorantApiClient.GetAsync<CurrencyData?>("https://valorant-api.com/v1", $"currencies/{uuid}");

        public static async Task<CurrencyData?> GetCurrencyByUuidAsync(string uuid) => await GetCurrencyAsync(uuid);
       
        public record CurrencyDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayNameSingular")] string DisplayNameSingular,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("largeIcon")] string LargeIcon,
            [property: JsonPropertyName("rewardPreviewIcon")] string RewardPreviewIcon,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record CurrenciesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<CurrencyDatum> Data
        );
        public record CurrencyData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] CurrencyDatum Data
        );
    }  
}
