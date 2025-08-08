namespace RadiantConnect.Network.PartyEndpoints.DataTypes;

public record GameRuleModifier(
    [property: JsonPropertyName("AllowGameModifiers")] bool AllowGameModifiers,
    [property: JsonPropertyName("PlayOutAllRounds")] bool PlayOutAllRounds,
    [property: JsonPropertyName("SkipMatchHistory")] bool SkipMatchHistory,
    [property: JsonPropertyName("TournamentMode")] bool TournamentMode,
    [property: JsonPropertyName("IsOvertimeWinByTwo")] string IsOvertimeWinByTwo
);

public record CustomGameSettings(
    [property: JsonPropertyName("Map")] string Map,
    [property: JsonPropertyName("Mode")] string Mode,
    [property: JsonPropertyName("UseBots")] bool UseBots,
    [property: JsonPropertyName("GamePod")] string GamePod,
    [property: JsonPropertyName("GameRules")] GameRuleModifier GameRules
);