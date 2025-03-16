## CurrentGamePlayer Record

```csharp
public record CurrentGamePlayer(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("MatchID")] string MatchId,
    [property: JsonPropertyName("Version")] long Version
);```