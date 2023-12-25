using System.Text.Json.Serialization;

// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record _21(
    [property: JsonPropertyName("rankedRatingThreshold")] int? RankedRatingThreshold,
    [property: JsonPropertyName("startingPage")] int? StartingPage,
    [property: JsonPropertyName("startingIndex")] int? StartingIndex
);

public record _24(
    [property: JsonPropertyName("rankedRatingThreshold")] int? RankedRatingThreshold,
    [property: JsonPropertyName("startingPage")] int? StartingPage,
    [property: JsonPropertyName("startingIndex")] int? StartingIndex
);

public record LeaderboardPlayer(
    [property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
    [property: JsonPropertyName("TitleID")] string TitleID,
    [property: JsonPropertyName("IsBanned")] bool? IsBanned,
    [property: JsonPropertyName("IsAnonymized")] bool? IsAnonymized,
    [property: JsonPropertyName("puuid")] string Puuid,
    [property: JsonPropertyName("gameName")] string GameName,
    [property: JsonPropertyName("tagLine")] string TagLine,
    [property: JsonPropertyName("leaderboardRank")] int? LeaderboardRank,
    [property: JsonPropertyName("rankedRating")] int? RankedRating,
    [property: JsonPropertyName("numberOfWins")] int? NumberOfWins,
    [property: JsonPropertyName("competitiveTier")] int? CompetitiveTier
);

public record Leaderboard(
    [property: JsonPropertyName("Deployment")] string Deployment,
    [property: JsonPropertyName("QueueID")] string QueueID,
    [property: JsonPropertyName("SeasonID")] string SeasonID,
    [property: JsonPropertyName("Players")] IReadOnlyList<LeaderboardPlayer> Players,
    [property: JsonPropertyName("totalPlayers")] int? TotalPlayers,
    [property: JsonPropertyName("immortalStartingPage")] int? ImmortalStartingPage,
    [property: JsonPropertyName("immortalStartingIndex")] int? ImmortalStartingIndex,
    [property: JsonPropertyName("topTierRRThreshold")] int? TopTierRRThreshold,
    [property: JsonPropertyName("tierDetails")] TierDetails TierDetails,
    [property: JsonPropertyName("startIndex")] int? StartIndex,
    [property: JsonPropertyName("query")] string Query
);

public record TierDetails(
    [property: JsonPropertyName("21")] _21 _21,
    [property: JsonPropertyName("24")] _24 _24
);