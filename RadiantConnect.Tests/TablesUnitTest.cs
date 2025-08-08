using RadiantConnect.Methods;
using static RadiantConnect.Tests.ValorantApi.Currencies;
using static RadiantConnect.Tests.ValorantApi.Agents;
using static RadiantConnect.Tests.ValorantApi.Maps;
using static RadiantConnect.Tests.ValorantApi.Weapons;

namespace RadiantConnect.Tests
{
	public class TablesUnitTest
	{
		/*
		 * This unit test will pull data from https://valorant-api.com/ and make sure all UUIDs are valid, and exist within RadiantConnect
		 */

		[Fact]
		public void CanLoadTables()
		{
			int count = ValorantTables.CurrencyIdToCurrency.Count;
			Assert.True(count > 0, "Failed to load tables");
			count = ValorantTables.AgentIdToAgent.Count;
			Assert.True(count > 0, "Failed to load tables");
			count = ValorantTables.GunIdToGun.Count;
			Assert.True(count > 0, "Failed to load tables");
			count = ValorantTables.InternalGameModeToGameMode.Count;
			Assert.True(count > 0, "Failed to load tables");
			count = ValorantTables.TierToRank.Count;
			Assert.True(count > 0, "Failed to load tables");
		}

		[Fact]
		public async Task TestAgents()
		{
			// Pull RadiantConnects internal Table
			Dictionary<string, string> internalAgentTable = ValorantTables.AgentIdToAgent;

			// Pull the data from Valorant API, and pull only playable (existing) characters.
			List<AgentData>? agents = await GetAgents();
			agents = agents?.Where(x => x.IsPlayableCharacter ?? false).ToList();

			// Make sure the net response actually was formed
			Assert.NotNull(agents);
			Assert.NotEmpty(agents);

			List<string> errors = [];
			// Compare the UUIDs from the API to the internal table
			foreach (AgentData agent in agents)
			{
				if (!internalAgentTable.TryGetValue(agent.Uuid, out _))
					errors.Add($"[Agent Missing] UUID {agent.Uuid} not found in internal table. {{ \"{agent.Uuid}\", \"{agent.DisplayName}\"}},");
				else if (!string.Equals(agent.DisplayName, internalAgentTable[agent.Uuid], StringComparison.OrdinalIgnoreCase))
					errors.Add($"[Agent Mismatch] UUID {agent.Uuid}: Expected '{agent.DisplayName}', Found '{internalAgentTable[agent.Uuid]}'");
			}

			if (errors.Count > 0)
			{
				string message = $"Agent validation failed:\n{string.Join("\n", errors)}";
				Assert.Fail(message);
			}
		}
    
		[Fact]
		public async Task TestCurrency()
		{
			// Pull RadiantConnects internal Table
			Dictionary<string, string> currencyIdToCurrency = ValorantTables.CurrencyIdToCurrency;

			// Pull the data from Valorant API
			List<Currency>? currencies = await GetCurrencies();

			// Make sure the net response actually was formed
			Assert.NotNull(currencies);
			Assert.NotEmpty(currencies);

			// Compare the UUIDs from the API to the internal table
			List<string> errors = [];

			// Currencies
			foreach (Currency currency in currencies)
			{
				if (!currencyIdToCurrency.TryGetValue(currency.Uuid, out _))
					errors.Add($"[Currency Missing] UUID {currency.Uuid} not found in internal table. {{ \"{currency.Uuid}\", \"{currency.DisplayName}\"}},");
				else if (!string.Equals(currency.DisplayName, currencyIdToCurrency[currency.Uuid], StringComparison.OrdinalIgnoreCase))
					errors.Add($"[Currency Mismatch] UUID {currency.Uuid}: Expected '{currency.DisplayName}', Found '{currencyIdToCurrency[currency.Uuid]}'");
			}


			if (errors.Count > 0)
			{
				string message = $"Currency validation failed:\n{string.Join("\n", errors)}";
				Assert.Fail(message);
			}
		}

		[Fact]
		public async Task TestMaps()
		{
			// Pull RadiantConnects internal Table
			Dictionary<string, string> internalMapNames = ValorantTables.InternalMapNames;

			// Pull the data from Valorant API
			List<MapsData>? maps = await GetMaps();

			// Make sure the net response actually was formed
			Assert.NotNull(maps);
			Assert.NotEmpty(maps);

			List<string> errors = [];

			// Compare the UUIDs from the API to the internal table
			foreach (MapsData map in maps)
			{
				string mapInternalName = map.MapUrl[(map.MapUrl.LastIndexOf('/') + 1)..];

				// Check UUID exists
				if (!internalMapNames.TryGetValue(map.Uuid, out _))
					errors.Add($"[UUID Missing] Map UUID {map.Uuid} not found in internal table. {{ \"{map.Uuid}\", \"{map.DisplayName}\"}},");

				// Check internal name exists
				if (!internalMapNames.TryGetValue(mapInternalName, out _))
					errors.Add($"[Internal Name Missing] Map internal name '{mapInternalName}' not found in internal table. {{ \"{mapInternalName}\", \"{map.DisplayName}\"}},");

				// Compare display name by UUID
				if (internalMapNames.TryGetValue(map.Uuid, out string? nameFromUuid) && !string.Equals(map.DisplayName, nameFromUuid, StringComparison.OrdinalIgnoreCase))
					errors.Add($"[UUID Name Mismatch] Map UUID {map.Uuid}: Expected '{map.DisplayName}', Found '{nameFromUuid}'");

				// Compare display name by internal name
				if (internalMapNames.TryGetValue(mapInternalName, out string? nameFromInternal) && !string.Equals(map.DisplayName, nameFromInternal, StringComparison.OrdinalIgnoreCase))
					errors.Add($"[Internal Name Mismatch] Map '{mapInternalName}': Expected '{map.DisplayName}', Found '{nameFromInternal}'");
			}

			if (errors.Count > 0)
			{
				string message = $"Map validation failed:\n{string.Join("\n", errors)}";
				Assert.Fail(message);
			}
		}
    
		[Fact]
		public async Task TestWeapons()
		{
			// Pull RadiantConnects internal Table
			Dictionary<string, string> internalWeaponNames = ValorantTables.GunIdToGun;

			// Pull the data from Valorant API
			List<WeaponData>? weapons = await GetWeapons();

			// Make sure the net response actually was formed
			Assert.NotNull(weapons);
			Assert.NotEmpty(weapons);

			// Compare the UUIDs from the API to the internal table

			List<string> errors = [];
			foreach (WeaponData weapon in weapons)
			{
				if (!internalWeaponNames.TryGetValue(weapon.Uuid, out _))
					errors.Add($"[Weapon Missing] UUID {weapon.Uuid} not found in internal table. {{ \"{weapon.Uuid}\", \"{weapon.DisplayName}\"}},");
				else if (!string.Equals(weapon.DisplayName, internalWeaponNames[weapon.Uuid], StringComparison.OrdinalIgnoreCase))
					errors.Add($"[Weapon Mismatch] UUID {weapon.Uuid}: Expected '{weapon.DisplayName}', Found '{internalWeaponNames[weapon.Uuid]}'");
			}

			if (errors.Count > 0)
			{
				string message = $"Weapon validation failed:\n{string.Join("\n", errors)}";
				Assert.Fail(message);
			}
		}
	}
}
