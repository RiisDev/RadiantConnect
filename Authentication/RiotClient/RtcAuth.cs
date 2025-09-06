using RadiantConnect.Authentication.DriverRiotAuth.Records;

namespace RadiantConnect.Authentication.RiotClient
{
	internal class RtcAuth
	{
		private static string RiotClientSettings => Path.Join(
			Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
			"AppData",
			"Local",
			"Riot Games",
			"Riot Client",
			"Data",
			"RiotGamesPrivateSettings.yaml"
		);

		internal async Task<RSOAuth?> Run(string? fileLocation = null, Authentication? auth = null, bool skipTdid = false, bool skipClid = false, bool skipCsid = false)
		{
			fileLocation ??= RiotClientSettings;

			if (auth == null) throw new Exception("Authentication somehow null, report bug to github, this should never be seen.");

			Dictionary<string, string?> cookieValues = await GetCookiesFromYaml(fileLocation);

			cookieValues.TryGetValue("ssid", out string? ssid);

			if (ssid.IsNullOrEmpty()) 
				throw new RadiantConnectAuthException("Failed to grab ssid from client cookies.");
			
			// Has built in checking
			if (!skipTdid && cookieValues["tdid"].IsNullOrEmpty())
				cookieValues["tdid"] = await GetTdidFallback(fileLocation);

			cookieValues.TryGetValue("clid", out string? clid);
			cookieValues.TryGetValue("csid", out string? csid);
			cookieValues.TryGetValue("tdid", out string? tdid);
			cookieValues.TryGetValue("asid", out string? asid);

			if (!skipClid && clid.IsNullOrEmpty())
				throw new RadiantConnectAuthException("Failed to grab clid from client cookies.");
			if (!skipCsid && csid.IsNullOrEmpty())
				throw new RadiantConnectAuthException("Failed to grab csid from client cookies.");
			if (!skipTdid && tdid.IsNullOrEmpty())
				throw new RadiantConnectAuthException("Failed to grab tdid from client cookies.");
			
			return await auth.AuthenticateWithSsid(
				ssid: ssid,
				clid: clid,
				csid: csid,
				tdid: tdid,
				asid: asid
			);
		}

		private static async Task<string> GetTdidFallback(string fileLocation)
		{
			Dictionary<string, object> yamlData = await GetYamlData(fileLocation);

			if (!yamlData.ContainsKey("rso-authenticator") || yamlData["rso-authenticator"] is not Dictionary<string, object> rsoAuthenticator)
				throw new RadiantConnectAuthException("Invalid Riot Client settings file, missing 'session' section.");

			if (!rsoAuthenticator.ContainsKey("tdid") || rsoAuthenticator["tdid"] is not Dictionary<string, object> tdidData)
				throw new RadiantConnectAuthException("Invalid Riot Client settings file, missing 'tdid' section.");

			tdidData.TryGetValue("value", out object? tdidValue);

			string tdid = tdidValue as string ?? string.Empty;

			if (tdid.IsNullOrEmpty())
				throw new RadiantConnectAuthException("Failed to grab tdid from client cookies.");

			return tdid;
		}

		private static async Task<Dictionary<string, string?>> GetCookiesFromYaml(string fileLocation)
		{
			Dictionary<string, object> yamlData = await GetYamlData(fileLocation);

			if (!yamlData.ContainsKey("riot-login") || yamlData["riot-login"] is not Dictionary<string, object> riotLoginData)
				throw new RadiantConnectAuthException("Invalid Riot Client settings file, missing 'session' section.");

			if (!riotLoginData.ContainsKey("persist") || riotLoginData["persist"] is not Dictionary<string, object> persistData)
				throw new RadiantConnectAuthException("Invalid Riot Client settings file, missing 'persist' section.");

			if (!persistData.ContainsKey("session") || persistData["session"] is not Dictionary<string, object> sessionData)
				throw new RadiantConnectAuthException("Invalid Riot Client settings file, missing 'session' section.");

			List<object> allCookies = [];

			foreach (KeyValuePair<string, object> kvp in sessionData)
				if (kvp.Value is List<object> cookiesList)
					allCookies.AddRange(cookiesList);

			return GetCookieValues(allCookies, "ssid", "clid", "csid", "tdid");
		}
		
		private static async Task<Dictionary<string, object>> GetYamlData(string fileLocation)
		{
			if (!File.Exists(fileLocation)) throw new FileNotFoundException("Riot Client persistent file not found.", fileLocation);
			if (!fileLocation.EndsWith(".yaml")) throw new Exception($"Invalid file detected, expected .yaml got {Path.GetExtension(fileLocation)}");

			string fileData = await File.ReadAllTextAsync(fileLocation);

			return ParseSimpleYaml(fileData);
		}

		private static Dictionary<string, object> ParseSimpleYaml(string yaml)
		{
			Dictionary<string, object> result = [];
			Stack<(int Indent, Dictionary<string, object> Dict)> stack = new();
			stack.Push((0, result));

			string? currentKey = null;

			foreach (string rawLine in yaml.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries))
			{
				string line = rawLine.Trim();

				if (line.Length == 0 || line.StartsWith("#")) continue;

				int indent = rawLine.Length - rawLine.TrimStart().Length;

				while (stack.Count > 1 && indent <= stack.Peek().Indent) stack.Pop();

				Dictionary<string, object> currentDict = stack.Peek().Dict;

				if (line.StartsWith("-"))
				{
					string value = line[1..].Trim();

					if (currentKey == null)
						throw new InvalidOperationException("List item found without a key");

					if (!currentDict.TryGetValue(currentKey, out object? existing) || existing is not List<object> list)
					{
						list = [];
						currentDict[currentKey] = list;
					}

					if (value.Contains(":"))
					{
						Dictionary<string, object> newDict = [];
						list.Add(newDict);
						stack.Push((indent + 2, newDict));

						string[] parts = value.Split(':', 2);
						newDict[parts[0].Trim()] = parts[1].Trim().Trim('"');
						currentKey = null;
					}
					else list.Add(value.Trim('"'));
				}
				else if (line.Contains(":"))
				{
					string[] parts = line.Split(':', 2);
					string key = parts[0].Trim();
					string value = parts[1].Trim();

					if (string.IsNullOrEmpty(value))
					{
						Dictionary<string, object> newDict = [];
						currentDict[key] = newDict;
						stack.Push((indent, newDict));
					}
					else currentDict[key] = value.Trim('"');

					currentKey = key;
				}
			}

			return result;
		}

		private static Dictionary<string, string?> GetCookieValues(List<object> cookies, params string[] keys)
		{
			Dictionary<string, string?> result = [];

			foreach (string key in keys)
				result[key] = null;

			foreach (object cookieObj in cookies)
			{
				if (cookieObj is not Dictionary<string, object> cookie) continue;
				if (!cookie.TryGetValue("name", out object? nameObj) || nameObj is not string name) continue;
				if (!Array.Exists(keys, k => k == name)) continue;

				result[name] = cookie.TryGetValue("value", out object? valueObj) && valueObj is string value
					? value
					: string.Empty;
			}

			return result;
		}
	}
}
