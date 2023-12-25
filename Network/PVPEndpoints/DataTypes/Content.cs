using System.Text.Json.Serialization;

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record Event(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("StartTime")] DateTime? StartTime,
    [property: JsonPropertyName("EndTime")] DateTime? EndTime,
    [property: JsonPropertyName("IsActive")] bool? IsActive
);

public record Content(
    [property: JsonPropertyName("DisabledIDs")] IReadOnlyList<object> DisabledIDs,
    [property: JsonPropertyName("Seasons")] IReadOnlyList<Season> Seasons,
    [property: JsonPropertyName("Events")] IReadOnlyList<Event> Events
);

public record Season(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Type")] string Type,
    [property: JsonPropertyName("StartTime")] DateTime? StartTime,
    [property: JsonPropertyName("EndTime")] DateTime? EndTime,
    [property: JsonPropertyName("IsActive")] bool? IsActive
);