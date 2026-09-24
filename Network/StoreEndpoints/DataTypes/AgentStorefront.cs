namespace RadiantConnect.Network.StoreEndpoints.DataTypes
{
	public record AgentStoreReward(
		[property: JsonPropertyName("ItemTypeID")] string ItemTypeId,
		[property: JsonPropertyName("ItemID")] string ItemId,
		[property: JsonPropertyName("Quantity")] long? Quantity
	);

	public record AgentStoreOffer(
		[property: JsonPropertyName("OfferID")] string OfferId,
		[property: JsonPropertyName("IsDirectPurchase")] bool? IsDirectPurchase,
		[property: JsonPropertyName("StartDate")] string StartDate,
		[property: JsonPropertyName("Cost")] Dictionary<string, long> Cost,
		[property: JsonPropertyName("Rewards")] IReadOnlyList<AgentStoreReward> Rewards
	);

	public record AgentStoreOffers(
		[property: JsonPropertyName("AgentID")] string AgentId,
		[property: JsonPropertyName("StoreOffers")] IReadOnlyList<AgentStoreOffer> StoreOffers
	);

	public record AgentStore(
		[property: JsonPropertyName("AgentStoreOffers")] IReadOnlyList<AgentStoreOffers> AgentStoreOffers,
		[property: JsonPropertyName("CurrentFeaturedAgent")] string CurrentFeaturedAgent,
		[property: JsonPropertyName("NextFeaturedAgent")] string NextFeaturedAgent
	);

	public record AgentStorefront(
		[property: JsonPropertyName("AgentStore")] AgentStore AgentStore
	);
}
