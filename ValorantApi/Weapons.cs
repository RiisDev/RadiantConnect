using System.Text.Json.Serialization;

namespace RadiantConnect.ValorantApi
{
    public static class Weapons
    {
        public static async Task<WeaponsData?> GetWeaponsAsync() =>
            await ValorantApiClient.GetAsync<WeaponsData?>("https://valorant-api.com/v1", "/weapons");

        public static async Task<WeaponSkinsData?> GetWeaponSkinsAsync() =>
            await ValorantApiClient.GetAsync<WeaponSkinsData?>("https://valorant-api.com/v1", "/weapons/skins");

        public static async Task<WeaponSkinChromasData?> GetWeaponSkinChromasAsync() =>
            await ValorantApiClient.GetAsync<WeaponSkinChromasData?>("https://valorant-api.com/v1", "/weapons/skinchromas");

        public static async Task<WeaponSkinLevelsData?> GetWeaponSkinLevelsAsync() =>
            await ValorantApiClient.GetAsync<WeaponSkinLevelsData?>("https://valorant-api.com/v1", "/weapons/skinlevels");

        public static async Task<WeaponData?> GetWeaponByUuidAsync(string weaponUuid) =>
            await ValorantApiClient.GetAsync<WeaponData?>("https://valorant-api.com/v1", $"/weapons/{weaponUuid}");

        public static async Task<WeaponSkinData?> GetWeaponSkinByUuidAsync(string weaponSkinUuid) =>
            await ValorantApiClient.GetAsync<WeaponSkinData?>("https://valorant-api.com/v1", $"/weapons/skins/{weaponSkinUuid}");

        public static async Task<WeaponSkinChromaData?> GetWeaponSkinChromaByUuidAsync(string weaponSkinChromaUuid) =>
            await ValorantApiClient.GetAsync<WeaponSkinChromaData?>("https://valorant-api.com/v1", $"/weapons/skinchromas/{weaponSkinChromaUuid}");

        public static async Task<WeaponSkinLevelData?> GetWeaponSkinLevelByUuidAsync(string weaponSkinLevelUuid) =>
            await ValorantApiClient.GetAsync<WeaponSkinLevelData?>("https://valorant-api.com/v1", $"/weapons/skinlevels/{weaponSkinLevelUuid}");

        public record WeaponsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<WeaponDatum> Data
        );

        public record WeaponData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] WeaponDatum Data
        );

        public record WeaponSkinsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<SkinDatum> Data
        );

        public record WeaponSkinData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] SkinDatum Data
        );

        public record WeaponSkinChromasData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<ChromaDatum> Data
        );

        public record WeaponSkinChromaData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] ChromaDatum Data
        );

        public record WeaponSkinLevelsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<SkinLevelDatum> Data
        );

        public record WeaponSkinLevelData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] SkinLevelDatum Data
        );

        public record SkinDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("themeUuid")] string ThemeUuid,
            [property: JsonPropertyName("contentTierUuid")] string ContentTierUuid,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("wallpaper")] string Wallpaper,
            [property: JsonPropertyName("assetPath")] string AssetPath,
            [property: JsonPropertyName("chromas")] IReadOnlyList<ChromaDatum> Chromas,
            [property: JsonPropertyName("levels")] IReadOnlyList<SkinLevelDatum> Levels
        );

        public record ChromaDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("fullRender")] string FullRender,
            [property: JsonPropertyName("swatch")] string Swatch,
            [property: JsonPropertyName("streamedVideo")] string StreamedVideo,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record SkinLevelDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("levelItem")] string LevelItem,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("streamedVideo")] string StreamedVideo,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record AdsStats(
           [property: JsonPropertyName("zoomMultiplier")] double? ZoomMultiplier,
           [property: JsonPropertyName("fireRate")] double? FireRate,
           [property: JsonPropertyName("runSpeedMultiplier")] double? RunSpeedMultiplier,
           [property: JsonPropertyName("burstCount")] int? BurstCount,
           [property: JsonPropertyName("firstBulletAccuracy")] double? FirstBulletAccuracy
        );

        public record AirBurstStats(
            [property: JsonPropertyName("shotgunPelletCount")] int? ShotgunPelletCount,
            [property: JsonPropertyName("burstDistance")] double? BurstDistance
        );

        public record AltShotgunStats(
            [property: JsonPropertyName("shotgunPelletCount")] int? ShotgunPelletCount,
            [property: JsonPropertyName("burstRate")] double? BurstRate
        );
        
        public record DamageRange(
            [property: JsonPropertyName("rangeStartMeters")] int? RangeStartMeters,
            [property: JsonPropertyName("rangeEndMeters")] int? RangeEndMeters,
            [property: JsonPropertyName("headDamage")] double? HeadDamage,
            [property: JsonPropertyName("bodyDamage")] int? BodyDamage,
            [property: JsonPropertyName("legDamage")] double? LegDamage
        );

        public record WeaponDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("category")] string Category,
            [property: JsonPropertyName("defaultSkinUuid")] string DefaultSkinUuid,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("killStreamIcon")] string KillStreamIcon,
            [property: JsonPropertyName("assetPath")] string AssetPath,
            [property: JsonPropertyName("weaponStats")] WeaponStats WeaponStats,
            [property: JsonPropertyName("shopData")] ShopData ShopData,
            [property: JsonPropertyName("skins")] IReadOnlyList<SkinDatum> Skins
        );

        public record GridPosition(
            [property: JsonPropertyName("row")] int? Row,
            [property: JsonPropertyName("column")] int? Column
        );
        
        public record ShopData(
            [property: JsonPropertyName("cost")] int? Cost,
            [property: JsonPropertyName("category")] string Category,
            [property: JsonPropertyName("shopOrderPriority")] int? ShopOrderPriority,
            [property: JsonPropertyName("categoryText")] string CategoryText,
            [property: JsonPropertyName("gridPosition")] GridPosition GridPosition,
            [property: JsonPropertyName("canBeTrashed")] bool? CanBeTrashed,
            [property: JsonPropertyName("image")] object Image,
            [property: JsonPropertyName("newImage")] string NewImage,
            [property: JsonPropertyName("newImage2")] object NewImage2,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record WeaponStats(
            [property: JsonPropertyName("fireRate")] double? FireRate,
            [property: JsonPropertyName("magazineSize")] int? MagazineSize,
            [property: JsonPropertyName("runSpeedMultiplier")] double? RunSpeedMultiplier,
            [property: JsonPropertyName("equipTimeSeconds")] double? EquipTimeSeconds,
            [property: JsonPropertyName("reloadTimeSeconds")] double? ReloadTimeSeconds,
            [property: JsonPropertyName("firstBulletAccuracy")] double? FirstBulletAccuracy,
            [property: JsonPropertyName("shotgunPelletCount")] int? ShotgunPelletCount,
            [property: JsonPropertyName("wallPenetration")] string WallPenetration,
            [property: JsonPropertyName("feature")] string Feature,
            [property: JsonPropertyName("fireMode")] string FireMode,
            [property: JsonPropertyName("altFireType")] string AltFireType,
            [property: JsonPropertyName("adsStats")] AdsStats AdsStats,
            [property: JsonPropertyName("altShotgunStats")] AltShotgunStats AltShotgunStats,
            [property: JsonPropertyName("airBurstStats")] AirBurstStats AirBurstStats,
            [property: JsonPropertyName("damageRanges")] IReadOnlyList<DamageRange> DamageRanges
        );
    }
}
