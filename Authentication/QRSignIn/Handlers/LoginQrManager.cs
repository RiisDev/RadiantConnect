using RadiantConnect.Authentication.QRSignIn.Modules;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Web;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    internal class LoginQrManager(HttpClient client)
    {
        private static readonly string TraceId = Guid.NewGuid().ToString("N");
        private static readonly string ParentId = Guid.NewGuid().ToString("N")[..16];
        private static readonly string SdkId = Guid.NewGuid().ToString();
        private readonly string _traceparent = $"00-{TraceId}-{ParentId}-00";

        internal static readonly Dictionary<Authentication.CountryCode, string> CountryRegion = new()
        {
            { Authentication.CountryCode.NA, "en-US" },
            { Authentication.CountryCode.KR, "ko-KR" },
            { Authentication.CountryCode.JP, "ja-JP" },
            { Authentication.CountryCode.CN, "zh-CN" },
            { Authentication.CountryCode.TW, "zh-TW" },
            { Authentication.CountryCode.EUW, "es-ES" },
            { Authentication.CountryCode.RU, "ru-RU" },
            { Authentication.CountryCode.TH, "th-TH" },
            { Authentication.CountryCode.VN, "vi-VN" },
            { Authentication.CountryCode.ID, "id-ID" },
            { Authentication.CountryCode.MY, "ms-MY" },
            { Authentication.CountryCode.EUN, "pl-PL" },
            { Authentication.CountryCode.TR, "tr-TR" },
            { Authentication.CountryCode.BR, "pt-BR" },
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

        internal async Task Stage1(string traceparent, Authentication.CountryCode countryCode)
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

            HttpResponseMessage response = await client.GetAsync(BuildUrlWithQueryParameters("https://clientconfig.rpg.riotgames.com/api/v1/config/public", queryParams));
            Debug.WriteLine($"Stage1: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();
        }

        internal async Task Stage2(string traceparent)
        {
            SetHeaders("auth.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0");

            HttpResponseMessage response = await client.GetAsync("https://auth.riotgames.com/.well-known/openid-configuration");
            Debug.WriteLine($"Stage2: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();
        }

        internal async Task<(string Cluster, string Suuid, string Timestamp)> Stage3(string traceparent, Authentication.CountryCode countryCode)
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

            HttpResponseMessage response = await client.PostAsJsonAsync("https://authenticate.riotgames.com/api/v1/login", postParams);
            Stage3Return? responseBody = await response.Content.ReadFromJsonAsync<Stage3Return>();

            client.DefaultRequestHeaders.Clear();

            if (responseBody == null ||
                string.IsNullOrEmpty(responseBody.Cluster) ||
                string.IsNullOrEmpty(responseBody.Suuid) ||
                string.IsNullOrEmpty(responseBody.Timestamp))
            {
                throw new RadiantConnectAuthException("Failed to find required fields");
            }

            return (responseBody.Cluster, responseBody.Suuid, responseBody.Timestamp);
        }

        internal async Task<BuiltData> Build(Authentication.CountryCode countryCode)
        {
            await Stage1( _traceparent, countryCode);

            await Stage2(_traceparent);

            (string cluster, string suuid, string timestamp) = await Stage3(_traceparent, countryCode);

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

        internal static string BuildUrlWithQueryParameters(string baseUrl, Dictionary<string, string> queryParams)
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
