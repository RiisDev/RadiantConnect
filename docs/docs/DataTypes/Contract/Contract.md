## ContractInternal Record

The `ContractInternal` record represents internal details of a contract within the RadiantConnect network.

### Properties

#### `ContractDefinitionID`

- Type: `string`
- Description: Represents the unique identifier for the contract definition.

#### `ContractProgression`

- Type: `ContractProgression`
- Description: Represents the progression details for the contract.

#### `ProgressionLevelReached`

- Type: `long`
- Description: Represents the level reached in the contract progression.

#### `ProgressionTowardsNextLevel`

- Type: `long`
- Description: Represents the progression towards the next level in the contract.

## ContractProgression Record

The `ContractProgression` record represents the progression details for a contract within the RadiantConnect network.

### Properties

#### `TotalProgressionEarned`

- Type: `long`
- Description: Represents the total progression earned for the contract.

#### `TotalProgressionEarnedVersion`

- Type: `long`
- Description: Represents the version of the total progression earned.

#### `HighestRewardedLevel`

- Type: `HighestRewardedLevel`
- Description: Represents the highest rewarded level in the contract progression.

## MatchSummary Record

The `MatchSummary` record represents a summary of match statistics within the RadiantConnect network.

### Properties

#### `RoundsTotal`

- Type: `long`
- Description: Represents the total number of rounds in the match.

#### `RoundsWon`

- Type: `long`
- Description: Represents the number of rounds won in the match.

## Mission Record

The `Mission` record represents a mission within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the mission.

#### `Objectives`

- Type: `Objectives`
- Description: Represents the objectives associated with the mission.

#### `Complete`

- Type: `bool`
- Description: Indicates whether the mission is complete.

#### `ExpirationTime`

- Type: `DateTime`
- Description: Represents the expiration time for the mission.

## MissionMetadata Record

The `MissionMetadata` record represents metadata information for missions within the RadiantConnect network.

### Properties

#### `NPECompleted`

- Type: `bool`
- Description: Indicates whether the NPE (New Player Experience) is completed.

#### `WeeklyCheckpoint`

- Type: `DateTime`
- Description: Represents the weekly checkpoint for mission metadata.

## ProcessedMatch Record

The `ProcessedMatch` record represents details of a processed match within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the processed match.

#### `StartTime`

- Type: `object`
- Description: Represents the start time of the processed match.

#### `XPGrants`

- Type: `object`
- Description: Represents XP grants associated with the processed match.

#### `RewardGrants`

- Type: `object`
- Description: Represents reward grants associated with the processed match.

#### `MissionDeltas`

- Type: `object`
- Description: Represents mission deltas associated with the processed match.

#### `ContractDeltas`

- Type: `object`
- Description: Represents contract deltas associated with the processed match.

#### `CouldProgressMissions`

- Type: `bool`
- Description: Indicates whether missions could be progressed in the processed match.

#### `MatchSummary`

- Type: `MatchSummary`
- Description: Represents the match summary details.

#### `RecruitmentProgressUpdate`

- Type: `RecruitmentProgressUpdate`
- Description: Represents the recruitment progress update details.

## RecruitmentProgressUpdate Record

The `RecruitmentProgressUpdate` record represents an update on recruitment progress within the RadiantConnect network.

### Properties

#### `GroupID`

- Type: `string`
- Description: Represents the unique identifier for the group associated with recruitment progress.

#### `ProgressBefore`

- Type: `long`
- Description: Represents the progress before the update.

#### `ProgressAfter`

- Type: `long`
- Description: Represents the progress after the update.

#### `MilestoneThreshold`

- Type: `long`
- Description: Represents the milestone threshold for the recruitment progress.

## Contract Record

The `Contract` record represents contract details within the RadiantConnect network.

### Properties

#### `Version`

- Type: `long`
- Description: Represents the version of the contract.

#### `Subject`

- Type: `string`
- Description: Represents the subject associated with the contract.

#### `Contracts`

- Type: `IReadOnlyList<ContractInternal>`
- Description: Represents a list of internal contracts associated with the contract.

#### `ProcessedMatches`

- Type: `IReadOnlyList<ProcessedMatch>`
- Description: Represents a list of processed matches associated with the contract.

#### `ActiveSpecialContract`

- Type: `string`
- Description: Represents the active special contract.

#### `Missions`

- Type: `IReadOnlyList<Mission>`
- Description: Represents a list of missions associated with the contract.

#### `MissionMetadata`

- Type: `MissionMetadata`
- Description: Represents the metadata information for missions associated with the contract.

