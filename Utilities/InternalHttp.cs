#pragma warning disable IDE0305
#pragma warning disable IDE0046

namespace RadiantConnect.Utilities
{
	internal static class InternalHttp
	{
		internal static CookieContainer CookieContainer { get; private set; } = new();
		private static HttpClient InternalClient => LazyClient.Value;

		internal static void ClearCookies() => CookieContainer = new CookieContainer();

		private static readonly Lazy<HttpClient> LazyClient = new(() =>
		{
			HttpClient client = new(
				new HttpClientHandler
				{
					AllowAutoRedirect = true,
					AutomaticDecompression = DecompressionMethods.All,
					CookieContainer = CookieContainer,
					UseCookies = true
				}
			)
			{
				Timeout = TimeSpan.FromSeconds(30)
			};
			return client;
		});

		internal static Dictionary<string, string> RequestHeaders = [];

		internal static string BuildQuery(string baseUrl, string endpoint)
		{
			if (string.IsNullOrEmpty(baseUrl)) return string.Empty;

			// We don't want the slash duplicated
			if (baseUrl[^1] == '/' && endpoint[0] == '/') endpoint = endpoint[1..];
			if (baseUrl[^1] != '/' && endpoint[0] != '/') baseUrl += "/";

			return $"{baseUrl}{endpoint}";
		}

		internal static async Task<string?> CreateRequest(HttpMethod method, string baseUrl, string endpoint, HttpContent? content = null)
		{
			try
			{
				string queryUrl = BuildQuery(baseUrl, endpoint);
				using HttpRequestMessage request = new(method, queryUrl);
				request.Content = content;
				
				foreach (KeyValuePair<string, string> header in RequestHeaders.Where(header => !string.IsNullOrEmpty(header.Value))) 
					request.Headers.TryAddWithoutValidation(header.Key, header.Value);

				using HttpResponseMessage response = await InternalClient
					.SendAsync(request, HttpCompletionOption.ResponseContentRead).ConfigureAwait(false);

				string responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				string logContent = $"[NetLog] Uri: {queryUrl}";
				logContent += $"\n[NetLog] Method: {method}";
				logContent += $"\n[NetLog] Status Code: {response.StatusCode}";
				logContent +=
					$"\n[NetLog] Request Headers: {JsonSerializer.Serialize(InternalClient.DefaultRequestHeaders.ToDictionary())}";
				logContent +=
					$"\n[NetLog] Response Headers: {JsonSerializer.Serialize(response.Headers.ToDictionary())}";
				logContent += $"\n[NetLog] Response: {responseContent}";

				Debug.WriteLine(logContent);

				return responseContent;
			}
			catch (Exception ex)
			{
				Debug.WriteLine($"[NetLog] Error: {ex.Message}");
				return null;
			}
			finally
			{
				InternalClient.DefaultRequestHeaders.Clear();
			}
		}

		internal static async Task<T?> GetAsync<T>(string baseUrl, string endPoint)
			=> await SendAndConvertAsync<T>(HttpMethod.Get, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task<T?> PostAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Post, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task<T?> PutAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Put, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task<T?> DeleteAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Delete, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task<T?> PatchAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await SendAndConvertAsync<T>(HttpMethod.Patch, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint)
			=> await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task<T?> HeadAsync<T>(string baseUrl, string endPoint)
			=> await SendAndConvertAsync<T>(HttpMethod.Head, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task<T?> OptionsAsync<T>(string baseUrl, string endPoint, HttpContent? httpContent)
			=> await SendAndConvertAsync<T>(HttpMethod.Options, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task GetAsync(string baseUrl, string endPoint)
			=> await CreateRequest(HttpMethod.Get, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task PostAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Post, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task PutAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Put, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task DeleteAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Delete, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task PatchAsync(string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> await CreateRequest(HttpMethod.Patch, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		internal static async Task OptionsAsync(string baseUrl, string endPoint)
			=> await CreateRequest(HttpMethod.Options, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task HeadAsync(string baseUrl, string endPoint)
			=> await CreateRequest(HttpMethod.Head, baseUrl, endPoint).ConfigureAwait(false);

		internal static async Task OptionsAsync(string baseUrl, string endPoint, HttpContent? httpContent)
			=> await CreateRequest(HttpMethod.Options, baseUrl, endPoint, httpContent).ConfigureAwait(false);

		private static async Task<T?> SendAndConvertAsync<T>(HttpMethod method, string baseUrl, string endPoint, HttpContent? httpContent = null)
			=> ConvertResponse<T>(await CreateRequest(method, baseUrl, endPoint, httpContent).ConfigureAwait(false));

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
