

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes;

public record _5Internal(
    [property: JsonPropertyName("0")] long? _0,
    [property: JsonPropertyName("1")] long? _1,
    [property: JsonPropertyName("10")] long? _10,
    [property: JsonPropertyName("11")] long? _11,
    [property: JsonPropertyName("12")] long? _12,
    [property: JsonPropertyName("13")] long? _13,
    [property: JsonPropertyName("14")] long? _14,
    [property: JsonPropertyName("15")] long? _15,
    [property: JsonPropertyName("16")] long? _16,
    [property: JsonPropertyName("17")] long? _17,
    [property: JsonPropertyName("18")] long? _18,
    [property: JsonPropertyName("19")] long? _19,
    [property: JsonPropertyName("2")] long? _2,
    [property: JsonPropertyName("20")] long? _20,
    [property: JsonPropertyName("21")] long? _21,
    [property: JsonPropertyName("22")] long? _22,
    [property: JsonPropertyName("23")] long? _23,
    [property: JsonPropertyName("24")] long? _24,
    [property: JsonPropertyName("25")] long? _25,
    [property: JsonPropertyName("26")] long? _26,
    [property: JsonPropertyName("27")] long? _27,
    [property: JsonPropertyName("3")] long? _3,
    [property: JsonPropertyName("4")] long? _4,
    [property: JsonPropertyName("5")] long? _5,
    [property: JsonPropertyName("6")] long? _6,
    [property: JsonPropertyName("7")] long? _7,
    [property: JsonPropertyName("8")] long? _8,
    [property: JsonPropertyName("9")] long? _9
);

