using System.Diagnostics;
using System.Net.Http.Headers;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

namespace RadiantConnect.Authentication.CaptchaRiotAuth
{
    internal class SuperMemory : IDisposable
    {
        internal class GetTaskResultJson { [JsonPropertyName("taskId")] public Guid? TaskId { get; init; } }

        internal class CheckCaptchaResponse { [JsonPropertyName("taskId")] public Guid TaskId { get; init; } [JsonPropertyName("process")] public string? Process { get; init; } [JsonPropertyName("generatedPassUuid")] public string? GeneratedPassUuid { get; init; } }

        internal HttpClient RiotHttpClient;
        internal HttpClient HttpHandler;
        internal CookieContainer Cookies;
        internal string Authorization;

        internal SuperMemory(string authorization)
        {
            Authorization = authorization;
            Cookies = new CookieContainer();
            RiotHttpClient = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                CookieContainer = Cookies,
                AutomaticDecompression = DecompressionMethods.All,
                UseCookies = true,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });


            // TODO - Actually get the real client version
            RiotHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "RiotClient/91.0.2.1870 rso-auth (Windows;10;;Professional, x64)");
            RiotHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Cache-Control", "no-cache");
            RiotHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
            RiotHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");

            HttpHandler = new HttpClient(new HttpClientHandler()
            {
                AllowAutoRedirect = true,
                AutomaticDecompression = DecompressionMethods.All,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            });

            HttpHandler.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Authorization);
        }

        internal async Task<GetTaskResultJson?> CreateTaskAsync(string siteUrl, string siteKey)
        {
            HttpRequestMessage request = new(HttpMethod.Post, "https://captchasolver.net/api/captcha/createtask");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                Type = "HCaptchaTaskProxyless", 
                SiteUrl = siteUrl, 
                SiteKey = siteKey, 
                Token = Authorization
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await HttpHandler.SendAsync(request);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GetTaskResultJson>(jsonResponse);
        }

        internal async Task<CheckCaptchaResponse?> GetTaskResult(GetTaskResultJson createTaskResult)
        {
            string json = JsonSerializer.Serialize(new GetTaskResultJson { TaskId = createTaskResult.TaskId });

            CheckCaptchaResponse? taskResultResponse;

            do
            {
                HttpRequestMessage request = new (HttpMethod.Post, "https://captchasolver.net/api/captcha/CheckTask");
                request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = await HttpHandler.SendAsync(request);
                string response = await responseMessage.Content.ReadAsStringAsync();

                taskResultResponse = JsonSerializer.Deserialize<CheckCaptchaResponse>(response);

                if (taskResultResponse?.Process == "Solved")
                    break;

                await Task.Delay(1500);
            } while (true);

            return taskResultResponse;
        }

        internal async Task<string> GetSolutionAsync(string siteUrl, string siteKey)
        {
            GetTaskResultJson? taskResult = await CreateTaskAsync(siteUrl, siteKey);
            if (taskResult?.TaskId == null)
                throw new InvalidOperationException("Failed to create task.");

            CheckCaptchaResponse? taskResultResponse = await GetTaskResult(taskResult);
            if (taskResultResponse?.GeneratedPassUuid == null)
                throw new InvalidOperationException("Failed to get task result.");

            return taskResultResponse.GeneratedPassUuid;
        }

        internal static string TokenUrlSafe(int byteLength)
        {
            byte[] tokenData = new byte[byteLength];
            RandomNumberGenerator.Fill(tokenData);
            string token = Convert.ToBase64String(tokenData).Replace('+', '-').Replace('/', '_').TrimEnd('=');
            return token;
        }

        internal async Task<object?> GetCaptchaToken()
        {
            HttpRequestMessage request = new(HttpMethod.Post, "https://auth.riotgames.com/api/v1/authorization");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                clientId = "riot-client",
                language = "",
                platform = "windows",
                remember = true,
                riot_identity = new
                {
                    language = "en_GB",
                    state = "auth",
                },
                sdkVersion = "Version().sdk",
                type = "auth",
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await RiotHttpClient.SendAsync(request);

            return JsonSerializer.Deserialize<dynamic>(await responseMessage.Content.ReadAsStringAsync());
        }

        internal async Task<string?> GetLoginToken(string username, string password, string code)
        {
            HttpRequestMessage request = new(HttpMethod.Put, "https://auth.riotgames.com/api/v1/authorization");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                riot_identity = new {
                    captcha = $"hcaptcha {code}",
                    language = "en_GB",
                    password = username,
                    remember = true,
                    username = password
                },
                type = "auth"
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await RiotHttpClient.SendAsync(request); // Get starter cookies

            dynamic? loginData = JsonSerializer.Deserialize<dynamic>(await responseMessage.Content.ReadAsStringAsync());

            return loginData?.success?.login_token;

        }

        internal async Task<RSOAuth?> BeginSignIn(string username, string password)
        {
            HttpRequestMessage request = new (HttpMethod.Post, "https://auth.riotgames.com/api/v1/authorization");
            request.Content = new StringContent(JsonSerializer.Serialize(new
            {
                client_id = "riot-client",
                nonce = TokenUrlSafe(16),
                redirect_uri = "http://localhost/redirect",
                response_type = "token id_token",
                scope = "account openid"
            }), Encoding.UTF8, "application/json");

            await RiotHttpClient.SendAsync(request); // Get starter cookies

            dynamic? captchaData = await GetCaptchaToken();

            string captchaResolve = await GetSolutionAsync(captchaData?.siteUrl, captchaData?.siteKey);

            string? loginToken = await GetLoginToken(username, password, captchaResolve);

            // get the actual auth headers now?

            return null;
        }

        public void Dispose() => RiotHttpClient.Dispose();
    }
}
