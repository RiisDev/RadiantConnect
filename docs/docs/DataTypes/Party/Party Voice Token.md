## PartyVoiceToken Record

```csharp
public record PartyVoiceToken(
    [property: JsonPropertyName("Token")] string? Token,
    [property: JsonPropertyName("Room")] string? Room
);```