// ReSharper disable CheckNamespace

namespace RadiantConnect
{
    public class RadiantConnectException(string message) : Exception(message);
    public class RadiantConnectXMPPException(string message) : Exception(message);
}
