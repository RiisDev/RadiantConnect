using System.Text.Json.Serialization;
namespace RadiantConnect.Network.LocalEndpoints.DataTypes;
// ReSharper disable All

public record Request(
    [property: JsonPropertyName("game_name")] string GameName,
    [property: JsonPropertyName("game_tag")] string GameTag,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("note")] string Note,
    [property: JsonPropertyName("pid")] string Pid,
    [property: JsonPropertyName("puuid")] string Puuid,
    [property: JsonPropertyName("region")] string Region,
    [property: JsonPropertyName("subscription")] string Subscription
);

public record InternalRequests(
    [property: JsonPropertyName("requests")] IReadOnlyList<Request> Requests
);