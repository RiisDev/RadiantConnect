namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record ActiveIntervention(
		[property: JsonPropertyName("InterventionName")] string InterventionName,
		[property: JsonPropertyName("Expiry")] string Expiry,
		[property: JsonPropertyName("IssuingTime")] string IssuingTime,
		[property: JsonPropertyName("OriginInfraction")] string OriginInfraction
	);

	public record AppliedInfraction(
		[property: JsonPropertyName("InfractionName")] string InfractionName,
		[property: JsonPropertyName("Severity")] string Severity,
		[property: JsonPropertyName("AppliedInterventions")] IReadOnlyList<ActiveIntervention> AppliedInterventions
	);

	public record InterventionCategory(
		[property: JsonPropertyName("BehaviorCategory")] string BehaviorCategory,
		[property: JsonPropertyName("BehaviorRatingName")] string BehaviorRatingName,
		[property: JsonPropertyName("LastRatingReduction")] string LastRatingReduction,
		[property: JsonPropertyName("ActiveInterventions")] IReadOnlyList<ActiveIntervention> ActiveInterventions,
		[property: JsonPropertyName("NextInterventionNames")] IReadOnlyList<string> NextInterventionNames,
		[property: JsonPropertyName("AppliedInfractions")] Dictionary<string, AppliedInfraction> AppliedInfractions
	);

	public record PlayerInterventions(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("InterventionsByCategory")] IReadOnlyList<InterventionCategory> InterventionsByCategory
	);
}
