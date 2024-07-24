using System.Text.Json.Serialization;

namespace RadiantConnect.Authentication.RiotAuth;

internal record RSOAuthReturn(
    string Type,
    string? Error,
    string Country,
    Response? Response
);

internal record Response(
    string Mode,
    Parameters Parameters
);

internal record Parameters(
    Uri Uri
);

internal record PutRiotRequest
{
    [JsonPropertyName("language")]
    public string? Language { get; init; }

    [JsonPropertyName("password")]
    public string? Password { get; init; }

    [JsonPropertyName("region")]
    public string? Region { get; init; }

    [JsonPropertyName("remember")]
    public bool Remember { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("username")]
    public string? Username { get; init; }
}

internal record RiotOpenId
{
    [JsonPropertyName("acr_values")]
    public string AcrValues { get; init; } = string.Empty;

    [JsonPropertyName("claims")]
    public string Claims { get; init; } = string.Empty;

    [JsonPropertyName("client_id")]
    public string ClientId { get; init; } = string.Empty;

    [JsonPropertyName("code_challenge")]
    public string CodeChallenge { get; init; } = string.Empty;

    [JsonPropertyName("code_challenge_method")]
    public string CodeChallengeMethod { get; init; } = string.Empty;

    [JsonPropertyName("nonce")]
    public string Nonce { get; init; } = Guid.NewGuid().ToString();

    [JsonPropertyName("redirect_uri")]
    public Uri RedirectUri { get; init; } = new("http://localhost/redirect");

    [JsonPropertyName("response_type")]
    public string ResponseType { get; init; } = "token id_token";

    [JsonPropertyName("scope")]
    public string Scope { get; init; } = string.Empty;
}
