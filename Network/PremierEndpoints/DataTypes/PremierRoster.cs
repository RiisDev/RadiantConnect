namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record RosterCustomization(
		[property: JsonPropertyName("icon")] string Icon,
		[property: JsonPropertyName("primaryColor")] string PrimaryColor,
		[property: JsonPropertyName("secondaryColor")] string SecondaryColor,
		[property: JsonPropertyName("tertiaryColor")] string TertiaryColor
	);

	public record RosterMember(
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("role")] string Role,
		[property: JsonPropertyName("roleId")] int RoleId,
		[property: JsonPropertyName("createdAt")] long CreatedAt
	);

	public record RosterSeasonInfo(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("isEnrolled")] bool IsEnrolled,
		[property: JsonPropertyName("conference")] string Conference,
		[property: JsonPropertyName("division")] int Division,
		[property: JsonPropertyName("isProvisionalDivision")] bool IsProvisionalDivision,
		[property: JsonPropertyName("promotionApplied")] bool PromotionApplied,
		[property: JsonPropertyName("points")] int Points,
		[property: JsonPropertyName("wins")] int Wins,
		[property: JsonPropertyName("gamesPlayed")] int GamesPlayed,
		[property: JsonPropertyName("seasonMatchRoundWins")] int SeasonMatchRoundWins,
		[property: JsonPropertyName("seasonMatchRoundsPlayed")] int SeasonMatchRoundsPlayed,
		[property: JsonPropertyName("crest")] string? Crest,
		[property: JsonPropertyName("hasEarnedPromotionForNextSeason")] bool HasEarnedPromotionForNextSeason,
		[property: JsonPropertyName("hasEarnedPrestige")] bool HasEarnedPrestige
	);

	public record RosterVersion(
		[property: JsonPropertyName("socialVersion")] int SocialVersion,
		[property: JsonPropertyName("premierVersion")] int PremierVersion
	);

	public record RosterPrestige(
		[property: JsonPropertyName("earnedPrestige")] bool EarnedPrestige,
		[property: JsonPropertyName("plating")] string? Plating
	);

	public record PremierRoster(
		[property: JsonPropertyName("rosterId")] string RosterId,
		[property: JsonPropertyName("affinity")] string Affinity,
		[property: JsonPropertyName("name")] string Name,
		[property: JsonPropertyName("tag")] string Tag,
		[property: JsonPropertyName("customization")] RosterCustomization? Customization,
		[property: JsonPropertyName("members")] IReadOnlyList<RosterMember>? Members,
		[property: JsonPropertyName("invites")] IReadOnlyList<object>? Invites,
		[property: JsonPropertyName("locks")] IReadOnlyList<object>? Locks,
		[property: JsonPropertyName("season")] RosterSeasonInfo? Season,
		[property: JsonPropertyName("minimumRequiredMembersForEnrollment")] int MinimumRequiredMembersForEnrollment,
		[property: JsonPropertyName("matchesSinceReset")] int MatchesSinceReset,
		[property: JsonPropertyName("tournamentsSinceReset")] int TournamentsSinceReset,
		[property: JsonPropertyName("version")] RosterVersion? Version,
		[property: JsonPropertyName("updatedAt")] long UpdatedAt,
		[property: JsonPropertyName("createdAt")] long CreatedAt,
		[property: JsonPropertyName("seasonalInfoBySeasonID")] IReadOnlyDictionary<string, RosterSeasonInfo>? SeasonalInfoBySeasonId,
		[property: JsonPropertyName("prestige")] RosterPrestige? Prestige
	);
}
