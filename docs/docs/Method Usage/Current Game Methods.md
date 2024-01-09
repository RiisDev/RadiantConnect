# Current Game Methods

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## GetCurrentGamePlayerAsync
This API endpoint is used to retrieve details of a current game player associated with a specific user.

```C#
Task<CurrentGamePlayer?> GetCurrentGamePlayerAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<CurrentGamePlayer?>` (CurrentGamePlayer Record) | A record class of the CurrentGamePlayer data.  |

## GetCurrentGameMatchAsync
This API endpoint is used to retrieve details of a current game match using the match ID.

```C#
Task<CurrentGameMatch?> GetCurrentGameMatchAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<CurrentGameMatch?>` (CurrentGameMatch Record) | A record class of the CurrentGameMatch data.  |

## GetCurrentGameLoadoutAsync
This API endpoint is used to retrieve the current game loadout details for a specific match.

```C#
Task<GameLoadout?> GetCurrentGameLoadoutAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<GameLoadout?>` (GameLoadout Record) | A record class of the GameLoadout data.  |

## QuitCurrentGameAsync
This API endpoint is used to quit the current game.

```C#
Task QuitCurrentGameAsync(string userId, string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task` | Returns a task indicating the completion of the operation.  |