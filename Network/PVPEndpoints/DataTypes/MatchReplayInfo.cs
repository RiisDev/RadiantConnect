namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record MatchReplayInfo(
		[property: JsonPropertyName("total")] long? Total,
		[property: JsonPropertyName("matchFileUrlsMap")] Dictionary<string, string> MatchFileUrlsMap
	);
}
