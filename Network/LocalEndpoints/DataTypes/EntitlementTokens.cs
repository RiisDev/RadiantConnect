namespace RadiantConnect.Network.LocalEndpoints.DataTypes
{
	public record EntitlementTokens(
		[property: JsonPropertyName("accessToken")] string AccessToken,
		[property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
		[property: JsonPropertyName("issuer")] string Issuer,
		[property: JsonPropertyName("subject")] string Subject,
		[property: JsonPropertyName("token")] string Token
	);
}

