using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Ceremonies
    {
        public static async Task<CeremoniesData?> GetCeremonies() => await ValorantApiClient.GetAsync<CeremoniesData>("https://valorant-api.com/v1", "/ceremonies");

        public static async Task<CeremonyData?> GetCeremony(string uuid) => await ValorantApiClient.GetAsync<CeremonyData>("https://valorant-api.com/v1", $"/ceremonies/{uuid}");

        public static async Task<CeremonyData?> GetCeremonyByUuid(string uuid) => await GetCeremony(uuid);

        public record Data(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record CeremoniesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record CeremonyData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] Data Data
        );
    }
}
