namespace RadiantConnect.Network.PartyEndpoints.DataTypes;

public record PartyVoiceToken(
    [property: JsonPropertyName("Token")] string? Token,
    [property: JsonPropertyName("Room")] string? Room
);