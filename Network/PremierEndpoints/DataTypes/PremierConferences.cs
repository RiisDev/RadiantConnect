namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PremierConferencesRoot(
		[property: JsonPropertyName("PremierConferences")] IReadOnlyList<PremierConference>? Conferences
	);
}
