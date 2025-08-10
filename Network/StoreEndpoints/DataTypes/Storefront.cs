
// ReSharper disable All

namespace RadiantConnect.Network.StoreEndpoints.DataTypes
{
	public record AccessoryStore(
		[property: JsonPropertyName("AccessoryStoreOffers")] IReadOnlyList<AccessoryStoreOffer> AccessoryStoreOffers,
		[property: JsonPropertyName("AccessoryStoreRemainingDurationInSeconds")] long? AccessoryStoreRemainingDurationInSeconds,
		[property: JsonPropertyName("StorefrontID")] string StorefrontID
	);

	public record AccessoryStoreOffer(
		[property: JsonPropertyName("Offer")] Offer Offer,
		[property: JsonPropertyName("ContractID")] string ContractID
	);

	public record Bundle(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("DataAssetID")] string DataAssetID,
		[property: JsonPropertyName("CurrencyID")] string CurrencyID,
		[property: JsonPropertyName("Items")] IReadOnlyList<Item> Items,
		[property: JsonPropertyName("ItemOffers")] IReadOnlyList<ItemOffer> ItemOffers,
		[property: JsonPropertyName("TotalBaseCost")] TotalBaseCost TotalBaseCost,
		[property: JsonPropertyName("TotalDiscountedCost")] TotalDiscountedCost TotalDiscountedCost,
		[property: JsonPropertyName("TotalDiscountPercent")] double? TotalDiscountPercent,
		[property: JsonPropertyName("DurationRemainingInSeconds")] long? DurationRemainingInSeconds,
		[property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly,
		[property: JsonPropertyName("IsGiftable")] long? IsGiftable
	);

	public record Bundle2(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("DataAssetID")] string DataAssetID,
		[property: JsonPropertyName("CurrencyID")] string CurrencyID,
		[property: JsonPropertyName("Items")] IReadOnlyList<Item> Items,
		[property: JsonPropertyName("ItemOffers")] IReadOnlyList<ItemOffer> ItemOffers,
		[property: JsonPropertyName("TotalBaseCost")] TotalBaseCost TotalBaseCost,
		[property: JsonPropertyName("TotalDiscountedCost")] TotalDiscountedCost TotalDiscountedCost,
		[property: JsonPropertyName("TotalDiscountPercent")] double? TotalDiscountPercent,
		[property: JsonPropertyName("DurationRemainingInSeconds")] long? DurationRemainingInSeconds,
		[property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly,
		[property: JsonPropertyName("IsGiftable")] long? IsGiftable
	);

