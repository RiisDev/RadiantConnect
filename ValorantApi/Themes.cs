using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Themes
    {
        public static async Task<ThemesData?> GetThemesAsync() => await ValorantApiClient.GetAsync<ThemesData?>("https://valorant-api.com/v1", "themes");

        public static async Task<ThemeData?> GetThemeAsync(string uuid) => await ValorantApiClient.GetAsync<ThemeData?>("https://valorant-api.com/v1", $"themes/{uuid}");

        public static async Task<ThemeData?> GetThemeByUuidAsync(string uuid) => await GetThemeAsync(uuid);

        public record ThemeDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("storeFeaturedImage")] object StoreFeaturedImage,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record ThemesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<ThemeDatum> Data
        );


        public record ThemeData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] ThemeDatum Data
        );

    }
}
