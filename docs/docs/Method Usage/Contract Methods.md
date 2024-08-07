# Contract Methods

!!! Info 
	This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

## GetItemUpgradesAsync
This API endpoint is used to retrieve item upgrade information.

```C#
Task<ItemUpgrade?> GetItemUpgradesAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<ItemUpgrade?>` (ItemUpgrade Record) | A record class of the ItemUpgrade data.  |

## GetContractsAsync
This API endpoint is used to retrieve contract information for a specific user.

```C#
Task<Contract?> GetContractsAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Contract?>` (Contract Record) | A record class of the Contract data.  |