using RadiantConnect.Authentication.QRSignIn.Modules;
using System.Collections.Specialized;
using System.Web;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
	internal sealed class LoginQrManager(HttpClient client)
	{
		private static readonly string TraceId = Guid.NewGuid().ToString("N");
		private static readonly string ParentId = Guid.NewGuid().ToString("N")[..16];
		private static readonly string SdkId = Guid.NewGuid().ToString();
		private readonly string _traceparent = $"00-{TraceId}-{ParentId}-00";

		internal static readonly Dictionary<Authentication.CountryCode, string> CountryRegion = new()
		{
			{ Authentication.CountryCode.Na, "en-US" },
			{ Authentication.CountryCode.Kr, "ko-KR" },
			{ Authentication.CountryCode.Jp, "ja-JP" },
			{ Authentication.CountryCode.Cn, "zh-CN" },
			{ Authentication.CountryCode.Tw, "zh-TW" },
			{ Authentication.CountryCode.Euw, "es-ES" },
			{ Authentication.CountryCode.Ru, "ru-RU" },
			{ Authentication.CountryCode.Th, "th-TH" },
			{ Authentication.CountryCode.Vn, "vi-VN" },
			{ Authentication.CountryCode.Id, "id-ID" },
			{ Authentication.CountryCode.My, "ms-MY" },
			{ Authentication.CountryCode.Eun, "pl-PL" },
			{ Authentication.CountryCode.Tr, "tr-TR" },
			{ Authentication.CountryCode.Br, "pt-BR" },
		};

		private void SetHeaders(string host, string traceparent, string useragent)
		{
			client.DefaultRequestHeaders.Clear();
			Dictionary<string, string> headers = new()
			{
				{ "Host", host },
				{ "User-Agent", useragent },
				{ "Accept-Encoding", "deflate, gzip, zstd" },
				{ "Accept", "application/json" },
				{ "Connection", "keep-alive" },
				{ "baggage", $"sdksid={SdkId}" },
				{ "traceparent", traceparent },
			};

			foreach (KeyValuePair<string, string> header in headers)
				client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
		}

		private async Task Stage1(string traceparent, Authentication.CountryCode countryCode)
		{
			SetHeaders("clientconfig.rpg.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 client-config (Windows;10;;Professional, x64) riot_client/0");

			Dictionary<string, string> queryParams = new()
			{
				{ "os", "windows" },
				{ "region", CountryRegion[countryCode] },
				{ "app", "Riot Client" },
				{ "version", "99.0.2.2539" },
				{ "patchline", "KeystoneFoundationLiveWin" },
			};

			HttpResponseMessage response = await client.GetAsync(BuildUrlWithQueryParameters("https://clientconfig.rpg.riotgames.com/api/v1/config/public", queryParams)).ConfigureAwait(false);
			Debug.WriteLine($"Stage1: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");

			client.DefaultRequestHeaders.Clear();
		}

		private async Task Stage2(string traceparent)
		{
			SetHeaders("auth.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0");

			HttpResponseMessage response = await client.GetAsync("https://auth.riotgames.com/.well-known/openid-configuration").ConfigureAwait(false);
			Debug.WriteLine($"Stage2: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");

			client.DefaultRequestHeaders.Clear();
		}

		private async Task<(string Cluster, string Suuid, string Timestamp)> Stage3(string traceparent, Authentication.CountryCode countryCode)
		{
			SetHeaders("authenticate.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-authenticator (Windows;10;;Professional, x64) riot_client/0");

			Dictionary<string, object?> postParams = new()
			{
				{ "client_id", "riot-client" },
				{ "language", CountryRegion[countryCode].Replace('-', '_') },
				{ "platform", "windows" },
				{ "remember", false },
				{ "type", "auth" },
				{ "qrcode", new { } },
				{ "sdkVersion", "24.11.0.4602" },
				// Additional args to match 'normal' request
				{ "apple", null },
				{ "campaign", null },
				{ "code", null },
				{ "facebook", null },
				{ "gamecenter", null },
				{ "google", null },
				{ "mockDeviceId", null },
				{ "mockPlatform", null },
				{ "multifactor", null },
				{ "nintendo", null },
				{ "playstation", null },
				{ "riot_identity", null },
				{ "riot_identity_signup", null },
				{ "rso", null },
				{ "xbox", null },
			};

			HttpResponseMessage response = await client.PostAsJsonAsync("https://authenticate.riotgames.com/api/v1/login", postParams).ConfigureAwait(false);
			Stage3Return? responseBody = await response.Content.ReadFromJsonAsync<Stage3Return>().ConfigureAwait(false);

			client.DefaultRequestHeaders.Clear();

			return responseBody == null ||
				   responseBody.Cluster.IsNullOrEmpty() ||
				   responseBody.Suuid.IsNullOrEmpty() ||
				   responseBody.Timestamp.IsNullOrEmpty()
				? throw new RadiantConnectAuthException("Failed to find required fields")
				: (responseBody.Cluster, responseBody.Suuid, responseBody.Timestamp);
		}

		internal async Task<BuiltData> Build(Authentication.CountryCode countryCode)
		{
			await Stage1( _traceparent, countryCode).ConfigureAwait(false);

			await Stage2(_traceparent).ConfigureAwait(false);

			(string cluster, string suuid, string timestamp) = await Stage3(_traceparent, countryCode).ConfigureAwait(false);

			return new BuiltData(
				LoginUrl: $"https://qrlogin.riotgames.com/riotmobile?cluster={cluster}&suuid={suuid}&timestamp={timestamp}&utm_source=riotclient&utm_medium=client&utm_campaign=qrlogin-riotmobile",
				Session: "",
				SdkSid: SdkId,
				Cluster: cluster,
				Suuid: suuid,
				Timestamp: timestamp,
				Language: CountryRegion[countryCode],
				CountryCode: countryCode
			);
		}

		private static string BuildUrlWithQueryParameters(string baseUrl, Dictionary<string, string> queryParams)
		{
			UriBuilder uriBuilder = new(baseUrl);
			if (uriBuilder is { Scheme: "https", Port: 443 } or { Scheme: "http", Port: 80 }) uriBuilder.Port = -1;
			NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
			foreach (KeyValuePair<string, string> param in queryParams) query[param.Key] = param.Value;
			uriBuilder.Query = query.ToString();
			return uriBuilder.ToString();
		}
	}
}
