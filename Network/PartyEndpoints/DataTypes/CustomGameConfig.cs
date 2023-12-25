using System.Text.Json.Serialization;

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes;

public record _5Internal(
    [property: JsonPropertyName("0")] int? _0,
    [property: JsonPropertyName("1")] int? _1,
    [property: JsonPropertyName("10")] int? _10,
    [property: JsonPropertyName("11")] int? _11,
    [property: JsonPropertyName("12")] int? _12,
    [property: JsonPropertyName("13")] int? _13,
    [property: JsonPropertyName("14")] int? _14,
    [property: JsonPropertyName("15")] int? _15,
    [property: JsonPropertyName("16")] int? _16,
    [property: JsonPropertyName("17")] int? _17,
    [property: JsonPropertyName("18")] int? _18,
    [property: JsonPropertyName("19")] int? _19,
    [property: JsonPropertyName("2")] int? _2,
    [property: JsonPropertyName("20")] int? _20,
    [property: JsonPropertyName("21")] int? _21,
    [property: JsonPropertyName("22")] int? _22,
    [property: JsonPropertyName("23")] int? _23,
    [property: JsonPropertyName("24")] int? _24,
    [property: JsonPropertyName("25")] int? _25,
    [property: JsonPropertyName("26")] int? _26,
    [property: JsonPropertyName("27")] int? _27,
    [property: JsonPropertyName("3")] int? _3,
    [property: JsonPropertyName("4")] int? _4,
    [property: JsonPropertyName("5")] int? _5,
    [property: JsonPropertyName("6")] int? _6,
    [property: JsonPropertyName("7")] int? _7,
    [property: JsonPropertyName("8")] int? _8,
    [property: JsonPropertyName("9")] int? _9
);

