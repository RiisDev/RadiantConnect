using System.Net.Http.Headers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Services;

#pragma warning disable IDE0046

namespace RadiantConnect.Network
{
    public class ValorantNet
    {
        internal static string LockFilePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local",
                "Riot Games", "Riot Client", "Config", "lockfile");

        public delegate void ValorantNetLog(string message);
        public event ValorantNetLog? OnLog;

        public RSOAuth? AuthCodes { get; set; }

        private readonly HttpClient _client = AuthUtil.BuildClient().Item1;

        public static int? GetAuthPort() => GetAuth()?.AuthorizationPort;

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

        private readonly string _defaultPlatform;
        private readonly string _defaultUserAgent;
        private readonly string _defaultClientVersion;

        private void ResetDefaultHeaders()
        {
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", _defaultPlatform);
            _client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", _defaultUserAgent);
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", _defaultClientVersion);
        }

        public ValorantNet(RSOAuth rsoAuth)
        {
            AuthCodes = rsoAuth;
            _client.Timeout = TimeSpan.FromSeconds(value: 10);

            ValorantVersionApiRoot? apiData = InternalHttp.GetAsync<ValorantVersionApiRoot>("https://valorant-api.com", "/v1/version").Result;

			if (apiData?.Data is null)
				throw new RadiantConnectException("Failed to get Valorant version data from API");

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

            _defaultPlatform = valorantClient.UserPlatform;
            _defaultUserAgent = $"ShooterGame/{valorantClient.BuildVersion} Windows/{valorantClient.UserClientVersion}";
            _defaultClientVersion = valorantClient.RiotClientVersion;

            ResetDefaultHeaders();
        }

        public ValorantNet(ValorantService? valorantClient = null)
        {
            _client.Timeout = TimeSpan.FromSeconds(10);
            _defaultPlatform = valorantClient?.ValorantClientVersion.UserPlatform ?? "";
            _defaultUserAgent = $"ShooterGame/{valorantClient?.ValorantClientVersion.BuildVersion} {valorantClient?.ValorantClientVersion.UserClientVersion}";
            _defaultClientVersion = valorantClient?.ValorantClientVersion.RiotClientVersion ?? "";

            ResetDefaultHeaders();
        }
		
        internal static UserAuth? GetAuth()
        {
            string fileText = LogService.ReadTextFile(LockFilePath);

            if (fileText.IsNullOrEmpty()) return null;

            string[] fileValues = fileText.Split(':');

            if (fileValues.Length < 3) return null;

            int authPort = int.Parse(fileValues[2]);
            string oAuth = fileValues[3];
            return new UserAuth(authPort, oAuth);
        }

        private async Task<(string, string)> GetAuthorizationToken()
        {
            OnLog?.Invoke("[ValorantNet Log] Getting local AuthorizationTokens");

            if (AuthCodes is not null)
            {
				if (string.IsNullOrEmpty(AuthCodes.AccessToken) || string.IsNullOrEmpty(AuthCodes.Entitlement)) 
					throw new RadiantConnectException("AuthCodes are not valid, AccessToken or EntitlementToken is empty.");
				
				return (AuthCodes.AccessToken, AuthCodes.Entitlement);
            }

            UserAuth? auth = GetAuth();
            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{auth?.OAuth}".ToBase64()}");
            HttpResponseMessage response = await _client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");
            
            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {await response.Content.ReadAsStringAsync()}");

			Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(response.Content.ReadAsStringAsync().Result);
            OnLog?.Invoke($"[ValorantNet GetAuthorizationToken Log] AccessToken: {entitlement?.AccessToken}\n[ValorantNet GetAuthorizationToken Log] EntitlementToken: {entitlement?.Token}");
            return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
        }
       
        private async Task ResetAuth()
        {
            try
            {
                _client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
                _client.DefaultRequestHeaders.Remove("Authorization");
            }
            catch {/**/}

            (string, string) authTokens = await GetAuthorizationToken();

            if (authTokens.Item1.IsNullOrEmpty()) throw new RadiantConnectException("Failed to get Authorization Token");
            if (authTokens.Item2.IsNullOrEmpty()) throw new RadiantConnectException("Failed to get JWT Token");

			_client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
            _client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);
        }

        private Task SetBasicAuth()
        {
            OnLog?.Invoke("[ValorantNet Log] Settings Basic Auth");

            if (AuthCodes is not null)
                throw new RadiantConnectException("Cannot use local endpoints with AuthCodes");

            _client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            _client.DefaultRequestHeaders.Remove("Authorization");

            _client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{GetAuth()?.OAuth}".ToBase64()}");

            return Task.CompletedTask;
        }

        private void SetCustomHeaders(Dictionary<string, string> headers)
        {
            _client.DefaultRequestHeaders.Clear();

            foreach ((string key, string value) in headers)
                _client.DefaultRequestHeaders.TryAddWithoutValidation(key, value);
        }

        public async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null, Dictionary<string, string>? customHeaders = null)
        {
            if (baseUrl.IsNullOrEmpty()) return string.Empty;

            if (!endPoint.IsNullOrEmpty())
            {
                if (baseUrl[^1] == '/' && endPoint[0] == '/') endPoint = endPoint[1..]; // Make sure the slash isn't duplicated
                if (baseUrl[^1] != '/' && endPoint[0] != '/') baseUrl += "/"; // Make sure it actually contains a slash
            }

            // I no longer need the loop, as the client will now handle edge-cases.
            if (!InternalValorantMethods.IsValorantProcessRunning() && AuthCodes is null) return string.Empty;

            if (baseUrl.Contains("127.0.0.1") && _client.DefaultRequestHeaders.Authorization?.Scheme != "Basic") await SetBasicAuth();
            else if (customHeaders is not null) SetCustomHeaders(customHeaders);
            else if (!baseUrl.Contains("127.0.0.1")) await ResetAuth();

            using HttpRequestMessage httpRequest = new();
            httpRequest.Method = MapHttpMethod(httpMethod);
            httpRequest.RequestUri = new Uri($"{baseUrl}{endPoint}");
            httpRequest.Content = content;

            using HttpResponseMessage responseMessage = await _client.SendAsync(httpRequest, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

            string responseContent = await responseMessage.Content.ReadAsStringAsync();

            if (customHeaders is not null)
                ResetDefaultHeaders();

            OnLog?.Invoke($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(_client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");

            return !responseMessage.IsSuccessStatusCode
	            ? throw new RadiantConnectNetworkStatusException(
		            $"\n[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Headers:{JsonSerializer.Serialize(_client.DefaultRequestHeaders.ToDictionary())}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}")
	            : responseContent;
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
                if (jsonData.IsNullOrEmpty() ||
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
    }
}