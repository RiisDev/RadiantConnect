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
			!LogService.ReadTextFile(LogService.LogPath).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries).Last().Contains("Log file closed");

		// Don't convert to LINQ, it's slower
		[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
		public static bool IsValorantProcessRunning()
		{
			foreach (Process process in Process.GetProcesses())
				if (process.ProcessName.Equals("VALORANT", StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}

		public static bool IsRiotClientRunning()
		{
			bool riotClientServicesFound = false;
			bool riotClientFound = false;

			foreach (Process process in Process.GetProcesses())
			{
				if (process.ProcessName.Equals("RiotClientServices", StringComparison.OrdinalIgnoreCase))
					riotClientServicesFound = true;

				if (process.ProcessName.Equals("Riot Client", StringComparison.OrdinalIgnoreCase))
					riotClientFound = true;

				if (riotClientServicesFound && riotClientFound)
					return true;
			}

			return false;
		}
	}
}
