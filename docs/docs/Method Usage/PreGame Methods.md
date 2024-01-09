# Pregame Methods

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## FetchPreGamePlayerAsync
This API endpoint is used to retrieve details of a pre-game player associated with a specific user.

``` C#
Task<PreGamePlayer?> FetchPreGamePlayerAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PreGamePlayer?>` (PreGamePlayer Record) | A record class of the PreGamePlayer data.  |

## FetchPreGameMatchAsync
This API endpoint is used to retrieve details of a pre-game match using the match ID.

``` C#
Task<PreGameMatch?> FetchPreGameMatchAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PreGameMatch?>` (PreGameMatch Record) | A record class of the PreGameMatch data.  |

## FetchPreGameLoadoutAsync
This API endpoint is used to retrieve the pre-game loadout details for a specific match.

``` C#
Task<GameLoadout?> FetchPreGameLoadoutAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<GameLoadout?>` (GameLoadout Record) | A record class of the GameLoadout data.  |

## SelectCharacterAsync
This API endpoint is used to select a character for a specific user in a pre-game match.

``` C#
Task<PreGameMatch?> SelectCharacterAsync(string matchId, Agent agent)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `Agent` (Agent Enum) | `Jett`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PreGameMatch?>` (PreGameMatch Record) | A record class of the PreGameMatch data.  |

## LockCharacterAsync
This API endpoint is used to lock a selected character for a specific user in a pre-game match.

``` C#
Task<PreGameMatch?> LockCharacterAsync(string matchId, Agent agent)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `Agent` (Agent Enum) | `Phoenix`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PreGameMatch?>` (PreGameMatch Record) | A record class of the PreGameMatch data.  |

## QuitGameAsync
This API endpoint is used to quit a pre-game match.

``` C#
Task QuitGameAsync(string matchId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Match ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task` | Returns a task indicating the completion of the operation.  |