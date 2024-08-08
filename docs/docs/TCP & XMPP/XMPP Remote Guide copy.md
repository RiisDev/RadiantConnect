# XMPP Remote Setup Guide

!!! Danger 
	The socket does not have any datatypes built in, it returns raw XML data from the socket

## Step 1: Initialize Auth Records

To use XMPP you must use the RSOAuth record type, specifically: Affinity, AccessToken, PasToken, Entitlement Token
You may also use the built in login methods to provide RSOAuth

```csharp
// Initialize Auth Client
Authentication authentication = new();

authentication.OnMultiFactorRequested += () =>
{
	Console.WriteLine("Enter MultiFactor Code: ");
	authentication.MultiFactorCode = Console.ReadLine();
};
authentication.OnDriverUpdate += status => Console.WriteLine($"[Driver] Authentication.DriverStatus.{status}");

DriverSettings driverSettings = new DriverSettings(
	string ProcessName = "msedge",
	string BrowserExecutable = @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe",
	bool KillBrowser = false,
	bool CacheCookies = true
);

RSOAuth rsoAuthData = authentication.AuthenticateWithDriver(username, password, driverSettings);

// Or if you can provide your own

RSOAuth rsoAuthData = new RSOAuth(
	Affinity: "na1",
	AccessToken: "accessToken",
	PasToken: "pasToken",
	Entitlement: "entitlementToken"
);

```

## Step 2: Connect to XMPP Server

This will begin the actual socket connection using provided RSOAuth.

```csharp
RemoteXMPP xmpp = new();
xmpp.OnMessage += data => Console.WriteLine($"[Message] {data}");
xmpp.OnXMPPProgress += status => Console.WriteLine($"[XMPP] XMPPStatus.{status}");
xmpp.OnXMPPProgress += status => Debug.WriteLine($"[XMPP] XMPPStatus.{status}");

await xmpp.InitiateRemoteXMPP(rsoAuthData);
```

## Step 3: Send Messages to XMPP

How to send messages to XMPP Client.

```csharp
await xmpp.SendMessage("<iq type=\"get\" id=\"2\"><query xmlns=\"jabber:iq:riotgames:roster\" last_state=\"true\" /></iq>");
```