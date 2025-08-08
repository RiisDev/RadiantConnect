namespace RadiantConnect.Network.ContractEndpoints.DataTypes;
public record AmountVersion(
    [property: JsonPropertyName("Amount")] long Amount,
    [property: JsonPropertyName("Version")] long Version
);

public record ContractInternal(
    [property: JsonPropertyName("ContractDefinitionID")] string ContractDefinitionId,
    [property: JsonPropertyName("ContractProgression")] ContractProgression ContractProgression,
    [property: JsonPropertyName("ProgressionLevelReached")] long ProgressionLevelReached,
    [property: JsonPropertyName("ProgressionTowardsNextLevel")] long ProgressionTowardsNextLevel
);

public record ContractProgression(
    [property: JsonPropertyName("TotalProgressionEarned")] long TotalProgressionEarned,
    [property: JsonPropertyName("TotalProgressionEarnedVersion")] long TotalProgressionEarnedVersion,
    [property: JsonPropertyName("HighestRewardedLevel")] HighestRewardedLevel HighestRewardedLevel
);
public record HighestRewardedLevel(
    [property: JsonExtensionData] Dictionary<string, JsonElement> RawJsonElements
)
{
    public Dictionary<string, AmountVersion> Levels =>
        RawJsonElements.ToDictionary(
            kvp => kvp.Key,
            kvp => JsonSerializer.Deserialize<AmountVersion>(kvp.Value.GetRawText())!
        );
}

public record MatchSummary(
    [property: JsonPropertyName("RoundsTotal")] long RoundsTotal,
    [property: JsonPropertyName("RoundsWon")] long RoundsWon
);

public record Mission(
    [property: JsonPropertyName("ID")] string Id,
    [property: JsonPropertyName("Objectives")] Objectives Objectives,
    [property: JsonPropertyName("Complete")] bool Complete,
    [property: JsonPropertyName("ExpirationTime")] DateTime ExpirationTime
);

public record MissionMetadata(
    [property: JsonPropertyName("NPECompleted")] bool NpeCompleted,
    [property: JsonPropertyName("WeeklyCheckpoint")] DateTime WeeklyCheckpoint
);

public record Objectives(
    [property: JsonPropertyName("16af3bdf-c353-4c8c-a47c-b4eeac154eaf")] long _16af3bdfC3534c8cA47cB4eeac154eaf,
    [property: JsonPropertyName("5c22a5ad-4d44-3451-5a09-589c35ff77cd")] long? _5c22a5ad4d4434515a09589c35ff77cd,
    [property: JsonPropertyName("2a35e500-4f64-f61c-341b-87b6e10f5c2f")] long? _2a35e5004f64F61c341b87b6e10f5c2f
);

public record ProcessedMatch(
    [property: JsonPropertyName("ID")] string Id,
    [property: JsonPropertyName("StartTime")] object StartTime,
    [property: JsonPropertyName("XPGrants")] object XpGrants,
    [property: JsonPropertyName("RewardGrants")] object RewardGrants,
    [property: JsonPropertyName("MissionDeltas")] object MissionDeltas,
    [property: JsonPropertyName("ContractDeltas")] object ContractDeltas,
    [property: JsonPropertyName("CouldProgressMissions")] bool CouldProgressMissions,
    [property: JsonPropertyName("MatchSummary")] MatchSummary MatchSummary,
    [property: JsonPropertyName("RecruitmentProgressUpdate")] RecruitmentProgressUpdate RecruitmentProgressUpdate
);

public record RecruitmentProgressUpdate(
    [property: JsonPropertyName("GroupID")] string GroupId,
    [property: JsonPropertyName("ProgressBefore")] long ProgressBefore,
    [property: JsonPropertyName("ProgressAfter")] long ProgressAfter,
    [property: JsonPropertyName("MilestoneThreshold")] long MilestoneThreshold
);

public record Contract(
    [property: JsonPropertyName("Version")] long Version,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Contracts")] IReadOnlyList<ContractInternal> Contracts,
    [property: JsonPropertyName("ProcessedMatches")] IReadOnlyList<ProcessedMatch> ProcessedMatches,
    [property: JsonPropertyName("ActiveSpecialContract")] string ActiveSpecialContract,
    [property: JsonPropertyName("Missions")] IReadOnlyList<Mission> Missions,
    [property: JsonPropertyName("MissionMetadata")] MissionMetadata MissionMetadata
);

