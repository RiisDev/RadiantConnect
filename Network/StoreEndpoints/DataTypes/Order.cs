namespace RadiantConnect.Network.StoreEndpoints.DataTypes
{
	public record OrderReward(
		[property: JsonPropertyName("RewardID")] string RewardId,
		[property: JsonPropertyName("Amount")] long? Amount
	);

	public record Order(
		[property: JsonPropertyName("OrderID")] string OrderId,
		[property: JsonPropertyName("Status")] string Status,
		[property: JsonPropertyName("OrderRewards")] Dictionary<string, IReadOnlyList<OrderReward>>? OrderRewards = null
	);
}
