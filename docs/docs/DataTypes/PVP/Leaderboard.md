## Leaderboard

```csharp

public record _21(
    [property: JsonPropertyName("rankedRatingThreshold")] long? RankedRatingThreshold,
    [property: JsonPropertyName("startingPage")] long? StartingPage,
    [property: JsonPropertyName("startingIndex")] long? StartingIndex
);

public record _24(
    [property: JsonPropertyName("rankedRatingThreshold")] long? RankedRatingThreshold,
    [property: JsonPropertyName("startingPage")] long? StartingPage,
    [property: JsonPropertyName("startingIndex")] long? StartingIndex
);

public record LeaderboardPlayer(
    [property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
    [property: JsonPropertyName("TitleID")] string TitleID,
    [property: JsonPropertyName("IsBanned")] bool? IsBanned,
    [property: JsonPropertyName("IsAnonymized")] bool? IsAnonymized,
    [property: JsonPropertyName("puuid")] string Puuid,
    [property: JsonPropertyName("gameName")] string GameName,
    [property: JsonPropertyName("tagLine")] string TagLine,
    [property: JsonPropertyName("leaderboardRank")] long? LeaderboardRank,
    [property: JsonPropertyName("rankedRating")] long? RankedRating,
    [property: JsonPropertyName("numberOfWins")] long? NumberOfWins,
    [property: JsonPropertyName("competitiveTier")] long? CompetitiveTier
);

public record Leaderboard(
    [property: JsonPropertyName("Deployment")] string Deployment,
    [property: JsonPropertyName("QueueID")] string QueueID,
    [property: JsonPropertyName("SeasonID")] string SeasonID,
    [property: JsonPropertyName("Players")] IReadOnlyList<LeaderboardPlayer> Players,
    [property: JsonPropertyName("totalPlayers")] long? TotalPlayers,
    [property: JsonPropertyName("immortalStartingPage")] long? ImmortalStartingPage,
    [property: JsonPropertyName("immortalStartingIndex")] long? ImmortalStartingIndex,
    [property: JsonPropertyName("topTierRRThreshold")] long? TopTierRRThreshold,
    [property: JsonPropertyName("tierDetails")] TierDetails TierDetails,
    [property: JsonPropertyName("startIndex")] long? StartIndex,
    [property: JsonPropertyName("query")] string Query
);

public record TierDetails(
    [property: JsonPropertyName("21")] _21 AscendantOne,
    [property: JsonPropertyName("24")] _24 ImmortalOne
);```