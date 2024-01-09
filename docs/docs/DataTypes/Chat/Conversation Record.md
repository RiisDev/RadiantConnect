## Conversation Record

The `Conversation` record represents a chat conversation within the RadiantConnect network.

### Properties

#### `Cid`

- Type: `string`
- Description: Represents the unique identifier for the conversation.

#### `DirectMessages`

- Type: `bool`
- Description: Indicates whether the conversation involves direct messages.

#### `GlobalReadership`

- Type: `bool`
- Description: Specifies if the conversation has global readership.

#### `MessageHistory`

- Type: `bool`
- Description: Indicates whether the conversation has message history.

#### `Mid`

- Type: `string`
- Description: Represents the unique identifier for the message.

#### `Muted`

- Type: `bool`
- Description: Indicates whether the conversation is muted.

#### `MutedRestriction`

- Type: `bool`
- Description: Specifies if there are muted restrictions in place.

#### `Type`

- Type: `string`
- Description: Represents the type of conversation.

#### `UiState`

- Type: `UiState`
- Description: Represents the user interface state for the conversation.

#### `UnreadCount`

- Type: `int`
- Description: Represents the count of unread messages in the conversation.

## Internal ChatInfo Record

The `ChatInfo` record contains information about chat conversations.

### Properties

#### `Conversations`

- Type: `IReadOnlyList<Conversation>`
- Description: Represents a list of conversations within the chat.

## Internal UiState Record

The `UiState` record represents the user interface state for a conversation.

### Properties

#### `ChangedSinceHidden`

- Type: `bool`
- Description: Indicates whether the UI state has changed since it was hidden.

#### `Hidden`

- Type: `bool`
- Description: Indicates whether the UI state is hidden.
