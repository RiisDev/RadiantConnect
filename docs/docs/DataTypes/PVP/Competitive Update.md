## Match Record

The `Match` record represents information about a competitive match in a PVP endpoint in the RadiantConnect network.

### Properties

- **MatchID**
  - Type: `string`
  - Description: Represents the unique identifier for the match.

- **MapID**
  - Type: `string`
  - Description: Represents the unique identifier for the map associated with the match.

- **SeasonID**
  - Type: `string`
  - Description: Represents the unique identifier for the season associated with the match.

- **MatchStartTime**
  - Type: `object`
  - Description: Represents the start time of the match.

- **TierAfterUpdate**
  - Type: `long?`
  - Description: Represents the tier after the update.

- **TierBeforeUpdate**
  - Type: `long?`
  - Description: Represents the tier before the update.

- **RankedRatingAfterUpdate**
  - Type: `long?`
  - Description: Represents the ranked rating after the update.

- **RankedRatingBeforeUpdate**
  - Type: `long?`
  - Description: Represents the ranked rating before the update.

- **RankedRatingEarned**
  - Type: `long?`
  - Description: Represents the ranked rating earned in the match.

- **RankedRatingPerformanceBonus**
  - Type: `long?`
  - Description: Represents the performance bonus for ranked rating.

- **CompetitiveMovement**
  - Type: `string`
  - Description: Represents the competitive movement.

- **AFKPenalty**
  - Type: `long?`
  - Description: Represents the AFK penalty.

## CompetitiveUpdate Record

The `CompetitiveUpdate` record represents an update related to competitive play in a PVP endpoint in the RadiantConnect network.

### Properties

- **Version**
  - Type: `long?`
  - Description: Represents the version of the competitive update.

- **Subject**
  - Type: `string`
  - Description: Represents the subject of the competitive update.

- **Matches**
  - Type: `IReadOnlyList<Match>`
  - Description: Represents a list of matches associated with the competitive update.
