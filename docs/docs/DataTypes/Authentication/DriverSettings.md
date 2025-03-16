## DriverSettings Record

```csharp
public record DriverSettings(
    string ProcessName = "msedge",
    string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
    bool KillBrowser = false,
    bool CacheCookies = true
);
```