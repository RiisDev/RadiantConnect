using System.Net;
using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Methods;
using RadiantConnect.Utilities;
using Cookie = System.Net.Cookie;

namespace RadiantConnect.Authentication.SSIDReAuth
{
    internal class SSIDAuthManager
    {
        internal async Task<RSOAuth> SignIn(string ssid)
        {
            (HttpClient client, CookieContainer container) = AuthUtil.BuildClient();

            container.Add(new Cookie("ssid", ssid, "/", "auth.riotgames.com"));
            HttpResponseMessage response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "https://auth.riotgames.com/authorize?redirect_uri=https%3A%2F%2Fplayvalorant.com%2Fopt_in&client_id=play-valorant-web-prod&response_type=token%20id_token&nonce=1&scope=account%20openid"));

            string? validAuthUrl = response.RequestMessage?.RequestUri?.ToString();

            if (string.IsNullOrEmpty(validAuthUrl))
                throw new RadiantConnectAuthException("Failed to get Auth Url");

            string accessToken = AuthUtil.ParseAccessToken(validAuthUrl);
            string idToken = AuthUtil.ParseIdToken(validAuthUrl);
            (string pasToken, string entitlementToken, object clientConfig, string _) = await AuthUtil.GetTokens(accessToken);

            JsonWebToken token = new (accessToken);
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
            );
        }
    }
}
