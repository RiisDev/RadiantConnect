using System.Text.Json.Serialization;
namespace RadiantConnect.Network.StoreEndpoints.DataTypes;
// ReSharper disable All

public record BalancesInternal(
    [property: JsonPropertyName("85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741")] long? _85ad13f73d1b51289eb27cd8ee0b5741,
    [property: JsonPropertyName("85ca954a-41f2-ce94-9b45-8ca3dd39a00d")] long? _85ca954a41f2Ce949b458ca3dd39a00d,
    [property: JsonPropertyName("e59aa87c-4cbf-517a-5983-6e81511be9b7")] long? E59aa87c4cbf517a59836e81511be9b7,
    [property: JsonPropertyName("f08d4ae3-939c-4576-ab26-09ce1f23bb37")] long? F08d4ae3939c4576Ab2609ce1f23bb37
);

public record BalancesMain(
    [property: JsonPropertyName("BalancesInternal")] BalancesInternal Balances
);