﻿// ReSharper disable CheckNamespace

#pragma warning disable IDE0130

namespace RadiantConnect
{
	public class RadiantConnectException(string message) : Exception(message);
	public class RadiantConnectNetworkStatusException(string statusCode) : Exception($"ValorantNet failed with data: {statusCode}");
	public class RadiantConnectXMPPException(string message) : Exception(message);
	public class RadiantConnectAuthException(string message) : Exception(message);
}
