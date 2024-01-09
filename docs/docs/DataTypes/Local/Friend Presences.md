## Presence Record

The `Presence` record represents the presence information of a user in the RadiantConnect network.

### Properties

#### `Actor`

- Type: `object`
- Description: Represents the actor associated with the presence.

#### `Basic`

- Type: `string`
- Description: Represents basic information associated with the presence.

#### `Details`

- Type: `object`
- Description: Represents additional details associated with the presence.

#### `GameName`

- Type: `string`
- Description: Represents the game name associated with the presence.

#### `GameTag`

- Type: `string`
- Description: Represents the game tag associated with the presence.

#### `Location`

- Type: `object`
- Description: Represents the location information associated with the presence.

#### `Msg`

- Type: `string`
- Description: Represents a message associated with the presence.

#### `Name`

- Type: `string`
- Description: Represents the name associated with the presence.

#### `Parties`

- Type: `IReadOnlyList<object>`
- Description: Represents a list of parties associated with the presence.

#### `Patchline`

- Type: `string`
- Description: Represents the patchline associated with the presence.

#### `Pid`

- Type: `string`
- Description: Represents the Pid associated with the presence.

#### `Platform`

- Type: `string`
- Description: Represents the platform associated with the presence.

#### `Private`

- Type: `string`
- Description: Represents the private information associated with the presence.

#### `PrivateJwt`

- Type: `object`
- Description: Represents the private JWT (JSON Web Token) associated with the presence.

#### `Product`

- Type: `string`
- Description: Represents the product associated with the presence.

#### `Puuid`

- Type: `string`
- Description: Represents the Puuid associated with the presence.

#### `Region`

- Type: `string`
- Description: Represents the region associated with the presence.

#### `Resource`

- Type: `string`
- Description: Represents the resource associated with the presence.

#### `State`

- Type: `string`
- Description: Represents the state associated with the presence.

#### `Summary`

- Type: `string`
- Description: Represents a summary associated with the presence.

#### `Time`

- Type: `object`
- Description: Represents the time information associated with the presence.

## FriendPresences Record

The `FriendPresences` record represents a list of friend presences in the RadiantConnect network.

### Properties

#### `Presences`

- Type: `IReadOnlyList<Presence>`
- Description: Represents a list of presences.
