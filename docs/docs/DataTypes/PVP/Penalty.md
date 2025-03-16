## Penalty Record

```csharp
public record Penalty(
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Penalties")] IReadOnlyList<object> Penalties,
    [property: JsonPropertyName("Version")] long? Version
);```