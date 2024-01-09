# PVP Methods

!!! Info 
    This is **Unofficial** and a fan-made project. Do not ask Riot or Valorant for support.

## FetchContentAsync
This API endpoint is used to retrieve the account XP details for a specific user.

```C#
Task<Content?> FetchContentAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | - |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Content?>` (Content Release) | A record class of the Content data.  |

## FetchAccountXPAsync
This API endpoint is used to retrieve the account XP details for a specific user.

```C#
Task<AccountXP?> FetchAccountXPAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<AccountXP?>` (AccountXP Record) | A record class of the AccountXP data.  |

## FetchPlayerLoadoutAsync
This API endpoint is used to retrieve the player loadout details for a specific user.

```C# 
Task<PlayerLoadout?> FetchPlayerLoadoutAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PlayerLoadout?>` (PlayerLoadout Record) | A record class of the PlayerLoadout data.  |

## FetchPlayerMMRAsync
This API endpoint is used to retrieve the Matchmaking Rating (MMR) details for a specific user.

```C#
Task<PlayerMMR?> FetchPlayerMMRAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PlayerMMR?>` (PlayerMMR Record) | A record class of the PlayerMMR data.  |

## FetchPlayerMatchHistoryAsync
This API endpoint is used to retrieve the match history details for a specific user.

```C#
Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<MatchHistory?>` (MatchHistory Record) | A record class of the MatchHistory data.  |

## FetchPlayerMatchHistoryByQueueIdAsync
This API endpoint is used to retrieve the match history details for a specific user.

```C#
Task<MatchHistory?> FetchPlayerMatchHistoryAsync(string userId, string queueId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |
| `string` (Queue ID) | `competitive`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<MatchHistory?>` (MatchHistory Record) | A record class of the MatchHistory data.  |

## FetchMatchInfoAsync
This API endpoint is used to retrieve detailed information about a specific match.

```C#
Task<MatchInfo?> FetchMatchInfoAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<MatchInfo?>` (MatchInfo Record) | A record class of the MatchInfo data.  |

## FetchCompetitveUpdatesAsync
This API endpoint is used to retrieve competitive update details for a specific user.

```C#
Task<CompetitiveUpdate?> FetchCompetitveUpdatesAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<CompetitiveUpdate?>` (CompetitiveUpdate Record) | A record class of the CompetitiveUpdate data.  |

## FetchLeaderboardAsync
This API endpoint is used to retrieve leaderboard details for a specific competitive season.

```C#
Task<Leaderboard?> FetchLeaderboardAsync(string seasonId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Season ID) | `2023-Season-1`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Leaderboard?>` (Leaderboard Record) | A record class of the Leaderboard data.  |

## FetchPenaltiesAsync
This API endpoint is used to retrieve penalty details.

```C#
Task<Penalty?> FetchPenaltiesAsync()
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Penalty?>` (Penalty Record) | A record class of the Penalty data.  |

## FetchClientConfigAsync
This API endpoint is used to retrieve client configuration details for a specific shard.

```C#
Task<Penalty?> FetchClientConfigAsync(LogService.ClientData.ShardType shard)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `LogService.ClientData.ShardType` (Shard Type) | `NA`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Penalty?>` (Penalty Record) | A record class of the Penalty data.  |

## SetPlayerLoadoutAsync
This API endpoint is used to set the player loadout for a specific user.

```C#
Task<PlayerLoadout?> SetPlayerLoadoutAsync(string userId, SetPlayerLoadout newLoadout)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |
| `SetPlayerLoadout` (New Loadout) | SetPlayer Record  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PlayerLoadout?>` (PlayerLoadout Record) | A record class of the PlayerLoadout data.  |

## FetchNameServiceReturn
This API endpoint is used to retrieve client configuration details for a specific shard.

```C#
Task<NameService?> FetchNameServiceReturn(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<NameService?>` (NameService Record) | A record class of the NameService data.  |