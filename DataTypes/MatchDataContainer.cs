using System.Text.Json.Serialization;

// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace RadiantConnect.DataTypes
{
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
        [property: JsonPropertyName("afkRounds")] double? AfkRounds,
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

    public record DamageData(
        [property: JsonPropertyName("receiver")] string Receiver,
        [property: JsonPropertyName("damage")] int? Damage,
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

    public record MatchInfo(
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
        [property: JsonPropertyName("15100f13-35a5-4eed-b055-10e07b4bd170")] int? _15100f1335a54eedB05510e07b4bd170,
        [property: JsonPropertyName("2ab98415-2073-493d-b3d5-9427327400b7")] int? _2ab984152073493dB3d59427327400b7,
        [property: JsonPropertyName("39259502-e4ed-4d63-800f-ae0cfa7d55ae")] int? _39259502E4ed4d63800fAe0cfa7d55ae,
        [property: JsonPropertyName("85642ac1-5072-4630-b8c2-18516276b557")] int? _85642ac150724630B8c218516276b557,
        [property: JsonPropertyName("a2a56502-a42a-4f34-bddc-290a7bfa1c47")] int? A2a56502A42a4f34Bddc290a7bfa1c47,
        [property: JsonPropertyName("c455bcf6-f0cd-435c-9806-614a4539cb4d")] int? C455bcf6F0cd435c9806614a4539cb4d,
        [property: JsonPropertyName("fde8895b-a5a8-4638-a9e0-b6a094fd7cef")] int? Fde8895bA5a84638A9e0B6a094fd7cef
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
        [property: JsonPropertyName("damage")] IReadOnlyList<DamageData> Damage,
        [property: JsonPropertyName("score")] int? Score,
        [property: JsonPropertyName("economy")] Economy Economy,
        [property: JsonPropertyName("ability")] Ability Ability,
        [property: JsonPropertyName("wasAfk")] bool? WasAfk,
        [property: JsonPropertyName("wasPenalized")] bool? WasPenalized,
        [property: JsonPropertyName("stayedInSpawn")] bool? StayedInSpawn
    );

    public record MatchDetails(
        [property: JsonPropertyName("matchInfo")] MatchInfo MatchInfo,
        [property: JsonPropertyName("players")] IReadOnlyList<Player> Players,
        [property: JsonPropertyName("bots")] IReadOnlyList<object> Bots,
        [property: JsonPropertyName("coaches")] IReadOnlyList<object> Coaches,
        [property: JsonPropertyName("teams")] IReadOnlyList<Team> Teams,
        [property: JsonPropertyName("roundResults")] IReadOnlyList<RoundResultData> RoundResults,
        [property: JsonPropertyName("kills")] IReadOnlyList<Kill> Kills
    );

    public record RoundDamage(
        [property: JsonPropertyName("round")] int? Round,
        [property: JsonPropertyName("receiver")] string Receiver,
        [property: JsonPropertyName("damage")] int? Damage
    );

    public record RoundResultData(
        [property: JsonPropertyName("roundNum")] int? RoundNum,
        [property: JsonPropertyName("roundResult")] string RoundResult,
        [property: JsonPropertyName("roundCeremony")] string RoundCeremony,
        [property: JsonPropertyName("winningTeam")] string WinningTeam,
        [property: JsonPropertyName("bombPlanter")] string BombPlanter,
        [property: JsonPropertyName("bombDefuser")] string BombDefuser,
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
        [property: JsonPropertyName("playerScores")] IReadOnlyList<PlayerScore> PlayerScores
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

    public record MatchData(
        [property: JsonPropertyName("MatchID")]
        string MatchID,
        [property: JsonPropertyName("MapID")] string MapID,
        [property: JsonPropertyName("SeasonID")]
        string SeasonID,
        [property: JsonPropertyName("MatchStartTime")]
        object MatchStartTime,
        [property: JsonPropertyName("TierAfterUpdate")]
        int? TierAfterUpdate,
        [property: JsonPropertyName("TierBeforeUpdate")]
        int? TierBeforeUpdate,
        [property: JsonPropertyName("RankedRatingAfterUpdate")]
        int? RankedRatingAfterUpdate,
        [property: JsonPropertyName("RankedRatingBeforeUpdate")]
        int? RankedRatingBeforeUpdate,
        [property: JsonPropertyName("RankedRatingEarned")]
        int? RankedRatingEarned,
        [property: JsonPropertyName("RankedRatingPerformanceBonus")]
        int? RankedRatingPerformanceBonus,
        [property: JsonPropertyName("CompetitiveMovement")]
        string CompetitiveMovement,
        [property: JsonPropertyName("AFKPenalty")]
        int? AFKPenalty
    )
    {
        public double? HeadshotPercentage { get; set; }
    };

    public record MatchDataContainer(
        [property: JsonPropertyName("Version")]
        int? Version,
        [property: JsonPropertyName("Subject")]
        string Subject,
        [property: JsonPropertyName("Matches")]
        IReadOnlyList<MatchData>? Matches
    )
    {
        public Dictionary<string, MatchDetails?>? MatchDetails { get; set; }
    };

}
