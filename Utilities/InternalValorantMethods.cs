using RadiantConnect.Services;

namespace RadiantConnect.Utilities
{
	public class InternalValorantMethods
	{
		public static bool ClientIsReady() =>
			IsValorantProcessRunning() &&
			Directory.Exists(Path.GetDirectoryName(LogService.LogPath)) &&
			File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
				"Local", "Riot Games", "Riot Client", "Config", "lockfile")) &&
			File.Exists(LogService.LogPath) &&
			!LogService.ReadTextFile(LogService.LogPath).Split('\n').Last().Contains("Log file closed");

		public static bool IsValorantProcessRunning() => Process.GetProcessesByName("VALORANT").Length > 0;

		public static bool IsRiotClientRunning() => Process.GetProcessesByName("RiotClientServices").Length > 0 && Process.GetProcessesByName("Riot Client").Length > 0;
	}
}
