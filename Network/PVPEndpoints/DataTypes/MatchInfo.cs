using System.Text.Json.Serialization;
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record Ability(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("grenadeEffects")] object GrenadeEffects,
    [property: JsonPropertyName("ability1Effects")] object Ability1Effects,
    [property: JsonPropertyName("ability2Effects")] object Ability2Effects,
    [property: JsonPropertyName("ultimateEffects")] object UltimateEffects
);

public record AbilityCasts(
    [property: JsonPropertyName("grenadeCasts")] int? GrenadeCasts,
    [property: JsonPropertyName("ability1Casts")] int? Ability1Casts,
    [property: JsonPropertyName("ability2Casts")] int? Ability2Casts,
    [property: JsonPropertyName("ultimateCasts")] int? UltimateCasts
);

public record AdaptiveBots(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("adaptiveBotAverageDurationMillisAllAttempts")] int? AdaptiveBotAverageDurationMillisAllAttempts,
    [property: JsonPropertyName("adaptiveBotAverageDurationMillisFirstAttempt")] int? AdaptiveBotAverageDurationMillisFirstAttempt,
    [property: JsonPropertyName("killDetailsFirstAttempt")] object KillDetailsFirstAttempt
);

public record BasicGunSkill(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis
);

public record BasicMovement(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis
);

public record BehaviorFactors(
    [property: JsonPropertyName("afkRounds")] int? AfkRounds,
    [property: JsonPropertyName("collisions")] double? Collisions,
    [property: JsonPropertyName("commsRatingRecovery")] int? CommsRatingRecovery,
    [property: JsonPropertyName("damageParticipationOutgoing")] int? DamageParticipationOutgoing,
    [property: JsonPropertyName("friendlyFireIncoming")] int? FriendlyFireIncoming,
    [property: JsonPropertyName("friendlyFireOutgoing")] int? FriendlyFireOutgoing,
    [property: JsonPropertyName("mouseMovement")] int? MouseMovement,
    [property: JsonPropertyName("selfDamage")] int? SelfDamage,
    [property: JsonPropertyName("stayedInSpawnRounds")] int? StayedInSpawnRounds
);

public record BombPlant(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis
);

public record Damage(
    [property: JsonPropertyName("receiver")] string Receiver,
    [property: JsonPropertyName("damage")] double? DamageInternal,
    [property: JsonPropertyName("legshots")] int? Legshots,
    [property: JsonPropertyName("bodyshots")] int? Bodyshots,
    [property: JsonPropertyName("headshots")] int? Headshots
);

public record DefendBombSite(
    [property: JsonPropertyName("idleTimeMillis")] int? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] int? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("success")] bool? Success
);

public record DefuseLocation(
    [property: JsonPropertyName("x")] int? X,
    [property: JsonPropertyName("y")] int? Y
);

public record DefusePlayerLocation(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("viewRadians")] double? ViewRadians,
    [property: JsonPropertyName("location")] Location Location
);

public record Economy(
    [property: JsonPropertyName("loadoutValue")] int? LoadoutValue,
    [property: JsonPropertyName("weapon")] string Weapon,
    [property: JsonPropertyName("armor")] string Armor,
    [property: JsonPropertyName("remaining")] int? Remaining,
    [property: JsonPropertyName("spent")] int? Spent
);

public record FinishingDamage(
    [property: JsonPropertyName("damageType")] string DamageType,
    [property: JsonPropertyName("damageItem")] string DamageItem,
    [property: JsonPropertyName("isSecondaryFireMode")] bool? IsSecondaryFireMode
);

public record Kill(
    [property: JsonPropertyName("gameTime")] int? GameTime,
    [property: JsonPropertyName("roundTime")] int? RoundTime,
    [property: JsonPropertyName("killer")] string Killer,
    [property: JsonPropertyName("victim")] string Victim,
    [property: JsonPropertyName("victimLocation")] VictimLocation VictimLocation,
    [property: JsonPropertyName("assistants")] IReadOnlyList<string> Assistants,
    [property: JsonPropertyName("playerLocations")] IReadOnlyList<PlayerLocation> PlayerLocations,
    [property: JsonPropertyName("finishingDamage")] FinishingDamage FinishingDamage,
    [property: JsonPropertyName("round")] int? Round
);

public record Location(
    [property: JsonPropertyName("x")] int? X,
    [property: JsonPropertyName("y")] int? Y
);

