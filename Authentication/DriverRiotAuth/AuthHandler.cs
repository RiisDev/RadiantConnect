using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Authentication.DriverRiotAuth
{
    internal class AuthHandler(string browserProcess, string browserExecutable, bool killBrowser)
    {
        internal IEnumerable<Process> GetBrowserProcess() => Process.GetProcessesByName(browserProcess).ToList();

        public delegate void MultiFactorEvent();
        public event MultiFactorEvent? OnMultiFactorRequested;

        public delegate void DriverEvent(Authentication.DriverStatus status);
        public event DriverEvent? OnDriverUpdate;

        public string? MultiFactorCode { get; set; }

        internal int DriverPort = Driver.FreeTcpPort();
        internal Random ActionIdGenerator = new();

        internal Process? WebDriver;

        internal ClientWebSocket Socket { get; set; } = null!;

        internal void DoDriverCheck()
        {
            List<Process> browserProcesses = GetBrowserProcess().ToList();

            // We have to kill all current chromium processes due to remote-debugging-DriverPort
            if (browserProcesses.Any() && !killBrowser)
                throw new RadiantConnectException($"{browserProcesses.First().ProcessName} is currently running, it must be closed or Initialize must be started with 'true'");
            if (browserProcesses.Any() && killBrowser)
                browserProcesses.ToList().ForEach(x => x.Kill());
            
            if (!File.Exists(browserExecutable))
                throw new RadiantConnectException($"Browser executable not found at {browserExecutable}");
        }

        internal void Log(Authentication.DriverStatus status) => OnDriverUpdate?.Invoke(status);

        internal async Task ClearCookies()
        {
            Dictionary<string, string> riotCookies = new()
            {
                { "sub", "account.riotgames.com" },
                { "ssid", "auth.riotgames.com" },
                { "tdid", ".riotgames.com" },
                { "csid", "auth.riotgames.com" },
                { "clid", "auth.riotgames.com" },
                { "id_token", ".riotgames.com" },
                { "PVPNET_TOKEN_NA", ".riotgames.com" },
                { "__Secure-access_token", ".playvalorant.com" },
                { "__Secure-refresh_token", "xsso.playvalorant.com" },
                { "__Secure-id_token", ".playvalorant.com" }
            };

            foreach (KeyValuePair<string, string> riotCookie in riotCookies)
            {
                Dictionary<string, object> clearCookies = new()
                {
                    { "id", ActionIdGenerator.Next() },
                    { "method", "Network.deleteCookies" },
                    { "params", new Dictionary<string, string>
                    {
                        {"name", riotCookie.Key},
                        {"domain", riotCookie.Value}
                    }}
                };

                await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(clearCookies))), WebSocketMessageType.Text, true, CancellationToken.None);
            }

        }

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?, string?)> Initialize(string username, string password)
        {

            Log(Authentication.DriverStatus.Checking_Existing_Processes);
            DoDriverCheck();

            Log(Authentication.DriverStatus.Creating_Driver);
            (WebDriver, string? socketUrl) = await Driver.StartDriver(browserExecutable, DriverPort);

            if (WebDriver == null)
                throw new RadiantConnectException("Failed to start browser driver");

            if (string.IsNullOrEmpty(socketUrl)) throw new RadiantConnectAuthException("Failed to find socket");

            Socket = new ClientWebSocket();

            await Socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);

            Task.Run(() => Driver.ListenAsync(Socket));

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
        }

        internal async Task<(IEnumerable<Cookie>, string)> GetAccessTokenRedirect()
        {
            CookieRoot? getCookies = await GetCookiesAsync("Riot Account Management");

            CookieContainer clientCookies = new();
            foreach (Cookie cookie in getCookies?.Result.Cookies!)
                clientCookies.Add(new System.Net.Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));

            WebDriver?.Kill(true); // Kill driver process no longer needed

            using HttpClient httpClient = new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true, // Not sure if needed, but will prevent weird cert errors
                AllowAutoRedirect = true,
                CookieContainer = clientCookies
            });

            HttpResponseMessage response = await httpClient.GetAsync("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");

            string redirectRequest = response.RequestMessage?.RequestUri?.ToString() ?? "";

            if (string.IsNullOrEmpty(redirectRequest))
                throw new RadiantConnectAuthException("Failed to redirect to Valorant RSO");
            if (!redirectRequest.Contains("access_token"))
                throw new RadiantConnectAuthException("Failed to get valorant auth");
            
            return (getCookies.Result.Cookies, ParseAccessToken(redirectRequest));
        }

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?, string?)> PerformSignInAsync(string username, string password)
        {
            Log(Authentication.DriverStatus.Clearing_Cached_Auth);
            await ClearCookies();

            Log(Authentication.DriverStatus.Redirecting_To_RSO);
            await Driver.NavigateTo("https://account.riotgames.com/", "Sign in", DriverPort, Socket);

            Log(Authentication.DriverStatus.Checking_RSO_Login_Page);
            if (await IsLoginPageDetectedAsync()) await SendLoginDataAsync(username, password);

            Log(Authentication.DriverStatus.Checking_RSO_Multi_Factor);
            if (await IsMfaRequiredAsync()) await HandleMfaAsync();

            Log(Authentication.DriverStatus.Logging_Into_Valorant);
            (IEnumerable<Cookie> cookies, string accessToken) = await GetAccessTokenRedirect();

            Log(Authentication.DriverStatus.SignIn_Completed);
            return await CheckCookieStatusAsync(accessToken, cookies);
        }
        
        internal async Task<bool> IsLoginPageDetectedAsync()
        {
            Dictionary<string, object> initialSignInCheck = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string>
                    {
                        { "expression", "document.getElementsByName('username').length>0" }
                    }
                }
            };

            string? response = await Driver.ExecuteOnPageWithResponse("Sign in", DriverPort, initialSignInCheck, "result\":{\"type\":\"boolean\",\"value\":true}", Socket, true);
            return response is not null && response.Contains("true");
        }

        internal async Task SendLoginDataAsync(string username, string password, bool rso = true)
        {
            Log(rso ? Authentication.DriverStatus.Logging_Into_RSO : Authentication.DriverStatus.Logging_Into_Valorant);

            Dictionary<string, object> loginData = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string>
                    {
                        { "expression", $"function set(e,t){{for(let[n,s]of(t(e),Object.entries(e)))n.includes(\"__reactEventHandlers\")&&s.onChange&&s.onChange({{target:e}})}}set(document.getElementsByName(\"username\")[0],e=>e.value=\"{username}\"),set(document.getElementsByName(\"password\")[0],e=>e.value=\"{password}\"),document.querySelectorAll(\"[data-testid='btn-signin-submit']\")[0].click();" }
                    }
                }
            };
            await Driver.ExecuteOnPageWithResponse("Sign in", DriverPort, loginData, "result\":{\"type\":\"undefined\"}", Socket);
        }

        internal async Task CheckError()
        {
            Dictionary<string, object> faData = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string>
                    {
                        { "expression", "document.querySelector(\"[data-testid='panel-title']\").innerText.includes(\"Oops!\")" }
                    }
                }
            };

            string? response = await Driver.ExecuteOnPageWithResponse("Sign in", DriverPort, faData, "", Socket, false, true);

            if (response is not null && response.Contains("result\":{\"type\":\"boolean\",\"value\":true}"))
            {
                throw new RadiantConnectAuthException("Error occurred during login");
            }
        }

        internal async Task<bool> IsMfaRequiredAsync()
        {
            bool mfaDetected = await Driver.PageExists("Verification Required", DriverPort);
            bool signInNotChanged = await Driver.PageExists("Sign in", DriverPort);
            
            if (signInNotChanged) await CheckError();
            if (!mfaDetected) return false;

            Dictionary<string, object> faData = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string>
                    {
                        { "expression", "document.querySelectorAll('input[minlength=\"1\"][maxlength=\"1\"][type=\"text\"][inputmode=\"numeric\"]').length == 6" }
                    }
                }
            };

            await Driver.ExecuteOnPageWithResponse("Verification Required", DriverPort, faData, "result\":{\"type\":\"boolean\",\"value\":true}", Socket);

            return true;
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

            await Driver.ExecuteOnPageWithResponse("Verification Required", DriverPort, mfaDataInput, "", Socket);

            Log(Authentication.DriverStatus.Multi_Factor_Completed);
        }

        internal async Task<CookieRoot?> GetCookiesAsync(string pageTitle)
        {
            Dictionary<string, object> cookieResponse = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Network.getAllCookies" }
            };

            string? cookieData = await Driver.ExecuteOnPageWithResponse(pageTitle, DriverPort, cookieResponse, "", Socket, false, true);
            return JsonSerializer.Deserialize<CookieRoot>(cookieData!);
        }

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?, string?)> CheckCookieStatusAsync(string accessToken, IEnumerable<Cookie> riotCookies)
        {
            (string pasToken, string entitlementToken, string clientConfig) = await GetTokens(accessToken);
            Log(Authentication.DriverStatus.Cookies_Received);
            return (riotCookies, accessToken, pasToken, entitlementToken, clientConfig);
        }

        internal string ParseAccessToken(string accessToken)
        {
            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }


        internal async Task<(string, string, string)> GetTokens(string accessToken)
        {
            Log(Authentication.DriverStatus.Grabbing_Required_Tokens);
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            // Get PAS token
            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            response.EnsureSuccessStatusCode();
            string pasToken = await response.Content.ReadAsStringAsync();


            // Get entitlement token
            httpClient.DefaultRequestHeaders.Accept.Clear();
            response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json"));
            response.EnsureSuccessStatusCode();
            string entitlementToken = (await response.Content.ReadFromJsonAsync<EntitleReturn>())?.EntitlementsToken ?? "";


            // Get client config
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", entitlementToken);
            response = await httpClient.GetAsync("https://clientconfig.rpg.riotgames.com/api/v1/config/player?app=Riot%20Client");
            response.EnsureSuccessStatusCode();
            string clientConfig = await response.Content.ReadAsStringAsync();

            return (pasToken, entitlementToken, clientConfig);
        }

        #region Old Methods (Keeping for backup/reference)

        //internal async Task<string?> GetAccessTokenOLD()
        //{
        //    Log(Authentication.DriverStatus.Grabbing_Access_Token);
        //    int retries = 0;
        //    string accessTokenUrl = string.Empty;
        //    using HttpClient httpClient = new();
        //    do
        //    {
        //        retries++;
        //        List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{DriverPort}/json");

        //        if (debugResponse is null) continue;
        //        if (debugResponse.Count == 0) continue;
        //        if (debugResponse.Any(x => x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token=")))
        //            accessTokenUrl = debugResponse.First(x => x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token=")).Url;

        //    } while (string.IsNullOrEmpty(accessTokenUrl) && retries >= 50);

        //    Regex accessTokenRegex = new("access_token=(.*?)&scope");
        //    string accessToken = accessTokenRegex.Match(accessTokenUrl).Groups[1].Value;
        //    return accessToken;
        //}

        //internal async Task<(IEnumerable<Cookie>?, string?, string?, string?, string?)> CheckCookieStatusOLDAsync() // Keep this for reference
        //{
        //    //Log(Authentication.DriverStatus.Redirecting_To_Valorant_RSO);
        //    //await DoRedirect("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");

        //    //Log(Authentication.DriverStatus.Checking_Valorant_Login_Page);
        //    //if (await IsLoginPageDetectedAsync()) await SendLoginDataAsync(username, password, false);

        //    //Log(Authentication.DriverStatus.Checking_Valorant_Multi_Factor);
        //    //if (await IsMfaRequiredAsync()) await HandleMfaAsync();

        //    Dictionary<string, object> cookieResponse = new()
        //    {
        //        { "id", ActionIdGenerator.Next() },
        //        { "method", "Network.getAllCookies" }
        //    };

        //    Log(Authentication.DriverStatus.Requesting_Cookies);

        //    string? accessToken = await GetAccessTokenOLD();
        //    string? pasToken = await GetPasToken(accessToken!);
        //    string? entitlementToken = await GetEntitlementToken(accessToken!);
        //    string? clientConfig = await GetClientConfig(accessToken!, entitlementToken!);

        //    CookieRoot? cookieRoot = await GetCookiesAsync();
        //    IEnumerable<Cookie>? riotCookies = cookieRoot?.Result.Cookies.ToList().Where(x => (x.Domain.Contains("riotgames.com") || x.Domain.Contains("valorant")) && x.Name is "tdid" or "ssid" or "sub" or "csid" or "clid" or "__Secure-access_token" or "__Secure-refresh_token" or "__Secure-id_token");

        //    if (riotCookies == null)
        //        throw new RadiantConnectAuthException("Could not find verified cookies.");

        //    WebDriver?.Kill(true); // Kill driver process

        //    Log(Authentication.DriverStatus.Cookies_Received);
        //    return (riotCookies, accessToken, pasToken, entitlementToken, clientConfig);
        //}

        #endregion

        public async Task Logout() => await Driver.NavigateTo("https://auth.riotgames.com/logout", "/logout", DriverPort, Socket);
    }
}
