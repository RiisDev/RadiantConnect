using System.Text.Json.Serialization;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record EndProgress(
    [property: JsonPropertyName("Level")] int? Level,
    [property: JsonPropertyName("XP")] int? XP
);

public record History(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("MatchStart")] DateTime? MatchStart,
    [property: JsonPropertyName("StartProgress")] StartProgress StartProgress,
    [property: JsonPropertyName("EndProgress")] EndProgress EndProgress,
    [property: JsonPropertyName("XPDelta")] int? XPDelta,
    [property: JsonPropertyName("XPSources")] IReadOnlyList<XPSource> XPSources,
    [property: JsonPropertyName("XPMultipliers")] IReadOnlyList<object> XPMultipliers
);

public record Progress(
    [property: JsonPropertyName("Level")] int? Level,
    [property: JsonPropertyName("XP")] int? XP
);

public record AccountXP(
    [property: JsonPropertyName("Version")] int? Version,
    [property: JsonPropertyName("Subject")] string Subject,
    [property: JsonPropertyName("Progress")] Progress Progress,
    [property: JsonPropertyName("History")] IReadOnlyList<History> History,
    [property: JsonPropertyName("LastTimeGrantedFirstWin")] string LastTimeGrantedFirstWin,
    [property: JsonPropertyName("NextTimeFirstWinAvailable")] string NextTimeFirstWinAvailable
);

public record StartProgress(
    [property: JsonPropertyName("Level")] int? Level,
    [property: JsonPropertyName("XP")] int? XP
);

public record XPSource(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Amount")] int? Amount
);