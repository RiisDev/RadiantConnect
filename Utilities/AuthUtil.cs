using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using RadiantConnect.Authentication.DriverRiotAuth.Records;

namespace RadiantConnect.Utilities
{
    internal static class AuthUtil
    {
        internal static string ParseAccessToken(string accessToken) => accessToken.ExtractValue(@"access_token=([^&\n\r]*)", 1); // Somehow broke this

        internal static string ParseIdToken(string accessToken) => accessToken.ExtractValue("id_token=(.*?)&token_type", 1);

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

        internal static async Task<(string, string, object, string, string)> GetTokens(string accessToken)
        {
            using HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Get PAS token
            HttpResponseMessage response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
            string pasToken = await response.Content.ReadAsStringAsync();

            // Get RMS PAS Token
            response = await httpClient.GetAsync("https://riot-geo.pas.si.riotgames.com/pas/v1/service/rms");
            string rmsToken = await response.Content.ReadAsStringAsync();

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
            object clientConfig;
            try
            {
                clientConfig = await response.Content.ReadFromJsonAsync<object>() ?? new { };
            }
            catch
            {
                clientConfig = new { data = "FAILED_TO_GRAB" };
            }

            return (pasToken, entitlementToken, clientConfig, userInfo, rmsToken);
        }

        internal static int GetFreePort()
        {
            using TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            return port;
        }


        internal static string GenerateNonce(int length = 16)
        {
            byte[] nonceBytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonceBytes);
            }
            return Convert.ToBase64String(nonceBytes);
        }
    }
}
