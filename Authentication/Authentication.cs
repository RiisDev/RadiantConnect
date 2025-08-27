using RadiantConnect.Authentication.CaptchaRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Handlers;
using RadiantConnect.Authentication.RiotClient;
using RadiantConnect.Authentication.SSIDReAuth;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        private readonly string[] _unSupportedBrowsers = ["firefox", "brave", "opera"];

        public enum DriverStatus {
            CheckingExistingProcesses,
            CreatingDriver,
            DriverCreated,
            BeginSignIn,
            LoggingIntoValorant,
            CaptchaFound,
            CaptchaSolved,
            CheckingRSOMultiFactor,
            GrabbingRequiredTokens,
            MultiFactorRequested,
            MultiFactorCompleted,
        }

        public enum CountryCode
        {
            Na,
            Kr,
            Jp,
            Cn,
            Tw,
            Euw,
            Ru,
            Tr,
            Th,
            Vn,
            Id,
            My,
            Eun,
            Br,
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
            get => AuthHandler.MultiFactorCode;
            set => AuthHandler.MultiFactorCode = value;
        }

        internal AuthHandler AuthHandler = null!;

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
            await Task.Delay(0); // Placeholder for actual implementation, and to prevent build errors in release mode.
            throw new RadiantConnectAuthException("Captcha authentication is not supported in this build. Please use the driver authentication method instead.");   
#endif
        }

        public async Task<RSOAuth?> AuthenticateWithSsid(string ssid, string? clid = "", string? csid = "", string? tdid = "", WebProxy? proxy = null) => await SsidAuthManager.Authenticate(ssid, clid, csid, tdid, proxy);

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
	            throw new RadiantConnectAuthException("Due to recent changes in the library, you must target windows to hide the driver automatically\nOn other OS's I am unable to hide the driver.\nTo use this authentication method add the parameter `acceptedTerms: true`");

            driverSettings ??= new DriverSettings();

            if (_unSupportedBrowsers.Contains(driverSettings.ProcessName))
                throw new RadiantConnectAuthException("Unsupported browser");

            AuthHandler = new AuthHandler(
                driverSettings.ProcessName,
                driverSettings.BrowserExecutable,
                driverSettings.KillBrowser,
                driverSettings.CacheCookies,
                driverSettings.UseHeadless
            );

            AuthHandler.OnMultiFactorRequested += () => OnMultiFactorRequested?.Invoke();
            AuthHandler.OnDriverUpdate += status => OnDriverUpdate?.Invoke(status);

            Task<(string, string, string, string)> authTask = AuthHandler.Authenticate(username, password);
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

                AuthHandler.Dispose();

                return await SsidAuthManager.Authenticate(ssid, clid, csid, tdid);
            }

            AuthHandler.Dispose();

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
		
        public async Task<RSOAuth?> RiotClientAuth(string? settingsFile = null) => await new RtcAuth().Run(settingsFile, this);
    }
}
