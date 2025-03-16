## Presence Record

```csharp
public record Presence(
    [property: JsonPropertyName("actor")] object Actor,
    [property: JsonPropertyName("basic")] string Basic,
    [property: JsonPropertyName("details")] object Details,
    [property: JsonPropertyName("game_name")] string GameName,
    [property: JsonPropertyName("game_tag")] string GameTag,
    [property: JsonPropertyName("location")] object Location,
    [property: JsonPropertyName("msg")] string Msg,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("parties")] IReadOnlyList<object> Parties,
    [property: JsonPropertyName("patchline")] string Patchline,
    [property: JsonPropertyName("pid")] string Pid,
    [property: JsonPropertyName("platform")] string Platform,
    [property: JsonPropertyName("private")] string Private,
    [property: JsonPropertyName("privateJwt")] object PrivateJwt,
    [property: JsonPropertyName("product")] string Product,
    [property: JsonPropertyName("puuid")] string Puuid,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("resource")] string Resource,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("summary")] string Summary,
    [property: JsonPropertyName("time")] object Time
);

public record FriendPresences(
    [property: JsonPropertyName("presences")] IReadOnlyList<Presence> Presences
);
```