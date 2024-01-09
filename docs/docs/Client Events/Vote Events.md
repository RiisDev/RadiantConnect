# Vote Events

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.


## OnVoteDeclared
This event is fired when a vote is called

```C#
bool OnVoteDeclared;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `bool` (Always true)                | `true`  |

## OnVoteInvoked
This event is fired when you choose Yes or No.

```C#
bool OnVoteInvoked;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `bool` (true for 'yes', false for 'no')  | `false`|