# Store Methods

!!! Info 
    This is **Unofficial** and a fan-made project. Do not ask Riot or Valorant for support.

## FetchStorefrontAsync
This API endpoint is used to retrieve the details of a storefront associated with a specific user.

```C#
Task<Storefront?> FetchStorefrontAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Storefront?>` (Storefront Record) | A record class of the Storefront data.  |

## FetchBalancesAsync
This API endpoint is used to retrieve the main balance details for a specific user.

```C#
Task<Storefront?> FetchBalancesAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Storefront?>` (Storefront Record) | A record class of the Storefront data.  |

## FetchOwnedItemByTypeAsync
This API endpoint is used to retrieve details of an owned item of a specific type for a given user.

```C#
Task<Storefront?> FetchOwnedItemByTypeAsync(ItemType itemType, string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `ItemType` (Enum) | `WeaponSkin`  |
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<Storefront?>` (Storefront Record) | A record class of the Storefront data.  |