public record AresriotAwsAtl1ProdNaGpAtlanta1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] int? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsChi1ProdNaGpChicago1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsDfw1ProdNaGpDallas1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] int? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUse1ProdNaGpAshburn1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] int? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUsw1ProdNaGpNorcal1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] int? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUsw2ProdNaGpOregon1(
    [property: JsonPropertyName("SecurityHash")] int? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record GamePodPingServiceInfo(
    [property: JsonPropertyName("aresriot.aws-atl1-prod.na-gp-atlanta-1")] AresriotAwsAtl1ProdNaGpAtlanta1 AresriotAwsAtl1ProdNaGpAtlanta1,
    [property: JsonPropertyName("aresriot.aws-chi1-prod.na-gp-chicago-1")] AresriotAwsChi1ProdNaGpChicago1 AresriotAwsChi1ProdNaGpChicago1,
    [property: JsonPropertyName("aresriot.aws-dfw1-prod.na-gp-dallas-1")] AresriotAwsDfw1ProdNaGpDallas1 AresriotAwsDfw1ProdNaGpDallas1,
    [property: JsonPropertyName("aresriot.aws-use1-prod.na-gp-ashburn-1")] AresriotAwsUse1ProdNaGpAshburn1 AresriotAwsUse1ProdNaGpAshburn1,
    [property: JsonPropertyName("aresriot.aws-usw1-prod.na-gp-norcal-1")] AresriotAwsUsw1ProdNaGpNorcal1 AresriotAwsUsw1ProdNaGpNorcal1,
    [property: JsonPropertyName("aresriot.aws-usw2-prod.na-gp-oregon-1")] AresriotAwsUsw2ProdNaGpOregon1 AresriotAwsUsw2ProdNaGpOregon1
);

public record GameRules(
    [property: JsonPropertyName("IsOvertimeWinByTwo")] string IsOvertimeWinByTwo,
    [property: JsonPropertyName("AllowLenientSurrender")] string AllowLenientSurrender,
    [property: JsonPropertyName("AllowDropOut")] string AllowDropOut,
    [property: JsonPropertyName("AssignRandomAgents")] string AssignRandomAgents,
    [property: JsonPropertyName("SkipPregame")] string SkipPregame,
    [property: JsonPropertyName("AllowMatchTimeouts")] string AllowMatchTimeouts,
    [property: JsonPropertyName("AllowOvertimeDrawVote")] string AllowOvertimeDrawVote,
    [property: JsonPropertyName("AllowOvertimePriorityVote")] string AllowOvertimePriorityVote,
    [property: JsonPropertyName("IsOvertimeWinByTwoCapped")] string IsOvertimeWinByTwoCapped,
    [property: JsonPropertyName("PremierTournamentMode")] string PremierTournamentMode
);

public record PartySkillDisparityCompetitiveTiersCeilings(
    [property: JsonPropertyName("0")] int? _0,
    [property: JsonPropertyName("1")] int? _1,
    [property: JsonPropertyName("10")] int? _10,
    [property: JsonPropertyName("11")] int? _11,
    [property: JsonPropertyName("12")] int? _12,
    [property: JsonPropertyName("13")] int? _13,
    [property: JsonPropertyName("14")] int? _14,
    [property: JsonPropertyName("15")] int? _15,
    [property: JsonPropertyName("16")] int? _16,
    [property: JsonPropertyName("17")] int? _17,
    [property: JsonPropertyName("18")] int? _18,
    [property: JsonPropertyName("19")] int? _19,
    [property: JsonPropertyName("2")] int? _2,
    [property: JsonPropertyName("20")] int? _20,
    [property: JsonPropertyName("21")] int? _21,
    [property: JsonPropertyName("22")] int? _22,
    [property: JsonPropertyName("23")] int? _23,
    [property: JsonPropertyName("24")] int? _24,
    [property: JsonPropertyName("25")] int? _25,
    [property: JsonPropertyName("26")] int? _26,
    [property: JsonPropertyName("27")] int? _27,
    [property: JsonPropertyName("3")] int? _3,
    [property: JsonPropertyName("4")] int? _4,
    [property: JsonPropertyName("5")] int? _5,
    [property: JsonPropertyName("6")] int? _6,
    [property: JsonPropertyName("7")] int? _7,
    [property: JsonPropertyName("8")] int? _8,
    [property: JsonPropertyName("9")] int? _9
);

public record PartySkillDisparityPartySizeCompetitiveTiersCeilings(
    [property: JsonPropertyName("5")] _5Internal _5
);

public record Queue(
    [property: JsonPropertyName("QueueID")] string QueueID,
    [property: JsonPropertyName("Enabled")] bool? Enabled,
    [property: JsonPropertyName("TeamSize")] int? TeamSize,
    [property: JsonPropertyName("NumTeams")] int? NumTeams,
    [property: JsonPropertyName("MaxPartySize")] int? MaxPartySize,
    [property: JsonPropertyName("MinPartySize")] int? MinPartySize,
    [property: JsonPropertyName("InvalidPartySizes")] IReadOnlyList<int?> InvalidPartySizes,
    [property: JsonPropertyName("MaxPartySizeHighSkill")] int? MaxPartySizeHighSkill,
    [property: JsonPropertyName("HighSkillTier")] int? HighSkillTier,
    [property: JsonPropertyName("MaxSkillTier")] int? MaxSkillTier,
    [property: JsonPropertyName("AllowFullPartyBypassSkillRestrictions")] bool? AllowFullPartyBypassSkillRestrictions,
    [property: JsonPropertyName("ApplyRRPenaltyToFullParty")] bool? ApplyRRPenaltyToFullParty,
    [property: JsonPropertyName("AllowFiveStackRestrictions")] bool? AllowFiveStackRestrictions,
    [property: JsonPropertyName("Mode")] string Mode,
    [property: JsonPropertyName("IsRanked")] bool? IsRanked,
    [property: JsonPropertyName("IsTournament")] bool? IsTournament,
    [property: JsonPropertyName("IsTournamentV2")] bool? IsTournamentV2,
    [property: JsonPropertyName("RequireRoster")] bool? RequireRoster,
    [property: JsonPropertyName("Priority")] int? Priority,
    [property: JsonPropertyName("PartyMaxCompetitiveTierRange")] int? PartyMaxCompetitiveTierRange,
    [property: JsonPropertyName("PartyMaxCompetitiveTierRangePlacementBuffer")] int? PartyMaxCompetitiveTierRangePlacementBuffer,
    [property: JsonPropertyName("FullPartyMaxCompetitiveTierRange")] int? FullPartyMaxCompetitiveTierRange,
    [property: JsonPropertyName("PartySkillDisparityCompetitiveTiersCeilings")] PartySkillDisparityCompetitiveTiersCeilings PartySkillDisparityCompetitiveTiersCeilings,
    [property: JsonPropertyName("PartySkillDisparityPartySizeCompetitiveTiersCeilings")] PartySkillDisparityPartySizeCompetitiveTiersCeilings PartySkillDisparityPartySizeCompetitiveTiersCeilings,
    [property: JsonPropertyName("UseAccountLevelRequirement")] bool? UseAccountLevelRequirement,
    [property: JsonPropertyName("MinimumAccountLevelRequired")] int? MinimumAccountLevelRequired,
    [property: JsonPropertyName("GameRules")] GameRules GameRules,
    [property: JsonPropertyName("SupportedPlatformTypes")] IReadOnlyList<string> SupportedPlatformTypes,
    [property: JsonPropertyName("DisabledContent")] IReadOnlyList<object> DisabledContent,
    [property: JsonPropertyName("queueFieldA")] IReadOnlyList<object> QueueFieldA,
    [property: JsonPropertyName("NextScheduleChangeSeconds")] int? NextScheduleChangeSeconds,
    [property: JsonPropertyName("TimeUntilNextScheduleChangeSeconds")] int? TimeUntilNextScheduleChangeSeconds,
    [property: JsonPropertyName("MapWeights")] IReadOnlyList<string> MapWeights
);

public record CustomGameConfig(
    [property: JsonPropertyName("Enabled")] bool? Enabled,
    [property: JsonPropertyName("EnabledMaps")] IReadOnlyList<string> EnabledMaps,
    [property: JsonPropertyName("EnabledModes")] IReadOnlyList<string> EnabledModes,
    [property: JsonPropertyName("Queues")] IReadOnlyList<Queue> Queues,
    [property: JsonPropertyName("GamePodPingServiceInfo")] GamePodPingServiceInfo GamePodPingServiceInfo
);