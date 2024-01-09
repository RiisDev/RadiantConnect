## Message Record

The `Message` record represents a message within the RadiantConnect network.

### Properties

#### `Body`

- Type: `string`
- Description: Represents the content of the message.

#### `Cid`

- Type: `string`
- Description: Represents the unique identifier for the conversation associated with the message.

#### `DroppedDueToThrottle`

- Type: `bool`
- Description: Indicates whether the message was dropped due to throttling.

#### `GameName`

- Type: `string`
- Description: Represents the name of the game associated with the message.

#### `GameTag`

- Type: `string`
- Description: Represents the tag of the game associated with the message.

#### `Id`

- Type: `string`
- Description: Represents the unique identifier for the message.

#### `Mid`

- Type: `string`
- Description: Represents the unique identifier for the message.

#### `Name`

- Type: `string`
- Description: Represents the name associated with the message.

#### `Pid`

- Type: `string`
- Description: Represents the player ID associated with the message.

#### `Puuid`

- Type: `string`
- Description: Represents the player UUID associated with the message.

#### `Read`

- Type: `bool`
- Description: Indicates whether the message has been read.

#### `Region`

- Type: `string`
- Description: Represents the region associated with the message.

#### `Time`

- Type: `string`
- Description: Represents the time the message was sent.

#### `Type`

- Type: `string`
- Description: Represents the type of message.

#### `UicEvent`

- Type: `bool`
- Description: Indicates whether the message is a UIC event.

## InternalMessages Record

The `InternalMessages` record contains a list of messages within the RadiantConnect network.

### Properties

#### `Messages`

- Type: `IReadOnlyList<Message>`
- Description: Represents a list of messages.

