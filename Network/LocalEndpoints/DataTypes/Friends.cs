#pragma warning disable CA1716
namespace RadiantConnect.Network.LocalEndpoints.DataTypes
{
	// ReSharper disable All

	public record Friend(
		[property: JsonPropertyName("activePlatform")] string ActivePlatform,
		[property: JsonPropertyName("displayGroup")] string DisplayGroup,
		[property: JsonPropertyName("game_name")] string GameName,
		[property: JsonPropertyName("game_tag")] string GameTag,
		[property: JsonPropertyName("group")] string Group,
		[property: JsonPropertyName("last_online_ts")] long? LastOnlineTs,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("note")] string Note,
		[property: JsonPropertyName("pid")] string Pid,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("region")] string Region
	);

	public record InternalFriends(
		[property: JsonPropertyName("friends")] IReadOnlyList<Friend> Friends
	);
}

