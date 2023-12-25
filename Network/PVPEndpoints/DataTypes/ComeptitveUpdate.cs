using System.Text.Json.Serialization;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record Match(
    [property: JsonPropertyName("MatchID")] string MatchID,
    [property: JsonPropertyName("MapID")] string MapID,
    [property: JsonPropertyName("SeasonID")] string SeasonID,
    [property: JsonPropertyName("MatchStartTime")] object MatchStartTime,
    [property: JsonPropertyName("TierAfterUpdate")] int? TierAfterUpdate,
    [property: JsonPropertyName("TierBeforeUpdate")] int? TierBeforeUpdate,
    [property: JsonPropertyName("RankedRatingAfterUpdate")] int? RankedRatingAfterUpdate,
    [property: JsonPropertyName("RankedRatingBeforeUpdate")] int? RankedRatingBeforeUpdate,
    [property: JsonPropertyName("RankedRatingEarned")] int? RankedRatingEarned,
    [property: JsonPropertyName("RankedRatingPerformanceBonus")] int? RankedRatingPerformanceBonus,
    [property: JsonPropertyName("CompetitiveMovement")] string CompetitiveMovement,
    [property: JsonPropertyName("AFKPenalty")] int? AFKPenalty
);

public record CompetitiveUpdate(
    [property: JsonPropertyName("Version")] int? Version,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Matches")] IReadOnlyList<Match> Matches
);