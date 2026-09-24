namespace RadiantConnect.Network.GoldstarsEndpoints.DataTypes
{
	public record Goldstar(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("tempT")] string TempT
	);
}
