using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using RadiantConnect.Authentication.QRSignIn.Modules;
using Timer = System.Timers.Timer;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    internal class TokenManager(Win32Form form, BuiltData qrData, HttpClient client)
    {
        internal string SdkData = Guid.NewGuid().ToString();

        internal async Task<string> GetLoginToken()
        {
            string traceId = Guid.NewGuid().ToString("N");
            string parentId = Guid.NewGuid().ToString("N")[..16];
            string traceparent = $"00-{traceId}-{parentId}-00";

            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "authenticate.riotgames.com" },
                { "user-agent", "RiotGamesApi/24.9.1.4445 rso-authenticator (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={qrData.SdkSid}" },
                { "traceparent", traceparent },
                { "country-code", qrData.CountryCode.ToString() },
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.GetAsync("https://authenticate.riotgames.com/api/v1/login");
            
            string responseData = await response.Content.ReadAsStringAsync();
            
            client.DefaultRequestHeaders.Clear();

            if (!responseData.Contains("success")) return "";

            QrDataSuccess? data = JsonSerializer.Deserialize<QrDataSuccess>(responseData);

            return data?.Success?.LoginToken ?? "";
        }

        internal async Task<string> GetAccessTokenStage1(string loginToken)
        {
            string traceId = Guid.NewGuid().ToString("N");
            string parentId = Guid.NewGuid().ToString("N")[..16];
            string traceparent = $"00-{traceId}-{parentId}-00";
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "auth.riotgames.com" },
                { "User-Agent", "RiotGamesApi/24.10.1.4471 rso-auth (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={SdkData}" },
                { "traceparent", traceparent },
            };

            Dictionary<string, object?> postParams = new()
            {
                { "authentication_type", null },
                { "code_verifier", "" },
                { "login_token", loginToken },
                { "persist_login", false },
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/login-token", postParams);

            Debug.WriteLine($"AccessTokenStage1: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();

            return traceparent;
        }

        internal async Task<string> GetAccessTokenStage2(string traceparent)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", "auth.riotgames.com" },
                { "User-Agent", "RiotGamesApi/24.10.1.4471 rso-auth (Windows;10;;Professional, x64) riot_client/0" },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={SdkData}" },
                { "traceparent", traceparent },
            };

            Dictionary<string, object?> postParams = new()
            {
                { "acr_values", "" },
                { "claims", "" },
                { "client_id", "riot-client" },
                { "code_challenge", "" },
                { "code_challenge_method", "" },
                { "nonce", Guid.NewGuid().ToString("N") },
                { "redirect_uri", "http://localhost/redirect" },
                { "response_type", "token id_token" },
                { "scope", "account email profile openid link lol_region id summoner offline_access ban" },
            };

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/authorization", postParams);

            Debug.WriteLine($"AccessTokenStage2: {await response.Content.ReadAsStringAsync()}");

            client.DefaultRequestHeaders.Clear();

            return traceparent;
        }

        internal async Task<string> GetAccessToken(string loginToken)
        {
            string traceData = await GetAccessTokenStage1(loginToken);
            string stage2Data = await GetAccessTokenStage2(traceData);
            return "";
        }

        internal void InitiateTimer()
        {
            Timer timer = new();
            timer.Interval = 1000;
            timer.AutoReset = true;

            Task.Run(async () =>
            {
                await Task.Delay(30000);

                if (Win32.IsWindow(form.WindowHandle))
                {
                    timer.Stop();
                    timer.Dispose();
                    form.Dispose();
                }
            });

            timer.Elapsed += async (_, _) =>
            {
                string loginToken = await GetLoginToken();
                if (string.IsNullOrEmpty(loginToken)) return;

                form.Dispose();

                string accessToken = await GetAccessToken(loginToken);

            };

            timer.Start();
        }
    }
}
