# RConnect Methods Methods

!!! Info 
    This is **Unofficial** and a fan-made project. Do not ask Riot or Valorant for support.


## GetRiotIdByPuuidAsync
This API endpoint is used to retrieve the name of the userid specified

```C#
Task<string?> GetRiotIdByPuuidAsync(string puuid)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<string?>` (Riot Id) | `Cinnamon Toast#Krunc` |


## GetPuuidByNameAsync
This API endpoint is used to retrieve the PUUID from the gamename#tagline

```C#
Task<string?> GetPuuidByNameAsync(string gameName, string tagLine)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot Game Name) | `Cinnamon Toast`  |
| `string` (Riot Tag Line) | `Krunc`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa` |


## GetPeakValorantRankAsync
This API endpoint is used to retrieve the peak valorant rank

```C#
Task<string?> GetPeakValorantRankAsync(string puuid)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Rank Name) | `Diamond 2` |


## GetValorantRankAsync
This API endpoint is used to retrieve the current valorant rank

```C#
Task<string?> GetValorantRankAsync(string puuid)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Rank Name) | `Platinum 2` |


## GetCurrentRankRatingAsync
This API endpoint is used to retrieve the current valorant rank

```C#
Task<int?> GetCurrentRankRatingAsync(string puuid)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `int` (Rank Rating) | `22` |


## GetRecentMatchStatsAsync
This API endpoint is used to retrieve the current valorant rank

```C#
Task<List<MatchStats?>> GetRecentMatchStatsAsync(string puuid)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `List<MatchStats?>` (List of matches and their stats) | List of MatchStats record |


## GetLastMatchLeaderboardAsync
This API endpoint is used to retrieve the current valorant rank

```C#
Task<MatchStats?> GetLastMatchLeaderboardAsync(string puuid, bool competitiveOnly = false)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |
| `bool` (Option: if you wanna get last competitive game) | `true/false`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `MatchStats?` (A Record of their match and the stats) | MatchStats record |


## GetMatchLeaderboardAsync
This API endpoint is used to retrieve the current valorant rank

```C#
Task<MatchStats?> GetMatchLeaderboardAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `MatchStats?` (A Record of their match and the stats) | MatchStats record |