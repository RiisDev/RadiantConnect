using System.Text.Json;
using RadiantConnect.Services;

namespace RadiantConnect.Tests
{
	internal class Client
	{
		public static bool MachineReady(bool valorantRequired = true)
		{
			string valorantPath = RiotPathService.GetValorantPath();

			return (File.Exists(valorantPath) && valorantRequired)
				   || Environment.UserName == "Riis"; // I want to be able to run these tests on my machine always.
		}

		private static readonly Lazy<HttpClient> LazyClient = new(() =>
		{
			HttpClient client = new()
			{
				Timeout = TimeSpan.FromSeconds(30)
			};
			client.DefaultRequestHeaders.Add("User-Agent", "TestUnit RadiantConnect/1.0");
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			return client;
		});

		internal static HttpClient HttpClient => LazyClient.Value;

		internal static async Task<T?> Get<T>(string endpoint, (string Key, string Value)[]? queryParams = null)
		{
			string queryString = string.Join("&", queryParams?.Select(kvp => $"{kvp.Key}={kvp.Value}") ?? []);

			if (!string.IsNullOrEmpty(queryString))
				queryString = "?" + queryString;

			HttpRequestMessage request = new(HttpMethod.Get, $"https://valorant-api.com/v1/{endpoint}{queryString}");
			HttpResponseMessage response = await HttpClient.SendAsync(request);

			if (!response.IsSuccessStatusCode)
				throw new Exception(
					$"Error fetching: {response.StatusCode}\n{await response.Content.ReadAsStringAsync()}");

			string jsonResponse = await response.Content.ReadAsStringAsync();

			if (typeof(T) == typeof(string))
				return (T)(object)jsonResponse;

			return JsonSerializer.Deserialize<T>(jsonResponse);
		}
	}
}
