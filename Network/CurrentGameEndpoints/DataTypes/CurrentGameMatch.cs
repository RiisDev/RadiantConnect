namespace RadiantConnect.Network.CurrentGameEndpoints.DataTypes
{
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
		[property: JsonPropertyName("TeamID")] string TeamId,
		[property: JsonPropertyName("CharacterID")] string CharacterId,
		[property: JsonPropertyName("PlayerIdentity")] PlayerIdentity PlayerIdentity,
		[property: JsonPropertyName("SeasonalBadgeInfo")] SeasonalBadgeInfo SeasonalBadgeInfo,
		[property: JsonPropertyName("IsCoach")] bool IsCoach,
		[property: JsonPropertyName("IsAssociated")] bool IsAssociated
	);

	public record PlayerIdentity(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("PlayerCardID")] string PlayerCardId,
		[property: JsonPropertyName("PlayerTitleID")] string PlayerTitleId,
		[property: JsonPropertyName("AccountLevel")] long AccountLevel,
		[property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderId,
		[property: JsonPropertyName("Incognito")] bool Incognito,
		[property: JsonPropertyName("HideAccountLevel")] bool HideAccountLevel
	);

	public record CurrentGameMatch(
		[property: JsonPropertyName("MatchID")] string MatchId,
		[property: JsonPropertyName("Version")] long Version,
		[property: JsonPropertyName("State")] string State,
		[property: JsonPropertyName("MapID")] string MapId,
		[property: JsonPropertyName("ModeID")] string ModeId,
		[property: JsonPropertyName("ProvisioningFlow")] string ProvisioningFlow,
		[property: JsonPropertyName("GamePodID")] string GamePodId,
		[property: JsonPropertyName("AllMUCName")] string AllMUCName,
		[property: JsonPropertyName("TeamMUCName")] string TeamMUCName,
		[property: JsonPropertyName("TeamVoiceID")] string TeamVoiceId,
		[property: JsonPropertyName("TeamMatchToken")] string TeamMatchToken,
		[property: JsonPropertyName("IsReconnectable")] bool IsReconnectable,
		[property: JsonPropertyName("ConnectionDetails")] ConnectionDetails ConnectionDetails,
		[property: JsonPropertyName("PostGameDetails")] object PostGameDetails,
		[property: JsonPropertyName("Players")] IReadOnlyList<Player> Players,
		[property: JsonPropertyName("MatchmakingData")] MatchmakingData MatchmakingData
	);

	public record MatchmakingData(
		[property: JsonPropertyName("QueueID")] string QueueId,
		[property: JsonPropertyName("IsRanked")] bool IsRanked
	);

	public record SeasonalBadgeInfo(
		[property: JsonPropertyName("SeasonID")] string SeasonId,
		[property: JsonPropertyName("NumberOfWins")] long NumberOfWins,
		[property: JsonPropertyName("WinsByTier")] object WinsByTier,
		[property: JsonPropertyName("Rank")] long Rank,
		[property: JsonPropertyName("LeaderboardRank")] long LeaderboardRank
	);
}

