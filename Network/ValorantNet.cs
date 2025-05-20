using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Services;
using RadiantConnect.Utilities;

namespace RadiantConnect.Network
{
    public class ValorantNet
    {
        public delegate void ValorantNetLog(string message);
        public event ValorantNetLog? OnLog;

        public RSOAuth? AuthCodes { get; set; }

        internal HttpClient Client = AuthUtil.BuildClient().Item1;

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

        internal string DefaultPlatform;
        internal string DefaultUserAgent;
        internal string DefaultClientVersion;

        internal void ResetDefaultHeaders()
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", DefaultPlatform);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", DefaultUserAgent);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", DefaultClientVersion);
        }

        public ValorantNet(RSOAuth rsoAuth)
        {
            AuthCodes = rsoAuth;
            Client.Timeout = TimeSpan.FromSeconds(value: 10);

            using HttpClient client = AuthUtil.BuildClient().Item1;
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

            DefaultPlatform = valorantClient.UserPlatform;
            DefaultUserAgent = $"ShooterGame/{valorantClient.BuildVersion} Windows/{valorantClient.UserClientVersion}";
            DefaultClientVersion = valorantClient.RiotClientVersion;

            ResetDefaultHeaders();
        }

        [Obsolete("Please use AuthenticateWithSSID, method is no longer maintained,", true)]
        public ValorantNet([SuppressMessage("ReSharper", "UnusedParameter.Local")] string _) => throw new NotSupportedException("Please use RSOAuth, method is no longer maintained,");

        public ValorantNet(ValorantService? valorantClient = null)
        {
            Client.Timeout = TimeSpan.FromSeconds(10);
            DefaultPlatform = valorantClient?.ValorantClientVersion.UserPlatform ?? "";
            DefaultUserAgent = $"ShooterGame/{valorantClient?.ValorantClientVersion.BuildVersion} {valorantClient?.ValorantClientVersion.UserClientVersion}";
            DefaultClientVersion = valorantClient?.ValorantClientVersion.RiotClientVersion ?? "";

            ResetDefaultHeaders();
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


        internal void SetCustomHeaders(Dictionary<string, string> headers)
        {
            Client.DefaultRequestHeaders.Clear();

            foreach ((string key, string value) in headers)
                Client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        public async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null, bool outputDebugData = false, Dictionary<string, string>? customHeaders = null)
        {
            if (string.IsNullOrEmpty(baseUrl)) return string.Empty;

            if (baseUrl[^1] == '/' && endPoint[0] == '/') { endPoint = endPoint[1..]; } // Make sure the slash isn't duplicated
            if (baseUrl[^1] != '/' && endPoint[0] != '/') baseUrl += "/"; // Make sure it actually contains a slash
            
            while (InternalValorantMethods.IsValorantProcessRunning() || AuthCodes is not null)
            {
                if (baseUrl.Contains("127.0.0.1") && Client.DefaultRequestHeaders.Authorization?.Scheme != "Basic") await SetBasicAuth();
                else if (customHeaders is not null) SetCustomHeaders(customHeaders);
                else await ResetAuth();

                HttpRequestMessage httpRequest = new();
                httpRequest.Method = InternalToHttpMethod[httpMethod];
                httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
                httpRequest.Content = content;

                HttpResponseMessage responseMessage = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                string responseContent = await responseMessage.Content.ReadAsStringAsync();

                if (customHeaders is not null)
                    ResetDefaultHeaders();

                Debug.WriteLine($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

                if (outputDebugData)
                    Console.WriteLine($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

                OnLog?.Invoke($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

                if (!responseMessage.IsSuccessStatusCode)
                    throw new RadiantConnectNetworkStatusException($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

                httpRequest.Dispose();
                responseMessage.Dispose();

                return responseContent;
            }

            return string.Empty;
        }

        public async Task GetAsync(string baseUrl, string endPoint) => await CreateRequest(HttpMethod.Get, baseUrl, endPoint);
        public async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
        {
            string? jsonData = await CreateRequest(HttpMethod.Get, baseUrl, endPoint);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task PostAsync(string baseUrl, string endPoint, HttpContent? httpContent = null) => await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent);
        public async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
        {
            string? jsonData = await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task PutAsync(string baseUrl, string endPoint, HttpContent httpContent) => await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent);
        public async Task<T?> PutAsync<T>(string baseUrl, string endPoint, HttpContent httpContent)
        {
            string? jsonData = await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent);

            if (endPoint == "name-service/v2/players") // I forget why this is here, but I'm too scared to remove it
            {
                jsonData = jsonData?.Trim();
                jsonData = jsonData?[1..^1];
            }

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task DeleteAsync(string baseUrl, string endPoint) => await CreateRequest(HttpMethod.Delete, baseUrl, endPoint);
        public async Task<T?> DeleteAsync<T>(string baseUrl, string endPoint)
        {
            string? jsonData = await CreateRequest(HttpMethod.Delete, baseUrl, endPoint);
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task PatchAsync(string baseUrl, string endPoint, HttpContent httpContent) => await CreateRequest(HttpMethod.Patch, baseUrl, endPoint, httpContent);
        public async Task<T?> PatchAsync<T>(string baseUrl, string endPoint, HttpContent httpContent)
        {
            string? jsonData = await CreateRequest(HttpMethod.Patch, baseUrl, endPoint, httpContent);
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task OptionsAsync(string baseUrl, string endPoint) => await CreateRequest(HttpMethod.Options, baseUrl, endPoint);
        public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint)
        {
            string? jsonData = await CreateRequest(HttpMethod.Options, baseUrl, endPoint);
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task HeadAsync(string baseUrl, string endPoint) => await CreateRequest(HttpMethod.Head, baseUrl, endPoint);
        public async Task<T?> HeadAsync<T>(string baseUrl, string endPoint)
        {
            string? jsonData = await CreateRequest(HttpMethod.Head, baseUrl, endPoint);
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        public async Task OptionsAsync(string baseUrl, string endPoint, HttpContent? httpContent) => await CreateRequest(HttpMethod.Options, baseUrl, endPoint, httpContent);
        public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent)
        {
            string? jsonData = await CreateRequest(HttpMethod.Options, baseUrl, endPoint, httpContent);
            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        #region  Records
        public record UserAuth(int AuthorizationPort, string OAuth);

        internal record Entitlement(
            [property: JsonPropertyName("accessToken")] string AccessToken,
            [property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
            [property: JsonPropertyName("issuer")] string Issuer,
            [property: JsonPropertyName("subject")] string Subject,
            [property: JsonPropertyName("token")] string Token
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