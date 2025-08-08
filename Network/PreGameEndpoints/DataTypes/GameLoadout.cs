namespace RadiantConnect.Network.PreGameEndpoints.DataTypes
{
	// ReSharper disable All

	public record Ghost(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Classic(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Melee(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record SkinChroma(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Item")] Item Item
	);

	public record Shorty(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Frenzy(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Spectre(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Guardian(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Ares(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Odin(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record _7725866571d14623Bc7244db9bd5b3b3(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Item")] Item Item
	);

	public record Bucky(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Vandal(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Operator(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Bulldug(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record AESSelection(
		[property: JsonPropertyName("SocketID")] string SocketID,
		[property: JsonPropertyName("AssetID")] string AssetID,
		[property: JsonPropertyName("TypeID")] string TypeID
	);

	public record Bcef87d6209b46c68b19Fbe40bd95abc(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Item")] Item Item
	);

	public record Marshal(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Buddy(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Item")] Item Item
	);

	public record Sheriff(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record SkinLevel(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("Item")] Item Item
	);

	public record Judge(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Phantom(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Expressions(
		[property: JsonPropertyName("AESSelections")] IReadOnlyList<AESSelection> AESSelections
	);

	public record Stinger(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID,
		[property: JsonPropertyName("Sockets")] Sockets Sockets
	);

	public record Item(
		[property: JsonPropertyName("ID")] string ID,
		[property: JsonPropertyName("TypeID")] string TypeID
	);

	public record Items(
		[property: JsonPropertyName("1baa85b4-4c70-1284-64bb-6481dfc3bb4e")] Ghost Ghost,
		[property: JsonPropertyName("29a0cfab-485b-f5d5-779a-b59f85e204a8")] Classic Classic,
		[property: JsonPropertyName("2f59173c-4bed-b6c3-2191-dea9b58be9c7")] Melee Melee,
		[property: JsonPropertyName("42da8ccc-40d5-affc-beec-15aa47b42eda")] Shorty Shorty,
		[property: JsonPropertyName("44d4e95c-4157-0037-81b2-17841bf2e8e3")] Frenzy Frenzy,
		[property: JsonPropertyName("462080d1-4035-2937-7c09-27aa2a5c27a7")] Spectre Spectre,
		[property: JsonPropertyName("4ade7faa-4cf1-8376-95ef-39884480959b")] Guardian Guardian,
		[property: JsonPropertyName("55d8a0f4-4274-ca67-fe2c-06ab45efdf58")] Ares Ares,
		[property: JsonPropertyName("63e6c2b6-4a8e-869c-3d4c-e38355226584")] Odin Odin,
		[property: JsonPropertyName("910be174-449b-c412-ab22-d0873436b21b")] Bucky Bucky,
		[property: JsonPropertyName("9c82e19d-4575-0200-1a81-3eacf00cf872")] Vandal Vandal,
		[property: JsonPropertyName("a03b24d3-4319-996d-0f8c-94bbfba1dfc7")] Operator Operator,
		[property: JsonPropertyName("ae3de142-4d85-2547-dd26-4e90bed35cf7")] Bulldug Bulldog,
		[property: JsonPropertyName("c4883e50-4494-202c-3ec3-6b8a9284f00b")] Marshal Marshal,
		[property: JsonPropertyName("e336c6b8-418d-9340-d77f-7a9e4cfe0702")] Sheriff Sheriff,
		[property: JsonPropertyName("ec845bf4-4f79-ddda-a3da-0db3774b2794")] Judge Judge,
		[property: JsonPropertyName("ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a")] Phantom Phantom,
		[property: JsonPropertyName("f7e1b454-4ad4-1063-ec0a-159e56b58941")] Stinger Stinger
	);

	public record Loadout(
		[property: JsonPropertyName("Subject")] string Subject,
		[property: JsonPropertyName("Sprays")] Sprays Sprays,
		[property: JsonPropertyName("Expressions")] Expressions Expressions,
		[property: JsonPropertyName("Items")] Items Items
	);

	public record GameLoadout(
		[property: JsonPropertyName("Loadouts")] IReadOnlyList<Loadout> Loadouts,
		[property: JsonPropertyName("LoadoutsValid")] bool LoadoutsValid
	);

	public record Sockets(
		[property: JsonPropertyName("3ad1b2b2-acdb-4524-852f-954a76ddae0a")] SkinChroma SkinChroma,
		[property: JsonPropertyName("77258665-71d1-4623-bc72-44db9bd5b3b3")] _7725866571d14623Bc7244db9bd5b3b3 _7725866571d14623Bc7244db9bd5b3b3,
		[property: JsonPropertyName("bcef87d6-209b-46c6-8b19-fbe40bd95abc")] Bcef87d6209b46c68b19Fbe40bd95abc Bcef87d6209b46c68b19Fbe40bd95abc,
		[property: JsonPropertyName("dd3bf334-87f3-40bd-b043-682a57a8dc3a")] Buddy Buddy,
		[property: JsonPropertyName("e7c63390-eda7-46e0-bb7a-a6abdacd2433")] SkinLevel SkinLevel
	);

	public record Sprays(
		[property: JsonPropertyName("SpraySelections")] IReadOnlyList<SpraySelection> SpraySelections
	);

	public record SpraySelection(
		[property: JsonPropertyName("SocketID")] string SocketID,
		[property: JsonPropertyName("SprayID")] string SprayID,
		[property: JsonPropertyName("LevelID")] string LevelID
	);
}

