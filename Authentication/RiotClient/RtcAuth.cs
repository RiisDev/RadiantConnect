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

		internal async Task<RSOAuth?> Run(string? fileLocation = null, Authentication? auth = null)
		{
			fileLocation ??= RiotClientSettings;

			if (auth == null) throw new Exception("Authentication somehow null, report bug to github, this should never be seen.");
			if (!File.Exists(fileLocation)) throw new FileNotFoundException("Riot Client persistent file not found.", fileLocation);
			if (!fileLocation.EndsWith(".yaml")) throw new Exception($"Invalid file detected, expected .yaml got {Path.GetExtension(fileLocation)}");

			string content = await File.ReadAllTextAsync(fileLocation);

			Dictionary<string, object> yamlData = ParseSimpleYaml(content);

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

			Dictionary<string, string?> cookieValues = GetCookieValues(allCookies, "ssid", "clid", "csid", "tdid");
			
			return await auth.AuthenticateWithSsid(
				ssid: cookieValues["ssid"] ?? throw new RadiantConnectAuthException("Failed to grab ssid from client cookies."), 
				clid: cookieValues["clid"] ?? throw new RadiantConnectAuthException("Failed to grab clid from client cookies."),
				csid: cookieValues["csid"] ?? throw new RadiantConnectAuthException("Failed to grab csid from client cookies."),
				tdid: cookieValues["tdid"] ?? throw new RadiantConnectAuthException("Failed to grab tdid from client cookies.")
			);
		}

		private static Dictionary<string, object> ParseSimpleYaml(string yaml)
		{
			Dictionary<string, object> result = new();
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
			Dictionary<string, string?> result = new();

			foreach (string key in keys)
				result[key] = null;

			foreach (object cookieObj in cookies)
			{
				if (cookieObj is not Dictionary<string, object> cookie) continue;
				if (!cookie.TryGetValue("name", out object? nameObj) || nameObj is not string name) continue;
				if (!Array.Exists(keys, k => k == name)) continue;

				if (cookie.TryGetValue("value", out object? valueObj) && valueObj is string value) result[name] = value;
				else result[name] = string.Empty;
			}

			return result;
		}
	}
}
