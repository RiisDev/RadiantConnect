
// ReSharper disable All

namespace RadiantConnect.Network.PVPEndpoints.DataTypes
{
	public record Gun(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("SkinID")] string SkinID,
		[property: JsonPropertyName("SkinLevelID")] string SkinLevelID,
		[property: JsonPropertyName("ChromaID")] string ChromaID,
		[property: JsonPropertyName("CharmInstanceID")] string CharmInstanceID,
		[property: JsonPropertyName("CharmID")] string CharmID,
		[property: JsonPropertyName("CharmLevelID")] string CharmLevelID,
		[property: JsonPropertyName("Attachments")] IReadOnlyList<object> Attachments
	);

	public record Identity(
		[property: JsonPropertyName("PlayerCardID")] string PlayerCardID,
		[property: JsonPropertyName("PlayerTitleID")] string PlayerTitleID,
		[property: JsonPropertyName("AccountLevel")] long? AccountLevel,
		[property: JsonPropertyName("PreferredLevelBorderID")] string PreferredLevelBorderID,
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
		[property: JsonPropertyName("EquipSlotID")] string EquipSlotID,
		[property: JsonPropertyName("SprayID")] string SprayID,
		[property: JsonPropertyName("SprayLevelID")] object SprayLevelID
	);
}