
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record MatchHistoryInternal(
		[property: JsonPropertyName("MatchID")] string MatchID,
		[property: JsonPropertyName("GameStartTime")] object GameStartTime,
		[property: JsonPropertyName("QueueID")] string QueueID
	);

	public record MatchHistory(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("BeginIndex")] long? BeginIndex,
		[property: JsonPropertyName("EndIndex")] long? EndIndex,
		[property: JsonPropertyName("Total")] long? Total,
		[property: JsonPropertyName("History")] IReadOnlyList<MatchHistoryInternal> History
	);
}