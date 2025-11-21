using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Services;
using System.Net.Http.Headers;

namespace RadiantConnect.Network
{
	public class ValorantNet
	{
		internal static string LockFilePath =
			Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local",
				"Riot Games", "Riot Client", "Config", "lockfile");

		public delegate void ValorantNetLog(string message);
		public event ValorantNetLog? OnLog;

		public RSOAuth? AuthCodes { get; set; }

		private readonly HttpClient _client = AuthUtil.BuildClient().Item1;

		public static int? GetAuthPort() => GetAuth()?.AuthorizationPort;

		private static System.Net.Http.HttpMethod MapHttpMethod(HttpMethod method) => method switch
		{
			HttpMethod.Get => System.Net.Http.HttpMethod.Get,
			HttpMethod.Post => System.Net.Http.HttpMethod.Post,
			HttpMethod.Put => System.Net.Http.HttpMethod.Put,
			HttpMethod.Delete => System.Net.Http.HttpMethod.Delete,
			HttpMethod.Patch => System.Net.Http.HttpMethod.Patch,
			HttpMethod.Options => System.Net.Http.HttpMethod.Options,
			HttpMethod.Head => System.Net.Http.HttpMethod.Head,
			_ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
		};

		public enum HttpMethod
		{
			Get,
			Post,
			Put,
			Delete,
			Patch,
			Options,
			Head
		}

		private readonly string _defaultPlatform;
		private readonly string _defaultUserAgent;
		private readonly string _defaultClientVersion;

		private readonly Cache _cache = new();

		private void ResetDefaultHeaders()
		{
			_client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", _defaultPlatform);
			_client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", _defaultUserAgent);
			_client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", _defaultClientVersion);
		}

		public ValorantNet(RSOAuth rsoAuth)
		{
			AuthCodes = rsoAuth;
			_client.Timeout = TimeSpan.FromSeconds(value: 10);

			ValorantVersionApiRoot? apiData = InternalHttp.GetAsync<ValorantVersionApiRoot>("https://api.radiantconnect.ca", "/api/version/latest").Result;

			if (apiData?.Data is null)
				throw new RadiantConnectException("Failed to get Valorant version data from API");

			ValorantService.Version valorantClient = new (
				RiotClientVersion: apiData.Data.RiotClientVersion,
				Branch: apiData.Data.Branch, 
				BuildVersion: apiData.Data.BuildVersion, 
				Changelist: apiData.Data.ManifestId, 
				EngineVersion: apiData.Data.EngineVersion, 
				VanguardVersion: apiData.Data.VanguardVersion,
				UserClientVersion: "10.0.19042.1.256.64bit",
				UserPlatform: "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9"
			);

			_defaultPlatform = valorantClient.UserPlatform;
			_defaultUserAgent = $"ShooterGame/{valorantClient.BuildVersion} Windows/{valorantClient.UserClientVersion}";

			GameVersionService.ValidateVersionData(apiData.Data.RiotClientVersion);
			_defaultClientVersion = valorantClient.RiotClientVersion;

			ResetDefaultHeaders();
		}

		public ValorantNet(ValorantService? valorantClient = null)
		{
			_client.Timeout = TimeSpan.FromSeconds(10);
			_defaultPlatform = valorantClient?.ValorantClientVersion.UserPlatform ?? "";
			_defaultUserAgent = $"ShooterGame/{valorantClient?.ValorantClientVersion.BuildVersion} {valorantClient?.ValorantClientVersion.UserClientVersion}";

			try
			{
				GameVersionService.ValidateVersionData(valorantClient?.ValorantClientVersion.RiotClientVersion ?? "");
				_defaultClientVersion = valorantClient?.ValorantClientVersion.RiotClientVersion ?? "";
			}
			catch
			{
				ValorantVersionApiRoot? apiData = InternalHttp.GetAsync<ValorantVersionApiRoot>("https://api.radiantconnect.ca", "/api/version/latest").Result;

				if (apiData?.Data is null)
					throw new RadiantConnectException("Failed to get Valorant version data from API");

				ValorantService.Version valorantClientVersion = new(
					RiotClientVersion: apiData.Data.RiotClientVersion,
					Branch: apiData.Data.Branch,
					BuildVersion: apiData.Data.BuildVersion,
					Changelist: apiData.Data.ManifestId,
					EngineVersion: apiData.Data.EngineVersion,
					VanguardVersion: apiData.Data.VanguardVersion,
					UserClientVersion: "10.0.19042.1.256.64bit",
					UserPlatform: "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9"
				);

				_defaultClientVersion = valorantClientVersion.RiotClientVersion;
				GameVersionService.ValidateVersionData(apiData.Data.RiotClientVersion);
			}
			
			ResetDefaultHeaders();
		}
		
