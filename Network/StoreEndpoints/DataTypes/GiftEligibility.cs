namespace RadiantConnect.Network.StoreEndpoints.DataTypes
{
	public record GiftEligibility(
		[property: JsonPropertyName("FailureReasons")] IReadOnlyList<string> FailureReasons,
		[property: JsonPropertyName("GiftingEligibleStatus")] long? GiftingEligibleStatus
	);

	public record GiftRecipientEligibility(
		[property: JsonPropertyName("PurchaserFailureReasons")] IReadOnlyList<string> PurchaserFailureReasons,
		[property: JsonPropertyName("RecipientFailureReasons")] IReadOnlyList<string> RecipientFailureReasons,
		[property: JsonPropertyName("RecipientOffers")] IReadOnlyList<object> RecipientOffers
	);
}
