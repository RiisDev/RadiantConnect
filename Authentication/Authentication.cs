using RadiantConnect.Authentication.DriverRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Handlers;
using RadiantConnect.Authentication.RiotClient;
using RadiantConnect.Authentication.SSIDReAuth;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

#pragma warning disable CA1001 // Authentication class does not own disposable fields directly

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

		
		public event Events.MultiFactorEvent? OnMultiFactorRequested;

		public event Events.DriverEvent? OnDriverUpdate;

		public event UrlBuilder? OnUrlBuilt;

		public string? MultiFactorCode
		{
			get => AuthHandler.MultiFactorCode;
			set => AuthHandler.MultiFactorCode = value;
		}

		internal AuthHandler AuthHandler = null!;

#pragma warning disable CA1822 // Shouldn't be static due to consistency with other methods
		public async Task<RSOAuth?> AuthenticateWithSsid(string ssid, string? clid = "", string? csid = "", string? tdid = "", string? asid = "", WebProxy? proxy = null) => await SsidAuthManager.Authenticate(ssid, clid, csid, tdid, asid, proxy).ConfigureAwait(false);
#pragma warning restore
		public async Task<RSOAuth?> AuthenticateWithQr(CountryCode countryCode, bool returnLoginUrl = false)
		{
			SignInManager manager = new(countryCode, returnLoginUrl);
			
			if (returnLoginUrl)
				manager.OnUrlBuilt += OnUrlBuilt;

			return await manager.Authenticate().ConfigureAwait(false);
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
			if (await Task.WhenAny(authTask, delayTask).ConfigureAwait(false) == authTask)
			{
				// Authentication completed within timeout
				(string ssid, string clid, string tdid, string csid) = await authTask.ConfigureAwait(false);
				Debug.WriteLine($"{DateTime.Now} LOGIN DONE");

				AuthHandler.Dispose();

				return await SsidAuthManager.Authenticate(ssid, clid, csid, tdid).ConfigureAwait(false);
			}

			AuthHandler.Dispose();

			Debug.WriteLine($"{DateTime.Now} LOGIN TIMEOUT");
			throw new TimeoutException("Authentication timed out after 45 seconds.");
		}

		public static async Task<IReadOnlyList<Cookie>?> GetCachedCookies()
		{
			string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
			return !File.Exists(cacheFile) ? null : JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile).ConfigureAwait(false))?.Result.Cookies;
		}

		public static async Task<string?> GetSsidFromDriverCache()
		{
			IEnumerable<Cookie>? cookiesData = await GetCachedCookies().ConfigureAwait(false);
			return cookiesData?.FirstOrDefault(x => x.Name == "ssid")?.Value;
		}
		
		public async Task<RSOAuth?> AuthenticateWithRiotClient(string? settingsFile = null, bool skipTdid = false, bool skipClid = false, bool skipCsid = false) => await new RtcAuth().Run(settingsFile, this, skipTdid, skipClid, skipCsid).ConfigureAwait(false);

#pragma warning disable CA1822 // Shouldn't be static due to consistency with other methods
		public async Task<RSOAuth?> AuthenticateWithLockFile() => await new LockFileAuth().Run().ConfigureAwait(false);
#pragma warning restore CA1822
	}
}
