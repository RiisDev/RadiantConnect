using RadiantConnect.Network;
#if WINDOWS
using static Microsoft.Win32.Registry;
#endif
namespace RadiantConnect.Services
{
	public partial class GameVersionService
	{
		internal static readonly char[] Separator = ['\0'];

		public record VersionData(string Branch, string BuildVersion, int VersionNumber, string BuiltData);

		private static int GetBuildNumberFromLog() => int.Parse(LogService.ReadTextFile(LogService.LogPath).ExtractValue(@"Build version: (\d+)", 1));
	   
		[GeneratedRegex(@"^release-\d\d.\d\d-shipping-\d{1,2}-\d{6,8}$")]
		internal static partial Regex VersionPattern();

		internal static ValorantNet.ValorantVersionApi GetVersionFromApi()
		{
			ValorantNet.ValorantVersionApiRoot versionApiRoot = InternalHttp.GetAsync<ValorantNet.ValorantVersionApiRoot>("https://api.radiantconnect.ca", "/api/version/latest").Result!;
			
			return versionApiRoot.Data ?? throw new RadiantConnectException("Failed to get fallback version from API.");
		}

		public static VersionData GetClientVersion(string filePath)
		{
			try
			{
				VersionData versionData = ParseVersionFromFile(filePath);
				ValidateVersionData(versionData);
				return versionData;
			}
			catch
			{
				ValorantNet.ValorantVersionApi fallback = GetVersionFromApi();
				VersionData versionData = BuildVersionDataFromApi(fallback);
				ValidateVersionData(versionData);
				return versionData;
			}
		}

		private static VersionData ParseVersionFromFile(string filePath)
		{
			using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
			using BinaryReader reader = new(fileStream, Encoding.Unicode);

			byte[] pattern = "+\0+\0A\0r\0e\0s\0-\0C\0o\0r\0e\0+\0"u8.ToArray();
			byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

			int pos = data.AsSpan().IndexOf(pattern);
			if (pos == -1)
				throw new RadiantConnectException("Pattern not found in file.");

			string[] block = Encoding.Unicode.GetString(data, pos, 256).Split(Separator, StringSplitOptions.RemoveEmptyEntries);
			if (block.Length < 4)
				throw new RadiantConnectException("Unexpected file block structure.");

			string branch = ExtractBranch(block.FirstOrDefault(x=> x.Contains("release")) ?? "");
			if (string.IsNullOrEmpty(branch))
				throw new RadiantConnectException("Branch not found in file.");

			string? baseMajorVersion = block.FirstOrDefault(x=> x.Length == 2);
			if (string.IsNullOrEmpty(baseMajorVersion))
				throw new RadiantConnectException($"Invalid base major version: {baseMajorVersion}");

			string? buildNumber = block.FirstOrDefault(x => x.StartsWith(branch.Replace("release-","")))?.Split('.').LastOrDefault()?.Trim();
			
			if (!int.TryParse(buildNumber, out int parsedBuildVersion))
				parsedBuildVersion = int.TryParse(GetProductVersionString(filePath), out int fallbackBuild)
					? fallbackBuild
					: GetBuildNumberFromLog();

			if (!int.TryParse(baseMajorVersion, out int baseMajorVersionParsed))
				throw new RadiantConnectException($"Invalid major version: {baseMajorVersion}");

			string builtData = $"{branch}-shipping-{baseMajorVersionParsed}-{parsedBuildVersion}";
			return new VersionData(branch, parsedBuildVersion.ToString(), baseMajorVersionParsed, builtData);
		}

		private static VersionData BuildVersionDataFromApi(ValorantNet.ValorantVersionApi versionApi)
		{
			if (versionApi.Branch == null || versionApi.BuildVersion == null)
				throw new RadiantConnectException("Fallback API provided null values.");

			string buildNumber = versionApi.Version.Split('.').LastOrDefault()
				?? throw new RadiantConnectException("Fallback API version format invalid.");

			if (!int.TryParse(buildNumber, out _))
				throw new RadiantConnectException($"Invalid fallback build number: {buildNumber}");

			string builtData = $"{versionApi.Branch}-shipping-{versionApi.BuildVersion}-{buildNumber}";

			return new VersionData(versionApi.Branch, buildNumber, int.Parse(versionApi.BuildVersion), builtData);
		}

		private static void ValidateVersionData(VersionData versionData)
		{
			if (!VersionPattern().IsMatch(versionData.BuiltData))
				throw new RadiantConnectException($"Invalid version format: {versionData.BuiltData}");
		}

		private static string ExtractBranch(string rawBranch) => rawBranch.Contains('+') ? rawBranch[(rawBranch.LastIndexOf('+') + 1)..] : rawBranch;

		internal static string GetOsVersion()
		{
			try
			{
#if WINDOWS
				return $"{Environment.OSVersion.Version}.{GetValue($@"{LocalMachine}\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "256")}.64bit";
#else
				return "10.0.19043.1.256.64bit";
#endif
			}
			catch { return "10.0.19043.1.256"; }
		}

		internal static string GetVersionHeader()
		{
			try { return $"Windows/{GetOsVersion()}"; }
			catch { return "Windows/10.0.19043.1.256.64bit"; }
		}

		internal static string GetArchitecture()
		{
			string? arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

			return (arch is null || !arch.Contains("64")) ? "Unknown" : arch.ToLowerInvariant();
		}

		private static readonly JsonSerializerOptions Base64Options = new() { WriteIndented = false };

		public static string GetClientPlatform()
		{
			Dictionary<string, string> platform = new()
			{
				{ "platformType", "PC" },
				{ "platformOS", "Windows" },
				{ "platformOSVersion", GetOsVersion() },
				{ "platformChipset", GetArchitecture() },
			};

			return JsonSerializer.Serialize(platform, Base64Options).ToBase64();
		}

		internal static string GetVanguardVersion()
		{
			string clientConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "VALORANT", "Config", "ClientConfiguration.json");
			string fileText = LogService.ReadTextFile(clientConfigPath);

			return string.IsNullOrEmpty(fileText) ? "-1" : fileText.ExtractValue("anticheat\\.vanguard\\.version\": \"(.*)\"", 1);
		}


		[DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern uint GetFileVersionInfoSize(string lptstrFilename, out uint lpdwHandle);

		[DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto)]
		internal static extern bool GetFileVersionInfo(string lptstrFilename, uint dwHandle, uint dwLen, byte[] lpData);

		[DllImport("version.dll", CharSet = CharSet.Auto)]
		internal static extern bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out uint puLen);

		// Thanks Floxay
		// https://gist.github.com/floxay/a6bdacbd8db2298be602d330a43976da#file-get_client_version-py-L29-L32
		internal static string? GetProductVersionString(string filePath)
		{
			uint size = GetFileVersionInfoSize(filePath, out uint _);
			if (size == 0) return null;

			byte[] buffer = new byte[size];

			if (!GetFileVersionInfo(filePath, 0, size, buffer)) return null;
			if (!VerQueryValue(buffer, @"\VarFileInfo\Translation", out IntPtr transPtr, out uint _))return null;

			ushort langId = (ushort)Marshal.ReadInt16(transPtr);
			ushort codePage = (ushort)Marshal.ReadInt16(transPtr, 2);

			string subBlock = $@"\StringFileInfo\{langId:X4}{codePage:X4}\ProductVersion";

			return VerQueryValue(buffer, subBlock, out IntPtr ptr, out uint len) && len > 0
				? Marshal.PtrToStringAuto(ptr)
				: null;
		}

	}
}
