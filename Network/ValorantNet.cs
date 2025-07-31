using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Services;
using RadiantConnect.Utilities;

namespace RadiantConnect.Network
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class ValorantNet
    {
        public delegate void ValorantNetLog(string message);
        public event ValorantNetLog? OnLog;

        public RSOAuth? AuthCodes { get; set; }

        internal HttpClient Client = AuthUtil.BuildClient().Item1;

        public static int? GetAuthPort(){return GetAuth()?.AuthorizationPort;}

        private static System.Net.Http.HttpMethod MapHttpMethod(HttpMethod method) => method switch
        {
            HttpMethod.Get => System.Net.Http.HttpMethod.Get,
            HttpMethod.Post => System.Net.Http.HttpMethod.Post,
            HttpMethod.Put => System.Net.Http.HttpMethod.Put,
            HttpMethod.Delete => System.Net.Http.HttpMethod.Delete,
            HttpMethod.Patch => System.Net.Http.HttpMethod.Patch,
            HttpMethod.Options => System.Net.Http.HttpMethod.Options,
            HttpMethod.Head => System.Net.Http.HttpMethod.Head,
            _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
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

            ValorantVersionApiRoot apiData = InternalHttp.GetAsync<ValorantVersionApiRoot>("https://valorant-api.com", "/v1/version").Result!;

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
                try{File.Copy(lockFile, $"{lockFile}.tmp", true);}catch{}
                fileText = File.ReadAllText($"{lockFile}.tmp");
            }
            finally
            {
                try {File.Delete($"{lockFile}.tmp");}catch{}
            }

            string[] fileValues = fileText.Split(':');

            if (fileValues.Length < 3) return null;

            int authPort = int.Parse(fileValues[2]);
            string oAuth = fileValues[3];
            return new UserAuth(authPort, oAuth);
        }

        internal async Task<(string, string)> GetAuthorizationToken()
        {
            OnLog?.Invoke("[ValorantNet Log] Getting local AuthorizationTokens");
            if (AuthCodes is not null) return (AuthCodes.AccessToken, AuthCodes.Entitlement)!;

            UserAuth? auth = GetAuth();
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{auth?.OAuth}".ToBase64()}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");
            
            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {await response.Content.ReadAsStringAsync()}");

            Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(response.Content.ReadAsStringAsync().Result);
            OnLog?.Invoke($"[ValorantNet GetAuthorizationToken Log] AccessToken: {entitlement?.AccessToken}\n[ValorantNet GetAuthorizationToken Log] EntitlementToken: {entitlement?.Token}");
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
            OnLog?.Invoke("[ValorantNet Log] Settings Basic Auth");
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

        public async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null, Dictionary<string, string>? customHeaders = null)
        {
            if (string.IsNullOrEmpty(baseUrl)) return string.Empty;

            if (!string.IsNullOrEmpty(endPoint))
            {
                if (baseUrl[^1] == '/' && endPoint[0] == '/') { endPoint = endPoint[1..]; } // Make sure the slash isn't duplicated
                if (baseUrl[^1] != '/' && endPoint[0] != '/') baseUrl += "/"; // Make sure it actually contains a slash
            }

            // I no longer need the loop, as the client will now handle edge-cases.
            if (!InternalValorantMethods.IsValorantProcessRunning() && AuthCodes is null) return string.Empty;

            if (baseUrl.Contains("127.0.0.1") && Client.DefaultRequestHeaders.Authorization?.Scheme != "Basic") await SetBasicAuth();
            else if (customHeaders is not null) SetCustomHeaders(customHeaders);
            else if (!baseUrl.Contains("127.0.0.1")) await ResetAuth();

            using HttpRequestMessage httpRequest = new();
            httpRequest.Method = MapHttpMethod(httpMethod);
            httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
            httpRequest.Content = content;

            using HttpResponseMessage responseMessage = await Client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

            string responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (customHeaders is not null)
                ResetDefaultHeaders();

            OnLog?.Invoke($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

            if (!responseMessage.IsSuccessStatusCode)
                throw new RadiantConnectNetworkStatusException($"\n[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");
                
            return responseContent;

        }

        public async Task<T?> GetAsync<T>(string baseUrl, string endPoint) 
            => await SendAndConvertAsync<T>(HttpMethod.Get, baseUrl, endPoint);

        public async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null) 
            => await SendAndConvertAsync<T>(HttpMethod.Post, baseUrl, endPoint, httpContent);

        public async Task<T?> PutAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
            => await SendAndConvertAsync<T>(HttpMethod.Put, baseUrl, endPoint, httpContent);
        
        public async Task<T?> DeleteAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
            => await SendAndConvertAsync<T>(HttpMethod.Delete, baseUrl, endPoint, httpContent);

        public async Task<T?> PatchAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
            => await SendAndConvertAsync<T>(HttpMethod.Patch, baseUrl, endPoint, httpContent);

        public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint) 
            => await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint);

        public async Task<T?> HeadAsync<T>(string baseUrl, string endPoint)
            => await SendAndConvertAsync<T>(HttpMethod.Head, baseUrl, endPoint);

        public async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent)
            => await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint, httpContent);

        public async Task GetAsync(string baseUrl, string endPoint)
            => await CreateRequest(HttpMethod.Get, baseUrl, endPoint);

        public async Task PostAsync(string baseUrl, string endPoint, HttpContent? httpContent = null) 
            => await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent);

        public async Task PutAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
            => await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent);

        public async Task DeleteAsync(string baseUrl, string endPoint, HttpContent? httpContent = null) 
            => await CreateRequest(HttpMethod.Delete, baseUrl, endPoint, httpContent);

        public async Task PatchAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
            => await CreateRequest(HttpMethod.Patch, baseUrl, endPoint, httpContent);

        public async Task OptionsAsync(string baseUrl, string endPoint)
            => await CreateRequest(HttpMethod.Options, baseUrl, endPoint);

        public async Task HeadAsync(string baseUrl, string endPoint) 
            => await CreateRequest(HttpMethod.Head, baseUrl, endPoint);

        public async Task OptionsAsync(string baseUrl, string endPoint, HttpContent? httpContent)
            => await CreateRequest(HttpMethod.Options, baseUrl, endPoint, httpContent);
        
        private async Task<T?> SendAndConvertAsync<T>(HttpMethod method, string baseUrl, string endPoint, HttpContent? httpContent = null) 
            => ConvertResponse<T>(await CreateRequest(method, baseUrl, endPoint, httpContent));

        private static T? ConvertResponse<T>(string? jsonData)
        {
            try
            {
                if (string.IsNullOrEmpty(jsonData) ||
                    string.Equals(jsonData.Trim(), "null", StringComparison.OrdinalIgnoreCase))
                    return default;

                Type targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

                if (targetType == typeof(string))
                    return (T)(object)jsonData;

                if (targetType == typeof(int) && int.TryParse(jsonData, out int intValue))
                    return (T)Convert.ChangeType(intValue, targetType);

                if (targetType == typeof(long) && long.TryParse(jsonData, out long longValue))
                    return (T)Convert.ChangeType(longValue, targetType);

                if (targetType == typeof(bool) && bool.TryParse(jsonData, out bool boolValue))
                    return (T)Convert.ChangeType(boolValue, targetType);

                if (targetType == typeof(double) && double.TryParse(jsonData, out double doubleValue))
                    return (T)Convert.ChangeType(doubleValue, targetType);

                if (targetType == typeof(decimal) && decimal.TryParse(jsonData, out decimal decimalValue))
                    return (T)Convert.ChangeType(decimalValue, targetType);

                if (targetType == typeof(float) && float.TryParse(jsonData, out float floatValue))
                    return (T)Convert.ChangeType(floatValue, targetType);

                if (targetType == typeof(DateTime) && DateTime.TryParse(jsonData, out DateTime dateValue))
                    return (T)Convert.ChangeType(dateValue, targetType);

                if (targetType == typeof(Guid) && Guid.TryParse(jsonData, out Guid guidValue))
                    return (T)Convert.ChangeType(guidValue, targetType);

                if (targetType.IsEnum && Enum.TryParse(targetType, jsonData, out object? enumValue))
                    return (T)enumValue;

                return JsonSerializer.Deserialize<T>(jsonData);
            }
            catch
            {
                throw new RadiantConnectException($"Failed to parse datatype: {typeof(T)}, given data: {jsonData}");
            }
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

        // Put this at the bottom, we don't need to see it everytime.
        [Obsolete("Please use AuthenticateWithSSID, method is no longer maintained,", true)]
        public ValorantNet([SuppressMessage("ReSharper", "UnusedParameter.Local")] string _) => throw new NotSupportedException("Please use RSOAuth, method is no longer maintained,");
    }
}