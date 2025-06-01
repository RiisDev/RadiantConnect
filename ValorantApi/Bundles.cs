using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Bundles
    {
        public static async Task<BundlesData?> GetBundles() => await ValorantApiClient.GetAsync<BundlesData?>("https://valorant-api.com/v1", "/bundles");
        public static async Task<BundleData?> GetBundle(string uuid) => await ValorantApiClient.GetAsync<BundleData?>("https://valorant-api.com/v1", $"/bundles/{uuid}");
        public static async Task<BundleData?> GetBundleByUuid(string uuid) => await GetBundle(uuid);

        public record Data(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayNameSubText")] string DisplayNameSubText,
            [property: JsonPropertyName("description")] string Description,
            [property: JsonPropertyName("extraDescription")] string ExtraDescription,
            [property: JsonPropertyName("promoDescription")] string PromoDescription,
            [property: JsonPropertyName("useAdditionalContext")] bool? UseAdditionalContext,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("displayIcon2")] string DisplayIcon2,
            [property: JsonPropertyName("logoIcon")] object LogoIcon,
            [property: JsonPropertyName("verticalPromoImage")] string VerticalPromoImage,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record BundlesData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record BundleData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] Data Data
        );


    }
}
