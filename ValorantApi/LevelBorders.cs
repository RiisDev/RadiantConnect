using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class LevelBorders
    {
        public static async Task<LevelBordersData?> GetLevelBordersAsync() => await ValorantApiClient.GetAsync<LevelBordersData?>("https://valorant-api.com/v1", "levelborders");

        public static async Task<LevelBorderData?> GetLevelBorderAsync(string uuid) => await ValorantApiClient.GetAsync<LevelBorderData?>("https://valorant-api.com/v1", $"levelborders/{uuid}");

        public static async Task<LevelBorderData?> GetLevelBorderByUuidAsync(string uuid) => await GetLevelBorderAsync(uuid);

        public record LevelBorderDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("startingLevel")] int? StartingLevel,
            [property: JsonPropertyName("levelNumberAppearance")] string LevelNumberAppearance,
            [property: JsonPropertyName("smallPlayerCardAppearance")] string SmallPlayerCardAppearance,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record LevelBordersData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<LevelBorderDatum> Data
        );

        public record LevelBorderData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] LevelBorderDatum Data
        );
    }  
}
