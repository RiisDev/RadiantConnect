# Val Socket Setup Guide

!!! Danger 
	The socket does not have any datatypes built in, it returns the json data that valorant intended you must parse by yourself!

## Step 1: Initialize RadiantConnect 

Initialize the RadiantConnect library by creating an instance of the `Initiator` class.

```csharp
Initiator Init = new Initiator();
```

## Step 2: Initialize ValSocket

Initialize the Socket library for usage.

```csharp
ValSocket socket = new(ValorantNet.GetAuth(), init);
```

## Step 3: Initialize the socket connection

This will begin the actual socket connection as well as subscribing to all the events

```csharp
socket.InitializeConnection();
```

## Step 4: Subscribe to the OnNewMessage event

This is where you'll receive the messages

```csharp
socket.OnNewMessage += (data) =>
{
	Debug.WriteLine($"Socket Message: {data}");
};
```

## Step 5: Profit

In the end it should look something like this example

```csharp
Initiator init = new();
ValSocket socket = new(ValorantNet.GetAuth()!, init);
socket.InitializeConnection();
socket.OnNewMessage += (data) =>
{
	Debug.WriteLine($"Socket Message: {data}");
};
```