

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	public record CheatData(
		[property: JsonPropertyName("GamePodOverride")] string GamePodOverride,
		[property: JsonPropertyName("ForcePostGameProcessing")] bool? ForcePostGameProcessing
	);

	public record CustomGameData(
		[property: JsonPropertyName("Settings")] Settings Settings,
		[property: JsonPropertyName("Membership")] Membership Membership,
		[property: JsonPropertyName("MaxPartySize")] long? MaxPartySize,
		[property: JsonPropertyName("AutobalanceEnabled")] bool? AutobalanceEnabled,
		[property: JsonPropertyName("AutobalanceMinPlayers")] long? AutobalanceMinPlayers,
		[property: JsonPropertyName("HasRecoveryData")] bool? HasRecoveryData
	);

	public record ErrorNotification(
		[property: JsonPropertyName("ErrorType")] string ErrorType,
		[property: JsonPropertyName("ErroredPlayers")] object ErroredPlayers
	);

	public record MatchmakingData(
		[property: JsonPropertyName("QueueID")] string QueueId,
		[property: JsonPropertyName("PreferredGamePods")] IReadOnlyList<string> PreferredGamePods,
		[property: JsonPropertyName("SkillDisparityRRPenalty")] long? SkillDisparityRRPenalty
	);

	public record Member(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("CompetitiveTier")] long? CompetitiveTier,
		[property: JsonPropertyName("PlayerIdentity")] PlayerIdentity PlayerIdentity,
		[property: JsonPropertyName("SeasonalBadgeInfo")] object SeasonalBadgeInfo,
		[property: JsonPropertyName("IsOwner")] bool? IsOwner,
		[property: JsonPropertyName("QueueEligibleRemainingAccountLevels")] long? QueueEligibleRemainingAccountLevels,
		[property: JsonPropertyName("Pings")] IReadOnlyList<PingInternal> Pings,
		[property: JsonPropertyName("IsReady")] bool? IsReady,
		[property: JsonPropertyName("IsModerator")] bool? IsModerator,
		[property: JsonPropertyName("UseBroadcastHUD")] bool? UseBroadcastHUD,
		[property: JsonPropertyName("PlatformType")] string PlatformType
	);

	public record Membership(
		[property: JsonPropertyName("teamOne")] object TeamOne,
		[property: JsonPropertyName("teamTwo")] object TeamTwo,
		[property: JsonPropertyName("teamSpectate")] object TeamSpectate,
		[property: JsonPropertyName("teamOneCoaches")] object TeamOneCoaches,
		[property: JsonPropertyName("teamTwoCoaches")] object TeamTwoCoaches
	);

	public record PingInternal(
		[property: JsonPropertyName("Ping")] long? Ping,
		[property: JsonPropertyName("GamePodID")] string GamePodID
	);

	public record PlayerIdentity(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("PlayerCardID")] string PlayerCardId,
		[property: JsonPropertyName("PlayerTitleID")] string PlayerTitleId,
		[property: JsonPropertyName("AccountLevel")] long? AccountLevel,
		[property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderId,
		[property: JsonPropertyName("Incognito")] bool? Incognito,
		[property: JsonPropertyName("HideAccountLevel")] bool? HideAccountLevel
	);

	public record Party(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("MUCName")] string MUCName,
		[property: JsonPropertyName("VoiceRoomID")] string VoiceRoomId,
		[property: JsonPropertyName("Version")] long? Version,
		[property: JsonPropertyName("ClientVersion")] string ClientVersion,
		[property: JsonPropertyName("Members")] IReadOnlyList<Member> Members,
		[property: JsonPropertyName("State")] string State,
		[property: JsonPropertyName("PreviousState")] string PreviousState,
		[property: JsonPropertyName("StateTransitionReason")] string StateTransitionReason,
		[property: JsonPropertyName("Accessibility")] string Accessibility,
		[property: JsonPropertyName("CustomGameData")] CustomGameData CustomGameData,
		[property: JsonPropertyName("MatchmakingData")] MatchmakingData MatchmakingData,
		[property: JsonPropertyName("Invites")] object Invites,
		[property: JsonPropertyName("Requests")] IReadOnlyList<object> Requests,
		[property: JsonPropertyName("QueueEntryTime")] DateTime? QueueEntryTime,
		[property: JsonPropertyName("ErrorNotification")] ErrorNotification ErrorNotification,
		[property: JsonPropertyName("RestrictedSeconds")] long? RestrictedSeconds,
		[property: JsonPropertyName("EligibleQueues")] IReadOnlyList<string> EligibleQueues,
		[property: JsonPropertyName("QueueIneligibilities")] IReadOnlyList<object> QueueIneligibilities,
		[property: JsonPropertyName("CheatData")] CheatData CheatData,
		[property: JsonPropertyName("XPBonuses")] IReadOnlyList<XPBonuse> XPBonuses,
		[property: JsonPropertyName("InviteCode")] string InviteCode
	);

	public record Settings(
		[property: JsonPropertyName("Map")] string Map,
		[property: JsonPropertyName("Mode")] string Mode,
		[property: JsonPropertyName("UseBots")] bool? UseBots,
		[property: JsonPropertyName("GamePod")] string GamePod,
		[property: JsonPropertyName("GameRules")] object GameRules
	);

	public record XPBonuse(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Applied")] bool? Applied
	);
}

