using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network.PVPEndpoints.DataTypes;

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
    internal class SocketHandler
    {
        public SocketHandler(ClientWebSocket socket, AuthHandler authHandler, int port)
        {
            Socket = socket;
            DriverPort = port;
            AuthHandler = authHandler;
        }

        internal static ClientWebSocket Socket = null!;
        internal static int DriverPort;
        internal static AuthHandler AuthHandler = null!;
        
        internal static Random ActionIdGenerator { get; } = new();

        #region StaticHandlers

        internal static async Task InitiateRuntimeHandles(ClientWebSocket socket, string username, string password)
        {
            int eventPassed = -1;

            DriverHandler.OnRuntimeChanged += Changed;

            Random hookRandomizer = new((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            List<Dictionary<string, object>> eventList = [];

            AddCommand("Page.enable");
            AddCommand("Runtime.enable");
            AddCommand("Network.enable");
            AddCommand("Network.setBlockedURLs", new Dictionary<string, object>
            {
                {
                    "urls", (string[])
                    [
                        "*.png",
                        "*.webp",
                        "*.css",
                        "*.ico",
                        "*.svg",
                        "*valorant.secure.dyn.riotcdn.net/*",
                        "*cdn.rgpub.io/*",
                        "*google-analytics.com/*",
                        "*googletagmanager.com/*",
                        "!*hcaptcha*"
                    ]
                }
            });

            AddCommand("Page.addScriptToEvaluateOnNewDocument", new Dictionary<string, object>
            {
                {
                    "source", BuildScript()
                }
            });

            AddCommand("Runtime.evaluate", new Dictionary<string, object>
            {
                {
                    "expression", "document.location.href = 'https://auth.riotgames.com/authorize?redirect_uri=https://playvalorant.com/opt_in&client_id=play-valorant-web-prod&response_type=token id_token&nonce=1&scope=account email profile openid link lol_region id summoner offline_access ban';"
                }
            });

            for (int eventId = 0; eventId < eventList.Count; eventId++)
            {
                await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(eventList[eventId]))), WebSocketMessageType.Text, true, CancellationToken.None);
                while (eventPassed != eventId) await Task.Delay(50);
            }

            DriverHandler.OnRuntimeChanged -= Changed;

            return;

            void Changed() => eventPassed++;

            void AddCommand(string method, Dictionary<string, object>? parameters = null)
            {
                int id = hookRandomizer.Next();
                Dictionary<string, object> dataToSend = new()
                {
                    { "id", id },
                    { "method", method }
                };
                if (parameters is not null)
                    dataToSend.Add("params", parameters);
                eventList.Add(dataToSend);
            }

            string BuildScript()
            {
                return ("""
                        (function () {
                            'use strict';
                            let signInDetected = false;
                            let mfaDetected = false;
                            let accessToken = false;
                        
                            function set(obj, callback) {
                                callback(obj);
                                for (let [k, v] of Object.entries(obj)) {
                                    if (k.includes('__reactEventHandlers') && v.onChange) {
                                        v.onChange({
                                            target: obj
                                        });
                                    }
                                }
                            }
                        
                            function doPageChecks() {
                        
                                if (document.title.includes('Sign in')) {
                                    if (document.getElementsByName('username').length > 0 && !signInDetected) {
                                        set(document.getElementsByName('username')[0], e => e.value = '%USERNAME_DATA%');
                                        set(document.getElementsByName('password')[0], e => e.value = '%PASSWORD_DATA%');
                                        setTimeout(() => {
                                            document.querySelectorAll('[data-testid=\'btn-signin-submit\']')[0].click();
                                        }, 1000)
                                        signInDetected = true;
                                    }
                                }
                        
                                if (document.title.includes('Verification Required')) {
                                    if (document.getElementsByTagName('h5')[0].innerText == 'Verification Required' && !mfaDetected) {
                                        console.log('[RADIANTCONNECT] MFA Detected');
                                        mfaDetected = true;
                                    }
                                }
                        
                                if (document.title.includes('Success')) {
                                    console.log("[RADIANTCONNECT] CAPTCHAREMOVED");
                                }
                        
                                if (document.location.href.includes('opt_in/#access_token=') && !accessToken) {
                                    console.log("[RADIANTCONNECT] CAPTCHAREMOVED");
                                    console.log(`[RADIANTCONNECT] Access Token: ${document.location.href}`);
                                    accessToken = true;
                                }
                        
                            }
                        
                            window.addEventListener('load', doPageChecks);
                        
                            let interval = setInterval(() => {
                                if (document.querySelector('title')) {
                                    new MutationObserver(mutations => mutations.forEach(m => m.type === 'childList' && doPageChecks())).observe(document.querySelector('title'), {
                                        childList: true
                                    });
                                    clearInterval(interval);
                                }
                            }, 5);
                        
                            let capInt = setInterval(() => {
                                if (document.title.includes('Sign in'))
                                {
                                    document.querySelectorAll('iframe[src*="hcaptcha"]').forEach((iframe) => {
                                        let displayer = iframe.parentNode?.parentNode;
                                        if (!displayer) return;
                                
                                        let computedStyle = window.getComputedStyle(displayer);
                                        let zIndex = computedStyle["z-index"];
                                        if (zIndex > 0) {
                                            console.log("[RADIANTCONNECT] CAPTCHAFOUND");
                                            clearInterval(capInt);
                                        }
                                    });
                                }
                            }, 150);
                        
                        })();
                        """).Replace("%PASSWORD_DATA%", password).Replace("%USERNAME_DATA%", username);
            }
        }

        internal static async Task NavigateTo(string url, string pageTitle, int port, ClientWebSocket socket, bool waitForPage = true)
        {
            Dictionary<string, object> dataToSend = new()
            {
                { "id",new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string> { {"expression", $"document.location.href = \"{url}\""} }
                }
            };

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            if (waitForPage)
                await DriverHandler.WaitForPage(pageTitle, port, socket, 999999);
        }

        internal static async Task<string?> ExecuteOnPageWithResponse(string pageTitle, int port, Dictionary<string, object> dataToSend, string expectedOutput, ClientWebSocket socket, bool output = false, bool skipCheck = false)
        {
            if (pageTitle != "") await DriverHandler.WaitForPage(pageTitle, port, socket);

            int id = (int)dataToSend["id"];
            TaskCompletionSource<string> tcs = new();
            DriverHandler.PendingRequests[id] = tcs;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            string response = await tcs.Task;

            if (output) Debug.WriteLine(response);
            if (!response.Contains(expectedOutput) && !skipCheck) throw new Exception("Expected output not found");

            return response;
        }

        #endregion
        
        internal async Task<CookieRoot?> GetCookiesAsync(string pageTitle)
        {
            Dictionary<string, object> cookieResponse = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Network.getAllCookies" }
            };

            string? cookieData = await ExecuteOnPageWithResponse(pageTitle, DriverPort, cookieResponse, "", Socket, false, true);
            return JsonSerializer.Deserialize<CookieRoot>(cookieData!);
        }

        internal async Task SetCookieCacheAsync()
        {
            string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";
            if (!File.Exists(cacheFile)) return;

            CookieRoot? cookieRoot = JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile));

            IReadOnlyList<Cookie>? cookiesData = cookieRoot?.Result.Cookies;

            List<object> cookieActual = [];
            cookieActual.AddRange(cookiesData!.Select(cookie => new Dictionary<string, object>()
            {
                { "name", cookie.Name },
                { "value", cookie.Value },
                { "domain", cookie.Domain },
                { "path", cookie.Path },
                { "expires", cookie.Expires ?? 0.0 },
                { "httpOnly", cookie.HttpOnly ?? true },
                { "secure", cookie.Secure ?? true }
            }));
            
            Dictionary<string, object> setCookiesRequest = new()
            {
                { "id", ActionIdGenerator.Next() },
                { "method", "Network.setCookies" },
                { "params", new { cookies = cookieActual } }
            };

            await Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(setCookiesRequest))), WebSocketMessageType.Text, true, CancellationToken.None);
        }

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

    }
}
