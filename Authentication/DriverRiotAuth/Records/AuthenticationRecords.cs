namespace RadiantConnect.Authentication.DriverRiotAuth.Records;

// ReSharper disable twice StringLiteralTypo
public record DriverSettings(
    string ProcessName = "msedge",
    string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
    bool KillBrowser = false,
    bool CacheCookies = true,
    bool UseHeadless = true
);

public record RSOAuth(
    string? Subject,
    string? Ssid,
    string? Tdid,
    string? Csid,
    string? Clid,
    string? AccessToken,
    string? PasToken,
    string? Entitlement,
    string? Affinity,
    string? ChatAffinity,
    object? ClientConfig,
    IEnumerable<Cookie>? RiotCookies,
    string? IdToken
)
{
    public string? RmsToken { get; set; }
};