using System.Text.Json.Serialization;

namespace RadiantConnect.Authentication.QRSignIn;

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

public record BuiltData(string LoginUrl, string Session, string SdkSid, string Cluster, string Suuid, string Timestamp);