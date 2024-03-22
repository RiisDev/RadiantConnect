# XMPP Setup Guide

!! danger
    The socket does not have any datatypes built in, it returns the xmp data that valorant intended you must parse by yourself!
!! danger
    This setup uses MITM (Man In The Middle) all riot services will need to be closed before running, if your software closes before riot your chat services will be offline until you restart your Riot Clients

## Step 1: Initialize RadiantConnect 

Initialize the XMPP Instance, it's suggested to run the KillRiot method before continuing

```csharp
ValXMPP.KillRiot();

Thread.Sleep(2000);

ValXMPP chatServer = new ValXMPP();
```

## Step 2: Initialize the socket connection

This will begin the actual socket connection and will start a Riot instance.

```csharp

chatServer.InitializeConnection();
```

## Step 3: Subscribe to the message events

This is where you'll receive the messages from server and client

```csharp
chatServer.OnServerMessage += (data)=>{
    Debug.WriteLine($"SERVER MESSAGE: {data}");
};
chatServer.OnClientMessage += (data)=>{
    Debug.WriteLine($"CLIENT MESSAGE: {data}");
};
```

## Step 3.1: Subscribe to new socket connections **Required for sending messages**
```csharp
chatServer.OnSocketCreated += (socketHandle) => {
    socketHandle.SendXmlMessageAsync(/*XML String*/);
};
```

## Step 3.2: Subscribe to valorant presence update event

```csharp
chatServer.OnValorantPresenceUpdated += (valorantPresence) => {
    // valorantPresence returns ValorantPresence data type
};
```

## Step 4: Profit

In the end it should look something like this example

```csharp
ValXMPP.KillRiot();

Thread.Sleep(2000);

ValXMPP chatServer = new();

chatServer.InitializeConnection();

chatServer.OnServerMessage += (data)=>{
    Debug.WriteLine($"SERVER MESSAGE: {data}");
};

chatServer.OnClientMessage += (data)=>{
    Debug.WriteLine($"SERVER MESSAGE: {data}");
};

chatServer.OnSocketCreated += (socketHandle) => {
    socketHandle.SendXmlMessageAsync("</presence>");
};

chatServer.OnValorantPresenceUpdated += (valorantPresence) =>
{
    Debug.WriteLine(valorantPresence.PartyId);
};
```