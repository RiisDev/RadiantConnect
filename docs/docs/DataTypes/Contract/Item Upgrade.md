## Cost Record

The `Cost` record represents the cost associated with a contract within the RadiantConnect network.

### Properties

#### `WalletCosts`

- Type: `IReadOnlyList<WalletCost>`
- Description: Represents a list of wallet costs associated with the contract.

## Definition Record

The `Definition` record represents the definition details of a contract within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the contract definition.

#### `Item`

- Type: `Item`
- Description: Represents the item associated with the contract definition.

#### `RequiredEntitlement`

- Type: `RequiredEntitlement`
- Description: Represents the required entitlement for the contract definition.

#### `ProgressionSchedule`

- Type: `ProgressionSchedule`
- Description: Represents the progression schedule for the contract definition.

#### `RewardSchedule`

- Type: `RewardSchedule`
- Description: Represents the reward schedule for the contract definition.

#### `Sidegrades`

- Type: `IReadOnlyList<Sidegrade>`
- Description: Represents a list of sidegrades associated with the contract definition.

## EntitlementReward Record

The `EntitlementReward` record represents the rewards associated with entitlements within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the item type ID associated with the entitlement reward.

#### `ItemID`

- Type: `string`
- Description: Represents the item ID associated with the entitlement reward.

#### `Amount`

- Type: `int`
- Description: Represents the amount of the entitlement reward.

## Item Record

The `Item` record represents an item within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the item type ID associated with the item.

#### `ItemID`

- Type: `string`
- Description: Represents the item ID associated with the item.

## Option Record

The `Option` record represents an option within the RadiantConnect network.

### Properties

#### `OptionID`

- Type: `string`
- Description: Represents the unique identifier for the option.

#### `Cost`

- Type: `Cost`
- Description: Represents the cost associated with the option.

#### `Rewards`

- Type: `IReadOnlyList<Reward>`
- Description: Represents a list of rewards associated with the option.

## Prerequisites Record

The `Prerequisites` record represents prerequisites for a contract within the RadiantConnect network.

### Properties

#### `RequiredEntitlements`

- Type: `IReadOnlyList<RequiredEntitlement>`
- Description: Represents a list of required entitlements for the prerequisites.

## ProgressionSchedule Record

The `ProgressionSchedule` record represents the progression schedule for a contract within the RadiantConnect network.

### Properties

#### `Name`

- Type: `string`
- Description: Represents the name of the progression schedule.

#### `ProgressionCurrencyID`

- Type: `string`
- Description: Represents the currency ID associated with the progression schedule.

#### `ProgressionDeltaPerLevel`

- Type: `IReadOnlyList<int>`
- Description: Represents the progression delta per level for the progression schedule.

## RequiredEntitlement Record

The `RequiredEntitlement` record represents a required entitlement for a contract within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the item type ID associated with the required entitlement.

#### `ItemID`

- Type: `string`
- Description: Represents the item ID associated with the required entitlement.

## RequiredEntitlement2 Record

The `RequiredEntitlement2` record represents an additional required entitlement for a contract within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the item type ID associated with the required entitlement.

#### `ItemID`

- Type: `string`
- Description: Represents the item ID associated with the required entitlement.

## Reward Record

The `Reward` record represents a reward within the RadiantConnect network.

### Properties

#### `ItemTypeID`

- Type: `string`
- Description: Represents the item type ID associated with the reward.

#### `ItemID`

- Type: `string`
- Description: Represents the item ID associated with the reward.

#### `Amount`

- Type: `long`
- Description: Represents the amount of the reward.

## RewardSchedule Record

The `RewardSchedule` record represents a reward schedule for a contract within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the unique identifier for the reward schedule.

#### `Name`

- Type: `string`
- Description: Represents the name of the reward schedule.

#### `Prerequisites`

- Type: `object`
- Description: Represents prerequisites associated with the reward schedule.

#### `RewardsPerLevel`

- Type: `IReadOnlyList<RewardsPerLevel>`
- Description: Represents a list of rewards per level associated with the reward schedule.

## RewardsPerLevel Record

The `RewardsPerLevel` record represents rewards per level for a reward schedule within the RadiantConnect network.

### Properties

#### `EntitlementRewards`

- Type: `IReadOnlyList<EntitlementReward>`
- Description: Represents a list of entitlement rewards per level.

#### `WalletRewards`

- Type: `object`
- Description: Represents wallet rewards associated with the rewards per level.

#### `CounterRewards`

- Type: `object`
- Description: Represents counter rewards associated with the rewards per level.

## ItemUpgrade Record

The `ItemUpgrade` record represents an upgrade for an item within the RadiantConnect network.

### Properties

#### `Definitions`

- Type: `IReadOnlyList<Definition>`
- Description: Represents a list of definitions associated with the item upgrade.

## Sidegrade Record

The `Sidegrade` record represents a sidegrade for a contract within the RadiantConnect network.

### Properties

#### `SidegradeID`

- Type: `string`
- Description: Represents the unique identifier for the sidegrade.

#### `Options`

- Type: `IReadOnlyList<Option>`
- Description: Represents a list of options associated with the sidegrade.

#### `Prerequisites`

- Type: `Prerequisites`
- Description: Represents the prerequisites associated with the sidegrade.

## WalletCost Record

The `WalletCost` record represents the wallet cost associated with a contract within the RadiantConnect network.

### Properties

#### `CurrencyID`

- Type: `string`
- Description: Represents the currency ID associated with the wallet cost.

#### `AmountToDeduct`

- Type: `int`
- Description: Represents the amount to deduct from the wallet for the cost.

