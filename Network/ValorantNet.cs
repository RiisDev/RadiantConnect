using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Methods;
using RadiantConnect.Services;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

namespace RadiantConnect.Network
{
    public class ValorantNet
    {
        public RSOAuth? AuthCodes { get; set; }

        internal HttpClient Client = new(new HttpClientHandler { ServerCertificateCustomValidationCallback = (_, _, _, _) => true });

        public static int? GetAuthPort(){return GetAuth()?.AuthorizationPort;}

        internal static Dictionary<HttpMethod, System.Net.Http.HttpMethod> InternalToHttpMethod = new()
        {
            { HttpMethod.Get, System.Net.Http.HttpMethod.Get },
            { HttpMethod.Post, System.Net.Http.HttpMethod.Post },
            { HttpMethod.Put, System.Net.Http.HttpMethod.Put },
            { HttpMethod.Delete, System.Net.Http.HttpMethod.Delete },
            { HttpMethod.Patch, System.Net.Http.HttpMethod.Patch },
            { HttpMethod.Options, System.Net.Http.HttpMethod.Options },
            { HttpMethod.Head, System.Net.Http.HttpMethod.Head },
        };

        public enum HttpMethod
        {
            Get,
            Post, 
            Put, 
            Delete,
            Patch,
            Options,
            Head
        }

        public ValorantNet(RSOAuth rsoAuth)
        {
            AuthCodes = rsoAuth;
            Client.Timeout = TimeSpan.FromSeconds(value: 10);

            using HttpClient client = new();
            ValorantVersionApiRoot apiData = client.GetFromJsonAsync<ValorantVersionApiRoot>(requestUri: "https://valorant-api.com/v1/version").Result!;

            ValorantService.Version valorantClient = new (
                RiotClientVersion: apiData.Data.RiotClientVersion.Replace("-shipping", ""), 
                Branch: "live", 
                BuildVersion: apiData.Data.BuildVersion, 
                Changelist: apiData.Data.ManifestId, 
                EngineVersion: apiData.Data.EngineVersion, 
                VanguardVersion: "0.0.0",
                UserClientVersion: "10.0.19042.1.256.64bit",
                UserPlatform: "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9"
            );

            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", valorantClient?.UserPlatform);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", value: $"ShooterGame/{valorantClient?.BuildVersion} {valorantClient?.UserClientVersion}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", valorantClient?.RiotClientVersion);

        }

        public ValorantNet(string ssid) : this(BuildRSOFromSsid(ssid).Result) {}

