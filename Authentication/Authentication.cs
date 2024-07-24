using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.RiotAuth;

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        public record RSOAuth(string AccessToken, string IdToken, string EntitlementToken, string UserInfo, string Puuid);

        public static async Task<RSOAuth?> Authenticate(string username, string password)
        {
            try
            {
                RSOClient valorantClient = new(
                    username,
                    password,
                    RSOClientType.ClientType.Valorant
                );

                Task<JsonWebToken> accessTokenTask = valorantClient.GetAccessTokenAsync();
                Task<JsonWebToken> idTokenTask = valorantClient.GetIdTokenAsync();
                Task<Task<string>> userInfoTask = accessTokenTask.ContinueWith(t => valorantClient.GetUserInfoAsync(t.Result));
                Task<Task<string>> entitlementTokenTask = accessTokenTask.ContinueWith(t => valorantClient.GetEntitlementsTokenAsync(t.Result));
                Task<string> puuidTask = valorantClient.GetPuuidAsync();

                await Task.WhenAll(accessTokenTask, idTokenTask, userInfoTask, entitlementTokenTask, puuidTask);

                return new RSOAuth(
                    AccessToken:
                    accessTokenTask.Result.EncodedToken,
                    IdToken: idTokenTask.Result.EncodedToken,
                    EntitlementToken: entitlementTokenTask.Result.Result,
                    UserInfo: userInfoTask.Result.Result,
                    Puuid: puuidTask.Result
                );
            }
            catch (Exception e)
            {
                throw new RadiantConnectException($"Something went wrong while trying to sign in (Possible CF Block): {e}");
            }
        }

    }
}
