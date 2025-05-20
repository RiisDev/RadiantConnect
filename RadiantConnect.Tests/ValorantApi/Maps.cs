using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RadiantConnect.Tests.ValorantApi
{
    internal class Maps
    {
        public static async Task<List<MapsData>?> GetMaps()
        {
            MapRoot? mapRoot = await Client.Get<MapRoot>("maps");
            return mapRoot?.Data.ToList();
        }
        
        public record Callout(
            [property: JsonPropertyName("regionName")] string RegionName,
            [property: JsonPropertyName("superRegionName")] string SuperRegionName,
            [property: JsonPropertyName("location")] Location Location
        );

        public record MapsData(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("narrativeDescription")] object NarrativeDescription,
            [property: JsonPropertyName("tacticalDescription")] string TacticalDescription,
            [property: JsonPropertyName("coordinates")] string Coordinates,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("listViewIcon")] string ListViewIcon,
            [property: JsonPropertyName("listViewIconTall")] string ListViewIconTall,
            [property: JsonPropertyName("splash")] string Splash,
            [property: JsonPropertyName("stylizedBackgroundImage")] string StylizedBackgroundImage,
            [property: JsonPropertyName("premierBackgroundImage")] string PremierBackgroundImage,
            [property: JsonPropertyName("assetPath")] string AssetPath,
            [property: JsonPropertyName("mapUrl")] string MapUrl,
            [property: JsonPropertyName("xMultiplier")] double? XMultiplier,
            [property: JsonPropertyName("yMultiplier")] double? YMultiplier,
            [property: JsonPropertyName("xScalarToAdd")] double? XScalarToAdd,
            [property: JsonPropertyName("yScalarToAdd")] double? YScalarToAdd,
            [property: JsonPropertyName("callouts")] IReadOnlyList<Callout> Callouts
        );

        public record Location(
            [property: JsonPropertyName("x")] double? X,
            [property: JsonPropertyName("y")] double? Y
        );

        public record MapRoot(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<MapsData> Data
        );


    }
}
