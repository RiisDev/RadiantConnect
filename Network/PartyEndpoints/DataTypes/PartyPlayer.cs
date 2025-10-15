

// ReSharper disable All

namespace RadiantConnect.Network.PartyEndpoints.DataTypes
{
	public record PlatformInfo(
		[property: JsonPropertyName("platformType")] string PlatformType,
		[property: JsonPropertyName("platformOS")] string PlatformOS,
		[property: JsonPropertyName("platformOSVersion")] string PlatformOSVersion,
		[property: JsonPropertyName("platformChipset")] string PlatformChipset
	);

	public record PartyPlayer(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("Version")] long? Version,
		[property: JsonPropertyName("CurrentPartyID")] string CurrentPartyId,
		[property: JsonPropertyName("Invites")] object Invites,
		[property: JsonPropertyName("Requests")] IReadOnlyList<object> Requests,
		[property: JsonPropertyName("PlatformInfo")] PlatformInfo PlatformInfo
	);
}
