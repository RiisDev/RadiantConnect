namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

// ReSharper disable All

public record SetPlayerLoadout(
    [property: JsonPropertyName("Guns")] List<Gun> Guns,
    [property: JsonPropertyName("Sprays")] List<Spray> Sprays,
    [property: JsonPropertyName("Identity")] Identity Identity,
    [property: JsonPropertyName("Incognito")] bool? Incognito
);