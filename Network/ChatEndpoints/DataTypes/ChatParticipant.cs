namespace RadiantConnect.Network.ChatEndpoints.DataTypes
{
	public record Participant(
		[property: JsonPropertyName("activePlatform")] object ActivePlatform,
		[property: JsonPropertyName("cid")] string Cid,
		[property: JsonPropertyName("game_name")] string GameName,
		[property: JsonPropertyName("game_tag")] string GameTag,
		[property: JsonPropertyName("muted")] bool Muted,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("pid")] string Pid,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("region")] string Region
	);

	public record ChatParticipant(
		[property: JsonPropertyName("participants")] IReadOnlyList<Participant> Participants
	);
}

