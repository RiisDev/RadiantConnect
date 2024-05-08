using System.Text.Json.Serialization;
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

public record Ability(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("grenadeEffects")] object GrenadeEffects,
    [property: JsonPropertyName("ability1Effects")] object Ability1Effects,
    [property: JsonPropertyName("ability2Effects")] object Ability2Effects,
    [property: JsonPropertyName("ultimateEffects")] object UltimateEffects
);

public record AbilityCasts(
    [property: JsonPropertyName("grenadeCasts")] long? GrenadeCasts,
    [property: JsonPropertyName("ability1Casts")] long? Ability1Casts,
    [property: JsonPropertyName("ability2Casts")] long? Ability2Casts,
    [property: JsonPropertyName("ultimateCasts")] long? UltimateCasts
);

public record AdaptiveBots(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("adaptiveBotAverageDurationMillisAllAttempts")] long? AdaptiveBotAverageDurationMillisAllAttempts,
    [property: JsonPropertyName("adaptiveBotAverageDurationMillisFirstAttempt")] long? AdaptiveBotAverageDurationMillisFirstAttempt,
    [property: JsonPropertyName("killDetailsFirstAttempt")] object KillDetailsFirstAttempt
);

public record BasicGunSkill(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis
);

public record BasicMovement(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis
);

public record BehaviorFactors(
    [property: JsonPropertyName("afkRounds")] double? AfkRounds,
    [property: JsonPropertyName("collisions")] double? Collisions,
    [property: JsonPropertyName("commsRatingRecovery")] double? CommsRatingRecovery,
    [property: JsonPropertyName("damageParticipationOutgoing")] double? DamageParticipationOutgoing,
    [property: JsonPropertyName("friendlyFireIncoming")] double? FriendlyFireIncoming,
    [property: JsonPropertyName("friendlyFireOutgoing")] double? FriendlyFireOutgoing,
    [property: JsonPropertyName("mouseMovement")] double? MouseMovement,
    [property: JsonPropertyName("selfDamage")] double? SelfDamage,
    [property: JsonPropertyName("stayedInSpawnRounds")] double? StayedInSpawnRounds
);

public record BombPlant(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis
);

public record Damage(
    [property: JsonPropertyName("receiver")] string Receiver,
    [property: JsonPropertyName("damage")] double? DamageInternal,
    [property: JsonPropertyName("legshots")] long? Legshots,
    [property: JsonPropertyName("bodyshots")] long? Bodyshots,
    [property: JsonPropertyName("headshots")] long? Headshots
);

public record DefendBombSite(
    [property: JsonPropertyName("idleTimeMillis")] long? IdleTimeMillis,
    [property: JsonPropertyName("objectiveCompleteTimeMillis")] long? ObjectiveCompleteTimeMillis,
    [property: JsonPropertyName("success")] bool? Success
);

public record DefuseLocation(
    [property: JsonPropertyName("x")] long? X,
    [property: JsonPropertyName("y")] long? Y
);

public record DefusePlayerLocation(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("viewRadians")] double? ViewRadians,
    [property: JsonPropertyName("location")] Location Location
);

public record Economy(
    [property: JsonPropertyName("loadoutValue")] long? LoadoutValue,
    [property: JsonPropertyName("weapon")] string Weapon,
    [property: JsonPropertyName("armor")] string Armor,
    [property: JsonPropertyName("remaining")] long? Remaining,
    [property: JsonPropertyName("spent")] long? Spent
);

public record FinishingDamage(
    [property: JsonPropertyName("damageType")] string DamageType,
    [property: JsonPropertyName("damageItem")] string DamageItem,
    [property: JsonPropertyName("isSecondaryFireMode")] bool? IsSecondaryFireMode
);

public record Kill(
    [property: JsonPropertyName("gameTime")] long? GameTime,
    [property: JsonPropertyName("roundTime")] long? RoundTime,
    [property: JsonPropertyName("killer")] string Killer,
    [property: JsonPropertyName("victim")] string Victim,
    [property: JsonPropertyName("victimLocation")] VictimLocation VictimLocation,
    [property: JsonPropertyName("assistants")] IReadOnlyList<string> Assistants,
    [property: JsonPropertyName("playerLocations")] IReadOnlyList<PlayerLocation> PlayerLocations,
    [property: JsonPropertyName("finishingDamage")] FinishingDamage FinishingDamage,
    [property: JsonPropertyName("round")] long? Round
);

public record Location(
    [property: JsonPropertyName("x")] long? X,
    [property: JsonPropertyName("y")] long? Y
);

