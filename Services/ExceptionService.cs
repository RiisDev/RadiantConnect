// ReSharper disable CheckNamespace

#pragma warning disable CA1032

namespace RadiantConnect
{
	/// <summary>
	/// Base exception type for RadiantConnect-related errors.
	/// </summary>
	public class RadiantConnectException(string message) : Exception(message);

	/// <summary>
	/// Exception thrown when the network request to ValorantNet fails.
	/// </summary>
	public class RadiantConnectNetworkStatusException(string statusCode)
		: Exception($"ValorantNet failed with data: {statusCode}");

	/// <summary>
	/// Exception thrown when an XMPP-related error occurs in RadiantConnect.
	/// </summary>
	public class RadiantConnectXMPPException(string message) : Exception(message);

	/// <summary>
	/// Exception thrown during authentication failures in RadiantConnect.
	/// </summary>
	public class RadiantConnectAuthException(string message) : Exception(message);
}
