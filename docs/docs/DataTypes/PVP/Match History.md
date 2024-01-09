## MatchHistoryInternal Record

The `MatchHistoryInternal` record represents internal details of a match history in the RadiantConnect network PVP endpoints.

### Properties

- **MatchID**
  - Type: `string`
  - Description: Represents the unique identifier for the match.

- **GameStartTime**
  - Type: `object`
  - Description: Represents the start time of the game.

- **QueueID**
  - Type: `string`
  - Description: Represents the unique identifier for the queue.

## MatchHistory Record

The `MatchHistory` record represents the overall match history in the RadiantConnect network PVP endpoints.

### Properties

- **Subject**
  - Type: `string`
  - Description: Represents the subject associated with the match history.

- **BeginIndex**
  - Type: `long?`
  - Description: Represents the beginning index for the match history.

- **EndIndex**
  - Type: `long?`
  - Description: Represents the ending index for the match history.

- **Total**
  - Type: `long?`
  - Description: Represents the total number of matches in the history.

- **History**
  - Type: `IReadOnlyList<MatchHistoryInternal>`
  - Description: Represents a list of match history internal details.
