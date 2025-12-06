// ReSharper disable All
#pragma warning disable CA1707
#pragma warning disable IDE0130

namespace RadiantConnect.Methods
{
	/// <summary>
	/// Provides static lookup tables and mappings for Valorant-related identifiers,
	/// including queues, agents, maps, items, ranks, and currencies.
	/// </summary>
	public static class ValorantTables
	{
		/// <summary>
		/// Represents the different queue types in Valorant.
		/// </summary>
		public enum QueueId
		{
			/// <summary>Unrated queue.</summary>
			unrated,

			/// <summary>Competitive queue.</summary>
			competitive,

			/// <summary>Swiftplay queue.</summary>
			swiftplay,

			/// <summary>Spike Rush queue.</summary>
			spikerush,

			/// <summary>Deathmatch queue.</summary>
			deathmatch,

			/// <summary>Escalation/ggteam queue.</summary>
			ggteam,

			/// <summary>Team Deathmatch (HURM) queue.</summary>
			hurm
		}

		/// <summary>
		/// Represents the current state of a party.
		/// </summary>
		public enum PartyState
		{
			/// <summary>Party is open for new members.</summary>
			OPEN,

			/// <summary>Party is closed; no new members can join.</summary>
			CLOSED
		}

		/// <summary>
		/// Represents the various item types in Valorant.
		/// </summary>
		public enum ItemType
		{
			/// <summary>Playable agents.</summary>
			Agents,

			/// <summary>Player contracts.</summary>
			Contracts,

			/// <summary>Sprays.</summary>
			Sprays,

			/// <summary>Gun buddies.</summary>
			GunBuddies,

			/// <summary>Player cards.</summary>
			Cards,

			/// <summary>Weapon skins.</summary>
			Skins,

			/// <summary>Skin variants (chromas).</summary>
			SkinVariants,

			/// <summary>Titles.</summary>
			Titles,

			/// <summary>Flexes or emotes.</summary>
			Flexes,

			/// <summary>Totems or in-game decorations.</summary>
			Totems,

			/// <summary>Player cards.</summary>
			PlayerCards
		}

		/// <summary>
		/// Represents playable agents in Valorant.
		/// </summary>
		public enum Agent
		{
			/// <summary>Astra agent.</summary>
			Astra,

			/// <summary>Breach agent.</summary>
			Breach,

			/// <summary>Brimstone agent.</summary>
			Brimstone,

			/// <summary>Chamber agent.</summary>
			Chamber,

			/// <summary>Clove agent.</summary>
			Clove,

			/// <summary>Cypher agent.</summary>
			Cypher,

			/// <summary>Deadlock agent.</summary>
			Deadlock,

			/// <summary>Fade agent.</summary>
			Fade,

			/// <summary>Gekko agent.</summary>
			Gekko,

			/// <summary>Harbor agent.</summary>
			Harbor,

			/// <summary>ISO agent.</summary>
			ISO,

			/// <summary>Jett agent.</summary>
			Jett,

			/// <summary>KAY/O agent.</summary>
			KAY_O,

			/// <summary>Killjoy agent.</summary>
			Killjoy,

			/// <summary>Neon agent.</summary>
			Neon,

			/// <summary>Omen agent.</summary>
			Omen,

			/// <summary>Phoenix agent.</summary>
			Phoenix,

			/// <summary>Raze agent.</summary>
			Raze,

			/// <summary>Reyna agent.</summary>
			Reyna,

			/// <summary>Sage agent.</summary>
			Sage,

			/// <summary>Skye agent.</summary>
			Skye,

			/// <summary>Sova agent.</summary>
			Sova,

			/// <summary>Tejo agent.</summary>
			Tejo,

			/// <summary>Viper agent.</summary>
			Viper,

			/// <summary>Veto agent.</summary>
			Veto,

			/// <summary>Vyse agent.</summary>
			Vyse,

			/// <summary>Waylay agent.</summary>
			Waylay,

