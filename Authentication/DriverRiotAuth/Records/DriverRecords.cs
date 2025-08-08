namespace RadiantConnect.Authentication.DriverRiotAuth.Records;


public record Cookie(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("value")] string Value,
    [property: JsonPropertyName("domain")] string Domain,
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("expires")] double? Expires,
    [property: JsonPropertyName("size")] int? Size,
    [property: JsonPropertyName("httpOnly")] bool? HttpOnly,
    [property: JsonPropertyName("secure")] bool? Secure,
    [property: JsonPropertyName("session")] bool? Session,
    [property: JsonPropertyName("priority")] string Priority,
    [property: JsonPropertyName("sameParty")] bool? SameParty,
    [property: JsonPropertyName("sourceScheme")] string SourceScheme,
    [property: JsonPropertyName("sourcePort")] int? SourcePort,
    [property: JsonPropertyName("sameSite")] string SameSite,
    [property: JsonPropertyName("partitionKey")] PartitionKey PartitionKey
);

public record PartitionKey(
    [property: JsonPropertyName("topLevelSite")] string TopLevelSite,
    [property: JsonPropertyName("hasCrossSiteAncestor")] bool? HasCrossSiteAncestor
);

internal record Result(
    [property: JsonPropertyName("cookies")] IReadOnlyList<Cookie> Cookies
);

internal record CookieRoot(
    [property: JsonPropertyName("id")] int? Id,
    [property: JsonPropertyName("result")] Result Result
);


internal record EdgeDev(
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("devtoolsFrontendUrl")] string DevtoolsFrontendUrl,
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("webSocketDebuggerUrl")] string WebSocketDebuggerUrl,
    [property: JsonPropertyName("parentId")] string ParentId
);
