namespace RadiantConnect.Network.PremierEndpoints.DataTypes
{
	public record PremierEvent(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Type")] string Type,
		[property: JsonPropertyName("StartDateTime")] string StartDateTime,
		[property: JsonPropertyName("EndDateTime")] string EndDateTime,
		[property: JsonPropertyName("SchedulePerConference")] object? SchedulePerConference,
		[property: JsonPropertyName("MapSelectionStrategy")] string MapSelectionStrategy,
		[property: JsonPropertyName("MapPoolMapIDs")] IReadOnlyList<string>? MapPoolMapIds,
		[property: JsonPropertyName("PointsRequiredToParticipate")] int PointsRequiredToParticipate
	);

	public record PremierConference(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("key")] string Key,
		[property: JsonPropertyName("isSuper")] bool IsSuper,
		[property: JsonPropertyName("gamePods")] IReadOnlyList<string>? GamePods,
		[property: JsonPropertyName("timezone")] string Timezone,
		[property: JsonPropertyName("superConference")] string? SuperConference,
		[property: JsonPropertyName("leaderboardPlayoffQualificationDateTime")] string LeaderboardPlayoffQualificationDateTime,
		[property: JsonPropertyName("leaderboardPromotionFinalizationDateTime")] string LeaderboardPromotionFinalizationDateTime
	);

	public record PremierDivision(
		[property: JsonPropertyName("Division")] int Division,
		[property: JsonPropertyName("DivisionName")] string DivisionName,
		[property: JsonPropertyName("DivisionGroup")] string DivisionGroup,
		[property: JsonPropertyName("EventPresetName")] string EventPresetName
	);

	public record PremierSeason(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("Name")] string Name,
		[property: JsonPropertyName("CompetitiveSeasonID")] string CompetitiveSeasonId,
		[property: JsonPropertyName("PreviousPremierSeasonID")] string? PreviousPremierSeasonId,
		[property: JsonPropertyName("NextPremierSeasonID")] string? NextPremierSeasonId,
		[property: JsonPropertyName("IsActive")] bool IsActive,
		[property: JsonPropertyName("StartTime")] string StartTime,
		[property: JsonPropertyName("EndTime")] string EndTime,
		[property: JsonPropertyName("Events")] IReadOnlyList<PremierEvent>? Events,
		[property: JsonPropertyName("ScheduledEvents")] IReadOnlyList<object>? ScheduledEvents,
		[property: JsonPropertyName("Conferences")] IReadOnlyList<PremierConference>? Conferences,
		[property: JsonPropertyName("Divisions")] IReadOnlyList<PremierDivision>? Divisions,
		[property: JsonPropertyName("DivisionThresholds")] IReadOnlyList<object>? DivisionThresholds,
		[property: JsonPropertyName("EventPresets")] IReadOnlyDictionary<string, object>? EventPresets,
		[property: JsonPropertyName("FlawlessPointRequirement")] int FlawlessPointRequirement,
		[property: JsonPropertyName("ChampionshipPointRequirement")] int ChampionshipPointRequirement,
		[property: JsonPropertyName("ChampionshipEventID")] string? ChampionshipEventId,
		[property: JsonPropertyName("EnrollmentPhaseStartDateTime")] string EnrollmentPhaseStartDateTime,
		[property: JsonPropertyName("EnrollmentPhaseEndDateTime")] string EnrollmentPhaseEndDateTime,
		[property: JsonPropertyName("LeaderboardPlayoffQualificationDateTime")] string LeaderboardPlayoffQualificationDateTime,
		[property: JsonPropertyName("LeaderboardPromotionFinalizationDateTime")] string LeaderboardPromotionFinalizationDateTime,
		[property: JsonPropertyName("LeaderboardFinalizationDateTime")] string LeaderboardFinalizationDateTime,
		[property: JsonPropertyName("ContenderEligibilityExpiryDateTime")] string ContenderEligibilityExpiryDateTime,
		[property: JsonPropertyName("StageLockTime")] string StageLockTime,
		[property: JsonPropertyName("ParticipationRewardsActIDs")] IReadOnlyList<string>? ParticipationRewardsActIds,
		[property: JsonPropertyName("ParticipationRewards")] object? ParticipationRewards,
		[property: JsonPropertyName("TournamentWinnerRewards")] object? TournamentWinnerRewards,
		[property: JsonPropertyName("DivisionWinnerRewards")] object? DivisionWinnerRewards
	);

	public record PremierSeasons(
		[property: JsonPropertyName("PremierSeasons")] IReadOnlyList<PremierSeason>? Seasons
	);
}