			/// <summary>Yoru agent.</summary>
			Yoru
		}

		/// <summary>
		/// Represents the different types of chat channels.
		/// </summary>
		public enum ChatType
		{
			/// <summary>Group chat channel.</summary>
			groupchat,

			/// <summary>Private or general chat channel.</summary>
			chat,

			/// <summary>System messages channel.</summary>
			system
		}

		/// <summary>
		/// Maps currency IDs to human-readable currency names.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> CurrencyIdToCurrency = new Dictionary<string, string>()
		{
			{ "85ad13f7-3d1b-5128-9eb2-7cd8ee0b5741", "Valorant Points" },
			{ "85ca954a-41f2-ce94-9b45-8ca3dd39a00d", "Kingdom Credits" },
			{ "e59aa87c-4cbf-517a-5983-6e81511be9b7", "Radianite Points" },
			{ "f08d4ae3-939c-4576-ab26-09ce1f23bb37", "Free Agents" },
		};

		/// <summary>
		/// Maps item types to their unique string identifiers.
		/// </summary>
		public static readonly IReadOnlyDictionary<ItemType, string> ItemTypeToId = new Dictionary<ItemType, string>()
		{
			{ ItemType.Agents, "01bb38e1-da47-4e6a-9b3d-945fe4655707" },
			{ ItemType.Contracts, "f85cb6f7-33e5-4dc8-b609-ec7212301948" },
			{ ItemType.Sprays, "d5f120f8-ff8c-4aac-92ea-f2b5acbe9475" },
			{ ItemType.GunBuddies, "dd3bf334-87f3-40bd-b043-682a57a8dc3a" },
			{ ItemType.Cards, "3f296c07-64c3-494c-923b-fe692a4fa1bd" },
			{ ItemType.Skins, "e7c63390-eda7-46e0-bb7a-a6abdacd2433" },
			{ ItemType.SkinVariants, "3ad1b2b2-acdb-4524-852f-954a76ddae0a" },
			{ ItemType.Titles, "de7caa6b-adf7-4588-bbd1-143831e786c6" },
			{ ItemType.Flexes, "03a572de-4234-31ed-d344-ababa488f981" },
			{ ItemType.Totems, "03a572de-4234-31ed-d344-ababa488f981" },
			{ ItemType.PlayerCards, "3f296c07-64c3-494c-923b-fe692a4fa1bd" }
		};

		/// <summary>
		/// Maps internal map IDs to their display names.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> InternalMapNames = new Dictionary<string, string>()
		{
			{ "1f10dab3-4294-3827-fa35-c2aa00213cf3", "Basic Training"},
			{ "2bee0dc9-4ffe-519b-1cbd-7fbe763a6047", "Haven" },
			{ "2c09d728-42d5-30d8-43dc-96a05cc7ee9d", "Drift" },
			{ "2c9d57ec-4431-9c5e-2939-8f9ef6dd5cba", "Bind" },
			{ "2fb9a4fd-47b8-4e7d-a969-74b4046ebd53", "Breeze" },
			{ "2fe4ed3a-450a-948b-6d6b-e89a78e680a9", "Lotus" },
			{ "7eaecc1b-4337-bbf6-6ab9-04b8f06b3319", "Ascent" },
			{ "224b0a95-48b9-f703-1bd8-67aca101a61f", "Abyss"},
			{ "690b3ed2-4dff-945b-8223-6da834e30d24", "District" },
			{ "5914d1e0-40c4-cfdd-6b88-eba06347686c", "The Range"},
			{ "12452a9d-48c3-0b02-e7eb-0381c3520404", "Kasbah" },
			{ "92584fbe-486a-b1b2-9faa-39b0f486b498", "Sunset" },
			{ "b529448b-4d60-346e-e89e-00a4c527a405", "Fracture" },
			{ "d6336a5a-428f-c591-98db-c8a291159134", "Glitch"},
			{ "d960549e-485c-e861-8d71-aa9d1aed12a2", "Split" },
			{ "de28aa9b-4cbe-1003-320e-6cb3ec309557", "Piazza" },
			{ "e2ad5c54-4114-a870-9641-8ea21279579a", "Icebox" },
			{ "ee613ee9-28b7-4beb-9666-08db13bb2244", "The Range" },
			{ "fd267378-4d1d-484f-ff52-77821ed10dc2", "Pearl" },
			{ "1c18ab1f-420d-0d8b-71d0-77ad3c439115", "Corrode" },
			{ "Rook", "Corrode" },
			{ "Ascent", "Ascent" },
			{ "Bonsai", "Split" },
			{ "Canyon", "Fracture" },
			{ "Duality", "Bind" },
			{ "Foxtrot", "Breeze" },
			{ "HURM_Alley", "District" },
			{ "HURM_Bowl", "Kasbah"},
			{ "HURM_Helix", "Drift"},
			{ "HURM_HighTide", "Glitch"},
			{ "HURM_Yard", "Piazza"},
			{ "Infinity", "Abyss"},
			{ "Jam", "Lotus" },
			{ "Juliett", "Sunset" },
			{ "NPEV2", "Basic Training"},
			{ "Pitt", "Pearl" },
			{ "Port", "Icebox" },
			{ "RangeV2", "The Range"},
			{ "Range", "The Range"},
			{ "Triad", "Haven" },
		};

