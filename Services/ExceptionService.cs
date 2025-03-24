// ReSharper disable CheckNamespace

using System.Net;

namespace RadiantConnect
{
    public class RadiantConnectException(string message) : Exception(message);
    public class RadiantConnectNetworkException(string message) : Exception(message);
    public class RadiantConnectNetworkStatusException(HttpStatusCode statusCode) : Exception($"ValorantNet returned status code; {statusCode}");
    public class RadiantConnectXMPPException(string message) : Exception(message);
    public class RadiantConnectAuthException(string message) : Exception(message);
    public class RadiantConnectShadowClientException(string message) : Exception(message);
}
