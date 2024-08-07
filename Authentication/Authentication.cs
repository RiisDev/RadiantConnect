using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Misc;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        public enum DriverStatus {
            Checking_Existing_Processes,
            Creating_Driver,
            Redirecting_To_RSO,
            Driver_Created,
            Begin_SignIn,
            Checking_Cached_Auth,
            Checking_Auth_Validity,
            Clearing_Cached_Auth,
            Checking_RSO_Login_Page,
            Logging_Into_RSO,
            Logging_Into_Valorant,
            Checking_RSO_Multi_Factor,
            Grabbing_Required_Tokens,
            Multi_Factor_Requested,
            Multi_Factor_Completed,
            SignIn_Completed,
            Cookies_Received
        }

        public enum CaptchaService;
        
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

        public string? MultiFactorCode
        {
            get => authHandler.MultiFactorCode;
            set => authHandler.MultiFactorCode = value;
        }

        internal AuthHandler authHandler = null!;

        public Task<RSOAuth?> AuthenticateWithCaptcha(string username, string password, CaptchaService service, string captchaAuthorization) => throw new NotImplementedException();

        private readonly string[] UnSupportedBrowsers = ["chrome", "firefox"];

        public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null)
        {   
            driverSettings ??= new DriverSettings();

            if (UnSupportedBrowsers.Contains(driverSettings.ProcessName))
                throw new RadiantConnectAuthException("Unsupported browser");

            authHandler = new AuthHandler(
                driverSettings.ProcessName,
                driverSettings.BrowserExecutable,
                driverSettings.KillBrowser,
                driverSettings.CacheCookies
            );

            authHandler.OnMultiFactorRequested += () => OnMultiFactorRequested?.Invoke();
            authHandler.OnDriverUpdate += status => OnDriverUpdate?.Invoke(status);

            (IEnumerable<Cookie>? cookies, string? accessToken, string? pasToken, string? entitlement, object? clientConfig, string? userInfo) = await authHandler.Initialize(username, password);

            if (cookies == null) return null;

            Dictionary<string, string> cookieDict = new();
            HashSet<string> seenNames = [];

            foreach (Cookie cookie in cookies)
                if (seenNames.Add(cookie.Name))
                    cookieDict[cookie.Name] = cookie.Value;

            string? rsoSubject = cookieDict.GetValueOrDefault("sub");
            string? rsoSsid = cookieDict.GetValueOrDefault("ssid");
            string? rsoTdid = cookieDict.GetValueOrDefault("tdid");
            string? rsoCsid = cookieDict.GetValueOrDefault("csid");
            string? rsoClid = cookieDict.GetValueOrDefault("clid");
            string? idToken = cookieDict.GetValueOrDefault("id_token");

            JsonWebToken jwt = new (pasToken);
            string? affinity = jwt.GetPayloadValue<string>("affinity");
            string? chatAffinity = jwt.GetPayloadValue<string>("desired.affinity");
            
            return new RSOAuth(rsoSubject, rsoSsid, rsoTdid, rsoCsid, rsoClid, idToken, accessToken, pasToken, entitlement, affinity, chatAffinity, clientConfig, cookies);
        }

        public async Task<string?> GetSsidFromDriverCache()
        {
            string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
            if (!File.Exists(cacheFile)) return null;
            CookieRoot? cookieRoot = JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile));
            IReadOnlyList<Cookie>? cookiesData = cookieRoot?.Result.Cookies;
            return cookiesData?.FirstOrDefault(x => x.Name == "ssid")?.Value;
        }

        public async Task Logout() => await authHandler.Logout();
    }
}
