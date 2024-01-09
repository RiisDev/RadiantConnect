## Participant Record

The `Participant` record represents a participant within the RadiantConnect network.

### Properties

#### `ActivePlatform`

- Type: `object`
- Description: Represents the active platform associated with the participant.

#### `Cid`

- Type: `string`
- Description: Represents the unique identifier for the conversation associated with the participant.

#### `GameName`

- Type: `string`
- Description: Represents the name of the game associated with the participant.

#### `GameTag`

- Type: `string`
- Description: Represents the tag of the game associated with the participant.

#### `Muted`

- Type: `bool`
- Description: Indicates whether the participant is muted.

#### `Name`

- Type: `string`
- Description: Represents the name of the participant.

#### `Pid`

- Type: `string`
- Description: Represents the player ID associated with the participant.

#### `Puuid`

- Type: `string`
- Description: Represents the player UUID associated with the participant.

#### `Region`

- Type: `string`
- Description: Represents the region associated with the participant.

## ChatParticipant Record

The `ChatParticipant` record contains information about participants within a chat.

### Properties

#### `Participants`

- Type: `IReadOnlyList<Participant>`
- Description: Represents a list of participants within the chat.

