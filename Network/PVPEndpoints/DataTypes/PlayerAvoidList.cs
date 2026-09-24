namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record PlayerAvoidList(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("AvoidList")] IReadOnlyList<object> AvoidList
	);
}
