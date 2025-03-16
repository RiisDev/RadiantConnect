## RSOAuth & RadiantConnectRSO Record

```csharp
public record RSOAuth(
    string? Subject,
    string? SSID,
    string? TDID,
    string? CSID,
    string? CLID,
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
    public string? RmsToken { get; set; } // Mutable field, not required in any instance
};
```