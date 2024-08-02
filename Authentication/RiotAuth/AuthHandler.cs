﻿using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

// ReSharper disable StringLiteralTypo

namespace RadiantConnect.Authentication.RiotAuth
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

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?)> Initialize(string username, string password)
        {
            Log(Authentication.DriverStatus.Checking_Existing_Processes);
            DoDriverCheck();

            Log(Authentication.DriverStatus.Creating_Driver);
            WebDriver = await Driver.StartDriver(browserExecutable, DriverPort);

            if (WebDriver == null)
                throw new RadiantConnectException("Failed to start browser driver");

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

        #region DriverAuthFlow

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?)> PerformSignInAsync(string username, string password)
        {
            Log(Authentication.DriverStatus.Checking_Existing_Auth);
            if (!string.IsNullOrEmpty(await Driver.GetSocketUrl("Riot Account Management", DriverPort)))
                return await CheckCookieStatusAsync();

            Log(Authentication.DriverStatus.Checking_For_Login_Page);
            if (await IsLoginPageDetectedAsync()) await SendLoginDataAsync(username, password);


            Log(Authentication.DriverStatus.Checking_For_Multi_Factor);
            if (await IsMfaRequiredAsync()) await HandleMfaAsync();


            Log(Authentication.DriverStatus.SignIn_Completed);
            return await CheckCookieStatusAsync();
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

            string? response = await Driver.ExecuteOnPageWithResponse("Sign in", DriverPort, initialSignInCheck, "result\":{\"type\":\"boolean\",\"value\":true}");

            return response is not null && response.Contains("true");
        }

        internal async Task SendLoginDataAsync(string username, string password)
        {
            Log(Authentication.DriverStatus.Logging_In);
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
            await Driver.ExecuteOnPageWithResponse("Sign in", DriverPort, loginData, "result\":{\"type\":\"undefined\"}");
        }

        internal async Task<string?> GetAccessToken()
        {
            Log(Authentication.DriverStatus.Grabbing_Access_Token);
            int retries = 0;
            string accessTokenUrl = string.Empty;
            using HttpClient httpClient = new();
            do
            {
                retries++;
                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{DriverPort}/json");

                if (debugResponse is null) continue;
                if (debugResponse.Count == 0) continue;
                if (debugResponse.Any(x=> x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token="))) 
                    accessTokenUrl = debugResponse.First(x => x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token=")).Url;

            } while (string.IsNullOrEmpty(accessTokenUrl) && retries >= 50);

            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            string accessToken = accessTokenRegex.Match(accessTokenUrl).Groups[1].Value;
            return accessToken;
        }

        internal async Task<string?> GetPasToken(string accessToken)
        {
            Log(Authentication.DriverStatus.Grabbing_PAS_Token);
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            return await response.Content.ReadAsStringAsync();
        }

        public record EntitleReturn(
            [property: JsonPropertyName("entitlements_token")] string EntitlementsToken
        );

        internal async Task<string?> GetEntitlementToken(string accessToken)
        {
            Log(Authentication.DriverStatus.Grabbing_PAS_Token);
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {accessToken}");
            HttpResponseMessage response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json"));
            EntitleReturn? entitleReturn = await response.Content.ReadFromJsonAsync<EntitleReturn>();
            return entitleReturn?.EntitlementsToken;
        }

        internal async Task<bool> IsMfaRequiredAsync()
        {
            string mfaCheck = await Driver.GetSocketUrl("Verification Required", DriverPort);

            if (string.IsNullOrEmpty(mfaCheck)) return false;

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

            await Driver.ExecuteOnPageWithResponse("Verification Required", DriverPort, faData, "result\":{\"type\":\"boolean\",\"value\":true}");

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

            await Driver.ExecuteOnPageWithResponse("Verification Required", DriverPort, mfaDataInput, "");

            Log(Authentication.DriverStatus.Multi_Factor_Completed);
        }

        internal async Task<(IEnumerable<Cookie>?, string?, string?, string?)> CheckCookieStatusAsync()
        {
            Dictionary<string, object> cookieResponse = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Network.getAllCookies" }
            };

            Log(Authentication.DriverStatus.Requesting_Cookies);

            string? accessToken = await GetAccessToken();
            string? pasToken = await GetPasToken(accessToken!);
            string? entitlementToken = await GetEntitlementToken(accessToken!);
            string? cookieData = await Driver.ExecuteOnPageWithResponse("", DriverPort, cookieResponse, "", false, true, true);
            CookieRoot? cookieRoot = JsonSerializer.Deserialize<CookieRoot>(cookieData!);
            IEnumerable<Cookie>? riotCookies = cookieRoot?.Result.Cookies.ToList().Where(x =>(x.Domain.Contains("riotgames.com") || x.Domain.Contains("valorant")) && x.Name is "tdid" or "ssid" or "sub" or "csid" or "clid" or "__Secure-access_token" or "__Secure-refresh_token" or "__Secure-id_token");
            
            if (riotCookies == null)
                throw new RadiantConnectAuthException("Could not find verified cookies.");

            WebDriver?.Kill(true); // Kill driver process

            Log(Authentication.DriverStatus.Cookies_Received);
            return (riotCookies, accessToken, pasToken, entitlementToken);
        }


        #endregion

        public async Task Logout() => await Driver.NavigateTo("https://auth.riotgames.com/logout", DriverPort);
    }
}