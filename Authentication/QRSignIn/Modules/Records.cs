using System.Text.Json.Serialization;

namespace RadiantConnect.Authentication.QRSignIn.Modules;

public record Auth(
    [property: JsonPropertyName("auth_method")] string AuthMethod
);

public record Captcha(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("hcaptcha")] Hcaptcha Hcaptcha
);

public record Hcaptcha(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("data")] string Data
);

public record Stage3Return(
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("auth")] Auth Auth,
    [property: JsonPropertyName("captcha")] Captcha Captcha,
    [property: JsonPropertyName("suuid")] string Suuid,
    [property: JsonPropertyName("cluster")] string Cluster,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("timestamp")] string Timestamp,
    [property: JsonPropertyName("platform")] string Platform
);

public record BuiltData(string LoginUrl, string Session, string SdkSid, string Cluster, string Suuid, string Timestamp, string Language, Authentication.CountryCode CountryCode);

public record QrDataSuccess(
    [property: JsonPropertyName("type")] string? Type,
    [property: JsonPropertyName("success")] Success? Success,
    [property: JsonPropertyName("country")] string? Country,
    [property: JsonPropertyName("timestamp")] string? Timestamp,
    [property: JsonPropertyName("platform")] string? Platform
);

public record Success(
    [property: JsonPropertyName("login_token")] string? LoginToken,
    [property: JsonPropertyName("redirect_url")] string? RedirectUrl,
    [property: JsonPropertyName("is_console_link_session")] bool? IsConsoleLinkSession,
    [property: JsonPropertyName("auth_method")] string? AuthMethod,
    [property: JsonPropertyName("puuid")] string? Puuid
);