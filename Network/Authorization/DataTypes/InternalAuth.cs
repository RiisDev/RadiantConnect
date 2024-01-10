namespace RadiantConnect.Network.Authorization.DataTypes;
// ReSharper disable All
public record AuthorizationHeaders(
    string XRiotEntitlementsJWT,
    string Bearer
);

public record InternalAuth(
    string Username,
    string Password,
    string UserId,
    bool HasMultiAuthenticator,
    ClientVersion ClientVersion,
    AuthorizationHeaders? AuthorizationHeaders
);