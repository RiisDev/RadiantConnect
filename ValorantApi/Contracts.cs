using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace RadiantConnect.ValorantApi
{
    public static class Contracts
    {
        public static async Task<ContractsData?> GetContractsAsync() => await ValorantApiClient.GetAsync<ContractsData>("https://valorant-api.com/v1", "/contracts");

        public static async Task<ContractData?> GetContractAsync(string contractUuid) => await ValorantApiClient.GetAsync<ContractData>("https://valorant-api.com/v1", $"/contracts/{contractUuid}");

        public static async Task<ContractData?> GetContractByUuid(string contractUuid) => await GetContractAsync(contractUuid);

        public record Chapter(
            [property: JsonPropertyName("isEpilogue")] bool? IsEpilogue,
            [property: JsonPropertyName("levels")] IReadOnlyList<Level> Levels,
            [property: JsonPropertyName("freeRewards")] IReadOnlyList<FreeReward> FreeRewards
        );

        public record Content(
            [property: JsonPropertyName("relationType")] string RelationType,
            [property: JsonPropertyName("relationUuid")] string RelationUuid,
            [property: JsonPropertyName("chapters")] IReadOnlyList<Chapter> Chapters,
            [property: JsonPropertyName("premiumRewardScheduleUuid")] string PremiumRewardScheduleUuid,
            [property: JsonPropertyName("premiumVPCost")] int? PremiumVPCost
        );

        public record Data(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("displayIcon")] string DisplayIcon,
            [property: JsonPropertyName("shipIt")] bool? ShipIt,
            [property: JsonPropertyName("useLevelVPCostOverride")] bool? UseLevelVPCostOverride,
            [property: JsonPropertyName("levelVPCostOverride")] int? LevelVPCostOverride,
            [property: JsonPropertyName("freeRewardScheduleUuid")] string FreeRewardScheduleUuid,
            [property: JsonPropertyName("content")] Content Content,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );

        public record FreeReward(
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("amount")] int? Amount,
            [property: JsonPropertyName("isHighlighted")] bool? IsHighlighted
        );

        public record Level(
            [property: JsonPropertyName("reward")] Reward Reward,
            [property: JsonPropertyName("xp")] int? Xp,
            [property: JsonPropertyName("vpCost")] int? VpCost,
            [property: JsonPropertyName("isPurchasableWithVP")] bool? IsPurchasableWithVP,
            [property: JsonPropertyName("doughCost")] int? DoughCost,
            [property: JsonPropertyName("isPurchasableWithDough")] bool? IsPurchasableWithDough
        );

        public record Reward(
            [property: JsonPropertyName("type")] string Type,
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("amount")] int? Amount,
            [property: JsonPropertyName("isHighlighted")] bool? IsHighlighted
        );

        public record ContractsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<Data> Data
        );

        public record ContractData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] Data Data
        );

    }
}
