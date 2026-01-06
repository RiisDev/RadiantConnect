namespace RadiantConnect.SocketServices.XMPP.DataTypes
{
	/// <summary>
	/// Represents presence and identity information for a Valorant player
	/// received through XMPP presence updates.
	/// </summary>
	/// <param name="ChatServer">
	/// The chat server domain associated with the player.
	/// </param>
	/// <param name="LobbyServer">
	/// The lobby server domain associated with the player.
	/// </param>
	/// <param name="Platform">
	/// The primary platform identifier for the player.
	/// </param>
	/// <param name="RiotId">
	/// The player's Riot display name.
	/// </param>
	/// <param name="TagLine">
	/// The Riot ID tagline associated with the player.
	/// </param>
	/// <param name="Puuid">
	/// The player's globally unique Riot PUUID.
	/// </param>
	/// <param name="Platforms">
	/// A mapping of platform identifiers to platform-specific metadata
	/// associated with the player.
	/// </param>
	/// <param name="Presence">
	/// The detailed Valorant presence information for the player.
	/// </param>
	public record PlayerPresence(
		string ChatServer,
		string LobbyServer,
		string Platform,
		string RiotId,
		string TagLine,
		string Puuid,
		Dictionary<string, string> Platforms,
		ValorantPresence Presence
	);
}