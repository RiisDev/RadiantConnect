using System.Collections.Specialized;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using Microsoft.IdentityModel.JsonWebTokens;

namespace RadiantConnect.Authentication.RiotAuth;

internal class RSOMethods(string username, string password, RSOClient client)
{
    private readonly HttpClient _httpClient = client.Http;
    private CookieContainer _cookies = client.Cookies;

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
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();
    }

    public async Task<string> GetPasTokenAsync(JsonWebToken accessToken)
    {
        HttpRequestMessage request = new(HttpMethod.Get, "https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.EncodedToken);
        HttpResponseMessage response = await _httpClient.SendAsync(request);
        return await response.Content.ReadAsStringAsync();

    }

    private bool called = false;

    public async Task<Uri> GetAuthResponseUriAsync(RiotOpenId post)
    {
        HttpResponseMessage message = await PostAuthRequestAsync(post);

#if DEBUG
        string content = await message.Content.ReadAsStringAsync();
        if (message.StatusCode != HttpStatusCode.OK)
            throw new RadiantConnectException($"Authorization failed:\n{JsonSerializer.Serialize(message)}\n\nReturn:\n{content}\n\nHeaders:\n{message.RequestMessage?.Headers}");
#else
        if (message.StatusCode != HttpStatusCode.OK)
            throw new RadiantConnectException($"Authorization failed: {message.StatusCode}");
#endif

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

    private Task<HttpResponseMessage> PostAuthRequestAsync(RiotOpenId post) => _httpClient.PostAsJsonAsync("/api/v1/authorization", post);

    private Task<HttpResponseMessage> PutAuthRequestAsync()
    {
        return _httpClient.PutAsJsonAsync("/api/v1/authorization", new PutRiotRequest
        {
            Language = "en_US",
            Password = password,
            Region = null,
            Remember = true,
            Type = "auth",
            Username = username
        });
    }

    public Task<HttpResponseMessage> DeleteSessionAsync() => _httpClient.DeleteAsync("/api/v1/session");
}
