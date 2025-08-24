# RadiantConnect

RadiantConnect is a powerful .NET library designed for seamless integration with Valorant's internal and external APIs and systems, users can hook game events, access XMPP systems, and authenticate all within the library.

With the addition of MITM-based XMPP integration, users can authenticate and interact with messaging and presence services effectively without hassle.

For a complete and detailed guide on all features, check out the official documentation:

[**RadiantConnect Documentation**](https://docs.radiantconnect.ca/csharp)

![C#](https://img.shields.io/badge/-.NET%208.0-blueviolet?style=for-the-badge&logo=windows&logoColor=white)
[![Support Server](https://img.shields.io/discord/477201632204161025.svg?label=Discord&logo=Discord&colorB=7289da&style=for-the-badge)](https://discord.gg/yyuggrH)
![GitHub](https://img.shields.io/github/license/RiisDev/RadiantConnect?style=for-the-badge)
![Nuget All Releases](https://img.shields.io/nuget/dt/RadiantConnect?label=Nuget%20Downloads&style=for-the-badge)
![Build](https://img.shields.io/github/actions/workflow/status/RiisDev/RadiantConnect/dotnet.yml?style=for-the-badge)
![Daily Table Test](https://img.shields.io/github/actions/workflow/status/RiisDev/RadiantConnect/daily.yml?style=for-the-badge&label=tables)

---

## Features

### **Basic API Usage**

RadiantConnect provides several useful methods to interact with Valorant's player data. Learn more about how to use the following APIs:

- **[PVP Methods](https://docs.radiantconnect.ca/csharp/api/pvp.html)**
  - GetMatchHistory
  - GetUserRank
  - And much much more.

### **Create Your Own API Endpoints**

RadiantConnect allows you to build your own API calls if RadiantConnect doesn't support it, using the built in ValorantNet library handles all authentication for you.

- **[Custom API Call](https://docs.radiantconnect.ca/csharp/services/valorantnet.html)**

### **Authentication Features**

RadiantConnect supports a variety of authentication methods to simplify user logins. Find out how to implement the following authentication methods:

- **[QR Code Authentication](https://docs.radiantconnect.ca/csharp/authentication/index.html#2-authenticatewithqr)**
- **[Automated Web Driver Authentication](https://docs.radiantconnect.ca/csharp/authentication/index.html#3-authenticatewithdriver)**
- **[SSID Authentication](https://docs.radiantconnect.ca/csharp/authentication/index.html#1-authenticatewithssid)**

### **Valorant Socket Support**

RadiantConnect supports various socket integrations for enhanced communication with the game. Learn more about the integration options:

- **[XMPP Integration](hhttps://docs.radiantconnect.ca/csharp/sockets/xmpp.html)**
- **[XMPP MITM Integration](https://docs.radiantconnect.ca/csharp/sockets/xmpp-mitm.html)**
- **[Local TCP Integration](hhttps://docs.radiantconnect.ca/csharp/sockets/tcp.html)**

### **Client Events**

RadiantConnect provides hooks for a variety of in-game events. Learn more about these events and how to use them:

- **[In-Game Events](https://docs.radiantconnect.ca/csharp/events/ingame.html)**
  - OnBuyMenuOpened
  - OnBuyMenuClosed
- **[Match Events](https://docs.radiantconnect.ca/csharp/events/match.html)**
  - Match\_Started
  - OnMapLoaded
- **[Menu Events](https://docs.radiantconnect.ca/csharp/events/menu.html)**
  - OnBattlePassView
  - OnAgentsView
  - OnCareerView
  - OnPlayScreen
  - OnEsportView
  - OnCollectionView
  - OnStoreView
  - OnPremierView
- **[Pre-Game Events](hhttps://docs.radiantconnect.ca/csharp/events/pregame.html)**
  - OnPreGamePlayerLoaded
  - OnPreGameMatchLoaded
  - OnAgentLockedIn
  - OnAgentSelected
- **[Queue Events](https://docs.radiantconnect.ca/csharp/events/queue.html)**
  - OnQueueChanged
  - OnEnteredQueue
  - OnLeftQueue
  - OnCustomGameLobbyCreated
  - OnTravelToMenu
  - OnMatchFound
- **[Round Events](https://docs.radiantconnect.ca/csharp/events/round.html)**
  - OnRoundStarted
  - OnRoundEnded
- **[Vote Events](hhttps://docs.radiantconnect.ca/csharp/events/vote.html)**
  - OnVoteDeclared
  - OnVoteInvoked
  - OnSurrenderCalled
  - OnTimeoutCalled
  - OnRemakeCalled

---

## Need Support?

If you have any questions, issues, or need assistance, feel free to join our Discord community. We're here to help!

**[Join our Discord Community](https://discord.gg/yyuggrH)**

You can also check out our [FAQ](https://discord.gg/yyuggrH) or [Submit an Issue](https://github.com/IrisV3rm/RadiantConnect/issues).

---

## Credits

### Huge Credits molenzwiebel for his Deceive code, helping make MITM Connection

- XMPP MITM Integration: @molenzwiebel | [Decieve](https://github.com/molenzwiebel/Deceive)
- Get Version From Executable: @floxay | [get_client_version.py](https://gist.github.com/floxay/a6bdacbd8db2298be602d330a43976da)
- Riot API Documentaiton: @techchrism | [Valorant API Docs](https://valapidocs.techchrism.me/)
- And can not stress this enough, the nice people at [Valorant App Developer Discord](https://discord.gg/a9yzrw3KAm) for assisting all the time.
