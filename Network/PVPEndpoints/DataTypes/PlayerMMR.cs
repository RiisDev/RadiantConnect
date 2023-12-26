// ReSharper disable All

using System.Text.Json.Serialization;

namespace RadiantConnect.Network.PVPEndpoints.DataTypes;

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
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Deathmatch(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Ggteam(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Hurm(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record LatestCompetitiveUpdate(
    [property: JsonPropertyName("MatchID")] string MatchID,
    [property: JsonPropertyName("MapID")] string MapID,
    [property: JsonPropertyName("SeasonID")] string SeasonID,
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
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Onefa(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Premier(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
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

public record SeasonalInfoBySeasonID(
    [property: JsonPropertyName("0df5adb9-4dcb-6899-1306-3e9860661dd3")] SeasonId _0df5adb94dcb689913063e9860661dd3,
    [property: JsonPropertyName("3f61c772-4560-cd3f-5d3f-a7ab5abda6b3")] SeasonId _3f61c7724560cd3f5d3fa7ab5abda6b3,
    [property: JsonPropertyName("0530b9c4-4980-f2ee-df5d-09864cd00542")] SeasonId _0530b9c44980f2eedf5d09864cd00542,
    [property: JsonPropertyName("46ea6166-4573-1128-9cea-60a15640059b")] SeasonId _46ea6166457311289cea60a15640059b,
    [property: JsonPropertyName("fcf2c8f4-4324-e50b-2e23-718e4a3ab046")] SeasonId _fcf2c8f44324e50b2e23718e4a3ab046,
    [property: JsonPropertyName("97b6e739-44cc-ffa7-49ad-398ba502ceb0")] SeasonId _97b6e73944ccffa749ad398ba502ceb0,
    [property: JsonPropertyName("ab57ef51-4e59-da91-cc8d-51a5a2b9b8ff")] SeasonId _ab57ef514e59da91cc8d51a5a2b9b8ff,
    [property: JsonPropertyName("52e9749a-429b-7060-99fe-4595426a0cf7")] SeasonId _52e9749a429b706099fe4595426a0cf7,
    [property: JsonPropertyName("71c81c67-4fae-ceb1-844c-aab2bb8710fa")] SeasonId _71c81c674faeceb1844caab2bb8710fa,
    [property: JsonPropertyName("2a27e5d2-4d30-c9e2-b15a-93b8909a442c")] SeasonId _2a27e5d24d30c9e2b15a93b8909a442c,
    [property: JsonPropertyName("4cb622e1-4244-6da3-7276-8daaf1c01be2")] SeasonId _4cb622e142446da372768daaf1c01be2,
    [property: JsonPropertyName("a16955a5-4ad0-f761-5e9e-389df1c892fb")] SeasonId _a16955a54ad0f7615e9e389df1c892fb,
    [property: JsonPropertyName("97b39124-46ce-8b55-8fd1-7cbf7ffe173f")] SeasonId _97b3912446ce8b558fd17cbf7ffe173f,
    [property: JsonPropertyName("573f53ac-41a5-3a7d-d9ce-d6a6298e5704")] SeasonId _573f53ac41a53a7dd9ced6a6298e5704,
    [property: JsonPropertyName("d929bc38-4ab6-7da4-94f0-ee84f8ac141e")] SeasonId _d929bc384ab67da494f0ee84f8ac141e,
    [property: JsonPropertyName("3e47230a-463c-a301-eb7d-67bb60357d4f")] SeasonId _3e47230a463ca301eb7d67bb60357d4f,
    [property: JsonPropertyName("808202d6-4f2b-a8ff-1feb-b3a0590ad79f")] SeasonId _808202d64f2ba8ff1febb3a0590ad79f,
    [property: JsonPropertyName("67e373c7-48f7-b422-641b-079ace30b427")] SeasonId _67e373c748f7b422641b079ace30b427,
    [property: JsonPropertyName("7a85de9a-4032-61a9-61d8-f4aa2b4a84b6")] SeasonId _7a85de9a403261a961d8f4aa2b4a84b6,
    [property: JsonPropertyName("aca29595-40e4-01f5-3f35-b1b3d304c96e")] SeasonId _aca2959540e401f53f35b1b3d304c96e,
    [property: JsonPropertyName("79f9d00f-433a-85d6-dfc3-60aef115e699")] SeasonId _79f9d00f433a85d6dfc360aef115e699,
    [property: JsonPropertyName("9c91a445-4f78-1baa-a3ea-8f8aadf4914d")] SeasonId _9c91a4454f781baaa3ea8f8aadf4914d,
    [property: JsonPropertyName("34093c29-4306-43de-452f-3f944bde22be")] SeasonId _34093c29430643de452f3f944bde22be,
    [property: JsonPropertyName("2de5423b-4aad-02ad-8d9b-c0a931958861")] SeasonId _2de5423b4aad02ad8d9bc0a931958861,
    [property: JsonPropertyName("3ec8084a-4e45-4d22-d801-f8a63e5a208b")] SeasonId _3ec8084a4e454d22d801f8a63e5a208b,
    [property: JsonPropertyName("0981a882-4e7d-371a-70c4-c3b4f46c504a")] SeasonId _0981a8824e7d371a70c4c3b4f46c504a,
    [property: JsonPropertyName("03dfd004-45d4-ebfd-ab0a-948ce780dac4")] SeasonId _03dfd00445d4ebfdab0a948ce780dac4,
    [property: JsonPropertyName("4401f9fd-4170-2e4c-4bc3-f3b4d7d150d1")] SeasonId _4401f9fd41702e4c4bc3f3b4d7d150d1,
    [property: JsonPropertyName("1a2fc1de-4f58-4a89-49d0-f28b720ff76f")] SeasonId _1a2fc1de4f584a8949d0f28b720ff76f,
    [property: JsonPropertyName("ec876e6c-43e8-fa63-ffc1-2e8d4db25525")] SeasonId _ec876e6c43e8fa63ffc12e8d4db25525,
    [property: JsonPropertyName("22d10d66-4d2a-a340-6c54-408c7bd53807")] SeasonId _22d10d664d2aa3406c54408c7bd53807,
    [property: JsonPropertyName("4539cac3-47ae-90e5-3d01-b3812ca3274e")] SeasonId _4539cac347ae90e53d01b3812ca3274e,
    [property: JsonPropertyName("843cc465-4f0a-7466-ccda-19a427f8e478")] SeasonId _843cc4654f0a7466ccda19a427f8e478
);

public record Seeding(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Snowball(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Spikerush(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Swiftplay(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record Unrated(
    [property: JsonPropertyName("TotalGamesNeededForRating")] long? TotalGamesNeededForRating,
    [property: JsonPropertyName("TotalGamesNeededForLeaderboard")] long? TotalGamesNeededForLeaderboard,
    [property: JsonPropertyName("CurrentSeasonGamesNeededForRating")] long? CurrentSeasonGamesNeededForRating,
    [property: JsonPropertyName("SeasonalInfoBySeasonID")] SeasonalInfoBySeasonID SeasonalInfoBySeasonID
);

public record WinsByTier(
    [property: JsonPropertyName("0")] long? _0,
    [property: JsonPropertyName("1")] long? _1,
    [property: JsonPropertyName("2")] long? _2,
    [property: JsonPropertyName("3")] long? _3,
    [property: JsonPropertyName("4")] long? _4,
    [property: JsonPropertyName("5")] long? _5,
    [property: JsonPropertyName("6")] long? _6,
    [property: JsonPropertyName("7")] long? _7,
    [property: JsonPropertyName("8")] long? _8,
    [property: JsonPropertyName("9")] long? _9,
    [property: JsonPropertyName("10")] long? _10,
    [property: JsonPropertyName("11")] long? _11,
    [property: JsonPropertyName("12")] long? _12,
    [property: JsonPropertyName("13")] long? _13,
    [property: JsonPropertyName("14")] long? _14,
    [property: JsonPropertyName("15")] long? _15,
    [property: JsonPropertyName("16")] long? _16,
    [property: JsonPropertyName("17")] long? _17,
    [property: JsonPropertyName("18")] long? _18,
    [property: JsonPropertyName("19")] long? _19,
    [property: JsonPropertyName("20")] long? _20,
    [property: JsonPropertyName("21")] long? _21,
    [property: JsonPropertyName("22")] long? _22,
    [property: JsonPropertyName("23")] long? _23,
    [property: JsonPropertyName("24")] long? _24,
    [property: JsonPropertyName("25")] long? _25,
    [property: JsonPropertyName("26")] long? _26,
    [property: JsonPropertyName("27")] long? _27,
    [property: JsonPropertyName("28")] long? _28,
    [property: JsonPropertyName("29")] long? _29
);