using System.Text.Json.Serialization;

namespace RadiantConnect.HenrikApi
{
    public class Leaderboard(HenrikClient henrikClient)
    {
        public async Task<LeaderboardData?> GetLeaderboardAsync(
            string region,
            string platform,
            string? puuid = null,
            string? name = null,
            string? tag = null,
            string? seasonShort = null,
            string? seasonId = null,
            int? size = null,
            int? startIndex = null)
        {
            Dictionary<string, string> queryParams = [];

            if (!string.IsNullOrWhiteSpace(puuid))
                queryParams["puuid"] = puuid;
            else if (!string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(tag))
            {
                queryParams["name"] = name;
                queryParams["tag"] = tag;
            }

            if (!string.IsNullOrWhiteSpace(seasonShort))
                queryParams["season_short"] = seasonShort;
            else if (!string.IsNullOrWhiteSpace(seasonId))
                queryParams["season_id"] = seasonId;

            if (size.HasValue)
                queryParams["size"] = size.Value.ToString();

            if (startIndex.HasValue)
                queryParams["start_index"] = startIndex.Value.ToString();

            return await henrikClient.GetAsync<LeaderboardData?>($"/valorant/v3/leaderboard/{region}/{platform}", queryParams.Select(kvp => (kvp.Key, kvp.Value)).ToArray());
        }

        public record LeaderboardDatum(
            [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt,
            [property: JsonPropertyName("thresholds")] IReadOnlyList<ThresholdData> Thresholds,
            [property: JsonPropertyName("players")] IReadOnlyList<Player> Players
        );

        public record Player(
            [property: JsonPropertyName("card")] string Card,
            [property: JsonPropertyName("title")] string Title,
            [property: JsonPropertyName("is_banned")] bool? IsBanned,
            [property: JsonPropertyName("is_anonymized")] bool? IsAnonymized,
            [property: JsonPropertyName("puuid")] string Puuid,
            [property: JsonPropertyName("name")] string Name,
            [property: JsonPropertyName("tag")] string Tag,
            [property: JsonPropertyName("leaderboard_rank")] int? LeaderboardRank,
            [property: JsonPropertyName("tier")] int? Tier,
            [property: JsonPropertyName("rr")] int? Rr,
            [property: JsonPropertyName("wins")] int? Wins,
            [property: JsonPropertyName("updated_at")] DateTime? UpdatedAt
        );

        public record LeaderboardData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] LeaderboardDatum Data
        );

        public record ThresholdData(
            [property: JsonPropertyName("tier")] int? Tier,
            [property: JsonPropertyName("start_index")] int? StartIndex,
            [property: JsonPropertyName("threshold")] int? Threshold
        );
    }
}
