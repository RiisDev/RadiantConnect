// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record Match(
		[property: JsonPropertyName("MatchID")] string MatchId,
		[property: JsonPropertyName("MapID")] string MapId,
		[property: JsonPropertyName("SeasonID")] string SeasonId,
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
}