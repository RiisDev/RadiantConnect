﻿using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.CaptchaRiotAuth;
using RadiantConnect.Authentication.DriverRiotAuth.Handlers;
using RadiantConnect.Authentication.DriverRiotAuth.Misc;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

// ReSharper disable StringLiteralTypo
// ReSharper disable IdentifierTypo
// ReSharper disable InconsistentNaming

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        public enum DriverStatus {
            Checking_Existing_Processes,
            Creating_Driver,
            Driver_Created,
            Begin_SignIn,
            Checking_Cached_Auth,
            Checking_RSO_Login_Page,
            Logging_Into_RSO,
            Logging_Into_Valorant,
            Checking_RSO_Multi_Factor,
            Grabbing_Required_Tokens,
            Multi_Factor_Requested,
            Multi_Factor_Completed,
            SignIn_Completed,
            Cookies_Received
        }

        public enum CaptchaService
        {
            SuperMemory,
            CaptchaSolverNet
        };
        
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

        public string? MultiFactorCode
        {
            get => authHandler.MultiFactorCode;
            set => authHandler.MultiFactorCode = value;
        }

        internal AuthHandler authHandler = null!;

        public async Task<RSOAuth?> AuthenticateWithCaptcha(string username, string password, CaptchaService service, string captchaAuthorization)
        {
#if DEBUG
            switch (service)
            {
                case CaptchaService.SuperMemory | CaptchaService.CaptchaSolverNet:
                    SuperMemory superMemory = new(captchaAuthorization);
                    RSOAuth? rsoData = await superMemory.BeginSignIn(username, password);
                    superMemory.Dispose();
                    return rsoData;
                default:
                    throw new NotImplementedException();
            }
#else

            throw new NotImplementedException();
#endif
        }

        private readonly string[] UnSupportedBrowsers = ["chrome", "firefox"];

        public async Task<RSOAuth?> AuthenticateWithDriver(string username, string password, DriverSettings? driverSettings = null)
        {   
            driverSettings ??= new DriverSettings();

            if (UnSupportedBrowsers.Contains(driverSettings.ProcessName))
                throw new RadiantConnectAuthException("Unsupported browser");

            authHandler = new AuthHandler(
                driverSettings.ProcessName,
                driverSettings.BrowserExecutable,
                driverSettings.KillBrowser,
                driverSettings.CacheCookies
            );

            authHandler.OnMultiFactorRequested += () => OnMultiFactorRequested?.Invoke();
            authHandler.OnDriverUpdate += status => OnDriverUpdate?.Invoke(status);

            (IEnumerable<Cookie>? cookies, string? accessToken, string? pasToken, string? entitlement, object? clientConfig, string? _) = await authHandler.Initialize(username, password);

            if (cookies == null) return null;

            Dictionary<string, string> cookieDict = new();
            HashSet<string> seenNames = [];

            IEnumerable<Cookie> riotCookies = cookies as Cookie[] ?? cookies.ToArray();

            foreach (Cookie cookie in riotCookies)
                if (seenNames.Add(cookie.Name))
                    cookieDict[cookie.Name] = cookie.Value;

            string? rsoSubject = cookieDict.GetValueOrDefault("sub");
            string? rsoSsid = cookieDict.GetValueOrDefault("ssid");
            string? rsoTdid = cookieDict.GetValueOrDefault("tdid");
            string? rsoCsid = cookieDict.GetValueOrDefault("csid");
            string? rsoClid = cookieDict.GetValueOrDefault("clid");

            JsonWebToken jwt = new (pasToken);
            string? affinity = jwt.GetPayloadValue<string>("affinity");
            string? chatAffinity = jwt.GetPayloadValue<string>("desired.affinity");
            
            return new RSOAuth(rsoSubject, rsoSsid, rsoTdid, rsoCsid, rsoClid, accessToken, pasToken, entitlement, affinity, chatAffinity, clientConfig, riotCookies);
        }

        public async Task<IReadOnlyList<Cookie>?> GetCachedCookies()
        {
            string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
            return !File.Exists(cacheFile) ? null : JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile))?.Result.Cookies;
        }

        public async Task<string?> GetSsidFromDriverCache()
        {
            IEnumerable<Cookie>? cookiesData = await GetCachedCookies();
            return cookiesData?.FirstOrDefault(x => x.Name == "ssid")?.Value;
        }

        public async Task<string?> PerformDriverCacheRequest(ValorantNet.HttpMethod httpMethod, string baseUrl, string endPoint, IEnumerable<Cookie> cookies, string userAgent = "", Dictionary<string, string>? extraHeaders = null, AuthenticationHeaderValue? authentication = null, HttpContent? content = null)
        {
            CookieContainer container = new();
            using HttpClient Client = new(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                CookieContainer = container,
                AutomaticDecompression = DecompressionMethods.All,
                UseCookies = true,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

            Client.DefaultRequestHeaders.Authorization = authentication;

            if (extraHeaders != null)
                foreach (KeyValuePair<string, string> header in extraHeaders)
                    Client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", userAgent);
            
            if (string.IsNullOrEmpty(baseUrl)) return string.Empty;
            try
            {
                HttpRequestMessage httpRequest = new();
                httpRequest.Method = ValorantNet.InternalToHttpMethod[httpMethod];
                httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
                httpRequest.Content = content;
               
                foreach (Cookie cookie in cookies)
                    container.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));

                HttpResponseMessage responseMessage = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                string responseContent = await responseMessage.Content.ReadAsStringAsync();
                Debug.WriteLine($"[Authorization Log] Client: {JsonSerializer.Serialize(Client)}\n[Authorization Log] Uri:{baseUrl}{endPoint}\n[Authorization Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[Authorization Log] Request Content: {JsonSerializer.Serialize(content)}\n[Authorization Log] Response Content:{responseContent}[Authorization Log] Response Data: {responseMessage}");

                httpRequest.Dispose();
                responseMessage.Dispose();
                return responseContent.Contains("<html>") || responseContent.Contains("errorCode") ? null : responseContent;
            }
            catch
            {
                return null;
            }
        }

        public async Task Logout() => await authHandler.Logout();
    }
}
