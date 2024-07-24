using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Microsoft.IdentityModel.JsonWebTokens;

namespace RadiantConnect.Authentication.RiotAuth;

internal class RSOMethods(string username, string password, HttpClient httpClient)
{
    public JsonWebToken GetAccessToken(Uri uri)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(uri.Fragment.TrimStart('#'));
        return new JsonWebToken(collection["access_token"]);
    }

    public JsonWebToken GetIdToken(Uri uri)
    {
        NameValueCollection collection = HttpUtility.ParseQueryString(uri.Fragment.TrimStart('#'));
        return new JsonWebToken(collection["id_token"]);
    }

    public string GetPuuid(JsonWebToken jwt) => jwt.Subject;

    public async Task<string> GetUserInfoAsync(JsonWebToken accessToken)
    {
        HttpRequestMessage request = new(HttpMethod.Get, "/userinfo");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.EncodedToken);
        HttpResponseMessage response = await httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<Uri> GetAuthResponseUriAsync(RiotOpenId post)
    {
        HttpResponseMessage message = await PostAuthRequestAsync(post);
        return await GetAuthResponseUriAsync(message);
    }

    private async Task<Uri> GetAuthResponseUriAsync(HttpResponseMessage message)
    {
        RSOAuthReturn? authorization = await message.Content.ReadFromJsonAsync<RSOAuthReturn>();

        if (authorization is { Error: null, Type: "auth" })
        {
            message = await PutAuthRequestAsync();
            authorization = await message.Content.ReadFromJsonAsync<RSOAuthReturn>();
        }

        return authorization switch
        {
            { Error: not null } => throw new ApplicationException($"Authorization failed: {authorization.Error}"),
            { Type: "response", Response: not null } => authorization.Response.Parameters.Uri,
            _ => throw new ApplicationException("Authorization failed: Cannot deserialize response.")
        };
    }

    private Task<HttpResponseMessage> PostAuthRequestAsync(RiotOpenId post) => httpClient.PostAsJsonAsync("/api/v1/authorization", post);

    private Task<HttpResponseMessage> PutAuthRequestAsync()
    {
        return httpClient.PutAsJsonAsync("/api/v1/authorization", new PutRiotRequest
        {
            Language = "en_US",
            Password = password,
            Region = null,
            Remember = true,
            Type = "auth",
            Username = username
        });
    }

    public Task<HttpResponseMessage> DeleteSessionAsync() => httpClient.DeleteAsync("/api/v1/session");
}