        public ValorantNet(ValorantService? valorantClient = null)
        {
            Client.Timeout = TimeSpan.FromSeconds(10);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", valorantClient?.ValorantClientVersion.UserPlatform);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", $"ShooterGame/{valorantClient?.ValorantClientVersion.BuildVersion} {valorantClient?.ValorantClientVersion.UserClientVersion}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion",valorantClient?.ValorantClientVersion.RiotClientVersion);
        }

        internal static UserAuth? GetAuth()
        {
            string lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "Riot Client", "Config", "lockfile");
            string? fileText;
            try
            {
                File.Copy(lockFile, $"{lockFile}.tmp", true);
                fileText = File.ReadAllText($"{lockFile}.tmp");
            }
            finally
            {
                File.Delete($"{lockFile}.tmp");
            }

            string[] fileValues = fileText.Split(':');

            if (fileValues.Length < 3) return null;

            int authPort = int.Parse(fileValues[2]);
            string oAuth = fileValues[3];

            return new UserAuth(authPort, oAuth);
        }

        internal async Task<(string, string)> GetAuthorizationToken()
        {
            if (AuthCodes is not null) return (AuthCodes.AccessToken, AuthCodes.Entitlement)!;

            UserAuth? auth = GetAuth();
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{auth?.OAuth}".ToBase64()}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");

            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}");

            Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(response.Content.ReadAsStringAsync().Result);

            return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
        }
       
        internal async Task ResetAuth()
        {
            try
            {
                Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
                Client.DefaultRequestHeaders.Remove("Authorization");
            }
            catch {/**/}

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return;

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);
        }

        internal async Task SetBasicAuth()
        {
            if (AuthCodes is not null)
                throw new RadiantConnectException("Cannot use local endpoints with AuthCodes");

            Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            Client.DefaultRequestHeaders.Remove("Authorization");

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return;

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{GetAuth()?.OAuth}".ToBase64()}");
        }

        internal async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null)
        {
            // Stupid hack to fix URLs that I imported incorrectly
            if (baseUrl[^1] == '/' && endPoint[0] == '/') { endPoint = endPoint[1..]; }

            if (string.IsNullOrEmpty(baseUrl)) return string.Empty;
            try
            {
                while (InternalValorantMethods.IsValorantProcessRunning() || AuthCodes is not null)
                {
                    if (baseUrl.Contains("127.0.0.1") && Client.DefaultRequestHeaders.Authorization?.Scheme != "Basic")
                        await SetBasicAuth();
                    else await ResetAuth();

                    HttpRequestMessage httpRequest = new();
                    httpRequest.Method = InternalToHttpMethod[httpMethod];
                    httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
                    httpRequest.Content = content;

                    HttpResponseMessage responseMessage = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                    switch (responseMessage)
                    {
                        case { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.InternalServerError }:
                        case { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }:
                            await ResetAuth();
                            continue;
                        case { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.NotFound }:
                        case { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.MethodNotAllowed }:
                            return null;
                    }

                    string responseContent = await responseMessage.Content.ReadAsStringAsync();
                    
                    Debug.WriteLine($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}[ValorantNet Log] Response Data: {responseMessage}");
                    
                    httpRequest.Dispose();
                    responseMessage.Dispose();
                    return responseContent.Contains("<html>") || responseContent.Contains("errorCode") ? null : responseContent;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        public async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
        {
            try
            {
                string? jsonData = await CreateRequest(HttpMethod.Get, baseUrl, endPoint);

                return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
            }
            catch
            {
                return default;
            }
        }

        public async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
        {
            string? jsonData = await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task<T?> PutAsync<T>(string baseUrl, string endPoint,HttpContent httpContent)
        {
            string? jsonData = await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent);
            if (endPoint == "name-service/v2/players")
            {
                jsonData = jsonData?.Trim();
                jsonData = jsonData?[1..^1];
            }
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        #region SSIDParse

        internal static string ParseAccessToken(string accessToken)
        {
            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }
        internal static string ParseIdToken(string accessToken)
        {
            Regex accessTokenRegex = new("id_token=(.*?)&token_type");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal static async Task<(string, string, string, string)> GetTokensFromSsid(string ssid)
        {
            CookieContainer container = new();
            using HttpClient httpClient = new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
                CookieContainer = container
            });

            container.Add(new System.Net.Cookie("ssid", ssid, "/", "auth.riotgames.com"));

            HttpResponseMessage response = await httpClient.GetAsync("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");
            string redirectRequest = response.RequestMessage?.RequestUri?.ToString() ?? "";

            if (string.IsNullOrEmpty(redirectRequest))
                throw new RadiantConnectAuthException("Failed to redirect to Valorant RSO");
            if (!redirectRequest.Contains("access_token"))
                throw new RadiantConnectAuthException("Failed to get Valorant RSO (Access_Token)");

            string accessToken = ParseAccessToken(redirectRequest);
            string idToken = ParseIdToken(redirectRequest);

            httpClient.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
            response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json"));
            string entitlementToken = (await response.Content.ReadFromJsonAsync<EntitleReturn>())?.EntitlementsToken ?? "";

            string cacheFile = $@"{Path.GetTempPath()}\RadiantConnect\cookies.json";

            if (File.Exists(cacheFile))
            {
                CookieCollection clientCookies = container.GetCookies(new Uri("https://auth.riotgames.com"));

                List<HttpClientCookie>? cookeRoots = JsonSerializer.Deserialize<List<HttpClientCookie>>(JsonSerializer.Serialize(clientCookies));
                CookieRoot? cookieRoot = JsonSerializer.Deserialize<CookieRoot>(await File.ReadAllTextAsync(cacheFile));

                List<Cookie>? cookies = cookieRoot?.Result.Cookies.ToList();

                foreach (HttpClientCookie cookie in cookeRoots!)
                {
                    if (cookies!.All(x => x.Name != cookie.Name)) continue;

                    Cookie newCookie = cookies!.First(x => x.Name == cookie.Name);
                    cookies![cookies.IndexOf(newCookie)] = newCookie with { Value = cookie.Value };
                }

                CookieRoot newCookieRoot = cookieRoot! with { Result = new Result(cookies!) };

                await File.WriteAllTextAsync(cacheFile, JsonSerializer.Serialize(newCookieRoot));
            }

            return (accessToken, entitlementToken, idToken, await GetPasToken(accessToken));
        }

        internal static async Task<string> GetPasToken(string accessToken)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            return await response.Content.ReadAsStringAsync();
        }

        internal static async Task<RSOAuth> BuildRSOFromSsid(string ssid)
        {
            (string accessToken, string entitlementToken, string idToken, string pasToken) = await GetTokensFromSsid(ssid);

            JsonWebToken jwt = new(pasToken);
            string? affinity = jwt.GetPayloadValue<string>("affinity");
            string? chatAffinity = jwt.GetPayloadValue<string>("desired.affinity");
            string? subject = new JsonWebToken(accessToken).GetPayloadValue<string>("sub");

            return new RSOAuth(
                subject,
                ssid,
                null,
                null,
                null,
                accessToken,
                pasToken,
                entitlementToken,
                affinity,
                chatAffinity,
                null,
                null,
                idToken
            );
        }

        #endregion
        #region  Records
        public record UserAuth(int AuthorizationPort, string OAuth);

        internal record Entitlement(
            [property: JsonPropertyName("accessToken")] string AccessToken,
            [property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
            [property: JsonPropertyName("issuer")] string Issuer,
            [property: JsonPropertyName("subject")] string Subject,
            [property: JsonPropertyName("token")] string Token
        );

        internal record HttpClientCookie(
            [property: JsonPropertyName("Comment")] string Comment,
            [property: JsonPropertyName("CommentUri")] object CommentUri,
            [property: JsonPropertyName("HttpOnly")] bool? HttpOnly,
            [property: JsonPropertyName("Discard")] bool? Discard,
            [property: JsonPropertyName("Domain")] string Domain,
            [property: JsonPropertyName("Expired")] bool? Expired,
            [property: JsonPropertyName("Expires")] DateTime? Expires,
            [property: JsonPropertyName("Name")] string Name,
            [property: JsonPropertyName("Path")] string Path,
            [property: JsonPropertyName("Port")] string Port,
            [property: JsonPropertyName("Secure")] bool? Secure,
            [property: JsonPropertyName("TimeStamp")] DateTime? TimeStamp,
            [property: JsonPropertyName("Value")] string Value,
            [property: JsonPropertyName("Version")] int? Version
        );

        public record ValorantVersionApi(
            [property: JsonPropertyName("manifestId")] string ManifestId,
            [property: JsonPropertyName("branch")] string Branch,
            [property: JsonPropertyName("version")] string Version,
            [property: JsonPropertyName("buildVersion")] string BuildVersion,
            [property: JsonPropertyName("engineVersion")] string EngineVersion,
            [property: JsonPropertyName("riotClientVersion")] string RiotClientVersion,
            [property: JsonPropertyName("riotClientBuild")] string RiotClientBuild,
            [property: JsonPropertyName("buildDate")] DateTime? BuildDate
        );

        public record ValorantVersionApiRoot(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] ValorantVersionApi Data
        );

        #endregion

    }
}