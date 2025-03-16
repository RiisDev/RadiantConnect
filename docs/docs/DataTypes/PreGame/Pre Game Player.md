## PreGamePlayer Record

```csharp
public record PreGamePlayer(
    [property: JsonPropertyName("Subject")] string? Subject,
    [property: JsonPropertyName("MatchID")] string? MatchId,
    [property: JsonPropertyName("Version")] long? Version
);
```