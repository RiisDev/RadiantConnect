using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RadiantConnect.Authentication.DriverRiotAuth.Misc;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
// ReSharper disable AccessToDisposedClosure <--- It's handled in DriverHandler

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
    internal class AuthHandler(string browserProcess, string browserExecutable, bool killBrowser, bool cacheCookies)
    {
        // Expression Methods
        internal void Log(Authentication.DriverStatus status) => OnDriverUpdate?.Invoke(status);

        // Events
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

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

            Task.Run(() => DriverHandler.ListenAsync(Socket));

            await SocketHandler.InitiateRuntimeHandles(Socket, username, password);
            
            Log(Authentication.DriverStatus.Driver_Created);

            try
            {
                Log(Authentication.DriverStatus.Begin_SignIn);
                return await PerformSignInAsync();
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

        internal readonly Dictionary<string, string> RiotUrls = new()
        {
            {"AccessToken", "https://playvalorant.com/en-us/opt_in/#access_token=" },
            {"LoginUrl", "https://auth.riotgames.com/authorize?redirect_uri=https://playvalorant.com/opt_in&client_id=play-valorant-web-prod&response_type=token id_token&nonce=1&scope=account email profile openid link lol_region id"},
            {"SignInDetected", "https://authenticate.riotgames.com/?client_id=play-valorant-web-prod&method=riot_identity&platform=web&redirect_uri=https%3A%2F%2Fauth.riotgames.com%2Fauthorize%3Fclient_id%3Dplay-valorant-web-prod%26nonce%3D1%26redirect_uri%3Dhttps%253A%252F%252Fplayvalorant.com%252Fopt_in%26response_type%3Dtoken%2520id_token%26scope%3Daccount%2520email%2520profile%2520openid%2520link%2520lol_region%2520id"}
        };

        internal async Task<(IEnumerable<Records.Cookie>?, string?, string?, string?, object?, string?)> PerformSignInAsync()
        {
            string accessTokenFound = string.Empty;
            Log(Authentication.DriverStatus.Logging_Into_Valorant);
            await SocketHandler.NavigateTo(RiotUrls["LoginUrl"], "VALORANT_RSO", DriverPort, Socket, false);

            DriverHandler.OnMfaDetected += async (_) =>
            {
                Log(Authentication.DriverStatus.Checking_RSO_Multi_Factor);
                await HandleMfaAsync();
            };

            DriverHandler.OnAccessTokenFound += (data) => accessTokenFound = data!;

            while (string.IsNullOrEmpty(accessTokenFound)) await Task.Delay(5);

            Log(Authentication.DriverStatus.Grabbing_Required_Tokens);
            (IEnumerable<Records.Cookie>? cookies, string accessToken) = await GetAccessTokenRedirect(accessTokenFound);

            Log(Authentication.DriverStatus.SignIn_Completed);
            return await GetRSOUserData(accessToken, cookies!);
        }
        
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
        
        internal async Task<(IEnumerable<Records.Cookie>?, string)> GetAccessTokenRedirect(string accessToken)
        {
            CookieRoot? getCookies = await SocketHandler.GetCookiesAsync("");

            if (cacheCookies)
            {
                Directory.CreateDirectory($@"{Path.GetTempPath()}\RadiantConnect\");
                await File.WriteAllTextAsync($@"{ Path.GetTempPath()}\RadiantConnect\cookies.json", JsonSerializer.Serialize(getCookies));
            }

            return (getCookies?.Result.Cookies, ParseAccessToken(accessToken));
        }

        internal async Task<bool> CheckCookieCache(string username)
        {
            try
            {
                if (!cacheCookies) return false;
                await SocketHandler.SetCookieCacheAsync();

                Log(Authentication.DriverStatus.Checking_Cached_Auth);
                HttpResponseMessage response = await PerformCookieRequest(RiotUrls["LoginUrl"]);
                string redirectRequest = response.RequestMessage?.RequestUri?.ToString() ?? "";

                return !string.IsNullOrEmpty(redirectRequest) && redirectRequest.Contains("access_token");
            }
            catch
            {
                return false;
            }
        }

        internal async Task<HttpResponseMessage> PerformCookieRequest(string url, AuthenticationHeaderValue? authentication = null)
        {
            CookieRoot? getCookies = await SocketHandler.GetCookiesAsync("");

            CookieContainer clientCookies = new();
            foreach (Records.Cookie cookie in getCookies?.Result.Cookies!)
            {
                if (cookie.Value.Contains("\u0022")) continue;
                clientCookies.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
            }
                

            using HttpClient httpClient = new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
                CookieContainer = clientCookies
            });

            if (authentication != null)
                httpClient.DefaultRequestHeaders.Authorization = authentication;

            return await httpClient.GetAsync(url);
        }

        internal async Task<(IEnumerable<Records.Cookie>?, string?, string?, string?, object?, string?)> GetRSOUserData(string accessToken, IEnumerable<Records.Cookie> riotCookies)
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
