using System.Text.Json.Serialization;
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record MatchHistoryInternal(
    [property: JsonPropertyName("MatchID")] string MatchID,
    [property: JsonPropertyName("GameStartTime")] object GameStartTime,
    [property: JsonPropertyName("QueueID")] string QueueID
);

public record MatchHistory(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("BeginIndex")] int? BeginIndex,
    [property: JsonPropertyName("EndIndex")] int? EndIndex,
    [property: JsonPropertyName("Total")] int? Total,
    [property: JsonPropertyName("History")] IReadOnlyList<MatchHistoryInternal> History
);