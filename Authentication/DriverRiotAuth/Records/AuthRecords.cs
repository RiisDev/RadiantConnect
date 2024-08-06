using System.Text.Json.Serialization;

namespace RadiantConnect.Authentication.DriverRiotAuth.Records;

internal record EntitleReturn(
    [property: JsonPropertyName("entitlements_token")] string EntitlementsToken
);

internal record Alias(
    [property: JsonPropertyName("game_name")] string GameName,
    [property: JsonPropertyName("tag_line")] string TagLine
);

internal record Mfa(
    [property: JsonPropertyName("verified")] bool? Verified
);

internal record UserInfo(
    [property: JsonPropertyName("sub")] string Sub,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("locale")] string Locale,
    [property: JsonPropertyName("username")] string Username,
    [property: JsonPropertyName("mfa")] Mfa Mfa,
    [property: JsonPropertyName("country")] string Country,
    [property: JsonPropertyName("email_status")] string EmailStatus,
    [property: JsonPropertyName("alias")] Alias Alias,
    [property: JsonPropertyName("amr")] IReadOnlyList<string> Amr,
    [property: JsonPropertyName("birth_date")] string BirthDate
);