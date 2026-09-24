namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record PlayerReportToken(
		[property: JsonPropertyName("Token")] string Token
	);
}
