## Ability Record

The `Ability` record represents information about the abilities of a player in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with the ability.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with the ability.

#### `GrenadeEffects`

- Type: `object`
- Description: Represents the effects associated with the grenade ability.

#### `Ability1Effects`

- Type: `object`
- Description: Represents the effects associated with the first ability.

#### `Ability2Effects`

- Type: `object`
- Description: Represents the effects associated with the second ability.

#### `UltimateEffects`

- Type: `object`
- Description: Represents the effects associated with the ultimate ability.

## AbilityCasts Record

The `AbilityCasts` record represents information about the casts of different abilities by a player in the RadiantConnect network.

### Properties

#### `GrenadeCasts`

- Type: `long?`
- Description: Represents the number of casts for the grenade ability.

#### `Ability1Casts`

- Type: `long?`
- Description: Represents the number of casts for the first ability.

#### `Ability2Casts`

- Type: `long?`
- Description: Represents the number of casts for the second ability.

#### `UltimateCasts`

- Type: `long?`
- Description: Represents the number of casts for the ultimate ability.

## AdaptiveBots Record

The `AdaptiveBots` record represents information about a player's interaction with adaptive bots in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with the interaction with adaptive bots.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with adaptive bots.

#### `AdaptiveBotAverageDurationMillisAllAttempts`

- Type: `long?`
- Description: Represents the average duration of interaction with adaptive bots across all attempts.

#### `AdaptiveBotAverageDurationMillisFirstAttempt`

- Type: `long?`
- Description: Represents the average duration of interaction with adaptive bots for the first attempt.

#### `KillDetailsFirstAttempt`

- Type: `object`
- Description: Represents the kill details for the first attempt with adaptive bots.

## BasicGunSkill Record

The `BasicGunSkill` record represents information about a player's basic gun skill in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with the basic gun skill.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with the basic gun skill.

## BasicMovement Record

The `BasicMovement` record represents information about a player's basic movement in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with basic movement.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with basic movement.

## BehaviorFactors Record

The `BehaviorFactors` record represents various factors contributing to a player's behavior in the RadiantConnect network.

### Properties

#### `AfkRounds`

- Type: `double?`
- Description: Represents the number of rounds a player was away from keyboard.

#### `Collisions`

- Type: `double?`
- Description: Represents the number of collisions a player had.

#### `CommsRatingRecovery`

- Type: `double?`
- Description: Represents the recovery of communication rating.

#### `DamageParticipationOutgoing`

- Type: `double?`
- Description: Represents the outgoing damage participation.

#### `FriendlyFireIncoming`

- Type: `double?`
- Description: Represents the incoming friendly fire.

#### `FriendlyFireOutgoing`

- Type: `double?`
- Description: Represents the outgoing friendly fire.

#### `MouseMovement`

- Type: `double?`
- Description: Represents the amount of mouse movement.

#### `SelfDamage`

- Type: `double?`
- Description: Represents the self-inflicted damage.

#### `StayedInSpawnRounds`

- Type: `double?`
- Description: Represents the number of rounds a player stayed in spawn.

## BombPlant Record

The `BombPlant` record represents information about a player planting a bomb in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with planting the bomb.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with bomb planting.

## Damage Record

The `Damage` record represents information about the damage dealt by a player in the RadiantConnect network.

### Properties

#### `Receiver`

- Type: `string`
- Description: Represents the player who received the damage.

#### `DamageInternal`

- Type: `double?`
- Description: Represents the amount of damage dealt.

#### `Legshots`

- Type: `long?`
- Description: Represents the number of leg shots.

#### `Bodyshots`

- Type: `long?`
- Description: Represents the number of body shots.

#### `Headshots`

- Type: `long?`
- Description: Represents the number of head shots.

## DefendBombSite Record

The `DefendBombSite` record represents information about a player defending a bomb site in the RadiantConnect network.

### Properties

#### `IdleTimeMillis`

- Type: `long?`
- Description: Represents the idle time associated with defending the bomb site.

#### `ObjectiveCompleteTimeMillis`

- Type: `long?`
- Description: Represents the time taken to complete objectives associated with defending the bomb site.

#### `Success`