		/// <summary>
		/// Maps internal gun IDs to human-readable gun names.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> GunIdToGun = new Dictionary<string, string>()
		{
			{ "0afb2636-4093-c63b-4ef1-1e97966e2a3e", "SPIKE" },
			{ "3de32920-4a8f-0499-7740-648a5bf95470", "Golden Gun" },
			{ "2f59173c-4bed-b6c3-2191-dea9b58be9c7", "Melee" },
			{ "63e6c2b6-4a8e-869c-3d4c-e38355226584", "Odin" },
			{ "55d8a0f4-4274-ca67-fe2c-06ab45efdf58", "Ares" },
			{ "9c82e19d-4575-0200-1a81-3eacf00cf872", "Vandal" },
			{ "ae3de142-4d85-2547-dd26-4e90bed35cf7", "Bulldog" },
			{ "ee8e8d15-496b-07ac-e5f6-8fae5d4c7b1a", "Phantom" },
			{ "ec845bf4-4f79-ddda-a3da-0db3774b2794", "Judge" },
			{ "910be174-449b-c412-ab22-d0873436b21b", "Bucky" },
			{ "44d4e95c-4157-0037-81b2-17841bf2e8e3", "Frenzy" },
			{ "29a0cfab-485b-f5d5-779a-b59f85e204a8", "Classic" },
			{ "1baa85b4-4c70-1284-64bb-6481dfc3bb4e", "Ghost" },
			{ "e336c6b8-418d-9340-d77f-7a9e4cfe0702", "Sheriff" },
			{ "42da8ccc-40d5-affc-beec-15aa47b42eda", "Shorty" },
			{ "a03b24d3-4319-996d-0f8c-94bbfba1dfc7", "Operator" },
			{ "5f0aaf7a-4289-3998-d5ff-eb9a5cf7ef5c", "Outlaw" },
			{ "4ade7faa-4cf1-8376-95ef-39884480959b", "Guardian" },
			{ "c4883e50-4494-202c-3ec3-6b8a9284f00b", "Marshal" },
			{ "462080d1-4035-2937-7c09-27aa2a5c27a7", "Spectre" },
			{ "f7e1b454-4ad4-1063-ec0a-159e56b58941", "Stinger" },
		};

