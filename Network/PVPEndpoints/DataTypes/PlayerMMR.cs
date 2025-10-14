// ReSharper disable All

using System.Text.Json.Serialization;

namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record SeasonId(
		[property: JsonPropertyName("SeasonID")] string SeasonID,
		[property: JsonPropertyName("NumberOfWins")] long? NumberOfWins,
		[property: JsonPropertyName("NumberOfWinsWithPlacements")] long? NumberOfWinsWithPlacements,
		[property: JsonPropertyName("NumberOfGames")] long? NumberOfGames,
		[property: JsonPropertyName("Rank")] long? Rank,
		[property: JsonPropertyName("CapstoneWins")] long? CapstoneWins,
		[property: JsonPropertyName("LeaderboardRank")] long? LeaderboardRank,
		[property: JsonPropertyName("CompetitiveTier")] long? CompetitiveTier,
		[property: JsonPropertyName("RankedRating")] long? RankedRating,
		[property: JsonPropertyName("WinsByTier")] WinsByTier WinsByTier,
		[property: JsonPropertyName("GamesNeededForRating")] long? GamesNeededForRating,
		[property: JsonPropertyName("TotalWinsNeededForRank")] long? TotalWinsNeededForRank
	);

	public record Competitive(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId>? SeasonalInfoBySeasonID
	);

	public record Deathmatch(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Ggteam(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Hurm(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record LatestCompetitiveUpdate(
		[property: JsonPropertyName("MatchID")] string MatchId,
		[property: JsonPropertyName("MapID")] string MapId,
		[property: JsonPropertyName("SeasonID")] string SeasonId,
		[property: JsonPropertyName("MatchStartTime")] long? MatchStartTime,
		[property: JsonPropertyName("TierAfterUpdate")] long? TierAfterUpdate,
		[property: JsonPropertyName("TierBeforeUpdate")] long? TierBeforeUpdate,
		[property: JsonPropertyName("RankedRatingAfterUpdate")] long? RankedRatingAfterUpdate,
		[property: JsonPropertyName("RankedRatingBeforeUpdate")] long? RankedRatingBeforeUpdate,
		[property: JsonPropertyName("RankedRatingEarned")] long? RankedRatingEarned,
		[property: JsonPropertyName("RankedRatingPerformanceBonus")] long? RankedRatingPerformanceBonus,
		[property: JsonPropertyName("CompetitiveMovement")] string CompetitiveMovement,
		[property: JsonPropertyName("AFKPenalty")] long? AFKPenalty
	);

	public record Newmap(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Onefa(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Premier(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record QueueSkills(
		[property: JsonPropertyName("competitive")] Competitive Competitive,
		[property: JsonPropertyName("deathmatch")] Deathmatch Deathmatch,
		[property: JsonPropertyName("ggteam")] Ggteam Ggteam,
		[property: JsonPropertyName("hurm")] Hurm Hurm,
		[property: JsonPropertyName("newmap")] Newmap Newmap,
		[property: JsonPropertyName("onefa")] Onefa Onefa,
		[property: JsonPropertyName("premier")] Premier Premier,
		[property: JsonPropertyName("seeding")] Seeding Seeding,
		[property: JsonPropertyName("snowball")] Snowball Snowball,
		[property: JsonPropertyName("spikerush")] Spikerush Spikerush,
		[property: JsonPropertyName("swiftplay")] Swiftplay Swiftplay,
		[property: JsonPropertyName("unrated")] Unrated Unrated
	);

	public record PlayerMMR(
		[property: JsonPropertyName("Version")] long? Version,
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("NewPlayerExperienceFinished")] bool? NewPlayerExperienceFinished,
		[property: JsonPropertyName("QueueSkills")] QueueSkills QueueSkills,
		[property: JsonPropertyName("LatestCompetitiveUpdate")] LatestCompetitiveUpdate LatestCompetitiveUpdate,
		[property: JsonPropertyName("IsLeaderboardAnonymized")] bool? IsLeaderboardAnonymized,
		[property: JsonPropertyName("IsActRankBadgeHidden")] bool? IsActRankBadgeHidden
	);

	public record Seeding(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Snowball(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Spikerush(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Swiftplay(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record Unrated(
		[property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
		[property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
		[property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
		[property: JsonPropertyName("SeasonalInfoBySeasonID")] Dictionary<string, SeasonId> SeasonalInfoBySeasonID
	);

	public record WinsByTier(
		[property: JsonPropertyName("0")] long? Unranked,
		[property: JsonPropertyName("1")] long? Unused1,
		[property: JsonPropertyName("2")] long? Unused2,
		[property: JsonPropertyName("3")] long? Iron1,
		[property: JsonPropertyName("4")] long? Iron2,
		[property: JsonPropertyName("5")] long? Iron3,
		[property: JsonPropertyName("6")] long? Bronze1,
		[property: JsonPropertyName("7")] long? Bronze2,
		[property: JsonPropertyName("8")] long? Bronze3,
		[property: JsonPropertyName("9")] long? Silver1,
		[property: JsonPropertyName("10")] long? Silver2,
		[property: JsonPropertyName("11")] long? Silver3,
		[property: JsonPropertyName("12")] long? Gold1,
		[property: JsonPropertyName("13")] long? Gold2,
		[property: JsonPropertyName("14")] long? Gold3,
		[property: JsonPropertyName("15")] long? Platinum1,
		[property: JsonPropertyName("16")] long? Platinum2,
		[property: JsonPropertyName("17")] long? Platinum3,
		[property: JsonPropertyName("18")] long? Diamond1,
		[property: JsonPropertyName("19")] long? Diamond2,
		[property: JsonPropertyName("20")] long? Diamond3,
		[property: JsonPropertyName("21")] long? Ascendant1,
		[property: JsonPropertyName("22")] long? Ascendant2,
		[property: JsonPropertyName("23")] long? Ascendant3,
		[property: JsonPropertyName("24")] long? Immortal1,
		[property: JsonPropertyName("25")] long? Immortal2,
		[property: JsonPropertyName("26")] long? Immortal3,
		[property: JsonPropertyName("27")] long? Radiant,
		[property: JsonPropertyName("28")] long? Unused3,
		[property: JsonPropertyName("29")] long? Unused4
	);
}