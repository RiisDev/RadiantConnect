## GameRuleModifier Record

The `GameRuleModifier` record represents the game rule modifiers within the RadiantConnect network.

### Properties

#### `AllowGameModifiers`

- Type: `bool`
- Description: Indicates whether game modifiers are allowed.

#### `PlayOutAllRounds`

- Type: `bool`
- Description: Indicates whether all rounds should be played out.

#### `SkipMatchHistory`

- Type: `bool`
- Description: Indicates whether match history should be skipped.

#### `TournamentMode`

- Type: `bool`
- Description: Indicates whether tournament mode is enabled.

#### `IsOvertimeWinByTwo`

- Type: `string`
- Description: Indicates whether overtime is won by two.

---

## CustomGameSettings Record

The `CustomGameSettings` record represents the custom game settings within the RadiantConnect network.

### Properties

#### `Map`

- Type: `string`
- Description: Represents the selected map for the custom game.

#### `Mode`

- Type: `string`
- Description: Represents the game mode for the custom game.

#### `UseBots`

- Type: `bool`
- Description: Indicates whether bots are used in the custom game.

#### `GamePod`

- Type: `string`
- Description: Represents the game pod information for the custom game.

#### `GameRules`

- Type: `GameRuleModifier`
- Description: Represents the game rule modifiers for the custom game.
