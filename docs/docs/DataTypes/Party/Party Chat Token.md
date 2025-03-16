## PartyChatToken Record

```csharp
public record PartyChatToken(
    [property: JsonPropertyName("Token")] string? Token,
    [property: JsonPropertyName("Room")] string? Room
);
```