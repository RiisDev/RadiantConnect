using System.Net.WebSockets;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

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

            // Clear old riot cookies before proceeding.
            AddCommand("Network.setCookies", new Dictionary<string, object>
            {
                {
                    "cookies", new object[]
                    {
                        BuildCookie("clid", "", "auth.riotgames.com"),
                        BuildCookie("ssid", "", "auth.riotgames.com"),
                        BuildCookie("tdid", "", "auth.riotgames.com"),
                        BuildCookie("csid", "", "auth.riotgames.com"),
                        BuildCookie("sub", "", "auth.riotgames.com"),
                        BuildCookie("authenticator.sid", "", "authenticate.riotgames.com"),
                        BuildCookie("_dd_s", "", "authenticate.riotgames.com"),
                        BuildCookie("__Secure-id_token", "", ".playvalorant.com"),
                        BuildCookie("__Secure-id_hint", "", ".playvalorant.com"),
                        BuildCookie("__Secure-refresh_token", "", "xsso.riotgames.com"),
                        BuildCookie("__cflb", "", "xsso.riotgames.com"),
                    }
                }
            });
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

            Dictionary<string, object> BuildCookie(string name, string value, string domain) => new()
            {
                { "name", name },
                { "value", value },
                { "domain", domain },
                { "path", "/" },
                { "secure", true },
                { "httpOnly", true },
                { "expires", DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 3600 }
            };

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

            string BuildScript() =>
	            ("""
	             (function () {
	             	'use strict';
	             	let signInDetected = false;
	             	let mfaDetected = false;
	             	let accessToken = false;
	             	let captchaFound = false;
	             
	             	// This should be cross localization from my testing
	             	function signInPageDetected() {
	             		return document.getElementById('rememberme') !== null && document.getElementsByName('username').length > 0 && document.getElementsByName('password').length > 0;
	             	}
	             
	             	// Instead of reading page title, check if 6 'single slot' numeric boxes exist
	             	function mfaPageDetected() {
	             		return document.querySelectorAll(`input[minlength='1'][maxlength='1'][inputmode='numeric'][value='']`).length === 6;
	             	}
	             
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
	             
	             		if (signInPageDetected() && !signInDetected) {
	             			set(document.getElementsByName('username')[0], e => e.value = '%USERNAME_DATA%');
	             			set(document.getElementsByName('password')[0], e => e.value = '%PASSWORD_DATA%');
	             			setTimeout(() => {
	             				document.querySelectorAll('[data-testid=\'btn-signin-submit\']')[0].click();
	             			}, 1500)
	             			signInDetected = true;
	             		}
	             
	             		if (mfaPageDetected() && !mfaDetected) {
	             			console.log('[RADIANTCONNECT] MFA Detected');
	             			mfaDetected = true;
	             		}
	             
	             		if (document.location.href.includes('opt_in/#access_token=') && !accessToken) {
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
	             		if (signInPageDetected()) {
	             			document.querySelectorAll('iframe[src*="hcaptcha"]').forEach((iframe) => {
	             				let displayer = iframe.parentNode?.parentNode;
	             				if (!displayer) return;
	             
	             				let computedStyle = window.getComputedStyle(displayer);
	             				let zIndex = computedStyle["z-index"];
	             				if (zIndex > 0 && !captchaFound) {
	             					console.log("[RADIANTCONNECT] CAPTCHAFOUND");
	             					captchaFound = true;
	             				}
	             				else if (zIndex <= 0 && captchaFound){
	             					console.log("[RADIANTCONNECT] CAPTCHAREMOVED");
	             					clearInterval(capInt);
	             				}
	             			});
	             		}
	             	}, 150);

	             })();
	             """).Replace("%PASSWORD_DATA%", password).Replace("%USERNAME_DATA%", username);
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
                await DriverHandler.WaitForPage(pageTitle, port, 999999);
        }

        internal static async Task<string?> ExecuteOnPageWithResponse(string pageTitle, int port, Dictionary<string, object> dataToSend, string expectedOutput, ClientWebSocket socket, bool output = false, bool skipCheck = false)
        {
            if (pageTitle != "") await DriverHandler.WaitForPage(pageTitle, port);

            int id = (int)dataToSend["id"];
            TaskCompletionSource<string> tcs = new();
            DriverHandler.PendingRequests[id] = tcs;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            string response = await tcs.Task;

            if (output) Debug.WriteLine(response);

            return !response.Contains(expectedOutput) && !skipCheck
	            ? throw new Exception("Expected output not found")
	            : response;
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
    }
}
