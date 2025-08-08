﻿namespace RadiantConnect.SocketServices.XMPP.DataTypes
{
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