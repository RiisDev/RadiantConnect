## NewMessage Record

The `NewMessage` record represents a new message within the RadiantConnect network.

### Properties

#### `ConversationId`

- Type: `string`
- Description: Represents the unique identifier for the conversation associated with the new message.

#### `Message`

- Type: `string`
- Description: Represents the content of the new message.

#### `ChatType`

- Type: `string`
- Description: Represents the type of the chat, possible values are `groupchat`, `chat`, or `system`.

### ChatType Enum

The `ChatType` enum represents the type of chat associated with a new message.

#### Enum Values:

- `groupchat`: Indicates a group chat.
- `chat`: Indicates a regular chat.
- `system`: Indicates a system message.

**Note:** Use the `ChatType` enum to specify the type of chat when creating a new message.

```csharp
// Example Usage:
var newMessage = new NewMessage
{
	ConversationId = "123",
	Message = "Hello, world!",
	ChatType = ChatType.chat.ToString()
};
```