		/// <summary>
		/// Maps internal agent IDs to agent names.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> AgentIdToAgent = new Dictionary<string, string>()
		{
			{ "41fb69c1-4189-7b37-f117-bcaf1e96f1bf", "Astra" },
			{ "5f8d3a7f-467b-97f3-062c-13acf203c006", "Breach" },
			{ "9f0d8ba9-4140-b941-57d3-a7ad57c6b417", "Brimstone" },
			{ "22697a3d-45bf-8dd7-4fec-84a9e28c69d7", "Chamber" },
			{ "1dbf2edd-4729-0984-3115-daa5eed44993", "Clove" },
			{ "117ed9e3-49f3-6512-3ccf-0cada7e3823b", "Cypher" },
			{ "cc8b64c8-4b25-4ff9-6e7f-37b4da43d235", "Deadlock" },
			{ "dade69b4-4f5a-8528-247b-219e5a1facd6", "Fade" },
			{ "e370fa57-4757-3604-3648-499e1f642d3f", "Gekko" },
			{ "95b78ed7-4637-86d9-7e41-71ba8c293152", "Harbor" },
			{ "0e38b510-41a8-5780-5e8f-568b2a4f2d6c", "ISO" },
			{ "add6443a-41bd-e414-f6ad-e58d267f4e95", "Jett" },
			{ "601dbbe7-43ce-be57-2a40-4abd24953621", "KAY/O" },
			{ "1e58de9c-4950-5125-93e9-a0aee9f98746", "Killjoy" },
			{ "bb2a4828-46eb-8cd1-e765-15848195d751", "Neon" },
			{ "8e253930-4c05-31dd-1b6c-968525494517", "Omen" },
			{ "eb93336a-449b-9c1b-0a54-a891f7921d69", "Phoenix" },
			{ "f94c3b30-42be-e959-889c-5aa313dba261", "Raze" },
			{ "a3bfb853-43b2-7238-a4f1-ad90e9e46bcc", "Reyna" },
			{ "569fdd95-4d10-43ab-ca70-79becc718b46", "Sage" },
			{ "6f2a04ca-43e0-be17-7f36-b3908627744d", "Skye" },
			{ "320b2a48-4d9b-a075-30f1-1f93a9b638fa", "Sova" },
			{ "b444168c-4e35-8076-db47-ef9bf368f384", "Tejo" },
			{ "707eab51-4836-f488-046a-cda6bf494859", "Viper" },
			{ "efba5359-4016-a1e5-7626-b1ae76895940", "Vyse" },
			{ "92eeef5d-43b5-1d4a-8d03-b3927a09034b", "Veto" },
			{ "df1cb487-4902-002e-5c17-d28e83e78588", "Waylay" },
			{ "7f94d92c-4234-0a36-9646-3a87eb8b5c89", "Yoru" }
		};
		