public record MatchInfoInternal(
    [property: JsonPropertyName("matchId")] string MatchId,
    [property: JsonPropertyName("mapId")] string MapId,
    [property: JsonPropertyName("gamePodId")] string GamePodId,
    [property: JsonPropertyName("gameLoopZone")] string GameLoopZone,
    [property: JsonPropertyName("gameServerAddress")] string GameServerAddress,
    [property: JsonPropertyName("gameVersion")] string GameVersion,
    [property: JsonPropertyName("gameLengthMillis")] int? GameLengthMillis,
    [property: JsonPropertyName("gameStartMillis")] long? GameStartMillis,
    [property: JsonPropertyName("provisioningFlowID")] string ProvisioningFlowID,
    [property: JsonPropertyName("isCompleted")] bool? IsCompleted,
    [property: JsonPropertyName("customGameName")] string CustomGameName,
    [property: JsonPropertyName("forcePostProcessing")] bool? ForcePostProcessing,
    [property: JsonPropertyName("queueID")] string QueueID,
    [property: JsonPropertyName("gameMode")] string GameMode,
    [property: JsonPropertyName("isRanked")] bool? IsRanked,
    [property: JsonPropertyName("isMatchSampled")] bool? IsMatchSampled,
    [property: JsonPropertyName("seasonId")] string SeasonId,
    [property: JsonPropertyName("completionState")] string CompletionState,
    [property: JsonPropertyName("platformType")] string PlatformType,
    [property: JsonPropertyName("premierMatchInfo")] PremierMatchInfo PremierMatchInfo,
    [property: JsonPropertyName("partyRRPenalties")] PartyRRPenalties PartyRRPenalties,
    [property: JsonPropertyName("shouldMatchDisablePenalties")] bool? ShouldMatchDisablePenalties
);

public record NewPlayerExperienceDetails(
    [property: JsonPropertyName("basicMovement")] BasicMovement BasicMovement,
    [property: JsonPropertyName("basicGunSkill")] BasicGunSkill BasicGunSkill,
    [property: JsonPropertyName("adaptiveBots")] AdaptiveBots AdaptiveBots,
    [property: JsonPropertyName("ability")] Ability Ability,
    [property: JsonPropertyName("bombPlant")] BombPlant BombPlant,
    [property: JsonPropertyName("defendBombSite")] DefendBombSite DefendBombSite,
    [property: JsonPropertyName("settingStatus")] SettingStatus SettingStatus,
    [property: JsonPropertyName("versionString")] string VersionString
);

public record PartyRRPenalties(
    [property: JsonPropertyName("28691b4c-131b-4d7d-8a82-a147509d4d20")] int? _28691b4c131b4d7d8a82A147509d4d20,
    [property: JsonPropertyName("35834f90-3164-47b1-9312-bbff3708f02e")] int? _35834f90316447b19312Bbff3708f02e,
    [property: JsonPropertyName("3674f365-73ac-4cb9-9a44-44e4c4d1c569")] int? _3674f36573ac4cb99a4444e4c4d1c569,
    [property: JsonPropertyName("3bdbe5a8-6a7a-4045-83ce-84ff88fb44c0")] int? _3bdbe5a86a7a404583ce84ff88fb44c0,
    [property: JsonPropertyName("74fdd330-4248-4e2b-85b6-645c94621542")] int? _74fdd33042484e2b85b6645c94621542,
    [property: JsonPropertyName("76339d78-2e20-4e9b-95cf-5734321565f4")] int? _76339d782e204e9b95cf5734321565f4,
    [property: JsonPropertyName("90ab3776-2104-4b5e-b950-6224df83843a")] int? _90ab377621044b5eB9506224df83843a,
    [property: JsonPropertyName("f71d00d2-a996-41fe-b9ac-57ff7530f1ab")] int? F71d00d2A99641feB9ac57ff7530f1ab
);

public record PlantLocation(
    [property: JsonPropertyName("x")] int? X,
    [property: JsonPropertyName("y")] int? Y
);

public record PlantPlayerLocation(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("viewRadians")] double? ViewRadians,
    [property: JsonPropertyName("location")] Location Location
);

public record PlatformInfo(
    [property: JsonPropertyName("platformType")] string PlatformType,
    [property: JsonPropertyName("platformOS")] string PlatformOS,
    [property: JsonPropertyName("platformOSVersion")] string PlatformOSVersion,
    [property: JsonPropertyName("platformChipset")] string PlatformChipset
);

public record Player(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("gameName")] string GameName,
    [property: JsonPropertyName("tagLine")] string TagLine,
    [property: JsonPropertyName("platformInfo")] PlatformInfo PlatformInfo,
    [property: JsonPropertyName("teamId")] string TeamId,
    [property: JsonPropertyName("partyId")] string PartyId,
    [property: JsonPropertyName("characterId")] string CharacterId,
    [property: JsonPropertyName("stats")] Stats Stats,
    [property: JsonPropertyName("roundDamage")] IReadOnlyList<RoundDamage> RoundDamage,
    [property: JsonPropertyName("competitiveTier")] int? CompetitiveTier,
    [property: JsonPropertyName("isObserver")] bool? IsObserver,
    [property: JsonPropertyName("playerCard")] string PlayerCard,
    [property: JsonPropertyName("playerTitle")] string PlayerTitle,
    [property: JsonPropertyName("preferredLevelBorder")] string PreferredLevelBorder,
    [property: JsonPropertyName("accountLevel")] int? AccountLevel,
    [property: JsonPropertyName("sessionPlaytimeMinutes")] int? SessionPlaytimeMinutes,
    [property: JsonPropertyName("xpModifications")] IReadOnlyList<XpModification> XpModifications,
    [property: JsonPropertyName("behaviorFactors")] BehaviorFactors BehaviorFactors,
    [property: JsonPropertyName("newPlayerExperienceDetails")] NewPlayerExperienceDetails NewPlayerExperienceDetails
);

