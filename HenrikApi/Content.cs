using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace RadiantConnect.HenrikApi
{
    public class Content(HenrikClient henrikClient)
    {
        public async Task<ContentData?> GetContentAsync() => await henrikClient.GetAsync<ContentData?>("/valorant/v1/content");

        public record Act(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("isActive")] bool? IsActive
        );

        public record Character(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record Charm(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record CharmLevel(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record Chroma(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record Equip(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record GameMode(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public record LocalizedName(
            [property: JsonPropertyName("ar-AE")] string ArAE,
            [property: JsonPropertyName("de-DE")] string DeDE,
            [property: JsonPropertyName("en-GB")] string EnGB,
            [property: JsonPropertyName("en-US")] string EnUS,
            [property: JsonPropertyName("es-ES")] string EsES,
            [property: JsonPropertyName("es-MX")] string EsMX,
            [property: JsonPropertyName("fr-FR")] string FrFR,
            [property: JsonPropertyName("id-ID")] string IdID,
            [property: JsonPropertyName("it-IT")] string ItIT,
            [property: JsonPropertyName("ja-JP")] string JaJP,
            [property: JsonPropertyName("ko-KR")] string KoKR,
            [property: JsonPropertyName("pl-PL")] string PlPL,
            [property: JsonPropertyName("pt-BR")] string PtBR,
            [property: JsonPropertyName("ru-RU")] string RuRU,
            [property: JsonPropertyName("th-TH")] string ThTH,
            [property: JsonPropertyName("tr-TR")] string TrTR,
            [property: JsonPropertyName("vi-VN")] string ViVN,
            [property: JsonPropertyName("zn-CN")] string ZnCN,
            [property: JsonPropertyName("zn-TW")] string ZnTW
        );

        public record Map(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record PlayerCard(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record PlayerTitle(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record ContentData(
            [property: JsonPropertyName("version")] string Version,
            [property: JsonPropertyName("characters")] IReadOnlyList<Character> Characters,
            [property: JsonPropertyName("maps")] IReadOnlyList<Map> Maps,
            [property: JsonPropertyName("chromas")] IReadOnlyList<Chroma> Chromas,
            [property: JsonPropertyName("skins")] IReadOnlyList<Skin> Skins,
            [property: JsonPropertyName("skinLevels")] IReadOnlyList<SkinLevel> SkinLevels,
            [property: JsonPropertyName("equips")] IReadOnlyList<Equip> Equips,
            [property: JsonPropertyName("gameModes")] IReadOnlyList<GameMode> GameModes,
            [property: JsonPropertyName("sprays")] IReadOnlyList<Spray> Sprays,
            [property: JsonPropertyName("sprayLevels")] IReadOnlyList<SprayLevel> SprayLevels,
            [property: JsonPropertyName("charms")] IReadOnlyList<Charm> Charms,
            [property: JsonPropertyName("charmLevels")] IReadOnlyList<CharmLevel> CharmLevels,
            [property: JsonPropertyName("playerCards")] IReadOnlyList<PlayerCard> PlayerCards,
            [property: JsonPropertyName("playerTitles")] IReadOnlyList<PlayerTitle> PlayerTitles,
            [property: JsonPropertyName("acts")] IReadOnlyList<Act> Acts
        );

        public record Skin(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record SkinLevel(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record Spray(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record SprayLevel(
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("localizedNames")] IReadOnlyList<LocalizedName> LocalizedNames,
            [property: JsonPropertyName("id")] string Id,
            [property: JsonPropertyName("assetName")] string AssetName,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );


    }
}
