![C#](https://img.shields.io/badge/-.NET%208.0-blueviolet?style=for-the-badge&logo=windows&logoColor=white) [![Support Server](https://img.shields.io/discord/477201632204161025.svg?label=Discord&logo=Discord&colorB=7289da&style=for-the-badge)](https://discord.gg/yyuggrH) ![GitHub](https://img.shields.io/github/license/IrisV3rm/RadiantConnect?style=for-the-badge) ![Nuget All Releases](https://img.shields.io/nuget/dt/RadiantConnect?label=Nuget%20Downloads&style=for-the-badge)

[Main Home Page](https://irisapp.ca/RadiantConnect/Home/index.html)

# RadiantConnect Quickstart Guide

## Credits

Valorant XMPP Integration: [Decieve](https://github.com/molenzwiebel/Deceive)

Valorant Version From Executable: [get_client_version.py](https://gist.github.com/floxay/a6bdacbd8db2298be602d330a43976da)

Valorant API Usage + Endpoints: [Valorant API Docs](https://valapidocs.techchrism.me/)

And all contributors :)
And can not stress this enough, the nice people at [Valorant App Developer Discord](https://discord.gg/a9yzrw3KAm) for assisting all the time. 

## Step 1: Download RadiantConnect from NuGet

Get started by downloading the RadiantConnect package from NuGet. You can find the package [here](https://www.nuget.org/packages/RadiantConnect).

```bash
dotnet add package RadiantConnect
```

## Step 2: Initialize RadiantConnect

Initialize the RadiantConnect library by creating an instance of the `Initiator` class.

```csharp
// Initialize RadiantConnect
Initiator Init = new Initiator();
```

## Step 3: Hook Desired Events

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