		internal static UserAuth? GetAuth()
		{
			string fileText = LogService.ReadTextFile(LockFilePath);

			if (fileText.IsNullOrEmpty()) return null;

			string[] fileValues = fileText.Split(':');

			if (fileValues.Length < 3) return null;

			int authPort = int.Parse(fileValues[2], StringExtensions.CultureInfo);
			string oAuth = fileValues[3];
			return new UserAuth(authPort, oAuth);
		}

		internal async Task<(string, string)> GetAuthorizationToken()
		{
			OnLog?.Invoke("[ValorantNet Log] Getting local AuthorizationTokens");

			if (AuthCodes is not null)
				return AuthCodes.AccessToken.IsNullOrEmpty() || AuthCodes.Entitlement.IsNullOrEmpty()
					? throw new RadiantConnectException(
						"AuthCodes are not valid, AccessToken or EntitlementToken is empty.")
					: (AuthCodes.AccessToken, AuthCodes.Entitlement);

			UserAuth? auth = GetAuth();
			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{auth?.OAuth}".ToBase64()}");
			HttpResponseMessage response = await _client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token").ConfigureAwait(false);
			
			if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");

			Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
			OnLog?.Invoke($"[ValorantNet GetAuthorizationToken Log] AccessToken: {entitlement?.AccessToken}\n[ValorantNet GetAuthorizationToken Log] EntitlementToken: {entitlement?.Token}");
			return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
		}
	   
		private async Task ResetAuth(bool resetAuth = false)
		{
			try
			{
				_client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
				_client.DefaultRequestHeaders.Remove("Authorization");
			}
			catch {/**/}

			if (!resetAuth && !_cache.AccessToken.IsNullOrEmpty() && !_cache.Jwt.IsNullOrEmpty())
			{
				_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {_cache.AccessToken}");
				_client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", _cache.Jwt);

				OnLog?.Invoke("[ValorantNet Log] Using cached Auth Tokens");

				return;
			}

			(string, string) authTokens = await GetAuthorizationToken().ConfigureAwait(false);

			if (authTokens.Item1.IsNullOrEmpty()) throw new RadiantConnectException("Failed to get Authorization Token");
			if (authTokens.Item2.IsNullOrEmpty()) throw new RadiantConnectException("Failed to get JWT Token");

			_cache.AccessToken = authTokens.Item1;
			_cache.Jwt = authTokens.Item2;

			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
			_client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);
		}

		private Task SetBasicAuth(bool resetAuth = false)
		{
			OnLog?.Invoke("[ValorantNet Log] Settings Basic Auth");

			if (AuthCodes is not null)
				throw new RadiantConnectException("Cannot use local endpoints with AuthCodes");

			_client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
			_client.DefaultRequestHeaders.Remove("Authorization");

			if (!resetAuth && !_cache.Basic.IsNullOrEmpty())
			{
				_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {_cache.Basic}");
				return Task.CompletedTask;
			}

			_cache.Basic = $"riot:{GetAuth()?.OAuth}".ToBase64();
			
			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {_cache.Basic}");

			return Task.CompletedTask;
		}

		private void SetCustomHeaders(Dictionary<string, string> headers)
		{
			_client.DefaultRequestHeaders.Clear();

			foreach ((string key, string value) in headers)
				_client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
		}

		private class Cache
		{
			public string Jwt { get; set; } = string.Empty;
			public string AccessToken { get; set; } = string.Empty;
			public string Basic { get; set; } = string.Empty;
		}

		public async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null, Dictionary<string, string>? customHeaders = null)
		{
			if (baseUrl.IsNullOrEmpty()) return string.Empty;

			if (!endPoint.IsNullOrEmpty())
			{
				if (baseUrl[^1] == '/' && endPoint[0] == '/') endPoint = endPoint[1..]; // Make sure the slash isn't duplicated
				if (baseUrl[^1] != '/' && endPoint[0] != '/') baseUrl += "/"; // Make sure it actually contains a slash
			}

			if (!(InternalValorantMethods.IsValorantProcessRunning() || InternalValorantMethods.IsRiotClientRunning()) && AuthCodes is null) return null;
			
			const int maxRetries = 3;
			int retryCount = 0;
			int backoffDelayMs = 5000;
			bool resetCache = false;

			while (retryCount < maxRetries)
			{
				// Set authentication headers
				if (baseUrl.Contains("127.0.0.1", StringComparison.Ordinal) && _client.DefaultRequestHeaders.Authorization?.Scheme != "Basic") await SetBasicAuth(resetCache).ConfigureAwait(false);
				else if (customHeaders is not null) SetCustomHeaders(customHeaders);
				else if (!baseUrl.Contains("127.0.0.1", StringComparison.Ordinal)) await ResetAuth(resetCache).ConfigureAwait(false);
				
				using HttpRequestMessage httpRequest = new();
				httpRequest.Method = MapHttpMethod(httpMethod);
				httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
				httpRequest.Content = content;

				using HttpResponseMessage responseMessage = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);
				string responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

				if (customHeaders is not null)
					ResetDefaultHeaders();

				if (retryCount == 0)
					OnLog?.Invoke($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(_client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");
				
				HttpStatusCode statusCode = responseMessage.StatusCode;

				switch ((int)statusCode)
				{
					case 200:
						content?.Dispose();
						return responseContent;
					case 401:
					case 403:
						OnLog?.Invoke("[ValorantNet Log] Unauthorized/Forbidden, resetting cache and retrying.");
						resetCache = true;
						content?.Dispose();
						break;
					case 429:
						OnLog?.Invoke($"[ValorantNet Log] Rate limited, waiting {backoffDelayMs / 1000} seconds before retrying.");
						await Task.Delay(backoffDelayMs).ConfigureAwait(false);
						backoffDelayMs *= 2;
						break;
					case 404:
						content?.Dispose();
						return null;
					default:
						throw new RadiantConnectNetworkStatusException($"\n[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(_client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");
				}
				
				retryCount++;

				if (!resetCache && backoffDelayMs == 5000) 
					return responseContent;

				OnLog?.Invoke($"[ValorantNet Log] Retrying... Attempt {retryCount} of {maxRetries}");
			}

			throw new RadiantConnectNetworkStatusException(
				$"[ValorantNet Log] Failed after {maxRetries} retries. Uri:{baseUrl}{endPoint}");

		}

		public async Task<T?> GetAsync<T>(string baseUrl, string endPoint) 
			=> await SendAndConvertAsync<T>(HttpMethod.Get, baseUrl, endPoint).ConfigureAwait(false);

		public async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Post, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task<T?> PutAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Put, baseUrl, endPoint, httpContent).ConfigureAwait(false);
		
		public async Task<T?> DeleteAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Delete, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task<T?> PatchAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Patch, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint) 
			=> await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint).ConfigureAwait(false);

		public async Task<T?> HeadAsync<T>(string baseUrl, string endPoint)
			=> await SendAndConvertAsync<T>(HttpMethod.Head, baseUrl, endPoint).ConfigureAwait(false);

		public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent)
			=> await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task GetAsync(string baseUrl, string endPoint)
			=> await CreateRequest(HttpMethod.Get, baseUrl, endPoint).ConfigureAwait(false);

		public async Task PostAsync(string baseUrl, string endPoint, HttpContent? httpContent = null) 
			=> await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task PutAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task DeleteAsync(string baseUrl, string endPoint, HttpContent? httpContent = null) 
			=> await CreateRequest(HttpMethod.Delete, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task PatchAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Patch, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		public async Task OptionsAsync(string baseUrl, string endPoint)
			=> await CreateRequest(HttpMethod.Options, baseUrl, endPoint).ConfigureAwait(false);

		public async Task HeadAsync(string baseUrl, string endPoint) 
			=> await CreateRequest(HttpMethod.Head, baseUrl, endPoint).ConfigureAwait(false);

		public async Task OptionsAsync(string baseUrl, string endPoint, HttpContent? httpContent)
			=> await CreateRequest(HttpMethod.Options, baseUrl, endPoint, httpContent).ConfigureAwait(false);
		
		private async Task<T?> SendAndConvertAsync<T>(HttpMethod method, string baseUrl, string endPoint, HttpContent? httpContent = null) 
			=> ConvertResponse<T>(await CreateRequest(method, baseUrl, endPoint, httpContent).ConfigureAwait(false));

		#pragma warning disable IDE0046 // Convert if possible
		private static T? ConvertResponse<T>(string? jsonData)
		{
			try
			{
				if (jsonData.IsNullOrEmpty() ||
					string.Equals(jsonData.Trim(), "null", StringComparison.OrdinalIgnoreCase))
					return default;

				Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

				if (targetType == typeof(string))
					return (T)(object)jsonData;

				if (targetType == typeof(int) && int.TryParse(jsonData, out int intValue))
					return (T)Convert.ChangeType(intValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(long) && long.TryParse(jsonData, out long longValue))
					return (T)Convert.ChangeType(longValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(bool) && bool.TryParse(jsonData, out bool boolValue))
					return (T)Convert.ChangeType(boolValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(double) && double.TryParse(jsonData, out double doubleValue))
					return (T)Convert.ChangeType(doubleValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(decimal) && decimal.TryParse(jsonData, out decimal decimalValue))
					return (T)Convert.ChangeType(decimalValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(float) && float.TryParse(jsonData, out float floatValue))
					return (T)Convert.ChangeType(floatValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(DateTime) && DateTime.TryParse(jsonData, out DateTime dateValue))
					return (T)Convert.ChangeType(dateValue, targetType, StringExtensions.CultureInfo);

				if (targetType == typeof(Guid) && Guid.TryParse(jsonData, out Guid guidValue))
					return (T)Convert.ChangeType(guidValue, targetType, StringExtensions.CultureInfo);

				if (targetType.IsEnum && Enum.TryParse(targetType, jsonData, out object? enumValue))
					return (T)enumValue;

				return JsonSerializer.Deserialize<T>(jsonData);
			}
			catch
			{
				throw new RadiantConnectException($"Failed to parse datatype: {typeof(T)}, given data: {jsonData}");
			}
		}


		#region  Records
		internal record UserAuth(int AuthorizationPort, string OAuth);

		private record Entitlement(
			[property: JsonPropertyName("accessToken")] string AccessToken,
			[property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
			[property: JsonPropertyName("issuer")] string Issuer,
			[property: JsonPropertyName("subject")] string Subject,
			[property: JsonPropertyName("token")] string Token
		);

		internal record ValorantVersionApi(
			[property: JsonPropertyName("manifest_id")] string ManifestId,
			[property: JsonPropertyName("branch")] string Branch,
			[property: JsonPropertyName("version")] string Version,
			[property: JsonPropertyName("build_version")] string BuildVersion,
			[property: JsonPropertyName("engine_version")] string EngineVersion,
			[property: JsonPropertyName("riot_client_version")] string RiotClientVersion,
			[property: JsonPropertyName("riot_client_build")] string RiotClientBuild,
			[property: JsonPropertyName("vanguard_version")] string VanguardVersion,
			[property: JsonPropertyName("build_date")] DateTime? BuildDate
		);

		internal record ValorantVersionApiRoot(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] ValorantVersionApi Data
		);

		#endregion
	}
}