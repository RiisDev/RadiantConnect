using System.Text.Json.Serialization;
// ReSharper disable All

namespace RadiantConnect.Network.StoreEndpoints.DataTypes;

public record AccessoryStore(
    [property: JsonPropertyName("AccessoryStoreOffers")] IReadOnlyList<AccessoryStoreOffer> AccessoryStoreOffers,
    [property: JsonPropertyName("AccessoryStoreRemainingDurationInSeconds")] long? AccessoryStoreRemainingDurationInSeconds,
    [property: JsonPropertyName("StorefrontID")] string StorefrontID
);

public record AccessoryStoreOffer(
    [property: JsonPropertyName("Offer")] Offer Offer,
    [property: JsonPropertyName("ContractID")] string ContractID
);

public record BonusStore(
    [property: JsonPropertyName("BonusStoreOffers")] IReadOnlyList<BonusStoreOffer> BonusStoreOffers,
    [property: JsonPropertyName("BonusStoreRemainingDurationInSeconds")] long? BonusStoreRemainingDurationInSeconds,
    [property: JsonPropertyName("BonusStoreSecondsSinceItStarted")] long? BonusStoreSecondsSinceItStarted
);

public record BonusStoreOffer(
    [property: JsonPropertyName("BonusOfferID")] string BonusOfferID,
    [property: JsonPropertyName("Offer")] Offer Offer,
    [property: JsonPropertyName("DiscountPercent")] long? DiscountPercent,
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
    [property: JsonPropertyName("TotalDiscountPercent")] long? TotalDiscountPercent,
    [property: JsonPropertyName("DurationRemainingInSeconds")] long? DurationRemainingInSeconds,
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
    [property: JsonPropertyName("DurationRemainingInSeconds")] long? DurationRemainingInSeconds,
    [property: JsonPropertyName("WholesaleOnly")] bool? WholesaleOnly
);

public record Cost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints,
    [property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] long? UnknownType
);

public record DiscountCosts(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints
);

public record DiscountedCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints
);

public record FeaturedBundle(
    [property: JsonPropertyName("Bundle")] Bundle Bundle,
    [property: JsonPropertyName("Bundles")] IReadOnlyList<Bundle> Bundles,
    [property: JsonPropertyName("BundleRemainingDurationInSeconds")] long? BundleRemainingDurationInSeconds
);

public record ItemInternal(
    [property: JsonPropertyName("Item")] ItemInternal Item,
    [property: JsonPropertyName("BasePrice")] long? BasePrice,
    [property: JsonPropertyName("CurrencyID")] string CurrencyID,
    [property: JsonPropertyName("DiscountPercent")] long? DiscountPercent,
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
    [property: JsonPropertyName("DiscountPercent")] long? DiscountPercent,
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
    [property: JsonPropertyName("Quantity")] long? Quantity
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
    [property: JsonPropertyName("SingleItemOffersRemainingDurationInSeconds")] long? SingleItemOffersRemainingDurationInSeconds
);

public record TotalBaseCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints
);

public record TotalDiscountedCost(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints
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