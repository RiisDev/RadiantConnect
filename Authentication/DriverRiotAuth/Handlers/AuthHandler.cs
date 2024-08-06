using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using RadiantConnect.Authentication.DriverRiotAuth.Misc;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
// ReSharper disable AccessToDisposedClosure <--- It's handled in DriverHandler

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
    internal class AuthHandler(string browserProcess, string browserExecutable, bool killBrowser)
    {
        // Expression Methods
        internal void Log(Authentication.DriverStatus status) => OnDriverUpdate?.Invoke(status);

        // Events
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

        // Internal Variables
        internal int DriverPort { get; } = Win32.GetFreePort();
        internal Random ActionIdGenerator { get; } = new();
        internal SocketHandler SocketHandler = null!;

        internal Process? WebDriver { get; set; }

        internal ClientWebSocket Socket { get; set; } = new();

        // User Variables
        public string? MultiFactorCode { get; set; }
        
        internal async Task<(IEnumerable<Records.Cookie>?, string?, string?, string?, object?, string?)> Initialize(string username, string password)
        {

            Log(Authentication.DriverStatus.Checking_Existing_Processes);
            DriverHandler.DoDriverCheck(browserProcess, browserExecutable, killBrowser);

            Log(Authentication.DriverStatus.Creating_Driver);
            (WebDriver, string? socketUrl) = await DriverHandler.StartDriver(browserExecutable, DriverPort);

            if (WebDriver == null)
                throw new RadiantConnectException("Failed to start browser driver");

            if (string.IsNullOrEmpty(socketUrl)) throw new RadiantConnectAuthException("Failed to find socket");

            Socket = new ClientWebSocket();
            await Socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);
            SocketHandler = new SocketHandler(Socket, this, DriverPort);

            await SocketHandler.InitiatePageEvents(Socket);

            Task.Run(() => DriverHandler.ListenAsync(Socket));

            Log(Authentication.DriverStatus.Driver_Created);

            try
            {
                Log(Authentication.DriverStatus.Begin_SignIn);
                return await PerformSignInAsync(username, password);
            }
            catch (Exception e)
            {
                WebDriver.Kill(true);
                throw new RadiantConnectAuthException(e.Message);
            }
            finally
            {
                Socket.Abort();
                Socket.Dispose();
                WebDriver.Kill(true);
            }
        }

        internal async Task<(IEnumerable<Records.Cookie>?, string?, string?, string?, object?, string?)> PerformSignInAsync(string username, string password)
        {
            bool navComplete = false;
            string navId = string.Empty;

            DriverHandler.OnFrameNavigation += (url, frame) => { if (url == "https://account.riotgames.com/") navId = frame; };
            DriverHandler.OnFrameLoaded += (_, frame) => { if (frame == navId) navComplete = true; };

            bool cookiesValid = await CheckCookieCache(username);

            if (cookiesValid) goto Complete;

            Log(Authentication.DriverStatus.Clearing_Cached_Auth);
            await SocketHandler.ClearCookies();

            Log(Authentication.DriverStatus.Redirecting_To_RSO);
            await SocketHandler.NavigateTo("https://account.riotgames.com/", "Sign in", DriverPort, Socket);

            Log(Authentication.DriverStatus.Checking_RSO_Login_Page);
            if (await SocketHandler.IsLoginPageDetectedAsync()) await SocketHandler.SendLoginDataAsync(username, password);

            Log(Authentication.DriverStatus.Checking_RSO_Multi_Factor);
            if (await SocketHandler.IsMfaRequiredAsync()) await HandleMfaAsync();

            while (!navComplete) await Task.Delay(100);

            Complete:
            Log(Authentication.DriverStatus.Logging_Into_Valorant);
            (IEnumerable<Records.Cookie>? cookies, string accessToken) = await GetAccessTokenRedirect();

            Log(Authentication.DriverStatus.SignIn_Completed);
            return await CheckCookieStatusAsync(accessToken, cookies!);
        }

        // This isn't in 'SocketHandler' due to MFA properties
        internal async Task HandleMfaAsync()
        {
            Log(Authentication.DriverStatus.Multi_Factor_Requested);
            OnMultiFactorRequested?.Invoke();

            while (string.IsNullOrEmpty(MultiFactorCode)) await Task.Delay(500); // Wait for MFA code to be set

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

            await SocketHandler.ExecuteOnPageWithResponse("Verification Required", DriverPort, mfaDataInput, "", Socket);

            Log(Authentication.DriverStatus.Multi_Factor_Completed);
        }

        internal async Task<(IEnumerable<Records.Cookie>?, string)> GetAccessTokenRedirect(bool killClient = true)
        {
            if (killClient)
                await DriverHandler.WaitForPage("Riot Account Management", DriverPort, Socket, 999999);

            CookieRoot? getCookies = await SocketHandler.GetCookiesAsync("Riot Account Management");
            HttpResponseMessage response = await PerformCookieRequest("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");

            string redirectRequest = response.RequestMessage?.RequestUri?.ToString() ?? "";

            if (string.IsNullOrEmpty(redirectRequest))
                throw new RadiantConnectAuthException("Failed to redirect to Valorant RSO");
            if (!redirectRequest.Contains("access_token"))
                throw new RadiantConnectAuthException("Failed to get valorant auth");

            return (getCookies?.Result.Cookies, ParseAccessToken(redirectRequest));
        }

        internal async Task<bool> CheckCookieCache(string username)
        {
            try
            {
                Log(Authentication.DriverStatus.Checking_Cached_Auth);
                (_, string accessToken) = await GetAccessTokenRedirect(false);

                HttpResponseMessage response = await PerformCookieRequest("https://account.riotgames.com/api/account/v1/user");
                UserInfo? userInfoData = await response.Content.ReadFromJsonAsync<UserInfo>();

                if (userInfoData == null || string.IsNullOrEmpty(accessToken))
                {
                    return false;
                }

                Log(Authentication.DriverStatus.Checking_Auth_Validity);
                string loggedUser = userInfoData.Username;
                string modifiedUser = loggedUser.Replace("*", "");
                string modifiedLocalUser = $"{username[..2]}{username[^2..]}";

                if (loggedUser.Length != username.Length || modifiedUser != modifiedLocalUser)
                {
                    return false;
                }

                (string pasToken, string entitlementToken, _, string userInfo) = await GetTokens(accessToken);

                return !string.IsNullOrEmpty(pasToken) && !string.IsNullOrEmpty(entitlementToken) && !string.IsNullOrEmpty(userInfo);
            }
            catch
            {
                return false;
            }
        }

        internal async Task<HttpResponseMessage> PerformCookieRequest(string url)
        {
            CookieRoot? getCookies = await SocketHandler.GetCookiesAsync("");

            CookieContainer clientCookies = new();
            foreach (Records.Cookie cookie in getCookies?.Result.Cookies!)
                clientCookies.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));

            using HttpClient httpClient = new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
                CookieContainer = clientCookies
            });

            return await httpClient.GetAsync(url);
        }

        internal async Task<(IEnumerable<Records.Cookie>?, string?, string?, string?, object?, string?)> CheckCookieStatusAsync(string accessToken, IEnumerable<Records.Cookie> riotCookies)
        {
            (string pasToken, string entitlementToken, object clientConfig, string userInfo) = await GetTokens(accessToken);
            Log(Authentication.DriverStatus.Cookies_Received);
            return (riotCookies, accessToken, pasToken, entitlementToken, clientConfig, userInfo);
        }

        internal string ParseAccessToken(string accessToken)
        {
            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal async Task<(string, string, object, string)> GetTokens(string accessToken)
        {
            Log(Authentication.DriverStatus.Grabbing_Required_Tokens);
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

        public async Task Logout() => await SocketHandler.NavigateTo("https://auth.riotgames.com/logout", "/logout", DriverPort, Socket);
    }
}
