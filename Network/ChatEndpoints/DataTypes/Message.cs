using System.Text.Json.Serialization;
namespace RadiantConnect.Network.ChatEndpoints.DataTypes;
// ReSharper disable All

public record Message(
    [property: JsonPropertyName("body")] string Body,
    [property: JsonPropertyName("cid")] string Cid,
    [property: JsonPropertyName("droppedDueToThrottle")] bool DroppedDueToThrottle,
    [property: JsonPropertyName("game_name")] string GameName,
    [property: JsonPropertyName("game_tag")] string GameTag,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("mid")] string Mid,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("pid")] string Pid,
    [property: JsonPropertyName("puuid")] string Puuid,
    [property: JsonPropertyName("read")] bool Read,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("time")] string Time,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("uicEvent")] bool UicEvent
);

public record InternalMessages(
    [property: JsonPropertyName("messages")] IReadOnlyList<Message> Messages
);