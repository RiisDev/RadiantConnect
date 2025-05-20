using System.Text.Json.Serialization;

namespace RadiantConnect.Tests.ValorantApi
{
    internal class Weapons
    {
        public static async Task<List<WeaponData>?> GetWeapons()
        {
            WeaponRoot? root = await Client.Get<WeaponRoot>("weapons");
            return root?.Data.ToList();
        }

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

        public record Chroma(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("fullRender")] string FullRender,
            [property: JsonPropertyName("swatch")] string Swatch,
            [property: JsonPropertyName("streamedVideo")] string StreamedVideo,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record DamageRange(
            [property: JsonPropertyName("rangeStartMeters")] int? RangeStartMeters,
            [property: JsonPropertyName("rangeEndMeters")] int? RangeEndMeters,
            [property: JsonPropertyName("headDamage")] double? HeadDamage,
            [property: JsonPropertyName("bodyDamage")] int? BodyDamage,
            [property: JsonPropertyName("legDamage")] double? LegDamage
        );

        public record WeaponData(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("category")] string Category,
            [property: JsonPropertyName("defaultSkinUuid")] string DefaultSkinUuid,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("killStreamIcon")] string KillStreamIcon,
            [property: JsonPropertyName("assetPath")] string AssetPath,
            [property: JsonPropertyName("weaponStats")] WeaponStats WeaponStats,
            [property: JsonPropertyName("shopData")] ShopData ShopData,
            [property: JsonPropertyName("skins")] IReadOnlyList<Skin> Skins
        );

        public record GridPosition(
            [property: JsonPropertyName("row")] int? Row,
            [property: JsonPropertyName("column")] int? Column
        );

        public record Level(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("levelItem")] string LevelItem,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("streamedVideo")] string StreamedVideo,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record WeaponRoot(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<WeaponData> Data
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

        public record Skin(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("themeUuid")] string ThemeUuid,
            [property: JsonPropertyName("contentTierUuid")] string ContentTierUuid,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("wallpaper")] string Wallpaper,
            [property: JsonPropertyName("assetPath")] string AssetPath,
            [property: JsonPropertyName("chromas")] IReadOnlyList<Chroma> Chromas,
            [property: JsonPropertyName("levels")] IReadOnlyList<Level> Levels
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
