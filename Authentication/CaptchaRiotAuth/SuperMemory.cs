using System.Net.Http.Headers;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace RadiantConnect.Authentication.CaptchaRiotAuth
{
    // Credit https://github.com/Trollicus/ for the original code
    internal class HttpHandler : IDisposable
    {
        private readonly HttpClient _client = new(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All
        });

        internal async Task<HttpResponseMessage> SendAsync(string uri, HttpMethod method, string json, RequestHeadersEx[] requestHeaders)
        {
            HttpRequestMessage request = new()
            {
                RequestUri = new Uri(uri),
                Method = method,
                Content = new StringContent(json)
                {
                    Headers =
                    {
                        ContentType = new MediaTypeHeaderValue("application/json")
                    }
                }
            };

            foreach (RequestHeadersEx requestHeader in requestHeaders) 
                request.Headers.TryAddWithoutValidation(requestHeader.Key, requestHeader.Value);

            return await _client.SendAsync(request);
        }

        internal record RequestHeadersEx(string Key, string Value);

        public void Dispose()
        {
            _client.Dispose();
        }
    }

    internal class SuperMemory
    {
        internal readonly HttpHandler HttpHandler = new();
        internal class GetTaskResultJson
        {
            [JsonPropertyName("taskId")] public Guid? TaskId { get; init; }
        }

        internal class CheckCaptchaResponse
        {
            [JsonPropertyName("taskId")]
            public Guid TaskId { get; init; }

            [JsonPropertyName("process")]
            public string? Process { get; init; }

            [JsonPropertyName("generatedPassUuid")]
            public string? GeneratedPassUuid { get; init; }
        }

        internal async Task<GetTaskResultJson?> CreateTaskAsync(string siteUrl, string siteKey, string token)
        {
            HttpResponseMessage response = await HttpHandler.SendAsync("https://captchasolver.net/api/captcha/createtask", HttpMethod.Post, $"{{ \"type\": \"HCaptchaTaskProxyless\", \"siteUrl\": \"{siteUrl}\", \"siteKey\": \"{siteKey}\", \"token\":\"{token}\" }}",
            [
                new HttpHandler.RequestHeadersEx("Authorization", "Basic"),
            ]);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<GetTaskResultJson>(jsonResponse);
        }

        internal async Task<CheckCaptchaResponse?> GetTaskResult(GetTaskResultJson createTaskResult)
        {
            string json = JsonSerializer.Serialize(new GetTaskResultJson { TaskId = createTaskResult.TaskId });

            CheckCaptchaResponse? taskResultResponse;

            do
            {
                HttpResponseMessage request = await HttpHandler.SendAsync("https://captchasolver.net/api/captcha/CheckTask", HttpMethod.Post, json, [
                        new HttpHandler.RequestHeadersEx("Authorization", "Basic"),
                ]);

                string response = await request.Content.ReadAsStringAsync();

                taskResultResponse = JsonSerializer.Deserialize<CheckCaptchaResponse>(response);

                if (taskResultResponse?.Process == "Solved")
                    break;

                Thread.Sleep(2000);
            } while (true);

            return taskResultResponse;
        }

        internal async Task<string> GetSolutionAsync(string siteUrl, string siteKey, string token)
        {
            GetTaskResultJson? taskResult = await CreateTaskAsync(siteUrl, siteKey, token);
            if (taskResult?.TaskId == null)
                throw new InvalidOperationException("Failed to create task.");

            CheckCaptchaResponse? taskResultResponse = await GetTaskResult(taskResult);
            if (taskResultResponse?.GeneratedPassUuid == null)
                throw new InvalidOperationException("Failed to get task result.");

            return taskResultResponse.GeneratedPassUuid;
        }
    }
}
