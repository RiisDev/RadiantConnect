## PartySetReady Record

The `PartySetReady` record represents information about a party in the RadiantConnect network with a specific focus on setting readiness.

### Properties

#### `ID`

- Type: `string`
- Description: Represents the identifier of the party.

#### `MUCName`

- Type: `string`
- Description: Represents the Multi-User Chat (MUC) name associated with the party.

#### `VoiceRoomID`

- Type: `string`
- Description: Represents the identifier of the voice room associated with the party.

#### `Version`

- Type: `long?`
- Description: Represents the version of the party.

#### `ClientVersion`

- Type: `string`
- Description: Represents the client version associated with the party.

#### `Members`

- Type: `IReadOnlyList<PartySetReadyMember>`
- Description: Represents the list of members in the party.

#### `State`

- Type: `string`
- Description: Represents the current state of the party.

#### `PreviousState`

- Type: `string`
- Description: Represents the previous state of the party.

#### `StateTransitionReason`

- Type: `string`
- Description: Represents the reason for the transition of the party state.

#### `Accessibility`

- Type: `string`
- Description: Represents the accessibility information of the party.

#### `CustomGameData`

- Type: `PartySetReadyCustomGameData`
- Description: Represents custom game data associated with the party.

#### `MatchmakingData`

- Type: `PartySetReadyMatchmakingData`
- Description: Represents matchmaking data associated with the party.

#### `Invites`

- Type: `object`
- Description: Represents the invites associated with the party.

#### `Requests`

- Type: `IReadOnlyList<object>`
- Description: Represents the list of requests associated with the party.

#### `QueueEntryTime`

- Type: `DateTime?`
- Description: Represents the date and time when the party entered the queue.

#### `ErrorNotification`

- Type: `PartySetReadyErrorNotification`
- Description: Represents error notification information associated with the party.

#### `RestrictedSeconds`

- Type: `long?`
- Description: Represents the number of restricted seconds for the party.

#### `EligibleQueues`

- Type: `IReadOnlyList<string>`
- Description: Represents the list of eligible queues for the party.

#### `QueueIneligibilities`

- Type: `IReadOnlyList<object>`
- Description: Represents the list of queue ineligibilities for the party.

#### `CheatData`

- Type: `PartySetReadyCheatData`
- Description: Represents cheat data associated with the party.

#### `XPBonuses`

- Type: `IReadOnlyList<PartySetReadyXPBonuse>`
- Description: Represents the list of XP bonuses associated with the party.

#### `InviteCode`

- Type: `string`
- Description: Represents the invite code associated with the party.

### PartySetReadyCheatData

#### `GamePodOverride`

- Type: `string`
- Description: Represents the game pod override information in cheat data.

#### `ForcePostGameProcessing`

- Type: `bool?`
- Description: Represents whether to force post-game processing in cheat data.

### PartySetReadyCustomGameData

#### `Settings`

- Type: `PartySetReadySettings`
- Description: Represents the settings associated with the custom game data.

#### `Membership`

- Type: `PartySetReadyMembership`
- Description: Represents the membership information associated with the custom game data.

#### `MaxPartySize`

- Type: `long?`
- Description: Represents the maximum party size associated with the custom game data.

#### `AutobalanceEnabled`

- Type: `bool?`
- Description: Represents whether autobalance is enabled in the custom game data.

#### `AutobalanceMinPlayers`

- Type: `long?`
- Description: Represents the minimum number of players for autobalance in the custom game data.

#### `HasRecoveryData`

- Type: `bool?`
- Description: Represents whether recovery data is available in the custom game data.

### PartySetReadyErrorNotification

#### `ErrorType`

- Type: `string`
- Description: Represents the type of error in the error notification.

#### `ErroredPlayers`

- Type: `object`
- Description: Represents the errored players in the error notification.

### PartySetReadyMatchmakingData

#### `QueueID`

- Type: `string`
- Description: Represents the queue identifier in matchmaking data.

#### `PreferredGamePods`

- Type: `IReadOnlyList<string>`
- Description: Represents the list of preferred game pods in matchmaking data.

#### `SkillDisparityRRPenalty`

- Type: `long?`
- Description: Represents the skill disparity RR penalty in matchmaking data.

### PartySetReadyMember

#### `Subject`

- Type: `string`
- Description: Represents the subject identifier of the member.

#### `CompetitiveTier`

- Type: `long?`
- Description: Represents the competitive tier of the member.

#### `PlayerIdentity`

- Type: `PartySetReadyPlayerIdentity`
- Description: Represents the player identity information of the member.

#### `SeasonalBadgeInfo`

- Type: `object`
- Description: Represents the seasonal badge information of the member.

#### `IsOwner`

- Type: `bool?`
- Description: Represents whether the member is the owner of the party.

#### `QueueEligibleRemainingAccountLevels`

- Type: `long?`
- Description: Represents the remaining account levels eligible for the queue.

#### `Pings`

- Type: `IReadOnlyList<PartySetReadyPingInternal>`
- Description: Represents the list of pings associated with the member.

#### `IsReady`

- Type: `bool?`
- Description: Represents whether the member is ready.

#### `IsModerator`

- Type: `bool?`
- Description: Represents whether the member is a moderator.

#### `UseBroadcastHUD`

- Type: `bool?`
- Description: Represents whether to use broadcast HUD.

#### `PlatformType`

- Type: `string`
- Description: Represents the platform type of the member.

### PartySetReadyMembership

#### `TeamOne`

- Type: `object`
- Description: Represents team one information in membership.

#### `TeamTwo`

- Type: `object`
- Description: Represents team two information in membership.

#### `TeamSpectate`

- Type: `object`
- Description: Represents team spectate information in membership.

#### `TeamOneCoaches`

- Type: `object`
- Description: Represents team one coaches information in membership.

#### `TeamTwoCoaches`

- Type: `object`
- Description: Represents team two coaches information in membership.

### PartySetReadyPingInternal

#### `Ping`

- Type: `long?`
- Description: Represents the ping associated with the member.

#### `GamePodID`

- Type: `string`
- Description: Represents the game pod identifier associated with the member.

### PartySetReadyPlayerIdentity

#### `Subject`

- Type: `string`
- Description: Represents the subject identifier of the player identity.

#### `PlayerCardID`

- Type: `string`
- Description: Represents the player card identifier.

#### `PlayerTitleID`

- Type: `string`
- Description: Represents the player title identifier.

#### `AccountLevel`

- Type: `long?`
- Description: Represents the account level.

#### `PreferredLevelBorderID`

- Type: `string`
- Description: Represents the preferred level border identifier.

#### `Incognito`

- Type: `bool?`
- Description: Represents whether the player is incognito.

#### `HideAccountLevel`

- Type: `bool?`
- Description: Represents whether to hide the account level.

### PartySetReadySettings

#### `Map`

- Type: `string`
- Description: Represents the map in party set ready settings.

#### `Mode`

- Type: `string`
- Description: Represents the mode in party set ready settings.

#### `UseBots`

- Type: `bool?`
- Description: Represents whether to use bots in party set ready settings.

#### `GamePod`

- Type: `string`
- Description: Represents the game pod in party set ready settings.

#### `GameRules`

- Type: `object`
- Description: Represents the game rules in party set ready settings.

### PartySetReadyXPBonuse

#### `ID`

- Type: `string`
- Description: Represents the identifier of the XP bonus.

#### `Applied`

- Type: `bool?`
- Description: Represents whether the XP bonus is applied.