		/// <summary>
		/// Maps agent enum values to their internal agent ID strings.
		/// </summary>
		public static readonly IReadOnlyDictionary<Agent, string> AgentToId = new Dictionary<Agent, string>()
		{
			{ Agent.Astra, "41fb69c1-4189-7b37-f117-bcaf1e96f1bf" },
			{ Agent.Breach, "5f8d3a7f-467b-97f3-062c-13acf203c006" },
			{ Agent.Brimstone, "9f0d8ba9-4140-b941-57d3-a7ad57c6b417" },
			{ Agent.Chamber, "22697a3d-45bf-8dd7-4fec-84a9e28c69d7" },
			{ Agent.Clove, "1dbf2edd-4729-0984-3115-daa5eed44993" },
			{ Agent.Cypher, "117ed9e3-49f3-6512-3ccf-0cada7e3823b" },
			{ Agent.Deadlock, "cc8b64c8-4b25-4ff9-6e7f-37b4da43d235" },
			{ Agent.Fade, "dade69b4-4f5a-8528-247b-219e5a1facd6" },
			{ Agent.Gekko, "e370fa57-4757-3604-3648-499e1f642d3f" },
			{ Agent.Harbor, "95b78ed7-4637-86d9-7e41-71ba8c293152" },
			{ Agent.ISO, "0e38b510-41a8-5780-5e8f-568b2a4f2d6c" },
			{ Agent.Jett, "add6443a-41bd-e414-f6ad-e58d267f4e95" },
			{ Agent.KAY_O, "601dbbe7-43ce-be57-2a40-4abd24953621" },
			{ Agent.Killjoy, "1e58de9c-4950-5125-93e9-a0aee9f98746" },
			{ Agent.Neon, "bb2a4828-46eb-8cd1-e765-15848195d751" },
			{ Agent.Omen, "8e253930-4c05-31dd-1b6c-968525494517" },
			{ Agent.Phoenix, "eb93336a-449b-9c1b-0a54-a891f7921d69" },
			{ Agent.Raze, "f94c3b30-42be-e959-889c-5aa313dba261" },
			{ Agent.Reyna, "a3bfb853-43b2-7238-a4f1-ad90e9e46bcc" },
			{ Agent.Sage, "569fdd95-4d10-43ab-ca70-79becc718b46" },
			{ Agent.Skye, "6f2a04ca-43e0-be17-7f36-b3908627744d" },
			{ Agent.Sova, "320b2a48-4d9b-a075-30f1-1f93a9b638fa" },
			{ Agent.Tejo, "b444168c-4e35-8076-db47-ef9bf368f384" },
			{ Agent.Viper, "707eab51-4836-f488-046a-cda6bf494859" },
			{ Agent.Vyse, "efba5359-4016-a1e5-7626-b1ae76895940" },
			{ Agent.Veto, "92eeef5d-43b5-1d4a-8d03-b3927a09034b" },
			{ Agent.Waylay, "df1cb487-4902-002e-5c17-d28e83e78588" },
			{ Agent.Yoru, "7f94d92c-4234-0a36-9646-3a87eb8b5c89" }
		};

		/// <summary>
		/// Maps competitive tier numbers to their rank names.
		/// </summary>
		public static readonly IReadOnlyDictionary<long, string> TierToRank = new Dictionary<long, string>()
		{
			{ 0, "Unranked" },
			{ 1, "Unranked" },
			{ 2, "Unranked" },
			{ 3, "Iron 1" },
			{ 4, "Iron 2" },
			{ 5, "Iron 3" },
			{ 6, "Bronze 1" },
			{ 7, "Bronze 2" },
			{ 8, "Bronze 3" },
			{ 9, "Silver 1" },
			{ 10, "Silver 2" },
			{ 11, "Silver 3" },
			{ 12, "Gold 1" },
			{ 13, "Gold 2" },
			{ 14, "Gold 3" },
			{ 15, "Platinum 1" },
			{ 16, "Platinum 2" },
			{ 17, "Platinum 3" },
			{ 18, "Diamond 1" },
			{ 19, "Diamond 2" },
			{ 20, "Diamond 3" },
			{ 21, "Ascendant 1" },
			{ 22, "Ascendant 2" },
			{ 23, "Ascendant 3" },
			{ 24, "Immortal 1" },
			{ 25, "Immortal 2" },
			{ 26, "Immortal 3" },
			{ 27, "Radiant" }
		};

		/// <summary>
		/// Maps internal game mode IDs to human-readable game mode names.
		/// </summary>
		public static readonly IReadOnlyDictionary<string, string> InternalGameModeToGameMode = new Dictionary<string, string>()
		{
			{ "newmap", "New Map"},
			{ "competitive", "Competitive"},
			{ "unrated", "Unrated"},
			{ "swiftplay", "Swiftplay"},
			{ "spikerush", "Spike Rush"},
			{ "deathmatch", "Deathmatch"},
			{ "ggteam", "Escalation"},
			{ "onefa", "Replication"},
			{ "hurm", "Team Deathmatch"},
			{ "custom", "Custom"},
			{ "snowball", "Snowball Fight"},
			{ "", "Custom"}
		};
	}
}
