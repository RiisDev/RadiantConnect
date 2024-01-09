## CheatData Record

The `CheatData` record holds cheat-related data within the RadiantConnect network.

### Properties

#### `GamePodOverride`

- Type: `string`
- Description: Represents the overridden game pod for cheating.

#### `ForcePostGameProcessing`

- Type: `bool?`
- Description: Indicates whether to force post-game processing.

---

## CustomGameData Record

The `CustomGameData` record represents custom game data within the RadiantConnect network.

### Properties

#### `Settings`

- Type: `Settings`
- Description: Represents the settings for the custom game.

#### `Membership`

- Type: `Membership`
- Description: Represents the membership information for the custom game.

#### `MaxPartySize`

- Type: `long?`
- Description: Represents the maximum party size for the custom game.

#### `AutobalanceEnabled`

- Type: `bool?`
- Description: Indicates whether autobalance is enabled for the custom game.

#### `AutobalanceMinPlayers`

- Type: `long?`
- Description: Represents the minimum players for autobalance in the custom game.

#### `HasRecoveryData`

- Type: `bool?`
- Description: Indicates whether the custom game has recovery data.

---

## ErrorNotification Record

The `ErrorNotification` record represents error notifications within the RadiantConnect network.

### Properties

#### `ErrorType`

- Type: `string`
- Description: Represents the type of error.

#### `ErroredPlayers`

- Type: `object`
- Description: Represents the players who encountered an error.

---

## MatchmakingData Record

The `MatchmakingData` record represents matchmaking data within the RadiantConnect network.

### Properties

#### `QueueID`

- Type: `string`
- Description: Represents the queue ID for matchmaking.

#### `PreferredGamePods`

- Type: `IReadOnlyList<string>`
- Description: Represents the preferred game pods for matchmaking.

#### `SkillDisparityRRPenalty`

- Type: `long?`
- Description: Represents the skill disparity RR penalty for matchmaking.

---

## Member Record

The `Member` record represents a member within a party in the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the subject of the member.

#### `CompetitiveTier`

- Type: `long?`
- Description: Represents the competitive tier of the member.

#### `PlayerIdentity`

- Type: `PlayerIdentity`
- Description: Represents the player identity information of the member.

#### `SeasonalBadgeInfo`

- Type: `object`
- Description: Represents the seasonal badge information of the member.

#### `IsOwner`

- Type: `bool?`
- Description: Indicates whether the member is the owner of the party.

#### `QueueEligibleRemainingAccountLevels`

- Type: `long?`
- Description: Represents the remaining account levels eligible for the queue.

#### `Pings`

- Type: `IReadOnlyList<PingInternal>`
- Description: Represents the pings associated with the member.

#### `IsReady`

- Type: `bool?`
- Description: Indicates whether the member is ready.

#### `IsModerator`

- Type: `bool?`
- Description: Indicates whether the member is a moderator.

#### `UseBroadcastHUD`

- Type: `bool?`
- Description: Indicates whether to use the broadcast HUD.

#### `PlatformType`

- Type: `string`
- Description: Represents the platform type of the member.

---

## Membership Record

The `Membership` record represents the membership information within the RadiantConnect network.

### Properties

#### `TeamOne`

- Type: `object`
- Description: Represents team one in the membership.

#### `TeamTwo`

- Type: `object`
- Description: Represents team two in the membership.

#### `TeamSpectate`

- Type: `object`
- Description: Represents the spectate team in the membership.

#### `TeamOneCoaches`

- Type: `object`
- Description: Represents the coaches for team one in the membership.

#### `TeamTwoCoaches`

- Type: `object`
- Description: Represents the coaches for team two in the membership.

---

## PingInternal Record

The `PingInternal` record represents internal ping data within the RadiantConnect network.

### Properties

#### `Ping`

- Type: `long?`
- Description: Represents the ping value.

#### `GamePodID`

- Type: `string`
- Description: Represents the GamePod ID associated with the ping.

---

## PlayerIdentity Record

The `PlayerIdentity` record represents player identity information within the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the subject of the player identity.

#### `PlayerCardID`

- Type: `string`
- Description: Represents the player card ID associated with the player identity.

#### `PlayerTitleID`

- Type: `string`
- Description: Represents the player title ID associated with the player identity.

#### `AccountLevel`

- Type: `long?`
- Description: Represents the account level of the player identity.

#### `PreferredLevelBorderID`

- Type: `string`
- Description: Represents the preferred level border ID of the player identity.

#### `Incognito`

- Type: `bool?`
- Description: Indicates whether the player identity is incognito.

#### `HideAccountLevel`

- Type: `bool?`
- Description: Indicates whether to hide the account level.

---

## Party Record

The `Party` record represents a party within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the ID of the party.

#### `MUCName`

- Type: `string`
- Description: Represents the MUC name of the party.

#### `VoiceRoomID`

- Type: `string`
- Description: Represents the voice room ID of the party.

#### `Version`

- Type: `long?`
- Description: Represents the version of the party.

#### `ClientVersion`

- Type: `string`
- Description: Represents the client version of the party.

#### `Members`

- Type: `IReadOnlyList<Member>`
- Description: Represents the members of the party.

#### `State`

- Type: `string`
- Description: Represents the current state of the party.

#### `PreviousState`

- Type: `string`
- Description: Represents the previous state of the party.

#### `StateTransitionReason`

- Type: `string`
- Description: Represents the reason for the state transition.

#### `Accessibility`

- Type: `string`
- Description: Represents the accessibility of the party.

#### `CustomGameData`

- Type: `CustomGameData`
- Description: Represents the custom game data associated with the party.

#### `MatchmakingData`

- Type: `MatchmakingData`
- Description: Represents the matchmaking data associated with the party.

#### `Invites`

- Type: `object`
- Description: Represents the invites associated with the party.

#### `Requests`

- Type: `IReadOnlyList<object>`
- Description: Represents the requests associated with the party.

#### `QueueEntryTime`

- Type: `DateTime?`
- Description: Represents the time when the party entered the queue.

#### `ErrorNotification`

- Type: `ErrorNotification`
- Description: Represents the error notification associated with the party.

#### `RestrictedSeconds`

- Type: `long?`
- Description: Represents the restricted seconds associated with the party.

#### `EligibleQueues`

- Type: `IReadOnlyList<string>`
- Description: Represents the eligible queues for the party.

#### `QueueIneligibilities`

- Type: `IReadOnlyList<object>`
- Description: Represents the queue ineligibilities associated with the party.

#### `CheatData`

- Type: `CheatData`
- Description: Represents the cheat data associated with the party.

#### `XPBonuses`

- Type: `IReadOnlyList<XPBonuse>`
- Description: Represents the XP bonuses associated with the party.

#### `InviteCode`

- Type: `string`
- Description: Represents the invite code associated with the party.

---

## Settings Record

The `Settings` record represents settings within the RadiantConnect network.

### Properties

#### `Map`

- Type: `string`
- Description: Represents the selected map for the settings.

#### `Mode`

- Type: `string`
- Description: Represents the game mode for the settings.

#### `UseBots`

- Type: `bool?`
- Description: Indicates whether bots are used in the settings.

#### `GamePod`

- Type: `string`
- Description: Represents the game pod information for the settings.

#### `GameRules`

- Type: `object`
- Description: Represents the game rules for the settings.

---

## XPBonuse Record

The `XPBonuse` record represents XP bonuses within the RadiantConnect network.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the ID of the XP bonus.

#### `Applied`

- Type: `bool?`
- Description: Indicates whether the XP bonus is applied.
