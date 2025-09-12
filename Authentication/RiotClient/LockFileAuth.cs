using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network;
using System.Net.Http.Headers;

namespace RadiantConnect.Authentication.RiotClient
{
	internal class LockFileAuth
	{
		[SuppressMessage("ReSharper", "InvertIf")] // Justification: Readability
		internal async Task<RSOAuth?> Run()
		{
			string lockFile = ValorantNet.LockFilePath;

			if (!File.Exists(lockFile))
				throw new RadiantConnectAuthException("Lockfile not found, is riot client logged in and running?");

			ValorantNet.UserAuth? userAuth = ValorantNet.GetAuth();

			if (userAuth == null)
				throw new RadiantConnectAuthException("Failed to grab auth from lockfile, is riot client logged in and running? 0x2");

			HttpClient client = new(new HttpClientHandler
			{
				AllowAutoRedirect = true,
				AutomaticDecompression = DecompressionMethods.All,
				ServerCertificateCustomValidationCallback = (_, _, _, _) => true
			});
			
			client.DefaultRequestHeaders.Authorization = AuthenticationHeaderValue.Parse($"Basic {$"riot:{userAuth.OAuth}".ToBase64()}");
			HttpResponseMessage response = await client.GetAsync($"https://127.0.0.1:{userAuth.AuthorizationPort}/riot-client-auth/v1/authorization");
			
			if (!response.IsSuccessStatusCode) 
				throw new RadiantConnectNetworkStatusException($"Failed to get LockFile tokens: {response.StatusCode.ToString()}");

			RSOClientReturn? rsoClientData = JsonSerializer.Deserialize<RSOClientReturn>(response.Content.ReadAsStringAsync().Result);
			
			if (rsoClientData == null)
				throw new RadiantConnectAuthException("Failed to parse RSO client data from lockfile auth.");

			client.Dispose();

			string authBearer = rsoClientData.AccessToken.Token;

			(string pasToken, string entitlementToken, object clientConfig, string userInfo, string _) = await AuthUtil.GetAuthTokensFromAccessToken(authBearer);

			JsonDocument document = JsonDocument.Parse(userInfo);
			string affinity = document.RootElement.GetProperty("original_platform_id").GetString() ?? "";

			string? ssid = null, clid = null, csid = null, tdid = null;

			if (File.Exists(RtcAuth.RiotClientSettings))
			{
				Dictionary<string, string?> cookieValues = await RtcAuth.GetCookiesFromYaml(RtcAuth.RiotClientSettings);

				if (cookieValues.Count >= 3)
				{
					cookieValues.TryGetValue("ssid", out ssid);

					if (cookieValues["tdid"].IsNullOrEmpty())
						cookieValues["tdid"] = await RtcAuth.GetTdidFallback(RtcAuth.RiotClientSettings);

					cookieValues.TryGetValue("clid", out clid);
					cookieValues.TryGetValue("csid", out csid);
					cookieValues.TryGetValue("tdid", out tdid);
				}
			}
			
			return new RSOAuth(
				Subject: rsoClientData.Puuid,
				Ssid: ssid,
				Tdid: tdid,
				Csid: csid,
				Clid: clid,
				AccessToken: authBearer,
				PasToken: pasToken,
				Entitlement: entitlementToken,
				Affinity: affinity,
				ChatAffinity: affinity,
				ClientConfig: clientConfig,
				RiotCookies: null,
				IdToken: rsoClientData.IdToken.Token
			);

		}
		public record AccessToken(
			[property: JsonPropertyName("clientId")] string ClientId,
			[property: JsonPropertyName("expiry")] int? Expiry,
			[property: JsonPropertyName("scopes")] IReadOnlyList<string> Scopes,
			[property: JsonPropertyName("token")] string Token
		);

		public record Acct(
			[property: JsonPropertyName("game_name")] string GameName,
			[property: JsonPropertyName("tag_line")] string TagLine
		);

		public record Claims(
			[property: JsonPropertyName("acct")] Acct Acct,
			[property: JsonPropertyName("acr")] string Acr,
			[property: JsonPropertyName("country")] string Country,
			[property: JsonPropertyName("login_country")] string LoginCountry
		);

		public record IdToken(
			[property: JsonPropertyName("clientId")] string ClientId,
			[property: JsonPropertyName("expiry")] int? Expiry,
			[property: JsonPropertyName("nonce")] string Nonce,
			[property: JsonPropertyName("token")] string Token
		);

		public record RSOClientReturn(
			[property: JsonPropertyName("accessToken")] AccessToken AccessToken,
			[property: JsonPropertyName("authenticationType")] string AuthenticationType,
			[property: JsonPropertyName("claims")] Claims Claims,
			[property: JsonPropertyName("idToken")] IdToken IdToken,
			[property: JsonPropertyName("puuid")] string Puuid
		);
	}
}
