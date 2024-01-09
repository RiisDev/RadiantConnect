## PlayerMMR Record

The `PlayerMMR` record represents the Matchmaking Rating (MMR) information of a player in the RadiantConnect network.

### Properties

#### `Version`

- Type: `long?`
- Description: Represents the version information associated with the player's MMR.

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier associated with the player.

#### `NewPlayerExperienceFinished`

- Type: `bool?`
- Description: Indicates whether the new player experience has been finished.

#### `QueueSkills`

- Type: `QueueSkills`
- Description: Represents the MMR information for different game queues.

#### `LatestCompetitiveUpdate`

- Type: `LatestCompetitiveUpdate`
- Description: Represents the latest competitive update information for the player.

#### `IsLeaderboardAnonymized`

- Type: `bool?`
- Description: Indicates whether the player's leaderboard information is anonymized.

#### `IsActRankBadgeHidden`

- Type: `bool?`
- Description: Indicates whether the Act Rank badge is hidden for the player.

### QueueSkills Record

The `QueueSkills` record represents the MMR information for different game queues.

#### `Competitive`

- Type: `Competitive`
- Description: Represents the MMR information for the competitive game queue.

#### `Deathmatch`

- Type: `Deathmatch`
- Description: Represents the MMR information for the deathmatch game queue.

#### `Ggteam`

- Type: `Ggteam`
- Description: Represents the MMR information for the ggteam game queue.

#### `Hurm`

- Type: `Hurm`
- Description: Represents the MMR information for the hurm game queue.

#### `Newmap`

- Type: `Newmap`
- Description: Represents the MMR information for the newmap game queue.

#### `Onefa`

- Type: `Onefa`
- Description: Represents the MMR information for the onefa game queue.

#### `Premier`

- Type: `Premier`
- Description: Represents the MMR information for the premier game queue.

#### `Seeding`

- Type: `Seeding`
- Description: Represents the MMR information for the seeding game queue.

#### `Snowball`

- Type: `Snowball`
- Description: Represents the MMR information for the snowball game queue.

#### `Spikerush`

- Type: `Spikerush`
- Description: Represents the MMR information for the spikerush game queue.

#### `Swiftplay`

- Type: `Swiftplay`
- Description: Represents the MMR information for the swiftplay game queue.

#### `Unrated`

- Type: `Unrated`
- Description: Represents the MMR information for the unrated game queue.

### LatestCompetitiveUpdate Record

The `LatestCompetitiveUpdate` record represents the latest competitive update information for a player.

#### `MatchID`

- Type: `string`
- Description: Represents the unique identifier associated with the match.

#### `MapID`

- Type: `string`
- Description: Represents the unique identifier associated with the map.

#### `SeasonID`

- Type: `string`
- Description: Represents the unique identifier associated with the season.

#### `MatchStartTime`

- Type: `long?`
- Description: Represents the start time of the match.

#### `TierAfterUpdate`

- Type: `long?`
- Description: Represents the tier after the MMR update.

#### `TierBeforeUpdate`

- Type: `long?`
- Description: Represents the tier before the MMR update.

#### `RankedRatingAfterUpdate`

- Type: `long?`
- Description: Represents the ranked rating after the MMR update.

#### `RankedRatingBeforeUpdate`

- Type: `long?`
- Description: Represents the ranked rating before the MMR update.

#### `RankedRatingEarned`

- Type: `long?`
- Description: Represents the ranked rating earned in the update.

#### `RankedRatingPerformanceBonus`

- Type: `long?`
- Description: Represents the performance bonus in ranked rating.

#### `CompetitiveMovement`

- Type: `string`
- Description: Represents the competitive movement of the player.

#### `AFKPenalty`

- Type: `long?`
- Description: Represents the AFK penalty applied to the player.

### WinsByTier Record

The `WinsByTier` record represents the number of wins for each tier.

#### `0` to `29`

- Type: `long?`
- Description: Represents the number of wins for the corresponding tier.
