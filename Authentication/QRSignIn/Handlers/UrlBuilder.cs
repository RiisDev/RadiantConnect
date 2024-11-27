using RadiantConnect.Authentication.QRSignIn.Modules;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Web;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    internal class UrlBuilder(HttpClient client)
    {
        private static readonly string TraceId = Guid.NewGuid().ToString("N");
        private static readonly string ParentId = Guid.NewGuid().ToString("N")[..16];
        private static readonly string SdkId = Guid.NewGuid().ToString();
        private readonly string _traceparent = $"00-{TraceId}-{ParentId}-00";

        internal static Dictionary<Authentication.CountryCode, string> CountryRegion = new()
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

        internal async Task Stage1(string sdkId, string traceparent, Authentication.CountryCode countryCode)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "clientconfig.rpg.riotgames.com" },
                { "User-Agent", "RiotGamesApi/24.9.1.4445 client-config (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={sdkId}" },
                { "traceparent", traceparent },
                { "country-code", countryCode.ToString() },
            };

            Dictionary<string, string> getParams = new()
            {
                { "os", "windows" },
                { "region", CountryRegion[countryCode] },
                { "app", "Riot Client" },
                { "version", "97.0.1.2366" },
                { "patchline", "KeystoneFoundationLiveWin" }
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.GetAsync(BuildUrlWithQueryParameters("https://clientconfig.rpg.riotgames.com/api/v1/config/public", getParams));

            Debug.WriteLine($"Stage1: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();
        }

        internal async Task Stage2(string sdkId, string traceparent, Authentication.CountryCode countryCode)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "auth.riotgames.com" },
                { "user-agent", "RiotGamesApi/24.9.1.4445 rso-auth (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={sdkId}" },
                { "traceparent", traceparent },
                { "country-code", countryCode.ToString() },
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.GetAsync("https://auth.riotgames.com/.well-known/openid-configuration");

            Debug.WriteLine($"Stage2: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();
        }

        internal async Task<(string, string, string)> Stage3(string sdkId, string traceparent, Authentication.CountryCode countryCode)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "authenticate.riotgames.com" },
                { "user-agent", "RiotGamesApi/24.9.1.4445 rso-authenticator (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={sdkId}" },
                { "traceparent", traceparent },
                { "country-code", countryCode.ToString() },
            };

            Dictionary<string, object> postParams = new()
            {
                { "client_id", "riot-client" },
                { "language", countryCode.ToString() },
                { "platform", "windows" },
                { "remember", false },
                { "type", "auth" },
                { "qrcode", new {} },
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.PostAsJsonAsync("https://authenticate.riotgames.com/api/v1/login", postParams);

            Stage3Return? responseBody = await response.Content.ReadFromJsonAsync<Stage3Return>();

            client.DefaultRequestHeaders.Clear();

            if (responseBody == null ||
                string.IsNullOrEmpty(responseBody.Cluster) ||
                string.IsNullOrEmpty(responseBody.Suuid) ||
                string.IsNullOrEmpty(responseBody.Timestamp))
                throw new RadiantConnectAuthException("Failed to find required fields");

            return (responseBody.Cluster, responseBody.Suuid, responseBody.Timestamp);
        }

        internal async Task<BuiltData> Build(Authentication.CountryCode countryCode)
        {
            await Stage1(SdkId, _traceparent, countryCode);

            await Stage2(SdkId, _traceparent, countryCode);

            (string cluster, string suuid, string timestamp) = await Stage3(SdkId, _traceparent, countryCode);

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
