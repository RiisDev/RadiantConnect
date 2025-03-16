## NameService Record

```csharp
public record NameService(
    [property: JsonPropertyName("DisplayName")] string DisplayName,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("GameName")] string GameName,
    [property: JsonPropertyName("TagLine")] string TagLine
);
```