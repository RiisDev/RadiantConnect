## AccessoryStore Record

The `AccessoryStore` record represents the accessory store within the RadiantConnect network.

### Properties

#### `AccessoryStoreOffers`

- Type: `IReadOnlyList<AccessoryStoreOffer>`
- Description: Represents a list of offers available in the accessory store.

#### `AccessoryStoreRemainingDurationInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the accessory store in seconds.

#### `StorefrontID`

- Type: `string`
- Description: Represents the unique identifier for the storefront.

## AccessoryStoreOffer Record

The `AccessoryStoreOffer` record represents an offer within the accessory store.

### Properties

#### `Offer`

- Type: `Offer`
- Description: Represents the details of the offer.

#### `ContractID`

- Type: `string`
- Description: Represents the unique identifier for the contract associated with the offer.

## BonusStore Record

The `BonusStore` record represents the bonus store within the RadiantConnect network.

### Properties

#### `BonusStoreOffers`

- Type: `IReadOnlyList<BonusStoreOffer>`
- Description: Represents a list of offers available in the bonus store.

#### `BonusStoreRemainingDurationInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the bonus store in seconds.

#### `BonusStoreSecondsSinceItStarted`

- Type: `long?`
- Description: Represents the seconds since the bonus store started.

## BonusStoreOffer Record

The `BonusStoreOffer` record represents an offer within the bonus store.

### Properties

#### `BonusOfferID`

- Type: `string`
- Description: Represents the unique identifier for the bonus offer.

#### `Offer`

- Type: `Offer`
- Description: Represents the details of the offer.

#### `DiscountPercent`

- Type: `long?`
- Description: Represents the discount percentage for the offer.

#### `DiscountCosts`

- Type: `DiscountCosts`
- Description: Represents the discounted costs for the offer.

#### `IsSeen`

- Type: `bool?`
- Description: Indicates whether the offer has been seen.

## Bundle Record

The `Bundle` record represents a bundle within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the bundle.

#### `DataAssetID`

- Type: `string`
- Description: Represents the unique identifier for the data asset associated with the bundle.

#### `CurrencyID`

- Type: `string`
- Description: Represents the unique identifier for the currency associated with the bundle.

#### `Items`

- Type: `IReadOnlyList<ItemInternal>`
- Description: Represents a list of internal items associated with the bundle.

#### `ItemOffers`

- Type: `object`
- Description: Represents the offers associated with the items in the bundle.

#### `TotalBaseCost`

- Type: `object`
- Description: Represents the total base cost of the bundle.

#### `TotalDiscountedCost`

- Type: `object`
- Description: Represents the total discounted cost of the bundle.

#### `TotalDiscountPercent`

- Type: `long?`
- Description: Represents the total discount percentage for the bundle.

#### `DurationRemainingInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the bundle in seconds.

#### `WholesaleOnly`

- Type: `bool?`
- Description: Indicates whether the bundle is available for wholesale only.

## Bundle2 Record

The `Bundle2` record represents another version of a bundle within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the bundle.

#### `DataAssetID`

- Type: `string`
- Description: Represents the unique identifier for the data asset associated with the bundle.

#### `CurrencyID`

- Type: `string`
- Description: Represents the unique identifier for the currency associated with the bundle.

#### `Items`

- Type: `IReadOnlyList<ItemInternal>`
- Description: Represents a list of internal items associated with the bundle.

#### `ItemOffers`

- Type: `IReadOnlyList<ItemOffer>`
- Description: Represents the offers associated with the items in the bundle.

#### `TotalBaseCost`

- Type: `TotalBaseCost`
- Description: Represents the total base cost of the bundle.

#### `TotalDiscountedCost`

- Type: `TotalDiscountedCost`
- Description: Represents the total discounted cost of the bundle.

#### `TotalDiscountPercent`

- Type: `double?`
- Description: Represents the total discount percentage for the bundle.

#### `DurationRemainingInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the bundle in seconds.

#### `WholesaleOnly`

- Type: `bool?`
- Description: Indicates whether the bundle is available for wholesale only.

## Cost Record

The `Cost` record represents costs within the RadiantConnect network.

### Properties

#### `85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741`

- Type: `long?`
- Description: Represents the cost associated with the identifier "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741".

#### `85ca954a-41f2-ce94-9b45-8ca3dd39a00d`

- Type: `long?`
- Description: Represents the cost associated with the identifier "85ca954a-41f2-ce94-9b45-8ca3dd39a00d".

## DiscountCosts Record

The `DiscountCosts` record represents discounted costs within the RadiantConnect network.

### Properties

#### `85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741`

- Type: `long?`
- Description: Represents the discounted costs associated with the identifier "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741".

## DiscountedCost Record

The `DiscountedCost` record represents discounted costs within the RadiantConnect network.

### Properties

#### `85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741`

- Type: `long?`
- Description: Represents the discounted costs associated with the identifier "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741".

## FeaturedBundle Record

The `FeaturedBundle` record represents a featured bundle within the RadiantConnect network.

### Properties

#### `Bundle`

- Type: `Bundle`
- Description: Represents the details of the featured bundle.

#### `Bundles`

- Type: `IReadOnlyList<Bundle>`
- Description: Represents a list of bundles associated with the featured bundle.

#### `BundleRemainingDurationInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the featured bundle in seconds.

## ItemInternal Record

The `ItemInternal` record represents an internal item within the RadiantConnect network.

### Properties

#### `Item`

- Type: `ItemInternal`
- Description: Represents the details of the internal item.

