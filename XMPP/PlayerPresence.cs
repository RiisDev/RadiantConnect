namespace RadiantConnect.XMPP;

public record PlayerPresence(
    string ChatServer,
    string LobbyServer,
    string Platform,
    string RiotId,
    string TagLine,
    Dictionary<string, string> Platforms,
    ValorantPresence Presence
);