namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	public record PartyChatToken(
		[property: JsonPropertyName("Token")] string? Token,
		[property: JsonPropertyName("Room")] string? Room
	);
}