using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using RadiantConnect.Authentication.QRSignIn.Modules;
using Timer = System.Timers.Timer;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    internal class TokenManager(Win32Form form, BuiltData qrData, HttpClient client)
    {
        internal static string GenerateTraceParent()
        {
            string traceId = Guid.NewGuid().ToString("N");
            string parentId = Guid.NewGuid().ToString("N")[..16];
            return $"00-{traceId}-{parentId}-00";
        }

        internal void SetHeaders(string host, string traceparent, string useragent, Dictionary<string, string>? additionalHeaders = null)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", host },
                { "User-Agent", useragent },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={qrData.SdkSid}" },
                { "traceparent", traceparent },
            };
            
            if (additionalHeaders != null)
                foreach (KeyValuePair<string, string> header in additionalHeaders)
                    headers[header.Key] = header.Value;

            foreach (KeyValuePair<string, string> header in headers)
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
        }

        internal async Task<string> GetLoginToken()
        {
            string traceparent = GenerateTraceParent();
            SetHeaders("authenticate.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-authenticator (Windows;10;;Professional, x64) riot_client/0");

            HttpResponseMessage response = await client.GetAsync("https://authenticate.riotgames.com/api/v1/login");
            string responseData = await response.Content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Clear();

            if (!responseData.Contains("success")) return string.Empty;

            QrDataSuccess? data = JsonSerializer.Deserialize<QrDataSuccess>(responseData);
            
            return data?.Success?.LoginToken ?? string.Empty;
        }

        internal async Task<string> GetAccessTokenStage1(string loginToken)
        {
            string traceparent = GenerateTraceParent();
            SetHeaders(
                host: "auth.riotgames.com", 
                traceparent: traceparent, 
                useragent: "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0", 
                additionalHeaders: new Dictionary<string, string>()
                {
                    {"Cache-Control", "no-cache"}
                }
            );

            Dictionary<string, object?> postParams = new()
            {
                { "authentication_type", null },
                { "code_verifier", "" },
                { "login_token", loginToken },
                { "persist_login", false },
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/login-token", postParams);
            Debug.WriteLine($"AccessTokenStage1: {await response.Content.ReadAsStringAsync()}");
            client.DefaultRequestHeaders.Clear();

            return traceparent;
        }

        internal async Task<string> GetAccessTokenStage2(string traceparent)
        {
            SetHeaders("auth.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0");

            Dictionary<string, object?> postParams = new()
            {
                { "acr_values", "" },
                { "claims", "" },
                { "client_id", "riot-client" },
                { "code_challenge", "" },
                { "code_challenge_method", "" },
                { "nonce", Guid.NewGuid().ToString("N")[..22] },
                { "redirect_uri", "http://localhost/redirect" },
                { "response_type", "token id_token" },
                { "scope", "account email profile openid link lol_region id summoner offline_access ban" }, // Just for shits, doesn't actually work
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/authorization", postParams);
            Debug.WriteLine($"AccessTokenStage2: {await response.Content.ReadAsStringAsync()}");
            client.DefaultRequestHeaders.Clear();

            return traceparent;
        }

        internal async Task<string> GetAccessToken(string loginToken)
        {
            string traceData = await GetAccessTokenStage1(loginToken);
            string stage2Data = await GetAccessTokenStage2(traceData);
            return stage2Data;
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
