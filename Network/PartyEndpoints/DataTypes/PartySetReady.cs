

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	public record PartySetReadyCheatData(
		[property: JsonPropertyName("GamePodOverride")] string GamePodOverride,
		[property: JsonPropertyName("ForcePostGameProcessing")] bool? ForcePostGameProcessing
	);

	public record PartySetReadyCustomGameData(
		[property: JsonPropertyName("Settings")] Settings Settings,
		[property: JsonPropertyName("Membership")] PartySetReadyMembership Membership,
		[property: JsonPropertyName("MaxPartySize")] long? MaxPartySize,
		[property: JsonPropertyName("AutobalanceEnabled")] bool? AutobalanceEnabled,
		[property: JsonPropertyName("AutobalanceMinPlayers")] long? AutobalanceMinPlayers,
		[property: JsonPropertyName("HasRecoveryData")] bool? HasRecoveryData
	);

	public record PartySetReadyErrorNotification(
		[property: JsonPropertyName("ErrorType")] string ErrorType,
		[property: JsonPropertyName("ErroredPlayers")] object ErroredPlayers
	);

	public record PartySetReadyMatchmakingData(
		[property: JsonPropertyName("QueueID")] string QueueID,
		[property: JsonPropertyName("PreferredGamePods")] IReadOnlyList<string> PreferredGamePods,
		[property: JsonPropertyName("SkillDisparityRRPenalty")] long? SkillDisparityRRPenalty
	);

	public record PartySetReadyMember(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("CompetitiveTier")] long? CompetitiveTier,
		[property: JsonPropertyName("PlayerIdentity")] PartySetReadyPlayerIdentity PlayerIdentity,
		[property: JsonPropertyName("SeasonalBadgeInfo")] object SeasonalBadgeInfo,
		[property: JsonPropertyName("IsOwner")] bool? IsOwner,
		[property: JsonPropertyName("QueueEligibleRemainingAccountLevels")] long? QueueEligibleRemainingAccountLevels,
		[property: JsonPropertyName("Pings")] IReadOnlyList<PartySetReadyPingInternal> Pings,
		[property: JsonPropertyName("IsReady")] bool? IsReady,
		[property: JsonPropertyName("IsModerator")] bool? IsModerator,
		[property: JsonPropertyName("UseBroadcastHUD")] bool? UseBroadcastHUD,
		[property: JsonPropertyName("PlatformType")] string PlatformType
	);

	public record PartySetReadyMembership(
		[property: JsonPropertyName("teamOne")] object TeamOne,
		[property: JsonPropertyName("teamTwo")] object TeamTwo,
		[property: JsonPropertyName("teamSpectate")] object TeamSpectate,
		[property: JsonPropertyName("teamOneCoaches")] object TeamOneCoaches,
		[property: JsonPropertyName("teamTwoCoaches")] object TeamTwoCoaches
	);

	public record PartySetReadyPingInternal(
		[property: JsonPropertyName("Ping")] long? Ping,
		[property: JsonPropertyName("GamePodID")] string GamePodID
	);

	public record PartySetReadyPlayerIdentity(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
		[property: JsonPropertyName("PlayerTitleID")] string PlayerTitleID,
		[property: JsonPropertyName("AccountLevel")] long? AccountLevel,
		[property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderID,
		[property: JsonPropertyName("Incognito")] bool? Incognito,
		[property: JsonPropertyName("HideAccountLevel")] bool? HideAccountLevel
	);

	public record PartySetReady(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("MUCName")] string MUCName,
		[property: JsonPropertyName("VoiceRoomID")] string VoiceRoomID,
		[property: JsonPropertyName("Version")] long? Version,
		[property: JsonPropertyName("ClientVersion")] string ClientVersion,
		[property: JsonPropertyName("Members")] IReadOnlyList<PartySetReadyMember> Members,
		[property: JsonPropertyName("State")] string State,
		[property: JsonPropertyName("PreviousState")] string PreviousState,
		[property: JsonPropertyName("StateTransitionReason")] string StateTransitionReason,
		[property: JsonPropertyName("Accessibility")] string Accessibility,
		[property: JsonPropertyName("CustomGameData")] PartySetReadyCustomGameData CustomGameData,
		[property: JsonPropertyName("MatchmakingData")] PartySetReadyMatchmakingData MatchmakingData,
		[property: JsonPropertyName("Invites")] object Invites,
		[property: JsonPropertyName("Requests")] IReadOnlyList<object> Requests,
		[property: JsonPropertyName("QueueEntryTime")] DateTime? QueueEntryTime,
		[property: JsonPropertyName("ErrorNotification")] PartySetReadyErrorNotification ErrorNotification,
		[property: JsonPropertyName("RestrictedSeconds")] long? RestrictedSeconds,
		[property: JsonPropertyName("EligibleQueues")] IReadOnlyList<string> EligibleQueues,
		[property: JsonPropertyName("QueueIneligibilities")] IReadOnlyList<object> QueueIneligibilities,
		[property: JsonPropertyName("CheatData")] PartySetReadyCheatData CheatData,
		[property: JsonPropertyName("XPBonuses")] IReadOnlyList<PartySetReadyXPBonuse> XPBonuses,
		[property: JsonPropertyName("InviteCode")] string InviteCode
	);

	public record PartySetReadySettings(
		[property: JsonPropertyName("Map")] string Map,
		[property: JsonPropertyName("Mode")] string Mode,
		[property: JsonPropertyName("UseBots")] bool? UseBots,
		[property: JsonPropertyName("GamePod")] string GamePod,
		[property: JsonPropertyName("GameRules")] object GameRules
	);

	public record PartySetReadyXPBonuse(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Applied")] bool? Applied
	);
}