using System.Text.Json.Serialization;

namespace RadiantConnect.Tests.ValorantApi
{
	internal class Agents
	{
		internal static async Task<List<AgentData>?> GetAgents()
		{
			AgentRoot? agentRoot = await Client.Get<AgentRoot>("agents");
			return agentRoot?.Data.ToList();
		}

		public record Ability(
			[property: JsonPropertyName("slot")] string Slot,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("description")] string Description,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon
		);

		public record AgentData(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("description")] string Description,
			[property: JsonPropertyName("developerName")] string DeveloperName,
			[property: JsonPropertyName("releaseDate")] DateTime? ReleaseDate,
			[property: JsonPropertyName("characterTags")] IReadOnlyList<string> CharacterTags,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("displayIconSmall")] string DisplayIconSmall,
			[property: JsonPropertyName("bustPortrait")] string BustPortrait,
			[property: JsonPropertyName("fullPortrait")] string FullPortrait,
			[property: JsonPropertyName("fullPortraitV2")] string FullPortraitV2,
			[property: JsonPropertyName("killfeedPortrait")] string KillfeedPortrait,
			[property: JsonPropertyName("background")] string Background,
			[property: JsonPropertyName("backgroundGradientColors")] IReadOnlyList<string> BackgroundGradientColors,
			[property: JsonPropertyName("assetPath")] string AssetPath,
			[property: JsonPropertyName("isFullPortraitRightFacing")] bool? IsFullPortraitRightFacing,
			[property: JsonPropertyName("isPlayableCharacter")] bool? IsPlayableCharacter,
			[property: JsonPropertyName("isAvailableForTest")] bool? IsAvailableForTest,
			[property: JsonPropertyName("isBaseContent")] bool? IsBaseContent,
			[property: JsonPropertyName("role")] Role Role,
			[property: JsonPropertyName("recruitmentData")] RecruitmentData RecruitmentData,
			[property: JsonPropertyName("abilities")] IReadOnlyList<Ability> Abilities,
			[property: JsonPropertyName("voiceLine")] object VoiceLine
		);

		public record RecruitmentData(
			[property: JsonPropertyName("counterId")] string CounterId,
			[property: JsonPropertyName("milestoneId")] string MilestoneId,
			[property: JsonPropertyName("milestoneThreshold")] int? MilestoneThreshold,
			[property: JsonPropertyName("useLevelVpCostOverride")] bool? UseLevelVpCostOverride,
			[property: JsonPropertyName("levelVpCostOverride")] int? LevelVpCostOverride,
			[property: JsonPropertyName("startDate")] DateTime? StartDate,
			[property: JsonPropertyName("endDate")] DateTime? EndDate
		);

		public record Role(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("description")] string Description,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record AgentRoot(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<AgentData> Data
		);


	}
}
