## EndProgress Record

The `EndProgress` record represents the end progress information in a PVP endpoint in the RadiantConnect network.

### Properties

#### `Level`

- Type: `long?`
- Description: Represents the level at the end of the progress.

#### `XP`

- Type: `long?`
- Description: Represents the XP (Experience Points) at the end of the progress.

## History Record

The `History` record represents the history information in a PVP endpoint in the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the identifier of the history.

#### `MatchStart`

- Type: `DateTime?`
- Description: Represents the start time of the match.

#### `StartProgress`

- Type: `StartProgress`
- Description: Represents the start progress information.

#### `EndProgress`

- Type: `EndProgress`
- Description: Represents the end progress information.

#### `XPDelta`

- Type: `long?`
- Description: Represents the XP (Experience Points) delta.

#### `XPSources`

- Type: `IReadOnlyList<XPSource>`
- Description: Represents a list of XP sources.

#### `XPMultipliers`

- Type: `IReadOnlyList<object>`
- Description: Represents a list of XP multipliers.

## Progress Record

The `Progress` record represents the progress information in a PVP endpoint in the RadiantConnect network.

### Properties

#### `Level`

- Type: `long?`
- Description: Represents the level of the progress.

#### `XP`

- Type: `long?`
- Description: Represents the XP (Experience Points) of the progress.

## AccountXP Record

The `AccountXP` record represents the account XP information in a PVP endpoint in the RadiantConnect network.

### Properties

#### `Version`

- Type: `long?`
- Description: Represents the version of the account XP.

#### `Subject`

- Type: `string`
- Description: Represents the subject of the account XP.

#### `Progress`

- Type: `Progress`
- Description: Represents the progress information.

#### `History`

- Type: `IReadOnlyList<History>`
- Description: Represents a list of XP history.

#### `LastTimeGrantedFirstWin`

- Type: `string`
- Description: Represents the last time the first win was granted.

#### `NextTimeFirstWinAvailable`

- Type: `string`
- Description: Represents the next time the first win will be available.

## StartProgress Record

The `StartProgress` record represents the start progress information in a PVP endpoint in the RadiantConnect network.

### Properties

#### `Level`

- Type: `long?`
- Description: Represents the level at the start of the progress.

#### `XP`

- Type: `long?`
- Description: Represents the XP (Experience Points) at the start of the progress.

## XPSource Record

The `XPSource` record represents an XP source in a PVP endpoint in the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the identifier of the XP source.

#### `Amount`

- Type: `long?`
- Description: Represents the amount of XP from the source.
