using System.Text.Json.Serialization;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record Match(
    [property: JsonPropertyName("MatchID")] string MatchID,
    [property: JsonPropertyName("MapID")] string MapID,
    [property: JsonPropertyName("SeasonID")] string SeasonID,
    [property: JsonPropertyName("MatchStartTime")] object MatchStartTime,
    [property: JsonPropertyName("TierAfterUpdate")] long? TierAfterUpdate,
    [property: JsonPropertyName("TierBeforeUpdate")] long? TierBeforeUpdate,
    [property: JsonPropertyName("RankedRatingAfterUpdate")] long? RankedRatingAfterUpdate,
    [property: JsonPropertyName("RankedRatingBeforeUpdate")] long? RankedRatingBeforeUpdate,
    [property: JsonPropertyName("RankedRatingEarned")] long? RankedRatingEarned,
    [property: JsonPropertyName("RankedRatingPerformanceBonus")] long? RankedRatingPerformanceBonus,
    [property: JsonPropertyName("CompetitiveMovement")] string CompetitiveMovement,
    [property: JsonPropertyName("AFKPenalty")] long? AFKPenalty
);

public record CompetitiveUpdate(
    [property: JsonPropertyName("Version")] long? Version,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Matches")] IReadOnlyList<Match> Matches
);