public record MatchInfoInternal(
    [property: JsonPropertyName("matchId")] string MatchId,
    [property: JsonPropertyName("mapId")] string MapId,
    [property: JsonPropertyName("gamePodId")] string GamePodId,
    [property: JsonPropertyName("gameLoopZone")] string GameLoopZone,
    [property: JsonPropertyName("gameServerAddress")] string GameServerAddress,
    [property: JsonPropertyName("gameVersion")] string GameVersion,
    [property: JsonPropertyName("gameLengthMillis")] long? GameLengthMillis,
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
    IReadOnlyList<string> PlayerUUIDs
);

public record PlantLocation(
    [property: JsonPropertyName("x")] long? X,
    [property: JsonPropertyName("y")] long? Y
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
    [property: JsonPropertyName("competitiveTier")] long? CompetitiveTier,
    [property: JsonPropertyName("isObserver")] bool? IsObserver,
    [property: JsonPropertyName("playerCard")] string PlayerCard,
    [property: JsonPropertyName("playerTitle")] string PlayerTitle,
    [property: JsonPropertyName("preferredLevelBorder")] string PreferredLevelBorder,
    [property: JsonPropertyName("accountLevel")] long? AccountLevel,
    [property: JsonPropertyName("sessionPlaytimeMinutes")] long? SessionPlaytimeMinutes,
    [property: JsonPropertyName("xpModifications")] IReadOnlyList<XpModification> XpModifications,
    [property: JsonPropertyName("behaviorFactors")] BehaviorFactors BehaviorFactors,
    [property: JsonPropertyName("newPlayerExperienceDetails")] NewPlayerExperienceDetails NewPlayerExperienceDetails
);

public record PlayerEconomy(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("loadoutValue")] long? LoadoutValue,
    [property: JsonPropertyName("weapon")] string Weapon,
    [property: JsonPropertyName("armor")] string Armor,
    [property: JsonPropertyName("remaining")] long? Remaining,
    [property: JsonPropertyName("spent")] long? Spent
);

public record PlayerLocation(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("viewRadians")] double? ViewRadians,
    [property: JsonPropertyName("location")] Location Location
);

public record PlayerScore(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("score")] long? Score
);

public record PlayerStat(
    [property: JsonPropertyName("subject")] string Subject,
    [property: JsonPropertyName("kills")] IReadOnlyList<Kill> Kills,
    [property: JsonPropertyName("damage")] IReadOnlyList<Damage> DamageInternal,
    [property: JsonPropertyName("score")] long? Score,
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
    [property: JsonPropertyName("round")] long? Round,
    [property: JsonPropertyName("receiver")] string Receiver,
    [property: JsonPropertyName("damage")] long? Damage
);

public record RoundResult(
    [property: JsonPropertyName("roundNum")] long? RoundNum,
    [property: JsonPropertyName("roundResult")] string RoundResultInternal,
    [property: JsonPropertyName("roundCeremony")] string RoundCeremony,
    [property: JsonPropertyName("winningTeam")] string WinningTeam,
    [property: JsonPropertyName("plantRoundTime")] long? PlantRoundTime,
    [property: JsonPropertyName("plantPlayerLocations")] IReadOnlyList<PlantPlayerLocation> PlantPlayerLocations,
    [property: JsonPropertyName("plantLocation")] PlantLocation PlantLocation,
    [property: JsonPropertyName("plantSite")] string PlantSite,
    [property: JsonPropertyName("defuseRoundTime")] long? DefuseRoundTime,
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
    [property: JsonPropertyName("score")] long? Score,
    [property: JsonPropertyName("roundsPlayed")] long? RoundsPlayed,
    [property: JsonPropertyName("kills")] long? Kills,
    [property: JsonPropertyName("deaths")] long? Deaths,
    [property: JsonPropertyName("assists")] long? Assists,
    [property: JsonPropertyName("playtimeMillis")] long? PlaytimeMillis,
    [property: JsonPropertyName("abilityCasts")] AbilityCasts AbilityCasts
);

public record Team(
    [property: JsonPropertyName("teamId")] string TeamId,
    [property: JsonPropertyName("won")] bool? Won,
    [property: JsonPropertyName("roundsPlayed")] long? RoundsPlayed,
    [property: JsonPropertyName("roundsWon")] long? RoundsWon,
    [property: JsonPropertyName("numPoints")] long? NumPoints
);

public record VictimLocation(
    [property: JsonPropertyName("x")] long? X,
    [property: JsonPropertyName("y")] long? Y
);

public record XpModification(
    [property: JsonPropertyName("Value")] double? Value,
    [property: JsonPropertyName("ID")] string ID
);