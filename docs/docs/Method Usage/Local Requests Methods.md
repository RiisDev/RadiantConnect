# Local Riot Client Methods

!!! Info 
    This is an **Unofficial** and fan-made project. Please refrain from seeking support from Riot or Valorant.

!!! Info
    Find available additional endpoints @ https://irisapp.ca/RadiantConnect/OpenAPI/

## GetHelpAsync
This API endpoint is used to get general help information.

```C#
Task<dynamic?> GetHelpAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<dynamic?>` (Dynamic Data) | Returns dynamic data.  |

## GetLocalSessionsAsync
This API endpoint is used to get information about local sessions.

```C#
Task<dynamic?> GetLocalSessionsAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<dynamic?>` (Dynamic Data) | Returns dynamic data.  |

## GetRSOInfoAsync
This API endpoint is used to get information about RSO (Riot Service Operations).

```C#
Task<dynamic?> GetRSOInfoAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<dynamic?>` (Dynamic Data) | Returns dynamic data.  |

## GetLocalSwaggerDocsAsync
This API endpoint is used to get local Swagger documentation.

```C#
Task<dynamic?> GetLocalSwaggerDocsAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<dynamic?>` (Dynamic Data) | Returns dynamic data.  |

## GetLocaleInfoAsync
This API endpoint is used to get information about the locale.

```C#
Task<LocaleInternal?> GetLocaleInfoAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<LocaleInternal?>` (LocaleInternal Record) | A record class of the LocaleInternal data.  |

## GetAliasInfoAsync
This API endpoint is used to get information about the active alias.

```C#
Task<AliasInfo?> GetAliasInfoAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<AliasInfo?>` (AliasInfo Record) | A record class of the AliasInfo data.  |

## GetEntitlementTokensAsync
This API endpoint is used to get entitlement tokens.

```C#
Task<EntitlementTokens?> GetEntitlementTokensAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<EntitlementTokens?>` (EntitlementTokens Record) | A record class of the EntitlementTokens data.  |

## GetLocalChatSessionAsync
This API endpoint is used to get information about the local chat session.

```C#
Task<LocalChatSession?> GetLocalChatSessionAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<LocalChatSession?>` (LocalChatSession Record) | A record class of the LocalChatSession data.  |

## GetLocalFriendsAsync
This API endpoint is used to get information about local friends.

```C#
Task<InternalFriends?> GetLocalFriendsAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<InternalFriends?>` (InternalFriends Record) | A record class of the InternalFriends data.  |

## GetFriendsPresencesAsync
This API endpoint is used to get information about friends' presences.

```C#
Task<FriendPresences?> GetFriendsPresencesAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<FriendPresences?>` (FriendPresences Record) | A record class of the FriendPresences data.  |

## GetFriendRequestsAsync
This API endpoint is used to get information about friend requests.

```C#
Task<InternalRequests?> GetFriendRequestsAsync()
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `None` | N/A  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task<InternalRequests?>` (InternalRequests Record) | A record class of the InternalRequests data.  |

## SendFriendRequestAsync
This API endpoint is used to send a friend request.

```C#
Task SendFriendRequestAsync(string gameName, string tagLine)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Game Name) | `JohnDoe`  |
| `string` (Tagline) | `1234`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task` | Returns a task indicating the completion of the operation.  |

## RemoveFriendRequestAsync
This API endpoint is used to remove a friend request.

```C#
Task RemoveFriendRequestAsync(string userId)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` (Riot PUUID) | `92018bd1-df7e-5dad-9e7b-f7358f9312fa`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `Task` | Returns a task indicating the completion of the operation.  |


## RemoveFriendRequestAsync
This API endpoint is used to use any local endpoints

```C#
Task<string?> PerformLocalRequestAsync(ValorantNet.HttpMethod method, string endpoint, HttpContent? content = null)
```

| **Method Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `ValorantNet.HttpMethod ` (Internal Http Method) | `ValorantNet.HttpMethod.Get`  |
| `string` (Endpoint found with swaggerdocs) | `/chat/v4/friends`  |
| `HttpContent` (Any desired post content) | `N/A`  |

| **Event Return Type** | **Example Return Value** |
|------------------------|--------------------------|
| `string` | Returns a string for the desired endpoint.  |