﻿using System.Diagnostics;
using System.Net.WebSockets;
using System.Text.Json;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Utilities;

// ReSharper disable AccessToDisposedClosure <--- It's handled in DriverHandler

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
    internal class AuthHandler(string browserProcess, string browserExecutable, bool killBrowser, bool cacheCookies, bool headless) : IDisposable
    {
        public Authentication.DriverStatus DriverStatus
        {
            get;
            set
            {
                field = value;
                OnDriverUpdate?.Invoke(value);
            }
        }

        // Events
        public event Events.MultiFactorEvent? OnMultiFactorRequested;

        public event Events.DriverEvent? OnDriverUpdate;

        internal int DriverPort { get; } = AuthUtil.GetFreePort();
        internal Random ActionIdGenerator { get; } = new();
        internal SocketHandler SocketHandler = null!;

        internal Process? WebDriver { get; set; }

        internal ClientWebSocket? Socket { get; set; } = new();

        // User Variables
        public string? MultiFactorCode { get; set; }

        internal async Task<(string, string, string, string)> Authenticate(string username, string password)
        {
            DriverStatus = Authentication.DriverStatus.Checking_Existing_Processes;
            DriverHandler.DoDriverCheck(browserProcess, browserExecutable, killBrowser);

            DriverStatus = Authentication.DriverStatus.Creating_Driver;
            (WebDriver, string? socketUrl) = await DriverHandler.StartDriver(browserExecutable, DriverPort, headless);

            if (WebDriver == null)
                throw new RadiantConnectException("Failed to start browser driver");

            if (string.IsNullOrEmpty(socketUrl)) throw new RadiantConnectAuthException("Failed to find socket");

            Socket = new ClientWebSocket();
            await Socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);
            SocketHandler = new SocketHandler(Socket, this, DriverPort);

            Task.Run(() => DriverHandler.ListenAsync(Socket));

            await SocketHandler.InitiateRuntimeHandles(Socket, username, password);

            DriverStatus = Authentication.DriverStatus.Driver_Created;

            try
            {
                DriverStatus = Authentication.DriverStatus.Begin_SignIn;
                return await PerformSignInAsync();
            }
            catch (Exception e)
            {
                WebDriver.Kill(true);
                throw new RadiantConnectAuthException(e.Message);
            }
            finally
            {
                Dispose();
            }
        }

        public void Dispose()
        {
            Socket?.Abort();
            Socket?.Dispose();
            Process.GetProcessesByName(browserProcess).ToList().ForEach(x => x.Kill()); // Kill driver processes
        }

        internal async Task<(string, string, string, string)> PerformSignInAsync()
        {
            string accessTokenFound = string.Empty;
            DriverStatus = Authentication.DriverStatus.Logging_Into_Valorant;

            DriverHandler.OnCaptchaFound += (_) => DriverStatus = Authentication.DriverStatus.Captcha_Found;
            DriverHandler.OnCaptchaRemoved += (_) => DriverStatus = Authentication.DriverStatus.Captcha_Solved;

            DriverHandler.OnMfaDetected += async (_) =>
            {
                DriverStatus = Authentication.DriverStatus.Checking_RSO_Multi_Factor;
                await HandleMfaAsync();
            };

            DriverHandler.OnAccessTokenFound += (data) => accessTokenFound = data!;

            while (string.IsNullOrEmpty(accessTokenFound)) await Task.Delay(5);

            DriverStatus = Authentication.DriverStatus.Grabbing_Required_Tokens;
            return await GetRsoCookiesFromDriver(accessTokenFound);
        }

        internal async Task HandleMfaAsync()
        {
            DriverStatus = Authentication.DriverStatus.Multi_Factor_Requested;
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

            await SocketHandler.ExecuteOnPageWithResponse("Verification Required", DriverPort, mfaDataInput, "", Socket!);

            DriverStatus = Authentication.DriverStatus.Multi_Factor_Completed;
        }

        internal async Task<(string, string, string, string)> GetRsoCookiesFromDriver(string accessToken)
        {
            CookieRoot? getCookies = await SocketHandler.GetCookiesAsync("");

            if (cacheCookies)
            {
                Directory.CreateDirectory($@"{Path.GetTempPath()}\RadiantConnect\");
                await File.WriteAllTextAsync($@"{Path.GetTempPath()}\RadiantConnect\cookies.json", JsonSerializer.Serialize(getCookies));
            }

            Dictionary<string, string> cookieDict = getCookies?.Result.Cookies.GroupBy(c => c.Name).ToDictionary(g => g.Key, g => g.First().Value) ?? [];

            string ssid = cookieDict.GetValueOrDefault("ssid", "");
            string clid = cookieDict.GetValueOrDefault("clid", "");
            string tdid = cookieDict.GetValueOrDefault("tdid", "");
            string csid = cookieDict.GetValueOrDefault("csid", "");

            if (string.IsNullOrEmpty(ssid) || string.IsNullOrEmpty(clid) || string.IsNullOrEmpty(tdid) || string.IsNullOrEmpty(csid))
                throw new RadiantConnectAuthException("Failed to gather required cookies");

            return (ssid, clid, tdid, csid);
        }
        
        public async Task Logout() => await SocketHandler.NavigateTo("https://auth.riotgames.com/logout", "/logout", DriverPort, Socket!);
    }
}
