// ReSharper disable All

using System.Text.Json.Serialization;

namespace RadiantConnect.Network.CurrentGameEndpoints.DataTypes;

public record CurrentGamePlayer(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("MatchID")] string MatchId,
    [property: JsonPropertyName("Version")] long Version
);