using System.Net.WebSockets;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
	internal class AuthHandler(string browserProcess, string browserExecutable, bool killBrowser, bool cacheCookies, bool headless) : IDisposable
	{
		public Authentication.DriverStatus DriverStatus
		{
			get;
			set
			{
				field = value;
				OnDriverUpdate?.Invoke(value);
			}
		}

		// Events
		public event Events.MultiFactorEvent? OnMultiFactorRequested;

		public event Events.DriverEvent? OnDriverUpdate;

		internal int DriverPort { get; } = AuthUtil.GetFreePort();
		internal Random ActionIdGenerator { get; } = new();
		internal SocketHandler SocketHandler = null!;

		internal Process? WebDriver { get; set; }

		internal ClientWebSocket? Socket { get; set; } = new();

		// User Variables
		public string? MultiFactorCode { get; set; }

		internal async Task<(string, string, string, string)> Authenticate(string username, string password)
		{
			DriverStatus = Authentication.DriverStatus.CheckingExistingProcesses;
			DriverHandler.DoDriverCheck(browserProcess, browserExecutable, killBrowser);

			DriverStatus = Authentication.DriverStatus.CreatingDriver;
			(WebDriver, string? socketUrl) = await DriverHandler.StartDriver(browserExecutable, DriverPort, headless);

			if (WebDriver == null)
				throw new RadiantConnectException("Failed to start browser driver");

			if (socketUrl.IsNullOrEmpty()) throw new RadiantConnectAuthException("Failed to find socket");

			Socket = new ClientWebSocket();

			try { await Socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None); }
			catch (WebSocketException e) { throw new RadiantConnectAuthException($"Failed to connect to socket please try again. {e.Message}"); }

			SocketHandler = new SocketHandler(Socket, this, DriverPort);

			_ = Task.Run(() => DriverHandler.ListenAsync(Socket));

			await SocketHandler.InitiateRuntimeHandles(Socket, username, password);

			DriverStatus = Authentication.DriverStatus.DriverCreated;

			try
			{
				DriverStatus = Authentication.DriverStatus.BeginSignIn;
				return await PerformSignInAsync();
			}
			catch (Exception e)
			{
				WebDriver.Kill(true);
				throw new RadiantConnectAuthException(e.Message);
			}
			finally
			{
				Dispose();
			}
		}

		public void Dispose()
		{
			Socket?.Abort();
			Socket?.Dispose();
			Process.GetProcessesByName(browserProcess).ToList().ForEach(x => x.Kill()); // Kill driver processes
		}

		internal async Task<(string, string, string, string)> PerformSignInAsync()
		{
			string accessTokenFound = string.Empty;
			DriverStatus = Authentication.DriverStatus.LoggingIntoValorant;

			DriverHandler.OnCaptchaFound += (_) => DriverStatus = Authentication.DriverStatus.CaptchaFound;
			DriverHandler.OnCaptchaRemoved += (_) => DriverStatus = Authentication.DriverStatus.CaptchaSolved;

			DriverHandler.OnMfaDetected += async (_) =>
			{
				DriverStatus = Authentication.DriverStatus.CheckingRSOMultiFactor;
				await HandleMfaAsync();
			};

			DriverHandler.OnAccessTokenFound += (data) => accessTokenFound = data!;

			while (accessTokenFound.IsNullOrEmpty()) await Task.Delay(5);

			DriverStatus = Authentication.DriverStatus.GrabbingRequiredTokens;

			return await GetRsoCookiesFromDriver();
		}

		internal async Task HandleMfaAsync()
		{
			DriverStatus = Authentication.DriverStatus.MultiFactorRequested;
			OnMultiFactorRequested?.Invoke();

			while (MultiFactorCode.IsNullOrEmpty()) await Task.Delay(500); // Wait for MFA code to be set

			MultiFactorCode = MultiFactorCode.Replace(" ", "");
			MultiFactorCode = MultiFactorCode.Trim();
			if (MultiFactorCode.Length != 6) throw new RadiantConnectAuthException("Invalid MFA code length");

			Dictionary<string, object> mfaDataInput = new()
			{
				{ "id", ActionIdGenerator.Next() },
				{ "method", "Runtime.evaluate" },
				{ "params", new Dictionary<string, string>
					{
						{ "expression", $"function set(e,t){{for(let[n,i]of(t(e),Object.entries(e)))n.includes(\"__reactEventHandlers\")&&i.onChange&&i.onChange({{target:e}})}}function setVerificationCodes(e){{let t=0;document.querySelectorAll('input[minlength=\"1\"][maxlength=\"1\"][type=\"text\"][inputmode=\"numeric\"]').forEach(n=>{{set(n,n=>n.value=e[t]),t++}})}} setVerificationCodes([{MultiFactorCode[0]},{MultiFactorCode[1]},{MultiFactorCode[2]},{MultiFactorCode[3]},{MultiFactorCode[4]},{MultiFactorCode[5]}]);document.querySelectorAll(\"[data-testid='btn-mfa-submit']\")[0].click();" }
					}
				}
			};

			await SocketHandler.ExecuteOnPageWithResponse(mfaDataInput);

			DriverStatus = Authentication.DriverStatus.MultiFactorCompleted;
		}

		internal async Task<(string, string, string, string)> GetRsoCookiesFromDriver()
		{
			CookieRoot? getCookies = await SocketHandler.GetCookiesAsync();

			// Dispose early, we don't need the browser.
			try { Dispose(); } catch {/**/}

			if (cacheCookies)
			{
				Directory.CreateDirectory($@"{Path.GetTempPath()}\RadiantConnect\");
				await File.WriteAllTextAsync($@"{Path.GetTempPath()}\RadiantConnect\cookies.json", JsonSerializer.Serialize(getCookies));
			}

			Dictionary<string, string> cookieDict = getCookies?.Result.Cookies.GroupBy(c => c.Name).ToDictionary(g => g.Key, g => g.First().Value) ?? [];

			string ssid = cookieDict.GetValueOrDefault("ssid", "");
			string clid = cookieDict.GetValueOrDefault("clid", "");
			string tdid = cookieDict.GetValueOrDefault("tdid", "");
			string csid = cookieDict.GetValueOrDefault("csid", "");

			return ssid.IsNullOrEmpty() || clid.IsNullOrEmpty() || tdid.IsNullOrEmpty() || csid.IsNullOrEmpty()
				? throw new RadiantConnectAuthException("Failed to gather required cookies")
				: (ssid, clid, tdid, csid);
		}

		[Obsolete("Logout is no longer function as cookies are cleared every run.", true)]
		public Task Logout() => throw new NotImplementedException("Logout is no longer functional as cookies are cleared each run.");
	}
}
