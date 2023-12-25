using System.Text.Json.Serialization;
// ReSharper disable All

namespace CyphersWatchfulEye.ValorantAPI.DataTypes
{
    public record CheatData(
        [property: JsonPropertyName("GamePodOverride")] string GamePodOverride,
        [property: JsonPropertyName("ForcePostGameProcessing")] bool? ForcePostGameProcessing
    );

    public record CustomGameData(
        [property: JsonPropertyName("Settings")] Settings Settings,
        [property: JsonPropertyName("Membership")] Membership Membership,
        [property: JsonPropertyName("MaxPartySize")] int? MaxPartySize,
        [property: JsonPropertyName("AutobalanceEnabled")] bool? AutobalanceEnabled,
        [property: JsonPropertyName("AutobalanceMinPlayers")] int? AutobalanceMinPlayers,
        [property: JsonPropertyName("HasRecoveryData")] bool? HasRecoveryData
    );

    public record ErrorNotification(
        [property: JsonPropertyName("ErrorType")] string ErrorType,
        [property: JsonPropertyName("ErroredPlayers")] object ErroredPlayers
    );

    public record GameRules(
        [property: JsonPropertyName("AllowGameModifiers")] object AllowGameModifiers,
        [property: JsonPropertyName("IsOvertimeWinByTwo")] object IsOvertimeWinByTwo,
        [property: JsonPropertyName("PlayOutAllRounds")] object PlayOutAllRounds,
        [property: JsonPropertyName("SkipMatchHistory")] object SkipMatchHistory,
        [property: JsonPropertyName("TournamentMode")] object TournamentMode
    );

    public record MatchmakingData(
        [property: JsonPropertyName("QueueID")] string? QueueID,
        [property: JsonPropertyName("PreferredGamePods")] IReadOnlyList<object> PreferredGamePods,
        [property: JsonPropertyName("SkillDisparityRRPenalty")] int? SkillDisparityRRPenalty
    );

    public record Member(
        [property: JsonPropertyName("Subject")] string Subject,
        [property: JsonPropertyName("CompetitiveTier")] int? CompetitiveTier,
        [property: JsonPropertyName("PlayerIdentity")] PlayerIdentity PlayerIdentity,
        [property: JsonPropertyName("SeasonalBadgeInfo")] object SeasonalBadgeInfo,
        [property: JsonPropertyName("IsOwner")] bool? IsOwner,
        [property: JsonPropertyName("QueueEligibleRemainingAccountLevels")] int? QueueEligibleRemainingAccountLevels,
        [property: JsonPropertyName("Pings")] IReadOnlyList<Ping> Pings,
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

    public record Ping(
        [property: JsonPropertyName("Ping")] int? PingInfo,
        [property: JsonPropertyName("GamePodID")] string GamePodID
    );

    public record PlayerIdentity(
        [property: JsonPropertyName("Subject")] string Subject,
        [property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
        [property: JsonPropertyName("PlayerTitleID")] string PlayerTitleID,
        [property: JsonPropertyName("AccountLevel")] int? AccountLevel,
        [property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderID,
        [property: JsonPropertyName("Incognito")] bool? Incognito,
        [property: JsonPropertyName("HideAccountLevel")] bool? HideAccountLevel
    );

    public record PartyInfo(
        [property: JsonPropertyName("ID")] string ID,
        [property: JsonPropertyName("MUCName")] string MUCName,
        [property: JsonPropertyName("VoiceRoomID")] string VoiceRoomID,
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
        [property: JsonPropertyName("QueueEntryTime")] string QueueEntryTime,
        [property: JsonPropertyName("ErrorNotification")] ErrorNotification ErrorNotification,
        [property: JsonPropertyName("RestrictedSeconds")] int? RestrictedSeconds,
        [property: JsonPropertyName("EligibleQueues")] IReadOnlyList<object> EligibleQueues,
        [property: JsonPropertyName("QueueIneligibilities")] IReadOnlyList<object> QueueIneligibilities,
        [property: JsonPropertyName("CheatData")] CheatData CheatData,
        [property: JsonPropertyName("XPBonuses")] IReadOnlyList<object> XPBonuses,
        [property: JsonPropertyName("InviteCode")] string InviteCode
    );

    public record Settings(
        [property: JsonPropertyName("Map")] string Map,
        [property: JsonPropertyName("Mode")] string Mode,
        [property: JsonPropertyName("UseBots")] bool? UseBots,
        [property: JsonPropertyName("GamePod")] string GamePod,
        [property: JsonPropertyName("GameRules")] GameRules GameRules
    );


}
