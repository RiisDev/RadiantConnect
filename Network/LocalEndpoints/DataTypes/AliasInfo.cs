namespace RadiantConnect.Network.LocalEndpoints.DataTypes
{
	public record AliasInfo(
		[property: JsonPropertyName("active")] bool? Active,
		[property: JsonPropertyName("created_datetime")] long? CreatedDatetime,
		[property: JsonPropertyName("game_name")] string GameName,
		[property: JsonPropertyName("summoner")] bool? Summoner,
		[property: JsonPropertyName("tag_line")] string TagLine
	);
}