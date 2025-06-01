using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class CompetitiveTiers
    {
        public static async Task<CompetitiveTiersData?> GetCompetitiveTiers() => await ValorantApiClient.GetAsync<CompetitiveTiersData>("https://valorant-api.com/v1", "competitivetiers");

        public static async Task<CompetitiveTierData?> GetCompetitiveTier(string uuid) => await ValorantApiClient.GetAsync<CompetitiveTierData>("https://valorant-api.com/v1", $"competitivetiers/{uuid}");

        public static async Task<CompetitiveTierData?> GetCompetitiveTierByUuid(string uuid) => await GetCompetitiveTier(uuid);

        public static async Task<CompetitiveTierData?> GetTierByUuid(string uuid) => await GetCompetitiveTier(uuid);

        public record Data(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("assetObjectName")] string AssetObjectName,
            [property: JsonPropertyName("tiers")] IReadOnlyList<TierData> Tiers,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record CompetitiveTiersData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record CompetitiveTierData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record TierData(
            [property: JsonPropertyName("tier")] int? Tier,
            [property: JsonPropertyName("tierName")] string TierName,
            [property: JsonPropertyName("division")] string Division,
            [property: JsonPropertyName("divisionName")] string DivisionName,
            [property: JsonPropertyName("color")] string Color,
            [property: JsonPropertyName("backgroundColor")] string BackgroundColor,
            [property: JsonPropertyName("smallIcon")] string SmallIcon,
            [property: JsonPropertyName("largeIcon")] string LargeIcon,
            [property: JsonPropertyName("rankTriangleDownIcon")] string RankTriangleDownIcon,
            [property: JsonPropertyName("rankTriangleUpIcon")] string RankTriangleUpIcon
        );


    }
}
