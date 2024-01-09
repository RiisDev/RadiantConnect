using System.Text.Json.Serialization;
namespace RadiantConnect.Network.Authorization.DataTypes;
public record Data(
    [property: JsonPropertyName("manifestId")] string ManifestId,
    [property: JsonPropertyName("branch")] string Branch,
    [property: JsonPropertyName("version")] string Version,
    [property: JsonPropertyName("buildVersion")] string BuildVersion,
    [property: JsonPropertyName("engineVersion")] string EngineVersion,
    [property: JsonPropertyName("riotClientVersion")] string RiotClientVersion,
    [property: JsonPropertyName("riotClientBuild")] string RiotClientBuild,
    [property: JsonPropertyName("buildDate")] DateTime? BuildDate
);

public record ClientVersion(
    [property: JsonPropertyName("status")] int? Status,
    [property: JsonPropertyName("data")] Data Data
);

