using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace RadiantConnect.HenrikApi
{
    [SuppressMessage("ReSharper", "MethodOverloadWithOptionalParameter")]
    public class HenrikClient(string apiKey)
    {
        private HttpClient Client => _lazyClient.Value;
        
        private readonly Lazy<HttpClient> _lazyClient = new(() =>
        {
            HttpClient client = new(
                new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    AutomaticDecompression = DecompressionMethods.All,
                    UseCookies = true
                }
            )
            {
                Timeout = TimeSpan.FromSeconds(30),
                DefaultRequestHeaders =
                {
                    { "User-Agent", "RadiantConnect RadiantConnect/1.0" },
                    { "Host", "api.henrikdev.xyz" },
                    { "Authorization", apiKey },
                }
            };
            return client;
        });
        
        internal static string BuildQuery(string baseUrl, string endpoint, (string Key, string Value)[]? queryParams = null)
        {
            if (string.IsNullOrEmpty(baseUrl)) return string.Empty;

            string queryString = string.Empty;
            if (queryParams is not null)
            {
                queryString = string.Join("&", queryParams.Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                if (!string.IsNullOrEmpty(queryString))
                    queryString = "?" + queryString;
            }

            // We don't want the slash duplicated
            if (baseUrl[^1] == '/' && endpoint[0] == '/') { endpoint = endpoint[1..]; }

            // If I forgot to add a slash, add it here.
            if (baseUrl[^1] != '/' && endpoint[0] != '/') baseUrl += "/";

            return $"{baseUrl}{endpoint}{queryString}";
        }

        internal async Task<string?> CreateRequest(HttpMethod method, string endpoint, (string Key, string Value)[]? queryParams = null, HttpContent? content = null)
        {
            try
            {
                string queryUrl = BuildQuery("https://api.henrik.dev/xyz", endpoint, queryParams);
                using HttpRequestMessage request = new(method, queryUrl);
                request.Content = content;
                
                using HttpResponseMessage response = await Client.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

                string responseContent = await response.Content.ReadAsStringAsync();

#if DEBUG
                string logContent = $"[NetLog] Uri: {queryUrl}";
                logContent += $"\n[NetLog] Method: {method}";
                logContent += $"\n[NetLog] Status Code: {response.StatusCode}";
                logContent += $"\n[NetLog] Request Headers: {JsonSerializer.Serialize(Client.DefaultRequestHeaders.ToDictionary())}";
                logContent += $"\n[NetLog] Response Headers: {JsonSerializer.Serialize(response.Headers.ToDictionary())}";
                logContent += $"\n[NetLog] Response: {responseContent}";

                Debug.WriteLine(logContent);
#endif
                return responseContent;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NetLog] Error: {ex.Message}");
                return null;
            }
        }

        internal async Task<T?> GetAsync<T>(string endPoint, (string Key, string Value)[]? queryParams = null)
            => await SendAndConvertAsync<T>(HttpMethod.Get, endPoint, queryParams);

        internal async Task<T?> PostAsync<T>(string endPoint, (string Key, string Value)[]? queryParams = null, HttpContent? httpContent = null)
            => await SendAndConvertAsync<T>(HttpMethod.Post, endPoint, queryParams, httpContent);

        private async Task<T?> SendAndConvertAsync<T>(HttpMethod method, string endPoint, (string Key, string Value)[]? queryParams = null, HttpContent? httpContent = null)
            => ConvertResponse<T>(await CreateRequest(method, endPoint, queryParams, httpContent));

        private static T? ConvertResponse<T>(string? jsonData)
        {
            if (string.IsNullOrEmpty(jsonData) || string.Equals(jsonData.Trim(), "null", StringComparison.OrdinalIgnoreCase))
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
    }
}