public record PlayerEconomy(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("loadoutValue")] int? LoadoutValue,
    [property: JsonPropertyName("weapon")] string Weapon,
    [property: JsonPropertyName("armor")] string Armor,
    [property: JsonPropertyName("remaining")] int? Remaining,
    [property: JsonPropertyName("spent")] int? Spent
);

public record PlayerLocation(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("viewRadians")] double? ViewRadians,
    [property: JsonPropertyName("location")] Location Location
);

public record PlayerScore(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("score")] int? Score
);

public record PlayerStat(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("kills")] IReadOnlyList<Kill> Kills,
    [property: JsonPropertyName("damage")] IReadOnlyList<Damage> DamageInternal,
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("economy")] Economy Economy,
    [property: JsonPropertyName("ability")] Ability Ability,
    [property: JsonPropertyName("wasAfk")] bool? WasAfk,
    [property: JsonPropertyName("wasPenalized")] bool? WasPenalized,
    [property: JsonPropertyName("stayedInSpawn")] bool? StayedInSpawn
);

public record PremierMatchInfo(
    [property: JsonPropertyName("premierSeasonId")] string PremierSeasonId,
    [property: JsonPropertyName("premierEventId")] string PremierEventId
);

public record MatchInfo(
    [property: JsonPropertyName("matchInfo")] MatchInfoInternal MatchInfoInternal,
    [property: JsonPropertyName("players")] IReadOnlyList<Player> Players,
    [property: JsonPropertyName("bots")] IReadOnlyList<object> Bots,
    [property: JsonPropertyName("coaches")] IReadOnlyList<object> Coaches,
    [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams,
    [property: JsonPropertyName("roundResults")] IReadOnlyList<RoundResult> RoundResults,
    [property: JsonPropertyName("kills")] IReadOnlyList<Kill> Kills
);

public record RoundDamage(
    [property: JsonPropertyName("round")] int? Round,
    [property: JsonPropertyName("receiver")] string Receiver,
    [property: JsonPropertyName("damage")] int? Damage
);

public record RoundResult(
    [property: JsonPropertyName("roundNum")] int? RoundNum,
    [property: JsonPropertyName("roundResult")] string RoundResultInternal,
    [property: JsonPropertyName("roundCeremony")] string RoundCeremony,
    [property: JsonPropertyName("winningTeam")] string WinningTeam,
    [property: JsonPropertyName("plantRoundTime")] int? PlantRoundTime,
    [property: JsonPropertyName("plantPlayerLocations")] IReadOnlyList<PlantPlayerLocation> PlantPlayerLocations,
    [property: JsonPropertyName("plantLocation")] PlantLocation PlantLocation,
    [property: JsonPropertyName("plantSite")] string PlantSite,
    [property: JsonPropertyName("defuseRoundTime")] int? DefuseRoundTime,
    [property: JsonPropertyName("defusePlayerLocations")] IReadOnlyList<DefusePlayerLocation> DefusePlayerLocations,
    [property: JsonPropertyName("defuseLocation")] DefuseLocation DefuseLocation,
    [property: JsonPropertyName("playerStats")] IReadOnlyList<PlayerStat> PlayerStats,
    [property: JsonPropertyName("roundResultCode")] string RoundResultCode,
    [property: JsonPropertyName("playerEconomies")] IReadOnlyList<PlayerEconomy> PlayerEconomies,
    [property: JsonPropertyName("playerScores")] IReadOnlyList<PlayerScore> PlayerScores,
    [property: JsonPropertyName("bombPlanter")] string BombPlanter,
    [property: JsonPropertyName("bombDefuser")] string BombDefuser
);

public record SettingStatus(
    [property: JsonPropertyName("isMouseSensitivityDefault")] bool? IsMouseSensitivityDefault,
    [property: JsonPropertyName("isCrosshairDefault")] bool? IsCrosshairDefault
);

public record Stats(
    [property: JsonPropertyName("score")] int? Score,
    [property: JsonPropertyName("roundsPlayed")] int? RoundsPlayed,
    [property: JsonPropertyName("kills")] int? Kills,
    [property: JsonPropertyName("deaths")] int? Deaths,
    [property: JsonPropertyName("assists")] int? Assists,
    [property: JsonPropertyName("playtimeMillis")] int? PlaytimeMillis,
    [property: JsonPropertyName("abilityCasts")] AbilityCasts AbilityCasts
);

public record Team(
    [property: JsonPropertyName("teamId")] string TeamId,
    [property: JsonPropertyName("won")] bool? Won,
    [property: JsonPropertyName("roundsPlayed")] int? RoundsPlayed,
    [property: JsonPropertyName("roundsWon")] int? RoundsWon,
    [property: JsonPropertyName("numPoints")] int? NumPoints
);

public record VictimLocation(
    [property: JsonPropertyName("x")] int? X,
    [property: JsonPropertyName("y")] int? Y
);

public record XpModification(
    [property: JsonPropertyName("Value")] double? Value,
    [property: JsonPropertyName("ID")] string ID
);