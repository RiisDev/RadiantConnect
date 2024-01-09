## ConnectionDetails Record

The `ConnectionDetails` record represents details about the connection to the game server within the RadiantConnect network.

### Properties

#### `GameServerHosts`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of game server hosts.

#### `GameServerHost`

- Type: `string`
- Description: Represents the game server host.

#### `GameServerPort`

- Type: `long`
- Description: Represents the port of the game server.

#### `GameServerObfuscatedIP`

- Type: `long`
- Description: Represents the obfuscated IP address of the game server.

#### `GameClientHash`

- Type: `long`
- Description: Represents the hash associated with the game client.

#### `PlayerKey`

- Type: `string`
- Description: Represents the key associated with the player.

## Player Record

The `Player` record represents a player's information in the current game within the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the subject identifier associated with the player.

#### `TeamID`

- Type: `string`
- Description: Represents the unique identifier for the team.

#### `CharacterID`

- Type: `string`
- Description: Represents the unique identifier for the character.

#### `PlayerIdentity`

- Type: `PlayerIdentity`
- Description: Represents the identity details of the player.

#### `SeasonalBadgeInfo`

- Type: `SeasonalBadgeInfo`
- Description: Represents the seasonal badge information for the player.

#### `IsCoach`

- Type: `bool`
- Description: Indicates whether the player is a coach.

#### `IsAssociated`

- Type: `bool`
- Description: Indicates whether the player is associated.

## PlayerIdentity Record

The `PlayerIdentity` record represents the identity details of a player within the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the subject identifier associated with the player identity.

#### `PlayerCardID`

- Type: `string`
- Description: Represents the unique identifier for the player card.

#### `PlayerTitleID`

- Type: `string`
- Description: Represents the unique identifier for the player title.

#### `AccountLevel`

- Type: `long`
- Description: Represents the account level of the player.

#### `PreferredLevelBorderID`

- Type: `string`
- Description: Represents the preferred level border identifier.

#### `Incognito`

- Type: `bool`
- Description: Indicates whether the player is in incognito mode.

#### `HideAccountLevel`

- Type: `bool`
- Description: Indicates whether the account level should be hidden.

## CurrentGameMatch Record

The `CurrentGameMatch` record represents details about the current game match within the RadiantConnect network.

### Properties

#### `MatchID`

- Type: `string`
- Description: Represents the unique identifier for the current game match.

#### `Version`

- Type: `long`
- Description: Represents the version of the current game match.

#### `State`

- Type: `string`
- Description: Represents the state of the current game match.

#### `MapID`

- Type: `string`
- Description: Represents the unique identifier for the map.

#### `ModeID`

- Type: `string`
- Description: Represents the unique identifier for the mode.

#### `ProvisioningFlow`

- Type: `string`
- Description: Represents the provisioning flow for the current game match.

#### `GamePodID`

- Type: `string`
- Description: Represents the unique identifier for the game pod.

#### `AllMUCName`

- Type: `string`
- Description: Represents the MUC (Multi-User Chat) name for all players.

#### `TeamMUCName`

- Type: `string`
- Description: Represents the MUC name for the team.

#### `TeamVoiceID`

- Type: `string`
- Description: Represents the unique identifier for the team's voice.

#### `TeamMatchToken`

- Type: `string`
- Description: Represents the match token for the team.

#### `IsReconnectable`

- Type: `bool`
- Description: Indicates whether the match is reconnectable.

#### `ConnectionDetails`

- Type: `ConnectionDetails`
- Description: Represents the connection details for the current game match.

#### `PostGameDetails`

- Type: `object`
- Description: Represents details about the post-game state (type can be specified).

#### `Players`

- Type: `IReadOnlyList<Player>`
- Description: Represents a list of players participating in the current game match.

#### `MatchmakingData`

- Type: `MatchmakingData`
- Description: Represents matchmaking data for the current game match.

## MatchmakingData Record

The `MatchmakingData` record represents matchmaking data for a current game match within the RadiantConnect network.

### Properties

#### `QueueID`

- Type: `string`
- Description: Represents the unique identifier for the matchmaking queue.

#### `IsRanked`

- Type: `bool`
- Description: Indicates whether the matchmaking is ranked.

## SeasonalBadgeInfo Record

The `SeasonalBadgeInfo` record represents seasonal badge information for a player within the RadiantConnect network.

### Properties

#### `SeasonID`

- Type: `string`
- Description: Represents the unique identifier for the season.

#### `NumberOfWins`

- Type: `long`
- Description: Represents the number of wins for the player in the season.

#### `WinsByTier`

- Type: `object`
- Description: Represents wins categorized by tier (type can be specified).

#### `Rank`

- Type: `long`
- Description: Represents the player's rank in the season.

#### `LeaderboardRank`

- Type: `long`
- Description: Represents the player's rank on the leaderboard.

