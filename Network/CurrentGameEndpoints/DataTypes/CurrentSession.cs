namespace RadiantConnect.Network.CurrentGameEndpoints.DataTypes;
public record ClientPlatformInfo(
    [property: JsonPropertyName("platformType")] string PlatformType,
    [property: JsonPropertyName("platformOS")] string PlatformOS,
    [property: JsonPropertyName("platformOSVersion")] string PlatformOSVersion,
    [property: JsonPropertyName("platformChipset")] string PlatformChipset,
    [property: JsonPropertyName("platformDevice")] string PlatformDevice
);

public record CurrentSession(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("cxnState")] string CxnState,
    [property: JsonPropertyName("cxnCloseReason")] string CxnCloseReason,
    [property: JsonPropertyName("clientID")] string ClientID,
    [property: JsonPropertyName("clientVersion")] string ClientVersion,
    [property: JsonPropertyName("loopState")] string LoopState,
    [property: JsonPropertyName("loopStateMetadata")] string LoopStateMetadata,
    [property: JsonPropertyName("version")] int? Version,
    [property: JsonPropertyName("lastHeartbeatTime")] DateTime? LastHeartbeatTime,
    [property: JsonPropertyName("expiredTime")] DateTime? ExpiredTime,
    [property: JsonPropertyName("heartbeatIntervalMillis")] int? HeartbeatIntervalMillis,
    [property: JsonPropertyName("playtimeNotification")] string PlaytimeNotification,
    [property: JsonPropertyName("playtimeMinutes")] int? PlaytimeMinutes,
    [property: JsonPropertyName("isRestricted")] bool? IsRestricted,
    [property: JsonPropertyName("userinfoValidTime")] DateTime? UserinfoValidTime,
    [property: JsonPropertyName("restrictionType")] string RestrictionType,
    [property: JsonPropertyName("clientPlatformInfo")] ClientPlatformInfo ClientPlatformInfo,
    [property: JsonPropertyName("connectionTime")] DateTime? ConnectionTime,
    [property: JsonPropertyName("shouldForceInvalidate")] bool? ShouldForceInvalidate
);