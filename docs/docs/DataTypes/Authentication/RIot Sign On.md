## RSOAuth & RadiantConnectRSO Record

Sorry, too lazy to write proper, so here is the record.

```csharp
public record RadiantConnectRSO(string SSID); // Supposedly all you need for authorization?

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
    object? ClientConfig,
    IEnumerable<Cookie> RiotCookies
);
```