using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.Security;
using System.Security.Authentication;
using System.Text.Json.Nodes;
using Microsoft.IdentityModel.JsonWebTokens;
using static RadiantConnect.Authentication.RiotAuth.RSOClientType;

namespace RadiantConnect.Authentication.RiotAuth;

public class RSOClientType
{
    public enum ClientType
    {
        Riot,
        Valorant
    }
}

internal class RSOClient(string username, string password, ClientType clientType)
{
    internal static Dictionary<ClientType, RiotOpenId> ClientData = new()
    {
        { ClientType.Riot, new RiotOpenId {ClientId = "riot-client", Scope = "openid link ban lol_region account"} },
        { ClientType.Valorant, new RiotOpenId {ClientId = "valorant-client", Scope = "openid ban link account"} }
    };

    internal HttpClient BuildClient()
    {
        HttpClient httpClient = new(new SocketsHttpHandler
        {
            SslOptions = new SslClientAuthenticationOptions
            {
                EnabledSslProtocols = SslProtocols.Tls13
            }
        });

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "RiotAuth/0.1");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/json");
        httpClient.BaseAddress = new Uri("https://auth.riotgames.com");
        return httpClient;
    }

    internal RSOMethods BuildSignOn()
    {
        return new RSOMethods(username, password, Http);
    }

    private HttpClient Http => BuildClient();
    private RSOMethods RiotSignOn => BuildSignOn();
    private RiotOpenId RiotOpenId => ClientData[clientType];

    private JsonWebToken? _accessToken;
    private JsonWebToken? _idToken;
    private string? _puuid;
    private string? _userInfo;
    
    public async Task<JsonWebToken> GetAccessTokenAsync()
    {
        if (_accessToken?.ValidTo > DateTime.UtcNow.AddMinutes(5))
            return _accessToken;

        Uri uri = await RiotSignOn.GetAuthResponseUriAsync(RiotOpenId);
        _accessToken = RiotSignOn.GetAccessToken(uri);
        return _accessToken ?? throw new InvalidOperationException("Unable to fetch access token.");
    }

    public async Task<JsonWebToken> GetIdTokenAsync()
    {
        if (_idToken?.ValidTo > DateTime.UtcNow.AddMinutes(5))
            return _idToken;

        Uri uri = await RiotSignOn.GetAuthResponseUriAsync(RiotOpenId);
        _idToken = RiotSignOn.GetIdToken(uri);
        return _idToken ?? throw new InvalidOperationException("Unable to fetch id token.");
    }

    public async Task<string> GetUserInfoAsync(JsonWebToken? accessToken = null)
    {
        if (_userInfo is not null)
            return _userInfo;

        accessToken ??= await GetAccessTokenAsync();
        _userInfo = await RiotSignOn.GetUserInfoAsync(accessToken);
        return _userInfo;
    }

    public async Task<string> GetPuuidAsync()
    {
        if (_puuid != null)
        {
            return _puuid;
        }

        JsonWebToken? token = _accessToken ?? _idToken;

        if (token != null)
        {
            _puuid = RiotSignOn.GetPuuid(token);
        }
        else
        {
            JsonWebToken accessToken = await GetAccessTokenAsync();
            _puuid = RiotSignOn.GetPuuid(accessToken);
        }

        return _puuid;
    }

    public async Task<string> GetEntitlementsTokenAsync(JsonWebToken? accessToken = null)
    {
        accessToken ??= await GetAccessTokenAsync();

        HttpRequestMessage request = new(HttpMethod.Post, "https://entitlements.auth.riotgames.com/api/token/v1"); // https://entitlements.esports.rpg.riotgames.com/api/token/v1
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.EncodedToken);
        request.Content = JsonContent.Create(new { urn = "urn:entitlement:%" });

        HttpResponseMessage response = await Http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        JsonNode? json = await response.Content.ReadFromJsonAsync<JsonNode>();

        return json?["entitlements_token"]?.GetValue<string>() ?? throw new InvalidOperationException("entitlements didn't respond with a token.");
    }
}
