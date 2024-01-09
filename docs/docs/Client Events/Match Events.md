# Match Events

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## OnMapLoaded
This event is fired when the map has loaded on the client end.

```C#
string OnMapLoaded;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Internal Map Name)              | `Juliett`                |
!!! danger
    This returns the internal map name, and not the client name.
??? note "Internal Map Dictionary"
    ```C#
    internal Dictionary<string, string> InternalMapNames = new()
    {
        {"Juliett", "Sunset"},
        {"Jam", "Lotus"},
        {"Pitt", "Pearl"},
        {"Canyon", "Fracture"},
        {"Foxtrot", "Breeze"},
        {"Port", "Icebox"},
        {"Ascent", "Ascent"},
        {"Bonsai", "Split"},
        {"Triad", "Haven"},
        {"Duality", "Bind"},
    };
    ```

## Match_Started
This event is fired when the match is started
```C#
string Match_Started;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string`               | `None`                   |

## OnMatchEnded
This event is fired when the match has ended

```C#
string OnMatchEnded;
```

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` (Winning Team)              | `Blue` OR `Red` |