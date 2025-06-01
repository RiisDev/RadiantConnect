using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Buddies
    {
        public static async Task<BuddiesData?> GetBuddies() => await ValorantApiClient.GetAsync<BuddiesData?>("https://valorant-api.com/v1", "/buddies");

        public static async Task<BuddyLevel?> GetBuddyLevels() => await ValorantApiClient.GetAsync<BuddyLevel?>("https://valorant-api.com/v1", "/buddies/levels");

        public static async Task<BuddyData?> GetBuddy(string uuid) => await ValorantApiClient.GetAsync<BuddyData?>("https://valorant-api.com/v1", $"/buddies/{uuid}");
        
        public static async Task<BuddyData?> GetBuddyByUuid(string uuid) => await GetBuddy(uuid);

        public static async Task<BuddyLevelsData?> GetBuddyLevelByLevelUuid(string uuid) => await ValorantApiClient.GetAsync<BuddyLevelsData?>("https://valorant-api.com/v1", $"/buddies/levels/{uuid}");

        public record Data(
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
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record BuddyData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] Data Data
        );

        public record BuddyLevelsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] BuddyLevel Data
        );
    }
}
