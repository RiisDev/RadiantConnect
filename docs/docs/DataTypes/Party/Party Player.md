## PartyPlayer Record

The `PartyPlayer` record represents information about a player within the context of a party in the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier associated with the player.

#### `Version`

- Type: `long?`
- Description: Represents the version information associated with the player.

#### `CurrentPartyID`

- Type: `string`
- Description: Represents the identifier of the current party the player is associated with.

#### `Invites`

- Type: `object`
- Description: Represents the invites associated with the player.

#### `Requests`

- Type: `IReadOnlyList<object>`
- Description: Represents the list of requests associated with the player.

#### `PlatformInfo`

- Type: `PlatformInfo`
- Description: Represents information about the platform of the player.

### PlatformInfo

#### `PlatformType`

- Type: `string`
- Description: Represents the type of platform.

#### `PlatformOS`

- Type: `string`
- Description: Represents the operating system of the platform.

#### `PlatformOSVersion`

- Type: `string`
- Description: Represents the version of the operating system.

#### `PlatformChipset`

- Type: `string`
- Description: Represents the chipset of the platform.
