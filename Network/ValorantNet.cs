using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Methods;
using RadiantConnect.Services;
using Cookie = RadiantConnect.Authentication.DriverRiotAuth.Records.Cookie;

namespace RadiantConnect.Network
{
    public class SuppliedAuth(string jsonWebToken, string authorization)
    {
        public string JsonWebToken { get; set; } = jsonWebToken;
        public string Authorization { get; set; } = authorization;

        public void SetJsonWebToken(string jsonWebToken) => JsonWebToken = jsonWebToken;
        public void SetAuthorization(string jsonWebToken) => Authorization = jsonWebToken;
    }

    public record UserAuth(int AuthorizationPort, string OAuth);

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

    public class ValorantNet
    {
        public RadiantConnectRSO? SuppliedAuth { get; set; }

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

        public ValorantNet(ValorantService? valorantClient = null, RadiantConnectRSO? suppliedAuth = null)
        {
            SuppliedAuth = suppliedAuth;
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

        internal readonly Regex AccessTokenRegex = new("access_token=(.*?)&scope", RegexOptions.Compiled);
        internal string ParseAccessToken(string accessToken)
        {
            return AccessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal async Task<(string, string)> GetTokensFromSsid()
        {
            CookieContainer container = new();
            using HttpClient httpClient = new(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
                AllowAutoRedirect = true,
                CookieContainer = container
            });

            container.Add(new System.Net.Cookie("ssid", SuppliedAuth?.SSID, "/", "auth.riotgames.com"));

            HttpResponseMessage response = await httpClient.GetAsync("https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid");
            string redirectRequest = response.RequestMessage?.RequestUri?.ToString() ?? "";

            if (string.IsNullOrEmpty(redirectRequest))
                throw new RadiantConnectAuthException("Failed to redirect to Valorant RSO");
            if (!redirectRequest.Contains("access_token"))
                throw new RadiantConnectAuthException("Failed to get Valorant RSO (Access_Token)");

            string accessToken = ParseAccessToken(redirectRequest);

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


            return (accessToken, entitlementToken);
        }

        internal async Task<(string, string)> GetAuthorizationToken()
        {
            if (SuppliedAuth is not null) return await GetTokensFromSsid();

            UserAuth? auth = GetAuth();
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{auth?.OAuth}".ToBase64()}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");

            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}");

            InternalRecords.Entitlement? entitlement = JsonSerializer.Deserialize<InternalRecords.Entitlement>(response.Content.ReadAsStringAsync().Result);

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
            if (SuppliedAuth is not null)
                throw new RadiantConnectException("Cannot use local endpoints with SuppliedAuth");

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
                while (InternalValorantMethods.IsValorantProcessRunning() || SuppliedAuth is not null)
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
    }

    public class InternalRecords
    {
        internal record Entitlement(
            [property: JsonPropertyName("accessToken")] string AccessToken,
            [property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
            [property: JsonPropertyName("issuer")] string Issuer,
            [property: JsonPropertyName("subject")] string Subject,
            [property: JsonPropertyName("token")] string Token
        );
    }
}