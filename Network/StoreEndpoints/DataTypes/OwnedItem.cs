

// ReSharper disable All

namespace RadiantConnect.Network.StoreEndpoints.DataTypes
{
	public record Entitlement(
		[property: JsonPropertyName("TypeID")] string TypeId,
		[property: JsonPropertyName("ItemID")] string ItemID,
		[property: JsonPropertyName("InstanceID")] string? InstanceId = null
	);

	public record OwnedItem(
		[property: JsonPropertyName("ItemTypeID")] string ItemTypeId,
		[property: JsonPropertyName("Entitlements")] IReadOnlyList<Entitlement> Entitlements
	);

	public record AllOwnedItems(
		[property: JsonPropertyName("EntitlementsByTypes")] IReadOnlyList<OwnedItem> EntitlementsByTypes
	);
}