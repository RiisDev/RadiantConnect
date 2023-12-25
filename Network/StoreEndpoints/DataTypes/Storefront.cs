using System.Text.Json.Serialization;
// ReSharper disable All

namespace RadiantConnect.Network.StoreEndpoints.DataTypes;

public record AccessoryStore(
    [property: JsonPropertyName("AccessoryStoreOffers")] IReadOnlyList<AccessoryStoreOffer> AccessoryStoreOffers,
    [property: JsonPropertyName("AccessoryStoreRemainingDurationInSeconds")] int? AccessoryStoreRemainingDurationInSeconds,
    [property: JsonPropertyName("StorefrontID")] string StorefrontID
);

public record AccessoryStoreOffer(
    [property: JsonPropertyName("Offer")] Offer Offer,
    [property: JsonPropertyName("ContractID")] string ContractID
);

public record BonusStore(
    [property: JsonPropertyName("BonusStoreOffers")] IReadOnlyList<BonusStoreOffer> BonusStoreOffers,
    [property: JsonPropertyName("BonusStoreRemainingDurationInSeconds")] int? BonusStoreRemainingDurationInSeconds,
    [property: JsonPropertyName("BonusStoreSecondsSinceItStarted")] int? BonusStoreSecondsSinceItStarted
);

public record BonusStoreOffer(
    [property: JsonPropertyName("BonusOfferID")] string BonusOfferID,
    [property: JsonPropertyName("Offer")] Offer Offer,
    [property: JsonPropertyName("DiscountPercent")] int? DiscountPercent,
    [property: JsonPropertyName("DiscountCosts")] DiscountCosts DiscountCosts,
    [property: JsonPropertyName("IsSeen")] bool? IsSeen
);

public record Bundle(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("DataAssetID")] string DataAssetID,
    [property: JsonPropertyName("CurrencyID")] string CurrencyID,
    [property: JsonPropertyName("Items")] IReadOnlyList<ItemInternal> Items,
    [property: JsonPropertyName("ItemOffers")] object ItemOffers,
    [property: JsonPropertyName("TotalBaseCost")] object TotalBaseCost,
    [property: JsonPropertyName("TotalDiscountedCost")] object TotalDiscountedCost,
    [property: JsonPropertyName("TotalDiscountPercent")] int? TotalDiscountPercent,
    [property: JsonPropertyName("DurationRemainingInSeconds")] int? DurationRemainingInSeconds,
    [property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly
);

public record Bundle2(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("DataAssetID")] string DataAssetID,
    [property: JsonPropertyName("CurrencyID")] string CurrencyID,
    [property: JsonPropertyName("Items")] IReadOnlyList<ItemInternal> Items,
    [property: JsonPropertyName("ItemOffers")] IReadOnlyList<ItemOffer> ItemOffers,
    [property: JsonPropertyName("TotalBaseCost")] TotalBaseCost TotalBaseCost,
    [property: JsonPropertyName("TotalDiscountedCost")] TotalDiscountedCost TotalDiscountedCost,
    [property: JsonPropertyName("TotalDiscountPercent")] double? TotalDiscountPercent,
    [property: JsonPropertyName("DurationRemainingInSeconds")] int? DurationRemainingInSeconds,
    [property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly
);

public record Cost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] int? _85ad13f73d1b51289eb27cd8ee0b5741,
    [property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] int? _85ca954a41f2Ce949b458ca3dd39a00d
);

public record DiscountCosts(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] int? _85ad13f73d1b51289eb27cd8ee0b5741
);

public record DiscountedCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] int? _85ad13f73d1b51289eb27cd8ee0b5741
);

public record FeaturedBundle(
    [property: JsonPropertyName("Bundle")] Bundle Bundle,
    [property: JsonPropertyName("Bundles")] IReadOnlyList<Bundle> Bundles,
    [property: JsonPropertyName("BundleRemainingDurationInSeconds")] int? BundleRemainingDurationInSeconds
);

public record ItemInternal(
    [property: JsonPropertyName("Item")] ItemInternal Item,
    [property: JsonPropertyName("BasePrice")] int? BasePrice,
    [property: JsonPropertyName("CurrencyID")] string CurrencyID,
    [property: JsonPropertyName("DiscountPercent")] int? DiscountPercent,
    [property: JsonPropertyName("DiscountedPrice")] int? DiscountedPrice,
    [property: JsonPropertyName("IsPromoItem")] bool? IsPromoItem
);

public record Item2(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID,
    [property: JsonPropertyName("Amount")] int? Amount
);

public record ItemOffer(
    [property: JsonPropertyName("BundleItemOfferID")] string BundleItemOfferID,
    [property: JsonPropertyName("Offer")] Offer Offer,
    [property: JsonPropertyName("DiscountPercent")] int? DiscountPercent,
    [property: JsonPropertyName("DiscountedCost")] DiscountedCost DiscountedCost
);

public record Offer(
    [property: JsonPropertyName("OfferID")] string OfferID,
    [property: JsonPropertyName("IsDirectPurchase")] bool? IsDirectPurchase,
    [property: JsonPropertyName("StartDate")] DateTime? StartDate,
    [property: JsonPropertyName("Cost")] Cost Cost,
    [property: JsonPropertyName("Rewards")] IReadOnlyList<Reward> Rewards
);

public record Reward(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID,
    [property: JsonPropertyName("Quantity")] int? Quantity
);

public record Storefront(
    [property: JsonPropertyName("FeaturedBundle")] FeaturedBundle FeaturedBundle,
    [property: JsonPropertyName("SkinsPanelLayout")] SkinsPanelLayout SkinsPanelLayout,
    [property: JsonPropertyName("UpgradeCurrencyStore")] UpgradeCurrencyStore UpgradeCurrencyStore,
    [property: JsonPropertyName("BonusStore")] BonusStore BonusStore,
    [property: JsonPropertyName("AccessoryStore")] AccessoryStore AccessoryStore
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
    [property: JsonPropertyName("SingleItemOffersRemainingDurationInSeconds")] int? SingleItemOffersRemainingDurationInSeconds
);

public record TotalBaseCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] int? _85ad13f73d1b51289eb27cd8ee0b5741
);

public record TotalDiscountedCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] int? _85ad13f73d1b51289eb27cd8ee0b5741
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