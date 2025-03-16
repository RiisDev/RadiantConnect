## ContractInternal Record
```csharp
public record ContractInternal(
    [property: JsonPropertyName("ContractDefinitionID")] string ContractDefinitionID,
    [property: JsonPropertyName("ContractProgression")] ContractProgression ContractProgression,
    [property: JsonPropertyName("ProgressionLevelReached")] long ProgressionLevelReached,
    [property: JsonPropertyName("ProgressionTowardsNextLevel")] long ProgressionTowardsNextLevel
);

public record ContractProgression(
    [property: JsonPropertyName("TotalProgressionEarned")] long TotalProgressionEarned,
    [property: JsonPropertyName("TotalProgressionEarnedVersion")] long TotalProgressionEarnedVersion,
    [property: JsonPropertyName("HighestRewardedLevel")] HighestRewardedLevel HighestRewardedLevel
);

public record MatchSummary(
    [property: JsonPropertyName("RoundsTotal")] long RoundsTotal,
    [property: JsonPropertyName("RoundsWon")] long RoundsWon
);

public record Mission(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Objectives")] Objectives Objectives,
    [property: JsonPropertyName("Complete")] bool Complete,
    [property: JsonPropertyName("ExpirationTime")] DateTime ExpirationTime
);

public record MissionMetadata(
    [property: JsonPropertyName("NPECompleted")] bool NPECompleted,
    [property: JsonPropertyName("WeeklyCheckpoint")] DateTime WeeklyCheckpoint
);

public record ProcessedMatch(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("StartTime")] object StartTime,
    [property: JsonPropertyName("XPGrants")] object XPGrants,
    [property: JsonPropertyName("RewardGrants")] object RewardGrants,
    [property: JsonPropertyName("MissionDeltas")] object MissionDeltas,
    [property: JsonPropertyName("ContractDeltas")] object ContractDeltas,
    [property: JsonPropertyName("CouldProgressMissions")] bool CouldProgressMissions,
    [property: JsonPropertyName("MatchSummary")] MatchSummary MatchSummary,
    [property: JsonPropertyName("RecruitmentProgressUpdate")] RecruitmentProgressUpdate RecruitmentProgressUpdate
);

public record RecruitmentProgressUpdate(
    [property: JsonPropertyName("GroupID")] string GroupID,
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
```