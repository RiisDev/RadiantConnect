using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using RadiantConnect.Methods;
using RadiantConnect.Services;

namespace RadiantConnect.Network
{
    public record UserAuth(int AuthorizationPort, string OAuth);

    public class ValorantNet
    {
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

        internal enum HttpMethod
        {
            Get,
            Post, 
            Put, 
            Delete,
            Patch,
            Options,
            Head
        }

        public ValorantNet(ValorantService? valorantClient = null)
        {
            Client.Timeout = TimeSpan.FromSeconds(10);
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientPlatform", "ew0KCSJwbGF0Zm9ybVR5cGUiOiAiUEMiLA0KCSJwbGF0Zm9ybU9TIjogIldpbmRvd3MiLA0KCSJwbGF0Zm9ybU9TVmVyc2lvbiI6ICIxMC4wLjE5MDQyLjEuMjU2LjY0Yml0IiwNCgkicGxhdGZvcm1DaGlwc2V0IjogIlVua25vd24iDQp9");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "ShooterGame/13 Windows/10.0.19043.1.256.64bit");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-ClientVersion",valorantClient?.ValorantClientVersion.RiotClientVersion);
        }

        public static UserAuth? GetAuth()
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
            UserAuth? auth = GetAuth();
            string toEncode = $"riot:{auth?.OAuth}";
            byte[] stringBytes = Encoding.UTF8.GetBytes(toEncode);
            string base64Encode = Convert.ToBase64String(stringBytes);
            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {base64Encode}");
            HttpResponseMessage response = await Client.GetAsync($"https://127.0.0.1:{auth?.AuthorizationPort}/entitlements/v1/token");

            if (!response.IsSuccessStatusCode) return ("", $"Failed to get entitlement | {response.StatusCode} | {response.Content.ReadAsStringAsync().Result}");

            InternalRecords.Entitlement? entitlement = JsonSerializer.Deserialize<InternalRecords.Entitlement>(response.Content.ReadAsStringAsync().Result);

            return (entitlement?.AccessToken ?? "", entitlement?.Token ?? "");
        }

        internal async Task ResetAuth()
        {
            Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            Client.DefaultRequestHeaders.Remove("Authorization");

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return;

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Bearer {authTokens.Item1}");
            Client.DefaultRequestHeaders.TryAddWithoutValidation("X-Riot-Entitlements-JWT", authTokens.Item2);
        }

        internal async Task SetBasicAuth()
        {
            Client.DefaultRequestHeaders.Remove("X-Riot-Entitlements-JWT");
            Client.DefaultRequestHeaders.Remove("Authorization");

            (string, string) authTokens = await GetAuthorizationToken();

            if (string.IsNullOrEmpty(authTokens.Item1)) return;

            Client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes($"riot:{GetAuth()?.OAuth}"))}");
        }

        internal async Task<string?> CreateRequest(HttpMethod httpMethod, string baseUrl, string endPoint, HttpContent? content = null)
        {
            while (InternalValorantMethods.IsValorantProcessRunning())
            {
                if (baseUrl.Contains("127.0.0.1") && Client.DefaultRequestHeaders.Authorization?.Scheme != "Basic") await SetBasicAuth();
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
                Debug.WriteLine($"[ValorantNet Log] Uri:{baseUrl}{endPoint}\n[ValorantNet Log] Request Content: {JsonSerializer.Serialize(content)}\n[ValorantNet Log] Response Content:{responseContent}\n[ValorantNet Log] Response Data: {responseMessage}");
                httpRequest.Dispose();
                responseMessage.Dispose();
                return (responseContent.Contains("<html>") || responseContent.Contains("errorCode")) ? null : responseContent;
            }
            return string.Empty;
        }

        internal async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
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

        internal async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
        {
            string? jsonData = await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent);

            return string.IsNullOrEmpty(jsonData) ? default : JsonSerializer.Deserialize<T>(jsonData);
        }

        internal async Task<T?> PutAsync<T>(string baseUrl, string endPoint,HttpContent httpContent)
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