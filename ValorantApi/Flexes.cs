namespace RadiantConnect.ValorantApi
{
    public static class Flexes
    {
        public static async Task<FlexesData?> GetFlexesAsync() => await ValorantApiClient.GetAsync<FlexesData?>("https://valorant-api.com/v1", "flex");

        public static async Task<FlexData?> GetFlexAsync(string uuid) => await ValorantApiClient.GetAsync<FlexData?>("https://valorant-api.com/v1", $"flex/{uuid}");

        public static async Task<FlexData?> GetFlexByUuidAsync(string uuid) => await GetFlexAsync(uuid);

        public record FlexDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayNameAllCaps")] string DisplayNameAllCaps,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record FlexesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<FlexDatum> Data
        );

        public record FlexData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] FlexDatum Data
        );
    }  
}
