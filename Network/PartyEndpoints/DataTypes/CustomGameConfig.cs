

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	public record NumericTierMap(
		[property: JsonExtensionData] Dictionary<string, JsonElement>? RawJsonElements
	)
	{
		public IReadOnlyDictionary<int, long?> Values =>
			RawJsonElements?
				.Where(kvp => int.TryParse(kvp.Key, out _))
				.ToDictionary(
					kvp => int.Parse(kvp.Key),
					kvp => kvp.Value.ValueKind == JsonValueKind.Number
						? kvp.Value.GetInt64()
						: (long?)null
				) ?? new Dictionary<int, long?>();
	}

	public record PartySizeTierMap(
		[property: JsonExtensionData] Dictionary<string, JsonElement>? RawJsonElements
	)
	{
		public IReadOnlyDictionary<int, NumericTierMap> Values =>
			RawJsonElements?
				.Where(kvp => int.TryParse(kvp.Key, out _))
				.ToDictionary(
					kvp => int.Parse(kvp.Key),
					kvp => JsonSerializer.Deserialize<NumericTierMap>(kvp.Value.GetRawText())!
				) ?? new Dictionary<int, NumericTierMap>();
	}

	public record GamePodRegionInfo(
		[property: JsonPropertyName("SecurityHash")] long? SecurityHash,
		[property: JsonPropertyName("ObfuscatedIP")] long? ObfuscatedIP,
		[property: JsonPropertyName("PingProxyAddress")] string PingProxyAddress,
		[property: JsonPropertyName("PingProxyAddresses")] IReadOnlyList<string> PingProxyAddresses
	);

	public record GamePodPingServiceInfo(
		[property: JsonExtensionData] Dictionary<string, JsonElement>? RawJsonElements
	)
	{
		public IReadOnlyDictionary<string, GamePodRegionInfo> Regions =>
			RawJsonElements is null
				? new Dictionary<string, GamePodRegionInfo>()
				: RawJsonElements
					.Where(kvp => kvp.Value.ValueKind == JsonValueKind.Object)
					.ToDictionary(
						kvp => kvp.Key,
						kvp => JsonSerializer.Deserialize<GamePodRegionInfo>(kvp.Value.GetRawText())!
					);
	}

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
		[property: JsonPropertyName("PartySkillDisparityCompetitiveTiersCeilings")] NumericTierMap PartySkillDisparityCompetitiveTiersCeilings,
		[property: JsonPropertyName("PartySkillDisparityPartySizeCompetitiveTiersCeilings")] PartySizeTierMap PartySkillDisparityPartySizeCompetitiveTiersCeilings,
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
}