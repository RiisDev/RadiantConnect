﻿
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Authentication.DriverRiotAuth.Records;

public record DriverSettings(
    string ProcessName = "msedge",
    string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
    bool KillBrowser = false
);

public record RSOAuth(
    string? Subject,
    string? SSID,
    string? TDID,
    string? CSID,
    string? CLID,
    string? IdToken,
    string? AccessToken,
    string? PasToken,
    string? Entitlement,
    string? Affinity,
    string? ChatAffinity,
    object? ClientConfig
);