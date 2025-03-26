using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.CaptchaRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Handlers;
using RadiantConnect.Network;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;
using TokenManager = RadiantConnect.Authentication.SSIDReAuth.SSIDAuthManager;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        private readonly string[] UnSupportedBrowsers = ["chrome", "firefox"];

        public enum DriverStatus {
            Checking_Existing_Processes,
            Creating_Driver,
            Driver_Created,
            Begin_SignIn,
            Checking_Cached_Auth,
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

        public enum CountryCode
        {
            NA,
            KR,
            JP,
            CN,
            TW,
            EUW,
            RU,
            TR,
            TH,
            VN,
            ID,
            MY,
            EUN,
            BR,
        }

        public enum CaptchaService
        {
            SuperMemory,
            CaptchaSolverNet
        };
        
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

        public event UrlBuilder? OnUrlBuilt;

        public string? MultiFactorCode
        {
            get => authHandler.MultiFactorCode;
            set => authHandler.MultiFactorCode = value;
        }

        internal AuthHandler? authHandler = null!;

        internal async Task<RSOAuth?> AuthenticateWithCaptcha(string username, string password, CaptchaService service, string captchaAuthorization)
        {
#if DEBUG
            switch (service)
            {
                case CaptchaService.SuperMemory | CaptchaService.CaptchaSolverNet:
                    SuperMemory superMemory = new(captchaAuthorization);
                    RSOAuth? rsoData = await superMemory.BeginSignIn(username, password);
                    superMemory.Dispose();
                    return rsoData;
                default:
                    throw new NotImplementedException();
            }
#else

            throw new NotImplementedException();
#endif
        }

        public async Task<RSOAuth?> AuthenticateWithSSID(string ssid) => await new TokenManager().Authenticate(ssid);

        public async Task<RSOAuth?> AuthenticateWithQr(CountryCode countryCode, bool returnLoginUrl = false)
        {
            SignInManager manager = new(countryCode, returnLoginUrl);
            
            if (returnLoginUrl)
                manager.OnUrlBuilt += OnUrlBuilt;

            return await manager.Authenticate();
        }

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

            Task<string> authTask = authHandler.Authenticate(username, password);
#if DEBUG
            Task delayTask = Task.Delay(TimeSpan.FromDays(1));
#else
            Task delayTask = Task.Delay(TimeSpan.FromSeconds(45));
#endif
            if (await Task.WhenAny(authTask, delayTask) == authTask)
            {
                // Authentication completed within timeout
                string ssid = await authTask;
                Debug.WriteLine($"{DateTime.Now} LOGIN DONE");

                authHandler?.Dispose();

                return await AuthenticateWithSSID(ssid);
            }

            authHandler?.Dispose();

            Debug.WriteLine($"{DateTime.Now} LOGIN TIMEOUT");
            throw new TimeoutException("Authentication timed out after 45 seconds.");
        }

        public async Task<IReadOnlyList<Cookie>?> GetCachedCookies()
        {
            string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
            return !File.Exists(cacheFile) ? null : JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile))?.Result.Cookies;
        }

        public async Task<string?> GetSsidFromDriverCache()
        {
            IEnumerable<Cookie>? cookiesData = await GetCachedCookies();
            return cookiesData?.FirstOrDefault(x => x.Name == "ssid")?.Value;
        }

        [Obsolete("Method is no longer used, please use 'AuthenticateWithSSID', will throw an error.", true)]
        public async Task<string?> PerformDriverCacheRequest(ValorantNet.HttpMethod httpMethod, string baseUrl, string endPoint, IEnumerable<Cookie> cookies, string userAgent = "", Dictionary<string, string>? extraHeaders = null, AuthenticationHeaderValue? authentication = null, HttpContent? content = null) 
            => throw new NotSupportedException("Method is no longer used, please use 'AuthenticateWithSSID'");

        public async Task Logout() => await authHandler?.Logout();
    }
}
