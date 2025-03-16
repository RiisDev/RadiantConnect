## NewMessage Record

```csharp
public enum ChatType
{
    groupchat,
    chat,
    system
}

public record NewMessage(
    [property: JsonPropertyName("cid")] string ConversationId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("type")] string ChatType
);
```