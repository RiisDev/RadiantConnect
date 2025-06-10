using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Gamemodes
    {
        public static async Task<GamemodesData?> GetGamemodesAsync() => await ValorantApiClient.GetAsync<GamemodesData?>("https://valorant-api.com/v1", "gamemodes");

        public static async Task<GamemodeData?> GetGamemodeAsync(string uuid) => await ValorantApiClient.GetAsync<GamemodeData?>("https://valorant-api.com/v1", $"gamemodes/{uuid}");

        public static async Task<GamemodeData?> GetGamemodeByUuidAsync(string uuid) => await GetGamemodeAsync(uuid);

        public record GamemodeDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("description")] string Description,
            [property: JsonPropertyName("duration")] string Duration,
            [property: JsonPropertyName("economyType")] string EconomyType,
            [property: JsonPropertyName("allowsMatchTimeouts")] bool? AllowsMatchTimeouts,
            [property: JsonPropertyName("isTeamVoiceAllowed")] bool? IsTeamVoiceAllowed,
            [property: JsonPropertyName("isMinimapHidden")] bool? IsMinimapHidden,
            [property: JsonPropertyName("orbCount")] int? OrbCount,
            [property: JsonPropertyName("roundsPerHalf")] int? RoundsPerHalf,
            [property: JsonPropertyName("teamRoles")] IReadOnlyList<string> TeamRoles,
            [property: JsonPropertyName("gameFeatureOverrides")] IReadOnlyList<GameFeatureOverride> GameFeatureOverrides,
            [property: JsonPropertyName("gameRuleBoolOverrides")] IReadOnlyList<GameRuleBoolOverride> GameRuleBoolOverrides,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("listViewIconTall")] string ListViewIconTall,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record GameFeatureOverride(
            [property: JsonPropertyName("featureName")] string FeatureName,
            [property: JsonPropertyName("state")] bool? State
        );

        public record GameRuleBoolOverride(
            [property: JsonPropertyName("ruleName")] string RuleName,
            [property: JsonPropertyName("state")] bool? State
        );

        public record GamemodesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<GamemodeDatum> Data
        );
        public record GamemodeData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] GamemodeDatum Data
        );
    }  
}
