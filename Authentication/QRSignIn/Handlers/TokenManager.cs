using System.Net.Http.Headers;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Authentication.QRSignIn.Modules;
using Timer = System.Timers.Timer;

namespace RadiantConnect.Authentication.QRSignIn.Handlers
{
	internal sealed class TokenManager(Process? form, BuiltData qrData, HttpClient client, bool returnUrl, CookieContainer container)
	{
		internal delegate void TokensFinished(RSOAuth authData);
		internal event TokensFinished? OnTokensFinished;

		private static string GenerateTraceParent()
		{
			string traceId = Guid.NewGuid().ToString("N");
			string parentId = Guid.NewGuid().ToString("N")[..16];
			return $"00-{traceId}-{parentId}-00";
		}

		private void SetHeaders(string host, string traceparent, string useragent, Dictionary<string, string>? additionalHeaders = null, string? sdk = null)
		{
			client.DefaultRequestHeaders.Clear();

			Dictionary<string, string> headers = new()
			{
				{ "Host", host },
				{ "User-Agent", useragent },
				{ "Accept-Encoding", "deflate, gzip, zstd" },
				{ "Accept", "application/json" },
				{ "Connection", "keep-alive" },
				{ "baggage", $"sdksid={sdk ?? qrData.SdkSid}" },
				{ "traceparent", traceparent },
			};
			
			if (additionalHeaders != null)
				foreach (KeyValuePair<string, string> header in additionalHeaders)
					headers[header.Key] = header.Value;

			foreach (KeyValuePair<string, string> header in headers)
				client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
		}

		private async Task<string> GetLoginToken()
		{
			string traceparent = GenerateTraceParent();
			SetHeaders("authenticate.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-authenticator (Windows;10;;Professional, x64) riot_client/0");

			HttpResponseMessage response = await client.GetAsync("https://authenticate.riotgames.com/api/v1/login").ConfigureAwait(false);
			string responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
			client.DefaultRequestHeaders.Clear();

			if (!responseData.Contains("success", StringComparison.InvariantCultureIgnoreCase)) return string.Empty;

			QrDataSuccess? data = JsonSerializer.Deserialize<QrDataSuccess>(responseData);
			
			return data?.Success?.LoginToken ?? string.Empty;
		}

		private async Task<string> GetAccessTokenStage1(string loginToken)
		{
			string traceparent = GenerateTraceParent();
			SetHeaders(
				host: "auth.riotgames.com",
				traceparent: traceparent,
				useragent: "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0",
				additionalHeaders: new Dictionary<string, string>
				{
					{ "Cache-Control", "no-cache" }
				},
				Guid.NewGuid().ToString()
			);

			Dictionary<string, object?> postParams = new()
			{
				{ "authentication_type", null },
				{ "code_verifier", "" },
				{ "login_token", loginToken },
				{ "persist_login", false },
			};

			// No idea why this section has to be different, using normal post caused it to error?

			string jsonContent = JsonSerializer.Serialize(postParams);

			using HttpRequestMessage requestMessage = new (HttpMethod.Post, "https://auth.riotgames.com/api/v1/login-token");
			requestMessage.Content = new StringContent(jsonContent)
			{
				Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
			};

			HttpResponseMessage response = await client.SendAsync(requestMessage).ConfigureAwait(false);

			if (response.StatusCode != HttpStatusCode.NoContent)
				throw new RadiantConnectAuthException("Failed to verify login_token");

			client.DefaultRequestHeaders.Clear();

			return traceparent;
		}

		private async Task<string> GetAccessTokenStage2(string traceparent)
		{
			SetHeaders("auth.riotgames.com", traceparent, "RiotGamesApi/24.11.0.4602 rso-auth (Windows;10;;Professional, x64) riot_client/0");

			Dictionary<string, object?> postParams = new()
			{
				{ "acr_values", "" },
				{ "claims", "" },
				{ "client_id", "riot-client" },
				{ "code_challenge", "" },
				{ "code_challenge_method", "" },
				{ "nonce", Guid.NewGuid().ToString("N")[..22] },
				{ "redirect_uri", "http://localhost/redirect" },
				{ "response_type", "token id_token" },
				{ "scope", "openid link lol_region lol summoner offline_access ban" }, // Just for shits, doesn't actually work
			};

			HttpResponseMessage response = await client.PostAsJsonAsync("https://auth.riotgames.com/api/v1/authorization", postParams).ConfigureAwait(false);
			Debug.WriteLine($"AccessTokenStage2: {await response.Content.ReadAsStringAsync().ConfigureAwait(false)}");
			client.DefaultRequestHeaders.Clear();

			AccessTokenReturn? accessTokenData = await response.Content.ReadFromJsonAsync<AccessTokenReturn>().ConfigureAwait(false);

			return accessTokenData?.Response.Parameters.Uri ?? "";
		}

		private async Task<(string, string)> GetAccessTokens(string loginToken)
		{
			string traceData = await GetAccessTokenStage1(loginToken).ConfigureAwait(false);
			string accessTokenUri = await GetAccessTokenStage2(traceData).ConfigureAwait(false);

			string[] urlTokens = accessTokenUri.Split('&');

			string idToken = urlTokens[^1][9..];
			string accessToken = urlTokens[^2][13..];

			return (accessToken, idToken);
		}

		[SuppressMessage("ReSharper", "RemoveRedundantBraces")]
		internal void InitiateTimer(string tempName)
		{
#pragma warning disable CA2000 // It literally gets disposed later in the code
            Timer timer = new();
#pragma warning restore CA2000
			timer.Interval = 1000;
			timer.AutoReset = true;

			if (!returnUrl)
			{
				Task.Run(async () =>
				{
					await Task.Delay(30000).ConfigureAwait(false);
					timer.Stop();
					timer.Dispose();
					try { form?.Kill(true); } catch {/**/}
					try { Process.GetProcessesByName(tempName).ToList().ForEach(x => x.Kill(true)); } catch {/**/}
					form?.Dispose();
				});
			}

			timer.Elapsed += async (_, _) =>
			{
				string loginToken = await GetLoginToken().ConfigureAwait(false);
				if (loginToken.IsNullOrEmpty()) return;

				timer.Stop();
				timer.Dispose();
				try { form?.Kill(true); } catch {/**/}
				try { Process.GetProcessesByName(tempName).ToList().ForEach(x => x.Kill(true)); } catch {/**/}
				form?.Dispose();

				(string accessToken, string idToken) = await GetAccessTokens(loginToken).ConfigureAwait(false);
				if (accessToken.IsNullOrEmpty()) return;

				(string pasToken, string entitlementToken, object clientConfig, string _, string rmsToken) = await AuthUtil.GetAuthTokensFromAccessToken(accessToken).ConfigureAwait(false);

				JsonWebToken jwt = new(pasToken);
				string affinity = jwt.GetRequiredPayloadValue<string>("affinity");
				string chatAffinity = jwt.GetRequiredPayloadValue<string>("desired.affinity");
				string subject = jwt.GetRequiredPayloadValue<string>("sub");

				CookieCollection cookies = container.GetAllCookies();
				
				OnTokensFinished?.Invoke(new RSOAuth(
					subject,
					cookies.First(x => x.Name == "ssid").Value,
					cookies.First(x => x.Name == "tdid").Value,
					cookies.First(x => x.Name == "csid").Value,
					cookies.First(x => x.Name == "clid").Value,
					accessToken,
					pasToken,
					entitlementToken,
					affinity,
					chatAffinity,
					clientConfig,
					null,
					idToken
				)
				{
					RmsToken = rmsToken
				});
			};

			timer.Start();
		}
	}
}
