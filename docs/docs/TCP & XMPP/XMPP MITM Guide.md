# XMPP MITM Setup Guide

---

# XMPP MITM Setup Guide (Updated)

!!! Danger
	The socket does **not** provide any automatic parsing — it only returns raw XMPP data as intended by Valorant. You **must** handle parsing yourself.
!!! Danger
	This setup uses **MITM (Man-In-The-Middle)** techniques. **Close all Riot services before running**.
  	If your software shuts down **before Riot**, chat services will remain offline until you restart all Riot clients.

---

## Overview

This guide explains how to set up `ValXMPP` for intercepting and interacting with Valorant’s XMPP chat system.
We now support **additional event hooks** and clearer separation between:

* **Proxy events** → Raw network-level in/out traffic.
* **Chat server events** → Raw XMPP client/server messages.

---

## Step 1: Kill Riot Processes

Before initializing anything, you **must** stop all Riot processes.

```csharp
ValXMPP.KillRiot();
Thread.Sleep(2000); // Ensure Riot has fully closed.
```

---

## Step 2: Initialize ValXMPP

Create and prepare the XMPP handler instance.

```csharp
ValXMPP xmpp = new();
```

---

## Step 3: Establish Connection

Start the underlying socket and connect to Riot services.

```csharp
xmpp.InitializeConnection();
```

---

## Step 4: Subscribe to Events

Here’s the **breakdown** of the available events:

---

### 🔶 **Network Proxy Events (Raw traffic)**

These capture **low-level traffic** going through the proxy — useful for debugging, doesn't offer much information for clients, but is still offered.

```csharp
xmpp.OnInboundMessage += (message) => 
    Debug.WriteLine($"[Proxy] InboundMessage: {message}");

xmpp.OnOutboundMessage += (message) => 
    Debug.WriteLine($"[Proxy] OutboundMessage: {message}");
```

---

### 🔶 **Chat Server Events (Parsed XMPP messages)**

These fire when the actual **chat/xmpp server** processes data:

```csharp
xmpp.OnClientMessage += (message) => 
    Debug.WriteLine($"[Chat] Client Message: {message}");

xmpp.OnServerMessage += (message) => 
    Debug.WriteLine($"[Chat] Server Message: {message}");
```

---

### 🔶 **Presence Update Events**

Presence updates come in **two forms**:

```csharp
// PlayerPresence — general player state
xmpp.OnPlayerPresenceUpdated += (presence) => 
    Debug.WriteLine($"Player Presence Updated: {JsonSerializer.Serialize(presence)}");

// ValorantPresence — Valorant-specific presence details
xmpp.OnValorantPresenceUpdated += (presence) => 
    Debug.WriteLine($"Valorant Presence Updated: {JsonSerializer.Serialize(presence)}");
```

---

## Final Example

Putting it all together:

```csharp
ValXMPP.KillRiot();

Thread.Sleep(2000);

ValXMPP xmpp = new();

xmpp.OnClientMessage += (message) => 
    Debug.WriteLine($"[Chat] Client Message: {message}");

xmpp.OnServerMessage += (message) => 
    Debug.WriteLine($"[Chat] Server Message: {message}");

xmpp.OnInboundMessage += (message) => 
    Debug.WriteLine($"[Proxy] InboundMessage: {message}");

xmpp.OnOutboundMessage += (message) => 
    Debug.WriteLine($"[Proxy] OutboundMessage: {message}");

xmpp.OnPlayerPresenceUpdated += (presence) => 
    Debug.WriteLine($"Player Presence Updated: {JsonSerializer.Serialize(presence)}");

xmpp.OnValorantPresenceUpdated += (presence) => 
    Debug.WriteLine($"Valorant Presence Updated: {JsonSerializer.Serialize(presence)}");

xmpp.InitializeConnection();
```

---

### Notes

✅ **OnClientMessage / OnServerMessage** → Use these for handling raw chat and XMPP connections.
✅ **OnInboundMessage / OnOutboundMessage** → Use these for observing **raw** proxy traffic, including non-chat data.
✅ **Presence Events** → Provide structured objects for player and game status updates.