# In-Game Methods

!!! Info 
	This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## OnBuyMenuOpened

This event is triggered when the buy menu is opened.

```C#
int OnBuyMenuOpened;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `int`				  | `1` (TRUE)			   |

## OnBuyMenuClosed

This event is triggered when the buy menu is closed.

```C#
int OnBuyMenuClosed;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `int`				  | `0` (FALSE)			  |

## OnUtilPlaced (Unused)

This event is triggered when an invalid actor element is used (Cypher Cam, Killjoy Util, Phoenix Blind).

```C#
string OnUtilPlaced;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Actor ID)			  | `Pawn_Killjoy_Q_StealthAlarmbot_C_2147405434`				 |