using System.Text.Json.Serialization;

namespace RadiantConnect.Network.CurrentGameEndpoints.DataTypes;

// ReSharper disable All

public record ConnectionDetails(
    [property: JsonPropertyName("GameServerHosts")] IReadOnlyList<string> GameServerHosts,
    [property: JsonPropertyName("GameServerHost")] string GameServerHost,
    [property: JsonPropertyName("GameServerPort")] long GameServerPort,
    [property: JsonPropertyName("GameServerObfuscatedIP")] long GameServerObfuscatedIP,
    [property: JsonPropertyName("GameClientHash")] long GameClientHash,
    [property: JsonPropertyName("PlayerKey")] string PlayerKey
);

public record Player(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("TeamID")] string TeamID,
    [property: JsonPropertyName("CharacterID")] string CharacterID,
    [property: JsonPropertyName("PlayerIdentity")] PlayerIdentity PlayerIdentity,
    [property: JsonPropertyName("SeasonalBadgeInfo")] SeasonalBadgeInfo SeasonalBadgeInfo,
    [property: JsonPropertyName("IsCoach")] bool IsCoach,
    [property: JsonPropertyName("IsAssociated")] bool IsAssociated
);

public record PlayerIdentity(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
    [property: JsonPropertyName("PlayerTitleID")] string PlayerTitleID,
    [property: JsonPropertyName("AccountLevel")] long AccountLevel,
    [property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderID,
    [property: JsonPropertyName("Incognito")] bool Incognito,
    [property: JsonPropertyName("HideAccountLevel")] bool HideAccountLevel
);

public record CurrentGameMatch(
    [property: JsonPropertyName("MatchID")] string MatchID,
    [property: JsonPropertyName("Version")] long Version,
    [property: JsonPropertyName("State")] string State,
    [property: JsonPropertyName("MapID")] string MapID,
    [property: JsonPropertyName("ModeID")] string ModeID,
    [property: JsonPropertyName("ProvisioningFlow")] string ProvisioningFlow,
    [property: JsonPropertyName("GamePodID")] string GamePodID,
    [property: JsonPropertyName("AllMUCName")] string AllMUCName,
    [property: JsonPropertyName("TeamMUCName")] string TeamMUCName,
    [property: JsonPropertyName("TeamVoiceID")] string TeamVoiceID,
    [property: JsonPropertyName("TeamMatchToken")] string TeamMatchToken,
    [property: JsonPropertyName("IsReconnectable")] bool IsReconnectable,
    [property: JsonPropertyName("ConnectionDetails")] ConnectionDetails ConnectionDetails,
    [property: JsonPropertyName("PostGameDetails")] object PostGameDetails,
    [property: JsonPropertyName("Players")] IReadOnlyList<Player> Players,
    [property: JsonPropertyName("MatchmakingData")] object MatchmakingData
);

public record SeasonalBadgeInfo(
    [property: JsonPropertyName("SeasonID")] string SeasonID,
    [property: JsonPropertyName("NumberOfWins")] long NumberOfWins,
    [property: JsonPropertyName("WinsByTier")] object WinsByTier,
    [property: JsonPropertyName("Rank")] long Rank,
    [property: JsonPropertyName("LeaderboardRank")] long LeaderboardRank
);

