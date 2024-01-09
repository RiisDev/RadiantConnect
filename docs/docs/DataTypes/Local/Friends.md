## Friend Record

The `Friend` record represents a friend in the RadiantConnect network.

### Properties

#### `ActivePlatform`

- Type: `string`
- Description: Represents the active platform associated with the friend.

#### `DisplayGroup`

- Type: `string`
- Description: Represents the display group associated with the friend.

#### `GameName`

- Type: `string`
- Description: Represents the game name associated with the friend.

#### `GameTag`

- Type: `string`
- Description: Represents the game tag associated with the friend.

#### `Group`

- Type: `string`
- Description: Represents the group associated with the friend.

#### `LastOnlineTs`

- Type: `long?`
- Description: Represents the timestamp when the friend was last online.

#### `Name`

- Type: `string`
- Description: Represents the name associated with the friend.

#### `Note`

- Type: `string`
- Description: Represents the note associated with the friend.

#### `Pid`

- Type: `string`
- Description: Represents the Pid associated with the friend.

#### `Puuid`

- Type: `string`
- Description: Represents the Puuid associated with the friend.

#### `Region`

- Type: `string`
- Description: Represents the region associated with the friend.

## InternalFriends Record

The `InternalFriends` record represents a list of internal friends in the RadiantConnect network.

### Properties

#### `Friends`

- Type: `IReadOnlyList<Friend>`
- Description: Represents a list of friends.
