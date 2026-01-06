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
	/// <summary>
	/// Provides multiple authentication flows for Riot/Valorant, including SSID, QR, driver-based, and Riot Client authentication methods.
	/// </summary>
	public class Authentication
	{
		private readonly string[] _unSupportedBrowsers = ["firefox", "brave", "opera"];

		/// <summary>
		/// Represents the various states of the browser automation driver during the authentication flow.
		/// </summary>
		public enum DriverStatus
		{
			/// <summary>
			/// Checking for existing browser processes before starting a new driver instance.
			/// </summary>
			CheckingExistingProcesses,

			/// <summary>
			/// Creating a new browser automation driver instance.
			/// </summary>
			CreatingDriver,

			/// <summary>
			/// Indicates that the browser driver has been successfully created.
			/// </summary>
			DriverCreated,

			/// <summary>
			/// Beginning the Riot sign-in process.
			/// </summary>
			BeginSignIn,

			/// <summary>
			/// Logging into the Valorant/Riot account.
			/// </summary>
			LoggingIntoValorant,

			/// <summary>
			/// A captcha challenge has been detected.
			/// </summary>
			CaptchaFound,

			/// <summary>
			/// The captcha challenge has been successfully solved.
			/// </summary>
			CaptchaSolved,

			/// <summary>
			/// Checking whether multi-factor authentication (MFA) is required.
			/// </summary>
			CheckingRSOMultiFactor,

			/// <summary>
			/// Retrieving the required RSO tokens after login.
			/// </summary>
			GrabbingRequiredTokens,

			/// <summary>
			/// Multi-factor authentication has been requested.
			/// </summary>
			MultiFactorRequested,

			/// <summary>
			/// Multi-factor authentication was successfully completed.
			/// </summary>
			MultiFactorCompleted,
		}

		/// <summary>
		/// Specifies region codes used for QR and SSID-based Riot authentication flows.
		/// </summary>
		public enum CountryCode
		{
			/// <summary>
			/// North America.
			/// </summary>
			Na,

			/// <summary>
			/// Korea.
			/// </summary>
			Kr,

			/// <summary>
			/// Japan.
			/// </summary>
			Jp,

			/// <summary>
			/// China.
			/// </summary>
			Cn,

			/// <summary>
			/// Taiwan.
			/// </summary>
			Tw,

			/// <summary>
			/// Europe West.
			/// </summary>
			Euw,

			/// <summary>
			/// Russia.
			/// </summary>
			Ru,

			/// <summary>
			/// Turkey.
			/// </summary>
			Tr,

			/// <summary>
			/// Thailand.
			/// </summary>
			Th,

			/// <summary>
			/// Vietnam.
			/// </summary>
			Vn,

			/// <summary>
			/// Indonesia.
			/// </summary>
			Id,

			/// <summary>
			/// Malaysia.
			/// </summary>
			My,

			/// <summary>
			/// Europe Nordic - East.
			/// </summary>
			Eun,

			/// <summary>
			/// Brazil.
			/// </summary>
			Br,
		}

		/// <summary>
		/// Raised when multi-factor authentication is requested during a driver-based sign-in flow.
		/// </summary>
		public event Events.MultiFactorEvent? OnMultiFactorRequested;

		/// <summary>
		/// Raised when the driver reports a status update during browser-based authentication.
		/// </summary>
		public event Events.DriverEvent? OnDriverUpdate;

		/// <summary>
		/// Raised when the QR authentication flow generates a login URL.
		/// </summary>
		public event UrlBuilder? OnUrlBuilt;

		/// <summary>
		/// Gets or sets the multi-factor authentication code used during sign-in flows that require MFA.
		/// </summary>
		public string? MultiFactorCode
		{
			get => AuthHandler.MultiFactorCode;
			set => AuthHandler.MultiFactorCode = value;
		}

		internal AuthHandler AuthHandler = null!;

		/// <summary>
		/// Authenticates using an SSID token and optional session identifiers.
		/// </summary>
		/// <param name="ssid">The SSID value.</param>
		/// <param name="clid">Optional CLID value.</param>
		/// <param name="csid">Optional CSID value.</param>
		/// <param name="tdid">Optional TDID value.</param>
		/// <param name="asid">Optional ASID value.</param>
		/// <param name="proxy">Optional proxy to route the request through.</param>
		/// <returns>The resulting RSO authentication tokens, or null if authentication fails.</returns>
		public async Task<RSOAuth?> AuthenticateWithSsid(string ssid, string? clid = "", string? csid = "", string? tdid = "", string? asid = "", WebProxy? proxy = null) => await SsidAuthManager.Authenticate(ssid, clid, csid, tdid, asid, proxy).ConfigureAwait(false);
		
		/// <summary>
		/// Begins the QR authentication process for a given region, optionally returning the login URL without completing authentication.
		/// </summary>
		/// <param name="countryCode">The region used for the QR session.</param>
		/// <param name="returnLoginUrl">Whether to return the login URL instead of continuing with authentication.</param>
		/// <returns>The RSO authentication result, or null if authentication does not complete.</returns>
		public async Task<RSOAuth?> AuthenticateWithQr(CountryCode countryCode, bool returnLoginUrl = false)
		{
			SignInManager manager = new(countryCode, returnLoginUrl);
			
			if (returnLoginUrl)
				manager.OnUrlBuilt += OnUrlBuilt;

			return await manager.Authenticate().ConfigureAwait(false);
		}

		/// <summary>
		/// Performs authentication using a browser automation driver. Requires explicit acceptance of driver-related terms.
		/// </summary>
		/// <param name="username">The Riot username.</param>
		/// <param name="password">The Riot password.</param>
		/// <param name="driverSettings">Optional custom driver settings.</param>
		/// <param name="acceptedTerms">Must be true to acknowledge platform restrictions.</param>
		/// <returns>The RSO authentication result, or null if authentication fails.</returns>
		/// <exception cref="RadiantConnectAuthException">Thrown if unsupported browser is used or terms are not accepted.</exception>
		/// <exception cref="TimeoutException">Thrown if authentication exceeds the allowed time limit.</exception>
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

		/// <summary>
		/// Retrieves the cached browser cookies used during previous authentication attempts.
		/// </summary>
		/// <returns>A read-only list of cookies, or null if the cache does not exist.</returns>
		public static async Task<IReadOnlyList<Cookie>?> GetCachedCookies()
		{
			string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
			return !File.Exists(cacheFile) ? null : JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile).ConfigureAwait(false))?.Result.Cookies;
		}

		/// <summary>
		/// Gets the SSID value from the cached browser cookie storage, if available.
		/// </summary>
		/// <returns>The SSID value, or null if not found.</returns>
		public static async Task<string?> GetSsidFromDriverCache()
		{
			IEnumerable<Cookie>? cookiesData = await GetCachedCookies().ConfigureAwait(false);
			return cookiesData?.FirstOrDefault(x => x.Name == "ssid")?.Value;
		}

		/// <summary>
		/// Authenticates using the Riot Client's local configuration and lockfiles.
		/// </summary>
		/// <param name="settingsFile">Optional custom settings file path.</param>
		/// <param name="skipTdid">Whether to skip TDID retrieval.</param>
		/// <param name="skipClid">Whether to skip CLID retrieval.</param>
		/// <param name="skipCsid">Whether to skip CSID retrieval.</param>
		/// <returns>The RSO authentication result, or null if authentication fails.</returns>
		public async Task<RSOAuth?> AuthenticateWithRiotClient(string? settingsFile = null, bool skipTdid = false, bool skipClid = false, bool skipCsid = false) => await new RtcAuth().Run(settingsFile, this, skipTdid, skipClid, skipCsid).ConfigureAwait(false);
		
		/// <summary>
		/// Authenticates using a lockfile generated by a running Riot Client instance.
		/// </summary>
		/// <returns>The RSO authentication result, or null if authentication fails.</returns>
		public async Task<RSOAuth?> AuthenticateWithLockFile() => await new LockFileAuth().Run().ConfigureAwait(false);
	}
}
