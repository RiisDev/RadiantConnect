## AllyTeam Record

The `AllyTeam` record represents an ally team in a pre-game match within the RadiantConnect network.

### Properties

#### `TeamID`

- Type: `string`
- Description: Represents the unique identifier for the ally team.

#### `Players`

- Type: `IReadOnlyList<Player>`
- Description: Represents the list of players in the ally team.

## CastedVotes Record

The `CastedVotes` record represents casted votes in a pre-game match within the RadiantConnect network.

### Properties

This record does not have any properties.

## Player Record

The `Player` record represents a player in a pre-game match within the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier for the player.

#### `CharacterID`

- Type: `string`
- Description: Represents the unique identifier for the character associated with the player.

#### `CharacterSelectionState`

- Type: `string`
- Description: Represents the state of character selection for the player.

#### `PregamePlayerState`

- Type: `string`
- Description: Represents the state of the player in the pre-game phase.

#### `CompetitiveTier`

- Type: `long`
- Description: Represents the competitive tier associated with the player.

#### `PlayerIdentity`

- Type: `PlayerIdentity`
- Description: Represents the identity information for the player.

#### `SeasonalBadgeInfo`

- Type: `SeasonalBadgeInfo`
- Description: Represents seasonal badge information for the player.

#### `IsCaptain`

- Type: `bool`
- Description: Indicates whether the player is the captain of the team.

## PlayerIdentity Record

The `PlayerIdentity` record represents the identity information for a player in a pre-game match within the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier for the player.

#### `PlayerCardID`

- Type: `string`
- Description: Represents the unique identifier for the player's card.

#### `PlayerTitleID`

- Type: `string`
- Description: Represents the unique identifier for the player's title.

#### `AccountLevel`

- Type: `long`
- Description: Represents the account level of the player.

#### `PreferredLevelBorderID`

- Type: `string`
- Description: Represents the preferred level border identifier for the player.

#### `Incognito`

- Type: `bool`
- Description: Indicates whether the player is in incognito mode.

#### `HideAccountLevel`

- Type: `bool`
- Description: Indicates whether the player's account level is hidden.

## PreGameMatch Record

The `PreGameMatch` record represents a pre-game match within the RadiantConnect network.

### Properties

... (skipping detailed description for brevity)

## SeasonalBadgeInfo Record

The `SeasonalBadgeInfo` record represents seasonal badge information for a player in a pre-game match within the RadiantConnect network.

### Properties

#### `SeasonID`

- Type: `string`
- Description: Represents the unique identifier for the season.

#### `NumberOfWins`

- Type: `long`
- Description: Represents the number of wins for the player in the season.

#### `WinsByTier`

- Type: `object`
- Description: Represents wins by tier information for the player.

#### `Rank`

- Type: `long`
- Description: Represents the rank of the player in the season.

#### `LeaderboardRank`

- Type: `long`
- Description: Represents the leaderboard rank of the player in the season.

## Team Record

The `Team` record represents a team in a pre-game match within the RadiantConnect network.

### Properties

#### `TeamID`

- Type: `string`
- Description: Represents the unique identifier for the team.

#### `Players`

- Type: `IReadOnlyList<Player>`
- Description: Represents the list of players in the team.
