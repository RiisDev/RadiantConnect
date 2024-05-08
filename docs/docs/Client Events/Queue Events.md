# Queue Events

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

!!! danger
    Some queues aren't named as they appear on the UI. Example: Team Deathmatch = ggteam.

## OnQueueChanged
This event is fired when you change queue types.

```C#
string OnQueueChanged;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`(Queue Id, Competitive, Unrated)               | `Competitive`  |


## OnEnteredQueue
This event is fired when you enter a given queue matchmaking.

```C#
string OnEnteredQueue;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`(Queue Id, Competitive, Unrated)               | `Competitive`  |


## OnLeftQueue
This event is fired when you leave a given queue matchmaking.

```C#
string OnLeftQueue;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`(Queue Id, Competitive, Unrated)               | `Competitive`  |


## OnCustomGameLobbyCreated 
This event is fired when you enter "Custom Game" queue type, and it will return CustomGameData data type.

```C#
CustomGameData OnCustomGameLobbyCreated;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `CustomGameData`       | A record class of the custom game data. |


## OnTravelToMenu 
This event is fired when you travel back to the main menu

```C#
void OnCustomGameLobbyCreated;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `null`       | Does not return anything |


## OnMatchFound 
This event is fired when the match found even message is sent

```C#
void OnMatchFound;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `null`       | Does not return anything |