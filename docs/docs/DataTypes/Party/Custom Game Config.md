## GamePodPingServiceInfo Record

The `GamePodPingServiceInfo` record represents information about the Game Pod Ping Service in the RadiantConnect network.

### Properties

#### `aresriot.aws-atl1-prod.na-gp-atlanta-1`

- Type: `AresriotAwsAtl1ProdNaGpAtlanta1`
- Description: Represents information specific to the Atlanta 1 Game Pod.

#### `aresriot.aws-chi1-prod.na-gp-chicago-1`

- Type: `AresriotAwsChi1ProdNaGpChicago1`
- Description: Represents information specific to the Chicago 1 Game Pod.

#### `aresriot.aws-dfw1-prod.na-gp-dallas-1`

- Type: `AresriotAwsDfw1ProdNaGpDallas1`
- Description: Represents information specific to the Dallas 1 Game Pod.

#### `aresriot.aws-use1-prod.na-gp-ashburn-1`

- Type: `AresriotAwsUse1ProdNaGpAshburn1`
- Description: Represents information specific to the Ashburn 1 Game Pod.

#### `aresriot.aws-usw1-prod.na-gp-norcal-1`

- Type: `AresriotAwsUsw1ProdNaGpNorcal1`
- Description: Represents information specific to the Norcal 1 Game Pod.

#### `aresriot.aws-usw2-prod.na-gp-oregon-1`

- Type: `AresriotAwsUsw2ProdNaGpOregon1`
- Description: Represents information specific to the Oregon 1 Game Pod.

## AresriotAwsAtl1ProdNaGpAtlanta1 Record

- Type: `record`
- Description: Represents information specific to the Atlanta 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## AresriotAwsChi1ProdNaGpChicago1 Record

- Type: `record`
- Description: Represents information specific to the Chicago 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## AresriotAwsDfw1ProdNaGpDallas1 Record

- Type: `record`
- Description: Represents information specific to the Dallas 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## AresriotAwsUse1ProdNaGpAshburn1 Record

- Type: `record`
- Description: Represents information specific to the Ashburn 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## AresriotAwsUsw1ProdNaGpNorcal1 Record

- Type: `record`
- Description: Represents information specific to the Norcal 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## AresriotAwsUsw2ProdNaGpOregon1 Record

- Type: `record`
- Description: Represents information specific to the Oregon 1 Game Pod.

### Properties

#### `SecurityHash`

- Type: `long?`
- Description: Represents the security hash associated with the Game Pod.

#### `ObfuscatedIP`

- Type: `long?`
- Description: Represents the obfuscated IP address associated with the Game Pod.

#### `PingProxyAddress`

- Type: `string`
- Description: Represents the ping proxy address associated with the Game Pod.

#### `PingProxyAddresses`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of ping proxy addresses associated with the Game Pod.

## GameRules Record

- Type: `record`
- Description: Represents game rules in the RadiantConnect network.

### Properties

#### `IsOvertimeWinByTwo`

- Type: `string`
- Description: Represents whether overtime is won by two.

#### `AllowLenientSurrender`

- Type: `string`
- Description: Represents whether lenient surrender is allowed.

#### `AllowDropOut`

- Type: `string`
- Description: Represents whether drop-out is allowed.

#### `AssignRandomAgents`

- Type: `string`
- Description: Represents whether random agents are assigned.

#### `SkipPregame`

- Type: `string`
- Description: Represents whether pregame is skipped.

#### `AllowMatchTimeouts`

- Type: `string`
- Description: Represents whether match timeouts are allowed.

#### `AllowOvertimeDrawVote`

- Type: `string`
- Description: Represents whether overtime draw votes are allowed.

#### `AllowOvertimePriorityVote`

- Type: `string`
- Description: Represents whether overtime priority votes are allowed.

#### `IsOvertimeWinByTwoCapped`

- Type: `string`
- Description: Represents whether overtime win by two is capped.

#### `PremierTournamentMode`

- Type: `string`
- Description: Represents the premier tournament mode.

## PartySkillDisparityCompetitiveTiersCeilings Record

- Type: `record`
- Description: Represents the competitive tiers ceilings for party skill disparity.

### Properties

The properties are named according to the competitive tiers, ranging from 0 to 27.

## PartySkillDisparityPartySizeCompetitiveTiersCeilings Record

- Type: `record`
- Description: Represents the party size competitive tiers ceilings for party skill disparity.

### Properties

#### `_5`

- Type: `_5Internal`
- Description: Represents the ceilings for party size 5 in competitive tiers.

## Queue Record

- Type: `record`
- Description: Represents a queue in the RadiantConnect network.

### Properties

#### `QueueID`

