namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PremierPlayer(
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("rosterId")] string? RosterId,
		[property: JsonPropertyName("invites")] IReadOnlyList<object>? Invites,
		[property: JsonPropertyName("version")] int? Version,
		[property: JsonPropertyName("createdAt")] long? CreatedAt,
		[property: JsonPropertyName("updatedAt")] long? UpdatedAt
	);
}
