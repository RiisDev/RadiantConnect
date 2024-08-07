using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
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

        internal static async Task InitiatePageEvents(ClientWebSocket socket)
        {
            Dictionary<string, object> dataToSend = new()
            {
                { "id",new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Next() },
                { "method", "Page.enable" }
            };

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);
        }

        internal static async Task NavigateTo(string url, string pageTitle, int port, ClientWebSocket socket)
        {
            Dictionary<string, object> dataToSend = new()
            {
                { "id",new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string> { {"expression", $"document.location.href = \"{url}\""} }
                }
            };

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

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

        internal async Task SendLoginDataAsync(string username, string password, bool rso = true)
        {
            AuthHandler.Log(rso ? Authentication.DriverStatus.Logging_Into_RSO : Authentication.DriverStatus.Logging_Into_Valorant);

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
            await ExecuteOnPageWithResponse("Sign in", DriverPort, loginData, "result\":{\"type\":\"undefined\"}", Socket);
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

            string? response = await ExecuteOnPageWithResponse("Sign in", DriverPort, faData, "", Socket, false, true);

            if (response is not null && response.Contains("result\":{\"type\":\"boolean\",\"value\":true}"))
            {
                throw new RadiantConnectAuthException("Error occurred during login");
            }
        }

        internal async Task<bool> IsMfaRequiredAsync()
        {
            bool mfaDetected = await DriverHandler.PageExists("Verification Required", DriverPort);
            bool signInNotChanged = await DriverHandler.PageExists("Sign in", DriverPort);

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

            await ExecuteOnPageWithResponse("Verification Required", DriverPort, faData, "result\":{\"type\":\"boolean\",\"value\":true}", Socket);

            return true;
        }

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
                { "expires", cookie.Expires },
                { "httpOnly", cookie.HttpOnly },
                { "secure", cookie.Secure }
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

            string? response = await ExecuteOnPageWithResponse("Sign in", DriverPort, initialSignInCheck, "result\":{\"type\":\"boolean\",\"value\":true}", Socket, true);
            return response is not null && response.Contains("true");
        }
    }
}
