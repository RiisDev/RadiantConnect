# Party Methods

!!! Info
	 This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## FetchPartyPlayerAsync
This API endpoint is used to retrieve details of a party player associated with a specific user.

```C#
Task<PartyPlayer?> FetchPartyPlayerAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PartyPlayer?>` (PartyPlayer Record) | A record class of the PartyPlayer data.  |

## FetchPartyAsync
This API endpoint is used to retrieve details of a party using the party ID.

```C#
Task<Party?> FetchPartyAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## FetchCustomGameConfigAsync
This API endpoint is used to retrieve custom game configuration details.

```C#
Task<CustomGameConfig?> FetchCustomGameConfigAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<CustomGameConfig?>` (CustomGameConfig Record) | A record class of the CustomGameConfig data.  |

## FetchPartyChatTokenAsync
This API endpoint is used to retrieve a chat token for a specific party.

```C#
Task<PartyChatToken?> FetchPartyChatTokenAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PartyChatToken?>` (PartyChatToken Record) | A record class of the PartyChatToken data.  |

## FetchPartyVoiceTokenAsync
This API endpoint is used to retrieve a voice token for a specific party.

```C#
Task<PartyVoiceToken?> FetchPartyVoiceTokenAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PartyVoiceToken?>` (PartyVoiceToken Record) | A record class of the PartyVoiceToken data.  |

## SetPartyReadyAsync
This API endpoint is used to set the ready status for a member in a party.

```C#
Task<PartySetReady?> SetPartyReadyAsync(string partyId, string userId, bool ready)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |
| `bool` (Ready Status) | `true`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<PartySetReady?>` (PartySetReady Record) | A record class of the PartySetReady data.  |

## RefreshCompetitveTierAsync
This API endpoint is used to refresh the competitive tier for a member in a party.

```C#
Task<Party?> RefreshCompetitveTierAsync(string partyId, string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## RefreshPlayerIdentityAsync
This API endpoint is used to refresh the player identity for a member in a party.

```C#
Task<Party?> RefreshPlayerIdentityAsync(string partyId, string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## RefreshPingsAsync
This API endpoint is used to refresh pings for a member in a party.

```C#
Task<Party?> RefreshPingsAsync(string partyId, string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## ChangeQueueAsync
This API endpoint is used to change the queue for a party.

```C#
Task<Party?> ChangeQueueAsync(string partyId, QueueId queueId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `QueueId` (QueueId Enum) | `Competitive`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## StartCustomGameeAsync
This API endpoint is used to start a custom game for a party.

```C#
Task<Party?> StartCustomGameeAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## EnterQueueAsync
This API endpoint is used to make a party enter the matchmaking queue.

```C#
Task<Party?> EnterQueueAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## LeaveQueueAsync
This API endpoint is used to make a party leave the matchmaking queue.

```C#
Task<Party?> LeaveQueueAsync(string partyId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## SetPartyOpenStatusAsync
This API endpoint is used to set the open status of a party.

```C#
Task<Party?> SetPartyOpenStatusAsync(string partyId, PartyState state)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `PartyState` (PartyState Enum) | `Open`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## SetCustomGameSettingsAsync
This API endpoint is used to set custom game settings for a party.

```C#
Task<Party?> SetCustomGameSettingsAsync(string partyId, CustomGameSettings gameSettings)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `CustomGameSettings` (CustomGameSettings Object) | CustomGameSettings Record |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## InvitePlayerAsync
This API endpoint is used to invite a player to a party.

```C#
Task<Party?> InvitePlayerAsync(string partyId, string name, string tagLine)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Party ID) | `e45fae2d-89b1-42a8-9d9a-cd69f0e522d7`  |
| `string` (Player Name) | `JohnDoe`  |
| `string` (Player Tagline) | `1234`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Party?>` (Party Record) | A record class of the Party data.  |

## KickFromPartyAsync
This API endpoint is used to kick a player from a party.

```C#
Task KickFromPartyAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task` | Returns a task indicating the completion of the operation.  |