using System.Text.Json.Serialization;

namespace RadiantConnect.Tests.ValorantApi
{
	internal class Currencies
	{
		public static async Task<List<Currency>?> GetCurrencies()
		{
			CurrencyRoot? agentRoot = await Client.Get<CurrencyRoot>("currencies");
			return agentRoot?.Data.ToList();
		}

		public record Currency(
			[property: JsonPropertyName("uuid")] string Uuid,
			[property: JsonPropertyName("displayName")] string DisplayName,
			[property: JsonPropertyName("displayNameSingular")] string DisplayNameSingular,
			[property: JsonPropertyName("displayIcon")] string DisplayIcon,
			[property: JsonPropertyName("largeIcon")] string LargeIcon,
			[property: JsonPropertyName("rewardPreviewIcon")] string RewardPreviewIcon,
			[property: JsonPropertyName("assetPath")] string AssetPath
		);

		public record CurrencyRoot(
			[property: JsonPropertyName("status")] int? Status,
			[property: JsonPropertyName("data")] IReadOnlyList<Currency> Data
		);
	}
}