public record AresriotAwsAtl1ProdNaGpAtlanta1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsChi1ProdNaGpChicago1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsDfw1ProdNaGpDallas1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUse1ProdNaGpAshburn1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUsw1ProdNaGpNorcal1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
    [property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
    [property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
    [property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
);

public record AresriotAwsUsw2ProdNaGpOregon1(
    [property: JsonPropertyName("SecurityHash")] long? SecurityHash,
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
    [property: JsonPropertyName("0")] long? _0,
    [property: JsonPropertyName("1")] long? _1,
    [property: JsonPropertyName("10")] long? _10,
    [property: JsonPropertyName("11")] long? _11,
    [property: JsonPropertyName("12")] long? _12,
    [property: JsonPropertyName("13")] long? _13,
    [property: JsonPropertyName("14")] long? _14,
    [property: JsonPropertyName("15")] long? _15,
    [property: JsonPropertyName("16")] long? _16,
    [property: JsonPropertyName("17")] long? _17,
    [property: JsonPropertyName("18")] long? _18,
    [property: JsonPropertyName("19")] long? _19,
    [property: JsonPropertyName("2")] long? _2,
    [property: JsonPropertyName("20")] long? _20,
    [property: JsonPropertyName("21")] long? _21,
    [property: JsonPropertyName("22")] long? _22,
    [property: JsonPropertyName("23")] long? _23,
    [property: JsonPropertyName("24")] long? _24,
    [property: JsonPropertyName("25")] long? _25,
    [property: JsonPropertyName("26")] long? _26,
    [property: JsonPropertyName("27")] long? _27,
    [property: JsonPropertyName("3")] long? _3,
    [property: JsonPropertyName("4")] long? _4,
    [property: JsonPropertyName("5")] long? _5,
    [property: JsonPropertyName("6")] long? _6,
    [property: JsonPropertyName("7")] long? _7,
    [property: JsonPropertyName("8")] long? _8,
    [property: JsonPropertyName("9")] long? _9
);

public record PartySkillDisparityPartySizeCompetitiveTiersCeilings(
    [property: JsonPropertyName("5")] _5Internal _5
);

public record Queue(
    [property: JsonPropertyName("QueueID")] string QueueID,
    [property: JsonPropertyName("Enabled")] bool? Enabled,
    [property: JsonPropertyName("TeamSize")] long? TeamSize,
    [property: JsonPropertyName("NumTeams")] long? NumTeams,
    [property: JsonPropertyName("MaxPartySize")] long? MaxPartySize,
    [property: JsonPropertyName("MinPartySize")] long? MinPartySize,
    [property: JsonPropertyName("InvalidPartySizes")] IReadOnlyList<int?> InvalidPartySizes,
    [property: JsonPropertyName("MaxPartySizeHighSkill")] long? MaxPartySizeHighSkill,
    [property: JsonPropertyName("HighSkillTier")] long? HighSkillTier,
    [property: JsonPropertyName("MaxSkillTier")] long? MaxSkillTier,
    [property: JsonPropertyName("AllowFullPartyBypassSkillRestrictions")] bool? AllowFullPartyBypassSkillRestrictions,
    [property: JsonPropertyName("ApplyRRPenaltyToFullParty")] bool? ApplyRRPenaltyToFullParty,
    [property: JsonPropertyName("AllowFiveStackRestrictions")] bool? AllowFiveStackRestrictions,
    [property: JsonPropertyName("Mode")] string Mode,
    [property: JsonPropertyName("IsRanked")] bool? IsRanked,
    [property: JsonPropertyName("IsTournament")] bool? IsTournament,
    [property: JsonPropertyName("IsTournamentV2")] bool? IsTournamentV2,
    [property: JsonPropertyName("RequireRoster")] bool? RequireRoster,
    [property: JsonPropertyName("Priority")] long? Priority,
    [property: JsonPropertyName("PartyMaxCompetitiveTierRange")] long? PartyMaxCompetitiveTierRange,
    [property: JsonPropertyName("PartyMaxCompetitiveTierRangePlacementBuffer")] long? PartyMaxCompetitiveTierRangePlacementBuffer,
    [property: JsonPropertyName("FullPartyMaxCompetitiveTierRange")] long? FullPartyMaxCompetitiveTierRange,
    [property: JsonPropertyName("PartySkillDisparityCompetitiveTiersCeilings")] PartySkillDisparityCompetitiveTiersCeilings PartySkillDisparityCompetitiveTiersCeilings,
    [property: JsonPropertyName("PartySkillDisparityPartySizeCompetitiveTiersCeilings")] PartySkillDisparityPartySizeCompetitiveTiersCeilings PartySkillDisparityPartySizeCompetitiveTiersCeilings,
    [property: JsonPropertyName("UseAccountLevelRequirement")] bool? UseAccountLevelRequirement,
    [property: JsonPropertyName("MinimumAccountLevelRequired")] long? MinimumAccountLevelRequired,
    [property: JsonPropertyName("GameRules")] GameRules GameRules,
    [property: JsonPropertyName("SupportedPlatformTypes")] IReadOnlyList<string> SupportedPlatformTypes,
    [property: JsonPropertyName("DisabledContent")] IReadOnlyList<object> DisabledContent,
    [property: JsonPropertyName("queueFieldA")] IReadOnlyList<object> QueueFieldA,
    [property: JsonPropertyName("NextScheduleChangeSeconds")] long? NextScheduleChangeSeconds,
    [property: JsonPropertyName("TimeUntilNextScheduleChangeSeconds")] long? TimeUntilNextScheduleChangeSeconds,
    [property: JsonPropertyName("MapWeights")] IReadOnlyList<string> MapWeights
);

public record CustomGameConfig(
    [property: JsonPropertyName("Enabled")] bool? Enabled,
    [property: JsonPropertyName("EnabledMaps")] IReadOnlyList<string> EnabledMaps,
    [property: JsonPropertyName("EnabledModes")] IReadOnlyList<string> EnabledModes,
    [property: JsonPropertyName("Queues")] IReadOnlyList<Queue> Queues,
    [property: JsonPropertyName("GamePodPingServiceInfo")] GamePodPingServiceInfo GamePodPingServiceInfo
);