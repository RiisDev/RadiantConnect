using Microsoft.IdentityModel.JsonWebTokens;
using RadiantConnect.Authentication.RiotAuth;

namespace RadiantConnect.Authentication
{
    public class Authentication
    {
        public record RSOAuth(string AccessToken, string IdToken, string EntitlementToken, string UserInfo, string Puuid, string Pas);

        public static async Task<RSOAuth?> Authenticate(string username, string password)
        {
            RSOClient valorantClient = new(
                username,
                password,
                RSOClientType.ClientType.Valorant
            );

            JsonWebToken? accessToken = await valorantClient.GetAccessTokenAsync();

            if (accessToken is null)
                throw new RadiantConnectException("Failed to get access token.");

            Task<JsonWebToken> idTokenTask = valorantClient.GetIdTokenAsync();
            Task<string> userInfoTask = valorantClient.GetUserInfoAsync(accessToken);
            Task<string> entitlementTokenTask = valorantClient.GetEntitlementsTokenAsync(accessToken);
            Task<string> pasTokenTask = valorantClient.GetPasTokenAsync(accessToken);
            Task<string> puuidTask = valorantClient.GetPuuidAsync();

            await Task.WhenAll(idTokenTask, userInfoTask, entitlementTokenTask, puuidTask, pasTokenTask);

            return new RSOAuth(
                AccessToken: accessToken.EncodedToken,
                IdToken: idTokenTask.Result.EncodedToken,
                EntitlementToken: entitlementTokenTask.Result,
                UserInfo: userInfoTask.Result,
                Puuid: puuidTask.Result,
                Pas: pasTokenTask.Result
            );
        }

    }
}
