# Pre-Game Events

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## OnPreGamePlayerLoaded
This event is fired when the user has entered agent select.

```C#
string OnPreGamePlayerLoaded;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Riot PUUID)  | `92018bd1-df7e-5dad-9e7b-f7358f9312fa` |

## OnPreGameMatchLoaded
This event is fired when the user has entered agent select.

```C#
string OnPreGameMatchLoaded;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`  (Match ID) | `e5958748-d5d7-4167-a5f8-ea2a14b1a5b6` |

## OnAgentLockedIn
This event is fired when you have locked into an agent

```C#
string OnAgentLockedIn;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Agent Name)   | `Jett`  |

## OnAgentSelected
This event is fired when you have selected an agent

```C#
string OnAgentSelected;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`(Agent Name) | `Jett`  |

!!! danger
    If the client is selecting different agents quite fast, it may not fire event.