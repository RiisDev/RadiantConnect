using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;
using RadiantConnect.Authentication.CaptchaRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Handlers;
using RadiantConnect.Authentication.SSIDReAuth;
using RadiantConnect.Network;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

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
            Logging_Into_Valorant,
            Checking_RSO_Multi_Factor,
            Grabbing_Required_Tokens,
            Multi_Factor_Requested,
            Multi_Factor_Completed,
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

        [Flags]
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

        internal AuthHandler authHandler = null!;

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

        public async Task<RSOAuth?> AuthenticateWithSSID(string ssid, string? clid = "", string? csid = "", string? tdid = "") => await SsidAuthManager.Authenticate(ssid, clid, csid, tdid);

        public async Task<RSOAuth?> AuthenticateWithQr(CountryCode countryCode, bool returnLoginUrl = false)
        {
            SignInManager manager = new(countryCode, returnLoginUrl);
            
            if (returnLoginUrl)
                manager.OnUrlBuilt += OnUrlBuilt;

            return await manager.Authenticate();
        }
        
        public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null, bool acceptedTerms = false)
        {
            if (!acceptedTerms)
            {
                throw new RadiantConnectAuthException("Due to recent changes in the library, you must target windows to hide the driver automatically\nOn other OS's I am unable to hide the driver.\nTo use this authentication method add the parameter `acceptedTerms: true`");
            }

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

            Task<(string, string, string, string)> authTask = authHandler.Authenticate(username, password);
#if DEBUG
            Task delayTask = Task.Delay(TimeSpan.FromDays(1));
#else
            Task delayTask = Task.Delay(TimeSpan.FromSeconds(45));
#endif
            if (await Task.WhenAny(authTask, delayTask) == authTask)
            {
                // Authentication completed within timeout
                (string ssid, string clid, string tdid, string csid) = await authTask;
                Debug.WriteLine($"{DateTime.Now} LOGIN DONE");

                authHandler.Dispose();

                return await SsidAuthManager.Authenticate(ssid, clid, csid, tdid);
            }

            authHandler.Dispose();

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

    }
}
