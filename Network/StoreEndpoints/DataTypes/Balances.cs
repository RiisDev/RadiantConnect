namespace RadiantConnect.Network.StoreEndpoints.DataTypes;
// ReSharper disable All

public record BalancesInternal(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? ValorantPoints,
    [property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] long? KingdomCredits,
    [property: JsonPropertyName("e59aa87c-4cbf-517a-5983-6e81511be9b7")] long? Radianite,
    [property: JsonPropertyName("f08d4ae3-939c-4576-ab26-09ce1f23bb37")] long? FreeAgents
);
public record Limit(
    [property: JsonPropertyName("amount")] int? Amount,
    [property: JsonPropertyName("limitType")] string LimitType
);

public record Limits(
    [property: JsonPropertyName("bdf142e6-72fa-5f47-8983-8a68e902abb5")] Limit? Limit
);
public record CreditLimits(
    [property: JsonPropertyName("Limits")] Limits? Limits
);

public record CurrencyLimits(
    [property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] CreditLimits? KingdomCreditsLimits,
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] CreditLimits? ValorantPointsLimits,
    [property: JsonPropertyName("e59aa87c-4cbf-517a-5983-6e81511be9b7")] CreditLimits? RadianiteLimits,
    [property: JsonPropertyName("f08d4ae3-939c-4576-ab26-09ce1f23bb37")] CreditLimits? FreeGanetsLimits
);

public record BalancesMain(
    [property: JsonPropertyName("Balances")] BalancesInternal Balances,
    [property: JsonPropertyName("CurrencyLimits")] CurrencyLimits CurrencyLimits
);