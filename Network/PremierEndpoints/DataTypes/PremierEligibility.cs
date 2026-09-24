namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PremierEligibility(
		[property: JsonPropertyName("subject")] string Subject,
		[property: JsonPropertyName("accountVerificationStatus")] bool AccountVerificationStatus,
		[property: JsonPropertyName("rankedPlacementCompletionStatus")] bool RankedPlacementCompletionStatus,
		[property: JsonPropertyName("matchLimitReached")] bool MatchLimitReached,
		[property: JsonPropertyName("isOnProvidedRoster")] bool IsOnProvidedRoster,
		[property: JsonPropertyName("playerRestrictions")] object? PlayerRestrictions
	);
}
