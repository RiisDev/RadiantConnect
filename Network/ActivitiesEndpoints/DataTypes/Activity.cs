namespace RadiantConnect.Network.ActivitiesEndpoints.DataTypes
{
	public record PartyActivity(
		[property: JsonPropertyName("activityId")] string ActivityId
	);
}
