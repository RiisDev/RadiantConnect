# XMPP Remote Setup Guide

!!! Danger 
	The socket does not have any datatypes built in, it returns raw XML data from the socket

## Step 1: Initialize Auth Records

To use XMPP you must use the [RSOAuth](https://irisapp.ca/RadiantConnect/DataTypes/Authentication/RIot%20Sign%20On/) record type
You may also use the built in login methods to provide RSOAuth

**Note: Initiator can be constructed in many different ways please read the [How To Documentation](https://irisapp.ca/RadiantConnect/Extra%20Data%20%26%20How%20To/How%20To/)**


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