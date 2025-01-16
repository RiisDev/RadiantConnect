using RadiantConnect.Authentication.DriverRiotAuth.Records;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace RadiantConnect.Authentication
{
    internal static class Util
    {
        internal static string ParseAccessToken(string accessToken)
        {
            Regex accessTokenRegex = new("access_token=(.*?)&scope");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal static string ParseIdToken(string accessToken)
        {
            Regex accessTokenRegex = new("id_token=(.*?)&token_type");
            return accessTokenRegex.Match(accessToken).Groups[1].Value;
        }

        internal static (HttpClient, CookieContainer) BuildClient()
        {
            CookieContainer cookieContainer = new();
            return (
                new HttpClient(new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                    CookieContainer = cookieContainer,
                    AutomaticDecompression = DecompressionMethods.All,
                    ServerCertificateCustomValidationCallback = (_, _, _, _) => true
                }),
                cookieContainer
            );
        }

        internal static async Task<(string, string, object, string)> GetTokens(string accessToken)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get PAS token
            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            string pasToken = await response.Content.ReadAsStringAsync();

            // GetUserInfo 
            response = await httpClient.GetAsync("https://auth.riotgames.com/userinfo");
            string userInfo = await response.Content.ReadAsStringAsync();

            // Get entitlement token
            httpClient.DefaultRequestHeaders.Accept.Clear();
            response = await httpClient.PostAsync("https://entitlements.auth.riotgames.com/api/token/v1", new StringContent("{}", Encoding.UTF8, "application/json"));
            string entitlementToken = (await response.Content.ReadFromJsonAsync<EntitleReturn>())?.EntitlementsToken ?? "";

            // Get client config
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("X-Riot-Entitlements-JWT", entitlementToken);
            response = await httpClient.GetAsync("https://clientconfig.rpg.riotgames.com/api/v1/config/player?app=Riot%20Client");
            object clientConfig = await response.Content.ReadFromJsonAsync<object>() ?? new { };

            return (pasToken, entitlementToken, clientConfig, userInfo);
        }
    }
}
