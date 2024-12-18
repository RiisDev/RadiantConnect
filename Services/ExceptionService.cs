// ReSharper disable CheckNamespace

namespace RadiantConnect
{
    public class RadiantConnectException(string message) : Exception(message);
    public class RadiantConnectXMPPException(string message) : Exception(message);
    public class RadiantConnectAuthException(string message) : Exception(message);
    public class RadiantConnectShadowClientException(string message) : Exception(message);
}
