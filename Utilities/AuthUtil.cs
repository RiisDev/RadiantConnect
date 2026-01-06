using RadiantConnect.Authentication.DriverRiotAuth.Records;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace RadiantConnect.Utilities
{
	/// <summary>
	/// Provides authentication-related helper methods for obtaining
	/// Riot XMPP and service authorization tokens.
	/// </summary>
	public static class AuthUtil
	{
		internal static string ParseUrlQuery(string url, string key)
		{
			if (IsValidUrl(url))
			{
				Dictionary<string, string> c = ParseUrlParameters(url);
				return key.IsNullOrEmpty()
					? string.Empty
					: c.TryGetValue(key, out string? value)
						? value
						: throw new RadiantConnectAuthException($"Failed to parse token {key} from query.");
			}

			try
			{
				JsonDocument jsonDoc = JsonDocument.Parse(url);
				JsonElement rootElement = jsonDoc.RootElement;
				if (rootElement.ValueKind != JsonValueKind.Object) return string.Empty;

				rootElement.TryGetProperty("params", out JsonElement paramsElement);
				if (paramsElement.ValueKind != JsonValueKind.Object) return string.Empty;

				paramsElement.TryGetProperty("args", out JsonElement argsElement);
				if (argsElement.ValueKind != JsonValueKind.Array) return string.Empty;

				if (argsElement.GetArrayLength() > 1) return string.Empty;
				if (argsElement[0].ValueKind != JsonValueKind.Object) return string.Empty;

				argsElement[0].TryGetProperty("value", out JsonElement logValue);
				if (logValue.ValueKind != JsonValueKind.String) return string.Empty;

				string logValueString = logValue.GetString() ?? string.Empty;

				string urlParsed =
					logValueString[logValueString.IndexOf("https", StringComparison.InvariantCultureIgnoreCase)..];

				if (urlParsed.IsNullOrEmpty() || key.IsNullOrEmpty()) return string.Empty;
				Dictionary<string, string> query = ParseUrlParameters(urlParsed);

				return query.TryGet(key) ??
					   throw new RadiantConnectAuthException($"Failed to parse token {key} from query.");
			}
			catch (JsonException)
			{
				throw new RadiantConnectAuthException($"Failed to parse {key} from driver body.");
			}
			catch (UriFormatException)
			{
				throw new RadiantConnectAuthException($"Failed to parse URL from driver body.");
			}

		}

		internal static string ParseAccessToken(string url) => ParseUrlQuery(url, "access_token"); // Somehow broke this

		internal static string ParseIdToken(string url) => ParseUrlQuery(url, "id_token");

		internal static (HttpClient, CookieContainer) BuildClient()
		{
			CookieContainer cookieContainer = new();
			return (
				new HttpClient(new HttpClientHandler
				{
					AllowAutoRedirect = true,
					CookieContainer = cookieContainer,
					AutomaticDecompression = DecompressionMethods.All,
					ServerCertificateCustomValidationCallback = (_, _, _, _) => true
				}),
				cookieContainer
			);
		}

		/// <summary>
		/// Retrieves all required authentication tokens and client configuration
		/// data using an existing Riot access token.
		/// </summary>
		/// <param name="accessToken">
		/// The OAuth access token used to authenticate with Riot services.
		/// </param>
		/// <returns>
		/// A tuple containing authentication tokens and related metadata.
		/// <list type="bullet">
		/// <item><description><c>pasToken</c> – The Platform Authorization Service (PAS) token.</description></item>
		/// <item><description><c>entitlementsToken</c> – The Riot entitlements token.</description></item>
		/// <item><description><c>clientConfig</c> – The resolved client configuration object.</description></item>
		/// <item><description><c>userInfo</c> – The authenticated user information payload.</description></item>
		/// <item><description><c>rmsToken</c> – The Riot Messaging Service (RMS) token.</description></item>
		/// </list>
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown when the provided access token is null or empty.
		/// </exception>
		/// <exception cref="HttpRequestException">
		/// Thrown when a network or service error occurs while retrieving tokens.
		/// </exception>
		public static async Task<(string pasToken, string entitlementsToken, object clientConfig, string userInfo, string rmsToken)> GetAuthTokensFromAccessToken(string accessToken)
		{
			using HttpClient httpClient = BuildClient().Item1;
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			// Get PAS token
			HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat").ConfigureAwait(false);
			string pasToken = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Get RMS PAS Token
			response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/rms").ConfigureAwait(false);
			string rmsToken = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// GetUserInfo 
			response = await httpClient.GetAsync("https://auth.riotgames.com/userinfo").ConfigureAwait(false);
			string userInfo = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

			// Get entitlement token
			httpClient.DefaultRequestHeaders.Accept.Clear();
			response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json")).ConfigureAwait(false);
			string entitlementToken = (await response.Content.ReadFromJsonAsync<EntitleReturn>().ConfigureAwait(false))?.EntitlementsToken ?? "";
			
			// Get client config
			httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", entitlementToken);
			response = await httpClient.GetAsync("https://clientconfig.rpg.riotgames.com/api/v1/config/player?app=Riot%20Client").ConfigureAwait(false);
			object clientConfig;
			try
			{
				clientConfig = await response.Content.ReadFromJsonAsync<object>().ConfigureAwait(false) ?? new { };
			}
			catch
			{
				clientConfig = new { data = "FAILED_TO_GRAB" };
			}

			return (pasToken, entitlementToken, clientConfig, userInfo, rmsToken);
		}

		internal static int GetFreePort()
		{
			using TcpListener listener = new(IPAddress.Loopback, 0);
			listener.Start();
			int port = ((IPEndPoint)listener.LocalEndpoint).Port;
			return port;
		}

		internal static string GenerateNonce(int length = 16)
		{
			byte[] nonceBytes = new byte[length];
			using (RandomNumberGenerator rng = RandomNumberGenerator.Create()) 
				rng.GetBytes(nonceBytes);
			return Convert.ToBase64String(nonceBytes);
		}

		internal static Dictionary<string, string> ParseUrlParameters(string url)
		{
			Dictionary<string, string> parameters = new(StringComparer.OrdinalIgnoreCase);
			
			if (url.IsNullOrEmpty()) return parameters;

			Uri uri = new (url);

			if (!uri.Query.IsNullOrEmpty()) AddParamsToDictionary(uri.Query.TrimStart('?'), parameters);

			// Handle the access token fragment
			if (string.IsNullOrWhiteSpace(uri.Fragment)) return parameters;
			AddParamsToDictionary(uri.Fragment.TrimStart('#'), parameters);

			return parameters;
		}

		private static void AddParamsToDictionary(string query, Dictionary<string, string> dict)
		{
			string[] pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);

			foreach (string pair in pairs)
			{
				string[] kv = pair.Split('=', 2);

				if (kv.Length != 2) continue;
				string key = WebUtility.UrlDecode(kv[0]);
				string value = WebUtility.UrlDecode(kv[1]);
				dict.TryAdd(key, value);
			}
		}

		private static bool IsValidUrl(string url) => Uri.TryCreate(url, UriKind.Absolute, out Uri? _);

		internal static string? TryGet(this Dictionary<string, string> host, string key) => host.GetValueOrDefault(key);
	}
}
