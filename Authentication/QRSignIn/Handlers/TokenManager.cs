﻿using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Modules;
using RadiantConnect.Network.LocalEndpoints.DataTypes;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using Timer = System.Timers.Timer;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
    internal class TokenManager(Win32Form form, BuiltData qrData, HttpClient client)
    {
        internal delegate void TokensFinished(RSOAuth authData);
        internal event TokensFinished OnTokensFinished;

        internal static string GenerateTraceParent()
        {
            string traceId = Guid.NewGuid().ToString("N");
            string parentId = Guid.NewGuid().ToString("N")[..16];
            return $"00-{traceId}-{parentId}-00";
        }

        internal void SetHeaders(string host, string traceparent, string useragent, Dictionary<string, string>? additionalHeaders = null, string? sdk = null)
        {
            client.DefaultRequestHeaders.Clear();

            Dictionary<string, string> headers = new()
            {
                { "Host", host },
                { "User-Agent", useragent },
                { "Accept-Encoding", "deflate, gzip, zstd" },
                { "Accept", "application/json" },
                { "Connection", "keep-alive" },
                { "baggage", $"sdksid={sdk ?? qrData.SdkSid}" },
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
                additionalHeaders: new Dictionary<string, string>
                {
                    { "Cache-Control", "no-cache" }
                },
                Guid.NewGuid().ToString()
            );

            Dictionary<string, object?> postParams = new()
            {
                { "authentication_type", null },
                { "code_verifier", "" },
                { "login_token", loginToken },
                { "persist_login", false },
            };

            // No idea why this section has to be different, using normal post caused it to error?

            string jsonContent = JsonSerializer.Serialize(postParams);

            HttpRequestMessage requestMessage = new (HttpMethod.Post, "https://auth.riotgames.com/api/v1/login-token")
            {
                Content = new StringContent(jsonContent)
                {
                    Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
                }
            };

            HttpResponseMessage response = await client.SendAsync(requestMessage);

            if (response.StatusCode != HttpStatusCode.NoContent)
                throw new RadiantConnectAuthException("Failed to verify login_token");

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
                { "scope", "openid link lol_region lol summoner offline_access ban" }, // Just for shits, doesn't actually work
            };

            HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/authorization", postParams);
            Debug.WriteLine($"AccessTokenStage2: {await response.Content.ReadAsStringAsync()}");
            client.DefaultRequestHeaders.Clear();

            AccessTokenReturn? accessTokenData = await response.Content.ReadFromJsonAsync<AccessTokenReturn>();

            return accessTokenData?.Response?.Parameters?.Uri ?? "";
        }

        internal string ParseAccessToken(string accessToken)
        {
            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal async Task<string> GetAccessToken(string loginToken)
        {
            string traceData = await GetAccessTokenStage1(loginToken);
            string accessTokenUri = await GetAccessTokenStage2(traceData);
            
            return ParseAccessToken(accessTokenUri);
        }

        internal async Task<(string, string, object, string)> GetTokens(string accessToken)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get PAS token
            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            string pasToken = await response.Content.ReadAsStringAsync();

            // GetUserInfo 
            response = await httpClient.GetAsync("https://auth.riotgames.com/userinfo");
            string userInfo = await response.Content.ReadAsStringAsync();

            // Get entitlement token
            httpClient.DefaultRequestHeaders.Accept.Clear();
            response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json"));
            string entitlementToken = (await response.Content.ReadFromJsonAsync<EntitleReturn>())?.EntitlementsToken ?? "";

            // Get client config
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", entitlementToken);
            response = await httpClient.GetAsync("https://clientconfig.rpg.riotgames.com/api/v1/config/player?app=Riot%20Client");
            object clientConfig = await response.Content.ReadFromJsonAsync<object>() ?? new { };

            return (pasToken, entitlementToken, clientConfig, userInfo);
        }

        internal void InitiateTimer()
        {
            Timer timer = new();
            timer.Interval = 1000;
            timer.AutoReset = true;

            Task.Run(async () =>
            {
                await Task.Delay(30000);

                if (User32.IsWindow(form.WindowHandle))
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
                if (string.IsNullOrEmpty(accessToken)) return;

                (string pasToken, string entitlementToken, object clientConfig, string userInfo) = await GetTokens(accessToken);

                JsonWebToken jwt = new(pasToken);
                string? affinity = jwt.GetPayloadValue<string>("affinity");
                string? chatAffinity = jwt.GetPayloadValue<string>("desired.affinity");
                string? subject = new JsonWebToken(accessToken).GetPayloadValue<string>("sub");

                OnTokensFinished?.Invoke(new RSOAuth(
                    subject,
                    null,
                    null,
                    null,
                    null,
                    accessToken,
                    pasToken,
                    entitlementToken,
                    affinity,
                    chatAffinity,
                    clientConfig,
                    null
                ));
            };

            timer.Start();
        }
    }
}
