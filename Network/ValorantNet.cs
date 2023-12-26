using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RadiantConnect.Services;

namespace RadiantConnect.Network
{
    public class ValorantNet
    {
        internal record Entitlement(
            [property: JsonPropertyName("accessToken")] string AccessToken,
            [property: JsonPropertyName("entitlements")] IReadOnlyList<object> Entitlements,
            [property: JsonPropertyName("issuer")] string Issuer,
            [property: JsonPropertyName("subject")] string Subject,
            [property: JsonPropertyName("token")] string Token
        );

        internal HttpClient Client = new(GetHttpClientHandler());

        public ValorantNet(ValorantService valorantClient)
        {
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion", valorantClient.ValorantClientVersion.RiotClientVersion);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ShooterGame/13 Windows/10.0.19043.1.256.64bit");
        }

        private static HttpClientHandler GetHttpClientHandler()
        {
            return new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
        }

        private static UserAuth? GetAuth()
        {
            string lockFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "Riot Client", "Config", "lockfile");
            string? fileText;
            try
            {
                File.Copy(lockFile, $"{lockFile}.tmp", true);
                using FileStream fs = new($"{lockFile}.tmp", FileMode.Open, FileAccess.Read, FileShare.Read);
                using StreamReader reader = new(fs);
                fileText = reader.ReadToEnd();
            }
            finally
            {
                File.Delete($"{lockFile}.tmp");
            }

            string[] fileValues = fileText.Split(':');

            if (fileValues.Length < 3) return null;

            int authPort = int.Parse(fileValues[2]);
            string oAuth = fileValues[3];

            return new UserAuth(lockFile, authPort, oAuth);
        }

        public int? GetAuthPort() { return GetAuth()?.AuthorizationPort; }

        private async Task<(string, string)> GetAuthorizationToken()
        {
            UserAuth? auth = GetAuth();
            string toEncode = $"riot:{auth?.OAuth}";
            byte[] stringBytes = Encoding.UTF8.GetBytes(toEncode);
            string base64Encode = Convert.ToBase64String(stringBytes);
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {base64Encode}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");

            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}");

            Entitlement? entitlement = JsonSerializer.Deserialize<Entitlement>(response.Content.ReadAsStringAsync().Result);

            return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
        }

        private async Task ResetAuth()
        {
            Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            Client.DefaultRequestHeaders.Remove("Authorization");

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return;

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);
        }

        public async Task<string?> GetAsync(string baseAddress, string endpoint)
        {
            while (true)
            {
                await ResetAuth();

                HttpResponseMessage response = await Client.GetAsync($"{baseAddress}{endpoint}");
                if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }) continue;
                if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string?> PostAsync(string baseAddress, string endpoint, HttpContent? postData = null)
        {
            while (true)
            {
                await ResetAuth();

                HttpResponseMessage response = await Client.PostAsync($"{baseAddress}{endpoint}", postData);

                if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }) continue;
                if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string?> DeleteAsync(string baseAddress, string endpoint)
        {
            while (true)
            {
                await ResetAuth();

                HttpResponseMessage response = await Client.DeleteAsync($"{baseAddress}{endpoint}");

                if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }) continue;
                if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string?> DeleteAsJsonAsync(string baseAddress, string endpoint, JsonContent value)
        {
            while (true)
            {
                await ResetAuth();

                using HttpRequestMessage request = new(HttpMethod.Delete, $"{baseAddress}{endpoint}");
                request.Content = new StringContent(JsonSerializer.Serialize(value));

                HttpResponseMessage response = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }) continue;
                if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

                return await response.Content.ReadAsStringAsync();
            }
        }

        public async Task<string?> PutAsync(string baseAddress, string endpoint, HttpContent content)
        {
            while (true)
            {
                await ResetAuth();

                HttpResponseMessage response = await Client.PutAsync($"{baseAddress}{endpoint}", content);

                if (response is { IsSuccessStatusCode: false, StatusCode: HttpStatusCode.Forbidden }) continue;
                if (!response.IsSuccessStatusCode) return $"Failed: {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}";

                return await response.Content.ReadAsStringAsync();
            }
        }

        internal static async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
        {
            string? jsonData = await PartyEndpoints.PartyEndpoints.Net.GetAsync(baseUrl, endPoint);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        internal static async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
        {
            string? jsonData = await PartyEndpoints.PartyEndpoints.Net.PostAsync(baseUrl, endPoint, httpContent);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        internal static async Task<T?> PutAsync<T>(string baseUrl, string endPoint, HttpContent httpContent)
        {
            string? jsonData = await PartyEndpoints.PartyEndpoints.Net.PutAsync(baseUrl, endPoint, httpContent);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}