namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	// ReSharper disable All

	public record SetPlayerLoadout(
		[property: JsonPropertyName("Guns")] IReadOnlyList<Gun> Guns,
		[property: JsonPropertyName("Sprays")] IReadOnlyList<Spray> Sprays,
		[property: JsonPropertyName("Identity")] Identity Identity,
		[property: JsonPropertyName("Incognito")] bool? Incognito
	);
}