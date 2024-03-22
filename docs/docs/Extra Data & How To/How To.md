# How To

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.


## How to get local PUUID
Invoke from the initiator 
```C#
Initiator init = new Initiator();

string clientId = init.Client.UserId;
```

## Local HttpMethods 
```C#
public enum HttpMethod
{
    Get,
    Post, 
    Put, 
    Delete,
    Patch,
    Options,
    Head
}
```