## MatchStats Record

Sorry, too lazy to write proper, so here is the record.

```csharp
public record Stats(
    long RoundsPlayed,
    long PlayerKills,
    long PlayerAssists,
    long PlayerDeaths,
    long AverageCombatScore,
    long Plants,
    long Defuses,
    long EconomyRating,
    long FirstBloods
);

public record Player(
    string Puuid,
    string GameName,
    string TagLine,
    string TeamId,
    string Character,
    bool Won,
    int AccountLevel,
    Stats Stats,
    AbilityCasts AbilityCasts
);

public record MatchStats(
    string MatchId,
    string MapName,
    string Pod,
    string QueueId,
    string SeasonId,
    string WinningTeam,
    List<Player> Players
);
```