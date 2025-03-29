# XMPP Usage

!!! Info
	This is unofficial documentation that I've tracking and testing, it will most likely break.

!!! tip
	This will be updated as I find / discover more!

This can be used via XMPPController or with any of the XMPP options I have provided.

Huge credit to [@giorgi-o](https://github.com/giorgi-o/CrossPlatformPlaying/wiki/Riot-Games) for the initial help and info on presence data.
Huge credit to [@floxay](https://github.com/floxay/py-valorant-rich-presence/wiki/How-it-works-(and-snippets)) for detailed information on the valorant presence data.

## Getting The Friends List | Credit to giorgi above

```xml
<iq type="get" id="2"><query xmlns="jabber:iq:riotgames:roster" last_state="true" /></iq>
```

## Subscribe to Presence Events | Credit to giorgi above
Was found sending this XML subscribes to any presence data from your friends list / current match, the formatting will be listed below
```xml
<presence/>
```
??? Valorant Presence Data
	```json
	{
		"isValid": true | false,
		"sessionLoopState": "string", // "MENUS", "PREGAME", "INGAME"
		"partyOwnerSessionLoopState": "string", //"MENUS", "PREGAME", "INGAME"
		"customGameName": "string", // "TeamOne", "TeamTwo", "TeamSpectate"
		"customGameTeam": "string", // "TeamOne", "TeamTwo", "TeamSpectate"
		"partyOwnerMatchMap": "string", // "/Game/Maps/Ascent/Ascent", "/Game/Maps/Duality/Duality", "/Game/Maps/Triad/Triad", "/Game/Maps/Port/Port", "/Game/Maps/Bonsai/Bonsai", "/Game/Maps/Poveglia/Range" (possibly more, use the official content api to get maps)
		"partyOwnerMatchCurrentTeam": "string", // "Blue", "Red"
		"partyOwnerMatchScoreAllyTeam":  1, // int
		"partyOwnerMatchScoreEnemyTeam": 1, // int
		"partyOwnerProvisioningFlow": "string", // "CustomGame", "SkillTest", "Matchmaking", "NewPlayerExperience", "Invalid"
		"provisioningFlow": "string", // "CustomGame", "SkillTest", "Matchmaking", "NewPlayerExperience", "Invalid"
		"matchMap": "string", // "/Game/Maps/Ascent/Ascent", "/Game/Maps/Duality/Duality", "/Game/Maps/Triad/Triad", "/Game/Maps/Port/Port", "/Game/Maps/Bonsai/Bonsai", "/Game/Maps/Poveglia/Range"
		"partyId": "string", // UUID
		"isPartyOwner": true | false,
		"partyName": "",
		"partyState": "string", // "DEFAULT", "CUSTOM_GAME_SETUP" "CUSTOM_GAME_STARTING", "MATCHMAKING", "STARTING_MATCHMAKING", "LEAVING_MATCHMAKING", "MATCHMADE_GAME_STARTING", "SOLO_EXPERIENCE_STARTING"
		"partyAccessibility": "string", // "CLOSED", "OPEN"
		"maxPartySize": 1,
		"queueId": "string", // "spikerush", "competitive", "deathmatch", "unrated", "snowball" or empty str (custom game)
		"partyLFM": "string", // bool
		"partyClientVersion": "string", // "release-02.01-shipping-6-511946"
		"partySize": 1,
		"partyVersion": 1234567, // unix epoch timestamp
		"queueEntryTime": "string", // "2021.01.01-01.01.01"
		"playerCardId": "string", // UUID
		"playerTitleId": "string", // UUID
		"isIdle":  true | false, // (away state)
	}
	```

## Getting Private Messages
This will was found to return / summon the previous chat messages with the desired user

| **Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` | The Recipitent UUID  |
| `string` | The communications affinity, `na1` |

```xml
 <iq type="get" id="get_archive_7">
	<query xmlns="jabber:iq:riotgames:archive">
		<with>{recipient}@{_affinity}.pvp.net</with>
	</query>
</iq>
```

## Sending Chat Messages
Note I found for this, the ID must be a unique unix timestamp followed by a :1.

| **Parameter Type** | **Method Parameter Example** |
|------------------------|--------------------------|
| `string` | The Recipitent UUID  |
| `string` | The communications affinity, `na1` |

```xml
<message id="{GetUnixTimestamp()}:1" to="{recipient}@{_affinity}.pvp.net" type="chat">
	<body>{message}</body>
</message>
```