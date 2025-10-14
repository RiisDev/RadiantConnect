
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record Gun(
		[property: JsonPropertyName("ID")] string Id,
		[property: JsonPropertyName("SkinID")] string SkinId,
		[property: JsonPropertyName("SkinLevelID")] string SkinLevelId,
		[property: JsonPropertyName("ChromaID")] string ChromaId,
		[property: JsonPropertyName("CharmInstanceID")] string CharmInstanceId,
		[property: JsonPropertyName("CharmID")] string CharmId,
		[property: JsonPropertyName("CharmLevelID")] string CharmLevelId,
		[property: JsonPropertyName("Attachments")] IReadOnlyList<object> Attachments
	);

	public record Identity(
		[property: JsonPropertyName("PlayerCardID")] string PlayerCardId,
		[property: JsonPropertyName("PlayerTitleID")] string PlayerTitleId,
		[property: JsonPropertyName("AccountLevel")] long? AccountLevel,
		[property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderId,
		[property: JsonPropertyName("HideAccountLevel")] bool? HideAccountLevel
	);

	public record PlayerLoadout(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("Version")] long? Version,
		[property: JsonPropertyName("Guns")] IReadOnlyList<Gun> Guns,
		[property: JsonPropertyName("Sprays")] IReadOnlyList<Spray> Sprays,
		[property: JsonPropertyName("Identity")] Identity Identity,
		[property: JsonPropertyName("Incognito")] bool? Incognito
	);

	public record Spray(
		[property: JsonPropertyName("EquipSlotID")] string EquipSlotId,
		[property: JsonPropertyName("SprayID")] string SprayId,
		[property: JsonPropertyName("SprayLevelID")] object SprayLevelID
	);
}