	public record Cost(
		[property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints,
		[property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] long? KingdomCredits
	);

	public record DiscountedCost(
		[property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints
	);

	public record FeaturedBundle(
		[property: JsonPropertyName("Bundle")] Bundle Bundle,
		[property: JsonPropertyName("Bundles")] IReadOnlyList<Bundle> Bundles,
		[property: JsonPropertyName("BundleRemainingDurationInSeconds")] long? BundleRemainingDurationInSeconds
	);

	public record Item(
		[property: JsonPropertyName("Item")] Item ItemData,
		[property: JsonPropertyName("BasePrice")] long? BasePrice,
		[property: JsonPropertyName("CurrencyID")] string CurrencyID,
		[property: JsonPropertyName("DiscountPercent")] double? DiscountPercent,
		[property: JsonPropertyName("DiscountedPrice")] long? DiscountedPrice,
		[property: JsonPropertyName("IsPromoItem")] bool? IsPromoItem
	);

	public record Item2(
		[property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
		[property: JsonPropertyName("ItemID")] string ItemID,
		[property: JsonPropertyName("Amount")] long? Amount
	);

	public record ItemOffer(
		[property: JsonPropertyName("BundleItemOfferID")] string BundleItemOfferID,
		[property: JsonPropertyName("Offer")] Offer Offer,
		[property: JsonPropertyName("DiscountPercent")] double? DiscountPercent,
		[property: JsonPropertyName("DiscountedCost")] DiscountedCost DiscountedCost
	);

	public record Offer(
		[property: JsonPropertyName("OfferID")] string OfferID,
		[property: JsonPropertyName("IsDirectPurchase")] bool? IsDirectPurchase,
		[property: JsonPropertyName("StartDate")] DateTime? StartDate,
		[property: JsonPropertyName("Cost")] Cost Cost,
		[property: JsonPropertyName("Rewards")] IReadOnlyList<Reward> Rewards
	);

	public record PluginOffers(
		[property: JsonPropertyName("StoreOffers")] IReadOnlyList<StoreOffer> StoreOffers,
		[property: JsonPropertyName("RemainingDurationInSeconds")] long? RemainingDurationInSeconds
	);

	public record PluginStore(
		[property: JsonPropertyName("PluginID")] string PluginID,
		[property: JsonPropertyName("PluginOffers")] PluginOffers PluginOffers
	);

	public record PurchaseInformation(
		[property: JsonPropertyName("DataAssetID")] string DataAssetID,
		[property: JsonPropertyName("OfferID")] string OfferID,
		[property: JsonPropertyName("OfferType")] long? OfferType,
		[property: JsonPropertyName("StartDate")] DateTime? StartDate,
		[property: JsonPropertyName("PrimaryCurrencyID")] string PrimaryCurrencyID,
		[property: JsonPropertyName("Cost")] Cost Cost,
		[property: JsonPropertyName("DiscountedCost")] DiscountedCost DiscountedCost,
		[property: JsonPropertyName("DiscountedPercentage")] long? DiscountedPercentage,
		[property: JsonPropertyName("Rewards")] IReadOnlyList<object> Rewards,
		[property: JsonPropertyName("AdditionalContext")] IReadOnlyList<object> AdditionalContext,
		[property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly,
		[property: JsonPropertyName("IsGiftable")] long? IsGiftable
	);

	public record Reward(
		[property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
		[property: JsonPropertyName("ItemID")] string ItemID,
		[property: JsonPropertyName("Quantity")] long? Quantity
	);

	public record Storefront(
		[property: JsonPropertyName("FeaturedBundle")] FeaturedBundle FeaturedBundle,
		[property: JsonPropertyName("SkinsPanelLayout")] SkinsPanelLayout SkinsPanelLayout,
		[property: JsonPropertyName("UpgradeCurrencyStore")] UpgradeCurrencyStore UpgradeCurrencyStore,
		[property: JsonPropertyName("AccessoryStore")] AccessoryStore AccessoryStore,
		[property: JsonPropertyName("PluginStores")] IReadOnlyList<PluginStore> PluginStores
	);

	public record SingleItemStoreOffer(
		[property: JsonPropertyName("OfferID")] string OfferID,
		[property: JsonPropertyName("IsDirectPurchase")] bool? IsDirectPurchase,
		[property: JsonPropertyName("StartDate")] string StartDate,
		[property: JsonPropertyName("Cost")] Cost Cost,
		[property: JsonPropertyName("Rewards")] IReadOnlyList<Reward> Rewards
	);

	public record SkinsPanelLayout(
		[property: JsonPropertyName("SingleItemOffers")] IReadOnlyList<string> SingleItemOffers,
		[property: JsonPropertyName("SingleItemStoreOffers")] IReadOnlyList<SingleItemStoreOffer> SingleItemStoreOffers,
		[property: JsonPropertyName("SingleItemOffersRemainingDurationInSeconds")] long? SingleItemOffersRemainingDurationInSeconds
	);

	public record StoreOffer(
		[property: JsonPropertyName("PurchaseInformation")] PurchaseInformation PurchaseInformation,
		[property: JsonPropertyName("SubOffers")] IReadOnlyList<SubOffer> SubOffers
	);

	public record SubOffer(
		[property: JsonPropertyName("PurchaseInformation")] PurchaseInformation PurchaseInformation
	);

	public record TotalBaseCost(
		[property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? _85ad13f73d1b51289eb27cd8ee0b5741
	);

	public record TotalDiscountedCost(
		[property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? _85ad13f73d1b51289eb27cd8ee0b5741
	);

	public record UpgradeCurrencyOffer(
		[property: JsonPropertyName("OfferID")] string OfferID,
		[property: JsonPropertyName("StorefrontItemID")] string StorefrontItemID,
		[property: JsonPropertyName("Offer")] Offer Offer,
		[property: JsonPropertyName("DiscountedPercent")] double? DiscountedPercent
	);

	public record UpgradeCurrencyStore(
		[property: JsonPropertyName("UpgradeCurrencyOffers")] IReadOnlyList<UpgradeCurrencyOffer> UpgradeCurrencyOffers
	);
}