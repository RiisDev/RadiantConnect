---
hide:
  - navigation
---
# RadiantConnect Quickstart Guide

## Step 1: Download RadiantConnect from NuGet

Get started by downloading the RadiantConnect package from NuGet. You can find the package [here](https://www.nuget.org/packages/RadiantConnect).

```bash
dotnet add package RadiantConnect
```

## Step 2: Initialize RadiantConnect

Initialize the RadiantConnect library by creating an instance of the `Initiator` class.

**Note: Initiator can be constructed in many different ways please read the [How To Documentation](https://irisapp.ca/RadiantConnect/Extra%20Data%20%26%20How%20To/How%20To/)**

## Step 3: If you're wanting any game hooks, hook desired events

Hook into the events related to the game queue to respond to various states.

```csharp
// Hook into Queue events
Init.GameEvents.Queue.OnEnteredQueue += _ => {
	Debug.WriteLine("Queue Entered");
};

Init.GameEvents.Queue.OnLeftQueue += _ => {
	Debug.WriteLine("Queue Left");
};

Init.GameEvents.Queue.OnQueueChanged += queueChangeType => {
	Debug.WriteLine($"Queue Changed to: {queueChangeType}");
};
```

## Step 4: Use Desired API Calls

Utilize the RadiantConnect API to make calls that suit your application needs. In this example, we fetch a player's MMR asynchronously.

```csharp
// Fetch Player MMR asynchronously
PlayerMMR? playerMMR = await Init.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(Init.ExternalSystem.ClientData.UserId);

Debug.WriteLine($"Player MMR: {playerMMR}");
```

## Need Support?

If you have any questions, issues, or need assistance, feel free to join our Discord server. Our community is here to help!

[**Join our Discord Community**](https://discord.gg/yyuggrH)

Feel free to explore additional API calls and events provided by RadiantConnect to enhance the functionality of your integration.

---

This quickstart guide provides a simple walkthrough to get you started with RadiantConnect. Refer to the detailed documentation for a comprehensive understanding of available features and customization options. Happy coding!