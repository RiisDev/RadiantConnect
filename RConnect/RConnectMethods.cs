using System.Diagnostics;
using System.Text.Json;
using RadiantConnect.Methods;
using RadiantConnect.Network;
using RadiantConnect.Network.LocalEndpoints.DataTypes;
using RadiantConnect.Network.PVPEndpoints.DataTypes;

namespace RadiantConnect.RConnect
{
    public static class RConnectMethods
    {
        public static bool IsValorantRunning() => Process.GetProcessesByName("VALORANT-Win64-Shipping").Length > 0;

        public static async Task<string?> GetRiotIdByPuuid(this Initiator initiator, string puuid)
        {
            NameService? serviceResponse = await initiator.Endpoints.PvpEndpoints.FetchNameServiceReturn(puuid);
           
            return $"{serviceResponse?.GameName}#{serviceResponse?.TagLine}";
        }

        public static async Task<string?> GetPuuidByName(this Initiator initiator, string gameName, string tagLine)
        {
            string? aliasReturn = await initiator.Endpoints.LocalEndpoints.PerformLocalRequestAsync(ValorantNet.HttpMethod.Get, $"/player-account/aliases/v1/lookup?gameName={gameName}&tagLine={tagLine}");

            string? parsedId = aliasReturn?[(aliasReturn.LastIndexOf(':') + 2)..^3];

            return parsedId;
        }

        public static async Task<string?> GetPeakValorantRank(this Initiator initiator, string puuid)
        {
            int highestTierFound = 0;
            PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid);
            try
            {
                if (playerMmr?.QueueSkills.Competitive.SeasonalInfoBySeasonID == null) return ValorantTables.TierToRank[0];
                if (playerMmr.QueueSkills.Competitive.SeasonalInfoBySeasonID.Values.Count <= 0) return ValorantTables.TierToRank[0];

                foreach (SeasonId data in playerMmr?.QueueSkills.Competitive.SeasonalInfoBySeasonID.Values!)
                    if (data.CompetitiveTier > highestTierFound)
                        highestTierFound = (int)data.CompetitiveTier!;
            }
            catch { return ValorantTables.TierToRank[highestTierFound]; }

            return ValorantTables.TierToRank[highestTierFound];
        }

        public static async Task<int?> GetCurrentRankRating(this Initiator initiator, string puuid)
        {
            PlayerMMR? playerMmr = await initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(puuid);

            return int.Parse(playerMmr?.LatestCompetitiveUpdate.RankedRatingAfterUpdate?.ToString()!);
        }

    }
}