- Type: `bool?`
- Description: Represents whether the defense was successful.

## DefuseLocation Record

The `DefuseLocation` record represents the location coordinates of a bomb defusal in the RadiantConnect network.

### Properties

#### `X`

- Type: `long?`
- Description: Represents the X-coordinate of the defusal location.

#### `Y`

- Type: `long?`
- Description: Represents the Y-coordinate of the defusal location.

## DefusePlayerLocation Record

The `DefusePlayerLocation` record represents the location details of a player during a bomb defusal in the RadiantConnect network.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier associated with the player.

#### `ViewRadians`

- Type: `double?`
- Description: Represents the viewing angle in radians.

#### `Location`

- Type: `Location`
- Description: Represents the location coordinates of the player.

## Economy Record

The `Economy` record represents information about a player's economy in the RadiantConnect network.

### Properties

#### `LoadoutValue`

- Type: `long?`
- Description: Represents the value of the player's loadout.

#### `Weapon`

- Type: `string`
- Description: Represents the weapon used by the player.

#### `Armor`

- Type: `string`
- Description: Represents the armor used by the player.

#### `Remaining`

- Type: `long?`
- Description: Represents the remaining economic resources.

#### `Spent`

- Type: `long?`
- Description: Represents the amount spent by the player.

## FinishingDamage Record

The `FinishingDamage` record represents information about the finishing damage dealt by a player in the RadiantConnect network.

### Properties

#### `DamageType`

- Type: `string`
- Description: Represents the type of damage.

#### `DamageItem`

- Type: `string`
- Description: Represents the item causing the damage.

#### `IsSecondaryFireMode`

- Type: `bool?`
- Description: Represents whether the secondary fire mode was used.

## Kill Record

The `Kill` record represents information about a player killing another player in the RadiantConnect network.

### Properties

#### `GameTime`

- Type: `long?`
- Description: Represents the time of the kill in the game.

#### `RoundTime`

- Type: `long?`
- Description: Represents the time of the kill in the round.

#### `Killer`

- Type: `string`
- Description: Represents the player who performed the kill.

#### `Victim`

- Type: `string`
- Description: Represents the player who was killed.

#### `VictimLocation`

- Type: `VictimLocation`
- Description: Represents the location coordinates of the killed player.

#### `Assistants`

- Type: `IReadOnlyList<string>`
- Description: Represents the list of players who assisted in the kill.

#### `PlayerLocations`

- Type: `IReadOnlyList<PlayerLocation>`
- Description: Represents the locations of players involved in the kill.

#### `FinishingDamage`

- Type: `FinishingDamage`
- Description: Represents the finishing damage details.

#### `Round`

- Type: `long?`
- Description: Represents the round in which the kill occurred.

## Location Record

The `Location` record represents the coordinates of a location in the RadiantConnect network.

### Properties

#### `X`

- Type: `long?`
- Description: Represents the X-coordinate of the location.

#### `Y`

- Type: `long?`
- Description: Represents the Y-coordinate of the location.

## MatchInfoInternal Record

The `MatchInfoInternal` record represents internal information about a match in the RadiantConnect network.

### Properties

#### `MatchId`

- Type: `string`
- Description: Represents the unique identifier associated with the match.

#### `MapId`

- Type: `string`
- Description: Represents the unique identifier associated with the map.

#### `GamePodId`

- Type: `string`
- Description: Represents the identifier of the game pod.

#### `GameLoopZone`

- Type: `string`
- Description: Represents the game loop zone.

#### `GameServerAddress`

- Type: `string`
- Description: Represents the address of the game server.

#### `GameVersion`

- Type: `string`
- Description: Represents the version of the game.

#### `GameLengthMillis`

- Type: `long?`
- Description: Represents the length of the game in milliseconds.

#### `GameStartMillis`

- Type: `long?`
- Description: Represents the start time of the game in milliseconds.

#### `ProvisioningFlowID`

- Type: `string`
- Description: Represents the identifier of the provisioning flow.

#### `IsCompleted`

- Type: `bool?`
- Description: Represents whether the match is completed.

#### `CustomGameName`

- Type: `string`
- Description: Represents the name of the custom game.

#### `ForcePostProcessing`

