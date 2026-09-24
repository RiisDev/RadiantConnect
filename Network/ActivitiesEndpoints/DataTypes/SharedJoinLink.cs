namespace RadiantConnect.Network.ActivitiesEndpoints.DataTypes
{
	public record SharedJoinLink(
		[property: JsonPropertyName("joinCode")] string JoinCode,
		[property: JsonPropertyName("smartUrl")] string SmartUrl,
		[property: JsonPropertyName("expiresAt")] DateTime ExpiresAt,
		[property: JsonPropertyName("isActive")] bool IsActive
	);
}
