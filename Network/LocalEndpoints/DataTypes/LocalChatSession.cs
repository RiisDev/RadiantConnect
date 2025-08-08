namespace RadiantConnect.Network.LocalEndpoints.DataTypes
{
	// ReSharper disable All
	public record LocalChatSession(
		[property: JsonPropertyName("federated")] bool? Federated,
		[property: JsonPropertyName("game_name")] string GameName,
		[property: JsonPropertyName("game_tag")] string GameTag,
		[property: JsonPropertyName("loaded")] bool? Loaded,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("pid")] string Pid,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("region")] string Region,
		[property: JsonPropertyName("resource")] string Resource,
		[property: JsonPropertyName("state")] string State
	);
}