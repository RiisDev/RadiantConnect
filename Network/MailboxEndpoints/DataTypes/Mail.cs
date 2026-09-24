namespace RadiantConnect.Network.MailboxEndpoints.DataTypes
{
	public record Mail(
		[property: JsonPropertyName("mailId")] string MailId,
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("message")] string Message,
		[property: JsonPropertyName("product")] string Product,
		[property: JsonPropertyName("region")] string? Region,
		[property: JsonPropertyName("tags")] IReadOnlyList<string> Tags,
		[property: JsonPropertyName("mailType")] string MailType,
		[property: JsonPropertyName("state")] string State,
		[property: JsonPropertyName("createdAt")] long CreatedAt
	);
}
