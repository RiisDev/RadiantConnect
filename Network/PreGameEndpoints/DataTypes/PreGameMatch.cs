

// ReSharper disable All

namespace RadiantConnect.Network.PreGameEndpoints.DataTypes
{
    public record AllyTeam(
        [property: JsonPropertyName("TeamID")] string TeamID,
        [property: JsonPropertyName("Players")] IReadOnlyList<Player> Players
    );

    public record CastedVotes();

    public record Player(
        [property: JsonPropertyName("Subject")] string Subject,
        [property: JsonPropertyName("CharacterID")] string CharacterID,
        [property: JsonPropertyName("CharacterSelectionState")] string CharacterSelectionState,
        [property: JsonPropertyName("PregamePlayerState")] string PregamePlayerState,
        [property: JsonPropertyName("CompetitiveTier")] long CompetitiveTier,
        [property: JsonPropertyName("PlayerIdentity")] PlayerIdentity PlayerIdentity,
        [property: JsonPropertyName("SeasonalBadgeInfo")] SeasonalBadgeInfo SeasonalBadgeInfo,
        [property: JsonPropertyName("IsCaptain")] bool IsCaptain
    );

    public record PlayerIdentity(
        [property: JsonPropertyName("Subject")] string Subject,
        [property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
        [property: JsonPropertyName("PlayerTitleID")] string PlayerTitleID,
        [property: JsonPropertyName("AccountLevel")] long AccountLevel,
        [property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderID,
        [property: JsonPropertyName("Incognito")] bool Incognito,
        [property: JsonPropertyName("HideAccountLevel")] bool HideAccountLevel
    );

    public record PreGameMatch(
        [property: JsonPropertyName("ID")] string ID,
        [property: JsonPropertyName("Version")] long Version,
        [property: JsonPropertyName("Teams")] IReadOnlyList<Team> Teams,
        [property: JsonPropertyName("AllyTeam")] AllyTeam AllyTeam,
        [property: JsonPropertyName("EnemyTeam")] object EnemyTeam,
        [property: JsonPropertyName("ObserverSubjects")] IReadOnlyList<object> ObserverSubjects,
        [property: JsonPropertyName("MatchCoaches")] IReadOnlyList<object> MatchCoaches,
        [property: JsonPropertyName("EnemyTeamSize")] long EnemyTeamSize,
        [property: JsonPropertyName("EnemyTeamLockCount")] long EnemyTeamLockCount,
        [property: JsonPropertyName("PregameState")] string PregameState,
        [property: JsonPropertyName("LastUpdated")] DateTime LastUpdated,
        [property: JsonPropertyName("MapID")] string MapID,
        [property: JsonPropertyName("MapSelectPool")] IReadOnlyList<object> MapSelectPool,
        [property: JsonPropertyName("BannedMapIDs")] IReadOnlyList<object> BannedMapIDs,
        [property: JsonPropertyName("CastedVotes")] CastedVotes CastedVotes,
        [property: JsonPropertyName("MapSelectSteps")] IReadOnlyList<object> MapSelectSteps,
        [property: JsonPropertyName("MapSelectStep")] long MapSelectStep,
        [property: JsonPropertyName("Team1")] string Team1,
        [property: JsonPropertyName("GamePodID")] string GamePodID,
        [property: JsonPropertyName("Mode")] string Mode,
        [property: JsonPropertyName("VoiceSessionID")] string VoiceSessionID,
        [property: JsonPropertyName("MUCName")] string MUCName,
        [property: JsonPropertyName("TeamMatchToken")] string TeamMatchToken,
        [property: JsonPropertyName("QueueID")] string QueueID,
        [property: JsonPropertyName("ProvisioningFlowID")] string ProvisioningFlowID,
        [property: JsonPropertyName("IsRanked")] bool IsRanked,
        [property: JsonPropertyName("PhaseTimeRemainingNS")] long PhaseTimeRemainingNS,
        [property: JsonPropertyName("StepTimeRemainingNS")] long StepTimeRemainingNS,
        [property: JsonPropertyName("altModesFlagADA")] bool AltModesFlagADA,
        [property: JsonPropertyName("TournamentMetadata")] object TournamentMetadata,
        [property: JsonPropertyName("RosterMetadata")] object RosterMetadata
    );

    public record SeasonalBadgeInfo(
        [property: JsonPropertyName("SeasonID")] string SeasonID,
        [property: JsonPropertyName("NumberOfWins")] long NumberOfWins,
        [property: JsonPropertyName("WinsByTier")] object WinsByTier,
        [property: JsonPropertyName("Rank")] long Rank,
        [property: JsonPropertyName("LeaderboardRank")] long LeaderboardRank
    );

    public record Team(
        [property: JsonPropertyName("TeamID")] string TeamID,
        [property: JsonPropertyName("Players")] IReadOnlyList<Player> Players
    );
}
