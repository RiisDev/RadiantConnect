namespace RadiantConnect.Network.GoldstarsEndpoints.DataTypes
{
	// ponytail: field names below (tempT/tempS/tempA/etc.) match Riot's actual wire JSON verbatim - not obfuscated by us.

	public record PlayerGoldstarEntry(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("tempC")] int TempC,
		[property: JsonPropertyName("tempB")] int TempB
	);

	public record PlayerGoldstarAct(
		[property: JsonPropertyName("tempS")] string TempS,
		[property: JsonPropertyName("tempA")] Dictionary<string, PlayerGoldstarEntry> TempA
	);

	public record PlayerGoldstarMatchEntry(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("tempV")] int TempV,
		[property: JsonPropertyName("tempB")] bool TempB
	);

	public record PlayerGoldstarMatchPlayer(
		[property: JsonPropertyName("tempA")] Dictionary<string, PlayerGoldstarMatchEntry> TempA
	);

	public record PlayerGoldstarMatch(
		[property: JsonPropertyName("id")] string Id,
		[property: JsonPropertyName("tempG")] int TempG,
		[property: JsonPropertyName("tempP")] Dictionary<string, PlayerGoldstarMatchPlayer> TempP
	);

	public record PlayerGoldstars(
		[property: JsonPropertyName("puuid")] string Puuid,
		[property: JsonPropertyName("version")] int Version,
		[property: JsonPropertyName("tempS")] Dictionary<string, PlayerGoldstarAct> TempS,
		[property: JsonPropertyName("tempM")] IReadOnlyList<PlayerGoldstarMatch> TempM
	);
}