- Type: `bool?`
- Description: Represents whether post-processing is forced.

#### `QueueID`

- Type: `string`
- Description: Represents the identifier of the queue.

#### `GameMode`

- Type: `string`
- Description: Represents the game mode.

#### `IsRanked`

- Type: `bool?`
- Description: Represents whether the match is ranked.

#### `IsMatchSampled`

- Type: `bool?`
- Description: Represents whether the match is sampled.

#### `SeasonId`

- Type: `string`
- Description: Represents the unique identifier associated with the season.

#### `CompletionState`

- Type: `string`
- Description: Represents the completion state of the match.

#### `PlatformType`

- Type: `string`
- Description: Represents the type of platform.

#### `PremierMatchInfo`

- Type: `PremierMatchInfo`
- Description: Represents premier match information.

#### `PartyRRPenalties`

- Type: `PartyRRPenalties`
- Description: Represents penalties associated with the match.

#### `ShouldMatchDisablePenalties`

- Type: `bool?`
- Description: Represents whether penalties should be disabled for the match.

## NewPlayerExperienceDetails Record

The `NewPlayerExperienceDetails` record represents details of a new player's experience in the RadiantConnect network.

### Properties

#### `BasicMovement`

- Type: `BasicMovement`
- Description: Represents details about the basic movement.

#### `BasicGunSkill`

- Type: `BasicGunSkill`
- Description: Represents details about basic gun skill.

#### `AdaptiveBots`

- Type: `AdaptiveBots`
- Description: Represents details about interaction with adaptive bots.

#### `Ability`

- Type: `Ability`
- Description: Represents details about player abilities.

#### `BombPlant`

- Type: `BombPlant`
- Description: Represents details about bomb planting.

#### `DefendBombSite`

- Type: `DefendBombSite`
- Description: Represents details about defending a bomb site.

#### `SettingStatus`

- Type: `SettingStatus`
- Description: Represents the status of player settings.

#### `VersionString`

- Type: `string`
- Description: Represents the version string.

## PartyRRPenalties Record

The `PartyRRPenalties` record represents penalties associated with a player in the RadiantConnect network.

### Properties

#### `_28691b4c131b4d7d8a82A147509d4d20`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_35834f90316447b19312Bbff3708f02e`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_3674f36573ac4cb99a4444e4c4d1c569`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_3bdbe5a86a7a404583ce84ff88fb44c0`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_74fdd33042484e2b85b6645c94621542`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_76339d782e204e9b95cf5734321565f4`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `_90ab377621044b5eB9506224df83843a`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

#### `F71d00d2A99641feB9ac57ff7530f1ab`

- Type: `long?`
- Description: Represents a penalty associated with a specific identifier.

## PlantLocation Record

The `PlantLocation` record represents the coordinates of a planted object.

### Properties

#### `X`

- Type: `long?`
- Description: The X-coordinate of the planted object.

#### `Y`

- Type: `long?`
- Description: The Y-coordinate of the planted object.

## PlantPlayerLocation Record

The `PlantPlayerLocation` record represents the location and view radians of a player during a planted round.

### Properties

#### `Subject`

- Type: `string`
- Description: Represents the unique identifier associated with the player.

#### `ViewRadians`

- Type: `double?`
- Description: Represents the view radians of the player.

#### `Location`

- Type: `Location`
- Description: Represents the location of the player.

## PlatformInfo Record

The `PlatformInfo` record represents information about the platform of a player.

### Properties

#### `PlatformType`

- Type: `string`
- Description: Represents the type of platform.

#### `PlatformOS`

- Type: `string`
- Description: Represents the operating system of the platform.

#### `PlatformOSVersion`

- Type: `string`
- Description: Represents the version of the operating system.

#### `PlatformChipset`

- Type: `string`
- Description: Represents the chipset of the platform.

## Player Record

The `Player` record represents information about a player in the RadiantConnect network.

( ... truncated for brevity ... )

## XpModification Record

The `XpModification` record represents modifications to a player's experience points.

### Properties

#### `Value`

- Type: `double?`
- Description: The value of the experience point modification.

#### `ID`

- Type: `string`
- Description: The unique identifier associated with the modification.

