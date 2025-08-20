using RadiantConnect.Authentication.DriverRiotAuth.Records;
using Cookie = System.Net.Cookie;

namespace RadiantConnect.Authentication.SSIDReAuth
{
    internal class SsidAuthManager
    {
	    internal static async Task<RSOAuth> Authenticate(string ssid, string? clid = "", string? csid = "", string? tdid = "", WebProxy? proxy = null)
	    {
		    CookieContainer container = new();
		    HttpClient client = new (new HttpClientHandler
		    {
			    AllowAutoRedirect = true,
			    CookieContainer = container,
			    AutomaticDecompression = DecompressionMethods.All,
			    ServerCertificateCustomValidationCallback = (_, _, _, _) => true,
				Proxy = proxy
		    });

			container.Add(new Cookie("ssid", ssid, "/", "auth.riotgames.com"));
		    container.Add(new Cookie("clid", clid, "/", "auth.riotgames.com"));
		    container.Add(new Cookie("csid", csid, "/", "auth.riotgames.com"));
		    container.Add(new Cookie("tdid", tdid, "/", "auth.riotgames.com"));

		    HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get,
			    "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid"));

		    string? validAuthUrl = response.RequestMessage?.RequestUri?.ToString();

		    if (validAuthUrl.IsNullOrEmpty())
			    throw new RadiantConnectAuthException("Failed to get Auth Url");

		    if (!validAuthUrl.Contains("access_token"))
			    throw new RadiantConnectAuthException(
				    "Failed to find access tokens in auth, note in certain regions you must specify CLID");

		    if (!validAuthUrl.Contains("id_token"))
			    throw new RadiantConnectAuthException(
				    "Failed to find id tokens in auth, note in certain regions you must specify CLID");

		    string accessToken = AuthUtil.ParseAccessToken(validAuthUrl);
		    string idToken = AuthUtil.ParseIdToken(validAuthUrl);
		    (string pasToken, string entitlementToken, object clientConfig, string _, string rmsToken) =
			    await AuthUtil.GetAuthTokensFromAccessToken(accessToken);

		    JsonWebToken token = new(accessToken);
		    string suuid = token.Subject;

		    JsonWebToken affinityJwt = new(pasToken);
		    string? affinity = affinityJwt.GetPayloadValue<string>("affinity");
		    string? chatAffinity = affinityJwt.GetPayloadValue<string>("desired.affinity");

		    CookieCollection cookies = container.GetAllCookies();

		    client.Dispose();

		    return new RSOAuth(
			    suuid,
			    ssid,
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
		    ) { RmsToken = rmsToken };
	    }
    }
}
