using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RadiantConnect.ValorantApi
{
    public static class ContentTiers
    {
        public static async Task<ContentTiersData?> GetContentTiers() => await ValorantApiClient.GetAsync<ContentTiersData>("https://valorant-api.com/v1", "contenttiers");

        public static async Task<ContentTierData?> GetContentTier(string uuid) => await ValorantApiClient.GetAsync<ContentTierData>("https://valorant-api.com/v1", $"contenttiers/{uuid}");

        public static async Task<ContentTierData?> GetContentTierByUuid(string uuid) => await GetContentTier(uuid);

        public record Data(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("devName")] string DevName,
            [property: JsonPropertyName("rank")] int? Rank,
            [property: JsonPropertyName("juiceValue")] int? JuiceValue,
            [property: JsonPropertyName("juiceCost")] int? JuiceCost,
            [property: JsonPropertyName("highlightColor")] string HighlightColor,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record ContentTiersData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record ContentTierData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] Data Data
        );

    }
}
