## Cost Record

```csharp
public record Cost(
    [property: JsonPropertyName("WalletCosts")] IReadOnlyList<WalletCost> WalletCosts
);

public record Definition(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Item")] Item Item,
    [property: JsonPropertyName("RequiredEntitlement")] RequiredEntitlement RequiredEntitlement,
    [property: JsonPropertyName("ProgressionSchedule")] ProgressionSchedule ProgressionSchedule,
    [property: JsonPropertyName("RewardSchedule")] RewardSchedule RewardSchedule,
    [property: JsonPropertyName("Sidegrades")] IReadOnlyList<Sidegrade> Sidegrades
);

public record EntitlementReward(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID,
    [property: JsonPropertyName("Amount")] int Amount
);

public record Item(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID
);

public record Option(
    [property: JsonPropertyName("OptionID")] string OptionID,
    [property: JsonPropertyName("Cost")] Cost Cost,
    [property: JsonPropertyName("Rewards")] IReadOnlyList<Reward> Rewards
);

public record Prerequisites(
    [property: JsonPropertyName("RequiredEntitlements")] IReadOnlyList<RequiredEntitlement> RequiredEntitlements
);

public record ProgressionSchedule(
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("ProgressionCurrencyID")] string ProgressionCurrencyID,
    [property: JsonPropertyName("ProgressionDeltaPerLevel")] IReadOnlyList<int> ProgressionDeltaPerLevel
);

public record RequiredEntitlement(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID
);

public record RequiredEntitlement2(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID
);

public record Reward(
    [property: JsonPropertyName("ItemTypeID")] string ItemTypeID,
    [property: JsonPropertyName("ItemID")] string ItemID,
    [property: JsonPropertyName("Amount")] long Amount
);

public record RewardSchedule(
    [property: JsonPropertyName("ID")] string ID,
    [property: JsonPropertyName("Name")] string Name,
    [property: JsonPropertyName("Prerequisites")] object Prerequisites,
    [property: JsonPropertyName("RewardsPerLevel")] IReadOnlyList<RewardsPerLevel> RewardsPerLevel
);

public record RewardsPerLevel(
    [property: JsonPropertyName("EntitlementRewards")] IReadOnlyList<EntitlementReward> EntitlementRewards,
    [property: JsonPropertyName("WalletRewards")] object WalletRewards,
    [property: JsonPropertyName("CounterRewards")] object CounterRewards
);

public record ItemUpgrade(
    [property: JsonPropertyName("Definitions")] IReadOnlyList<Definition> Definitions
);

public record Sidegrade(
    [property: JsonPropertyName("SidegradeID")] string SidegradeID,
    [property: JsonPropertyName("Options")] IReadOnlyList<Option> Options,
    [property: JsonPropertyName("Prerequisites")] Prerequisites Prerequisites
);

public record WalletCost(
    [property: JsonPropertyName("CurrencyID")] string CurrencyID,
    [property: JsonPropertyName("AmountToDeduct")] int AmountToDeduct
);```