#### `BasePrice`

- Type: `long?`
- Description: Represents the base price of the internal item.

#### `CurrencyID`

- Type: `string`
- Description: Represents the unique identifier for the currency associated with the internal item.

#### `DiscountPercent`

- Type: `long?`
- Description: Represents the discount percentage for the internal item.

#### `DiscountedPrice`

- Type: `long?`
- Description: Represents the discounted price of the internal item.

#### `IsPromoItem`

- Type: `bool?`
- Description: Indicates whether the internal item is a promotional item.

## Item2 Record

The `Item2` record represents another version of an item within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the unique identifier for the item type.

#### `ItemID`

- Type: `string`
- Description: Represents the unique identifier for the item.

#### `Amount`

- Type: `long?`
- Description: Represents the amount associated with the item.

## ItemOffer Record

The `ItemOffer` record represents an offer associated with an item within the RadiantConnect network.

### Properties

#### `BundleItemOfferID`

- Type: `string`
- Description: Represents the unique identifier for the bundle item offer.

#### `Offer`

- Type: `Offer`
- Description: Represents the details of the offer associated with the item.

#### `DiscountPercent`

- Type: `long?`
- Description: Represents the discount percentage for the offer.

#### `DiscountedCost`

- Type: `DiscountedCost`
- Description: Represents the discounted costs for the offer.

## Offer Record

The `Offer` record represents an offer within the RadiantConnect network.

### Properties

#### `OfferID`

- Type: `string`
- Description: Represents the unique identifier for the offer.

#### `IsDirectPurchase`

- Type: `bool?`
- Description: Indicates whether the offer is available for direct purchase.

#### `StartDate`

- Type: `DateTime?`
- Description: Represents the start date of the offer.

#### `Cost`

- Type: `Cost`
- Description: Represents the costs associated with the offer.

#### `Rewards`

- Type: `IReadOnlyList<Reward>`
- Description: Represents a list of rewards associated with the offer.

## Reward Record

The `Reward` record represents a reward within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the unique identifier for the item type.

#### `ItemID`

- Type: `string`
- Description: Represents the unique identifier for the item.

#### `Quantity`

- Type: `long?`
- Description: Represents the quantity of the reward.

## Storefront Record

The `Storefront` record represents a storefront within the RadiantConnect network.

### Properties

#### `FeaturedBundle`

- Type: `FeaturedBundle`
- Description: Represents the featured bundle within the storefront.

#### `SkinsPanelLayout`

- Type: `SkinsPanelLayout`
- Description: Represents the layout of the skins panel within the storefront.

#### `UpgradeCurrencyStore`

- Type: `UpgradeCurrencyStore`
- Description: Represents the upgrade currency store within the storefront.

#### `BonusStore`

- Type: `BonusStore`
- Description: Represents the bonus store within the storefront.

#### `AccessoryStore`

- Type: `AccessoryStore`
- Description: Represents the accessory store within the storefront.

## SingleItemStoreOffer Record

The `SingleItemStoreOffer` record represents an offer for a single item within the RadiantConnect network.

### Properties

#### `OfferID`

- Type: `string`
- Description: Represents the unique identifier for the offer.

#### `IsDirectPurchase`

- Type: `bool?`
- Description: Indicates whether the offer is available for direct purchase.

#### `StartDate`

- Type: `string`
- Description: Represents the start date of the offer.

#### `Cost`

- Type: `Cost`
- Description: Represents the costs associated with the offer.

#### `Rewards`

- Type: `IReadOnlyList<Reward>`
- Description: Represents a list of rewards associated with the offer.

## SkinsPanelLayout Record

The `SkinsPanelLayout` record represents the layout of the skins panel within the RadiantConnect network.

### Properties

#### `SingleItemOffers`

- Type: `IReadOnlyList<string>`
- Description: Represents a list of offers available for single items in the skins panel.

#### `SingleItemStoreOffers`

- Type: `IReadOnlyList<SingleItemStoreOffer>`
- Description: Represents a list of store offers available for single items in the skins panel.

#### `SingleItemOffersRemainingDurationInSeconds`

- Type: `long?`
- Description: Represents the remaining duration of the single item offers in seconds.

## TotalBaseCost Record

The `TotalBaseCost` record represents the total base cost within the RadiantConnect network.

### Properties

#### `85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741`

- Type: `long?`
- Description: Represents the total base cost associated with the identifier "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741".

## TotalDiscountedCost Record

The `TotalDiscountedCost` record represents the total discounted cost within the RadiantConnect network.

### Properties

#### `85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741`

- Type: `long?`
- Description: Represents the total discounted cost associated with the identifier "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741".

## UpgradeCurrencyOffer Record

The `UpgradeCurrencyOffer` record represents an offer for upgrading currency within the RadiantConnect network.

### Properties

#### `OfferID`

- Type: `string`
- Description: Represents the unique identifier for the offer.

#### `StorefrontItemID`

- Type: `string`
- Description: Represents the unique identifier for the storefront item associated with the offer.

#### `Offer`

- Type: `Offer`
- Description: Represents the details of the offer.

#### `DiscountedPercent`

- Type: `double?`
- Description: Represents the discounted percentage for the offer.

## UpgradeCurrencyStore Record

The `UpgradeCurrencyStore` record represents the upgrade currency store within the RadiantConnect network.

### Properties

#### `UpgradeCurrencyOffers`

- Type: `IReadOnlyList<UpgradeCurrencyOffer>`
- Description: Represents a list of offers available in the upgrade currency store.