- Type: `string`
- Description: Represents the identifier of the queue.

#### `Enabled`

- Type: `bool?`
- Description: Represents whether the queue is enabled.

#### `TeamSize`

- Type: `long?`
- Description: Represents the team size for the queue.

#### `NumTeams`

- Type: `long?`
- Description: Represents the number of teams for the queue.

#### `MaxPartySize`

- Type: `long?`
- Description: Represents the maximum party size for the queue.

#### `MinPartySize`

- Type: `long?`
- Description: Represents the minimum party size for the queue.

#### `InvalidPartySizes`

- Type: `IReadOnlyList<int?>`
- Description: Represents a list of invalid party sizes for the queue.

#### `MaxPartySizeHighSkill`

- Type: `long?`
- Description: Represents the maximum party size for high-skill players in the queue.

#### `HighSkillTier`

- Type: `long?`
- Description: Represents the high-skill tier for the queue.

#### `MaxSkillTier`

- Type: `long?`
- Description: Represents the maximum skill tier for the queue.

#### `AllowFullPartyBypassSkillRestrictions`

- Type: `bool?`
- Description: Represents whether a full party is allowed to bypass skill restrictions.

#### `ApplyRRPenaltyToFullParty`

- Type: `bool?`
- Description: Represents whether RR penalty is applied to a full party.

#### `AllowFiveStackRestrictions`

- Type: `bool?`
- Description: Represents whether five-stack restrictions are allowed.

#### `Mode`

- Type: `string`
- Description: Represents the mode of the queue.

#### `IsRanked`

- Type: `bool?`
- Description: Represents whether the queue is ranked.

#### `IsTournament`

- Type: `bool?`
- Description: Represents whether the queue is a tournament.

#### `IsTournamentV2`

- Type: `bool?`
- Description: Represents whether the queue is a tournament version 2.

#### `RequireRoster`

- Type: `bool?`
- Description: Represents whether a roster is required for the queue.

#### `Priority`

- Type: `long?`
- Description: Represents the priority of the queue.

#### `PartyMaxCompetitiveTierRange`

- Type: `long?`
- Description: Represents the maximum competitive tier range for a party in the queue.

#### `PartyMaxCompetitiveTierRangePlacementBuffer`

- Type: `long?`
- Description: Represents the placement buffer for the maximum competitive tier range for a party in the queue.

#### `FullPartyMaxCompetitiveTierRange`

- Type: `long?`
- Description: Represents the maximum competitive tier range for a full party in the queue.

#### `PartySkillDisparityCompetitiveTiersCeilings`

- Type: `PartySkillDisparityCompetitiveTiersCeilings`
- Description: Represents the competitive tiers ceilings for party skill disparity in the queue.

#### `PartySkillDisparityPartySizeCompetitiveTiersCeilings`

- Type: `PartySkillDisparityPartySizeCompetitiveTiersCeilings`
- Description: Represents the party size competitive tiers ceilings for party skill disparity in the queue.

#### `UseAccountLevelRequirement`

- Type: `bool?`
- Description: Represents whether an account level requirement is used for the queue.

#### `MinimumAccountLevelRequired`

- Type: `long?`
- Description: Represents the minimum account level required for the queue.

#### `GameRules`

- Type: `GameRules`
- Description: Represents the game rules for the queue.

#### `SupportedPlatformTypes`

- Type: `IReadOnlyList<string>`
- Description: Represents the supported platform types for the queue.

#### `DisabledContent`

- Type: `IReadOnlyList<object>`
- Description: Represents disabled content for the queue.

#### `QueueFieldA`

- Type: `IReadOnlyList<object>`
- Description: Represents field A for the queue.

#### `NextScheduleChangeSeconds`

- Type: `long?`
- Description: Represents the seconds until the next schedule change for the queue.

#### `TimeUntilNextScheduleChangeSeconds`

- Type: `long?`
- Description: Represents the time until the next schedule change for the queue.

#### `MapWeights`

- Type: `IReadOnlyList<string>`
- Description: Represents the map weights for the queue.

## CustomGameConfig Record

- Type: `record`
- Description: Represents the configuration for a custom game in the RadiantConnect network.

### Properties

#### `Enabled`

- Type: `bool?`
- Description: Represents whether custom games are enabled.

#### `EnabledMaps`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of enabled maps for custom games.

#### `EnabledModes`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of enabled modes for custom games.

#### `Queues`

- Type: `IReadOnlyList<Queue>`
- Description: Represents a list of queues available for custom games.

#### `GamePodPingServiceInfo`

- Type: `GamePodPingServiceInfo`
- Description: Represents information about the Game Pod Ping Service for custom games.
