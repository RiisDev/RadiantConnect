using System.Text.Json.Serialization;
// ReSharper disable All

namespace CyphersWatchfulEye.ValorantAPI.DataTypes
{
    public record ChangeQueue(
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
        [property: JsonPropertyName("QueueEntryTime")] DateTime? QueueEntryTime,
        [property: JsonPropertyName("ErrorNotification")] ErrorNotification ErrorNotification,
        [property: JsonPropertyName("RestrictedSeconds")] int? RestrictedSeconds,
        [property: JsonPropertyName("EligibleQueues")] IReadOnlyList<string> EligibleQueues,
        [property: JsonPropertyName("QueueIneligibilities")] IReadOnlyList<object> QueueIneligibilities,
        [property: JsonPropertyName("CheatData")] CheatData CheatData,
        [property: JsonPropertyName("XPBonuses")] IReadOnlyList<XPBonuse> XPBonuses,
        [property: JsonPropertyName("InviteCode")] string InviteCode
    );

    public record XPBonuse(
        [property: JsonPropertyName("ID")] string ID,
        [property: JsonPropertyName("Applied")] bool? Applied
    );

}
