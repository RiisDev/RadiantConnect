namespace RadiantConnect.ValorantApi
{
    public static class Bundles
    {
        public static async Task<BundlesData?> GetBundlesAsync() => await ValorantApiClient.GetAsync<BundlesData?>("https://valorant-api.com/v1", "/bundles");
        public static async Task<BundleData?> GetBundleAsync(string uuid) => await ValorantApiClient.GetAsync<BundleData?>("https://valorant-api.com/v1", $"/bundles/{uuid}");
        public static async Task<BundleData?> GetBundleByUuidAsync(string uuid) => await GetBundleAsync(uuid);

        public record BundleDatum(
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
            [property: JsonPropertyName("data")] IReadOnlyList<BundleDatum> Data
        );

        public record BundleData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] BundleDatum Data
        );


    }
}
