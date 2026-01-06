using RadiantConnect.Services;

namespace RadiantConnect.Utilities
{
	/// <summary>
	/// Provides internal helper methods for detecting the runtime state
	/// of Valorant and Riot Client processes.
	/// </summary>
	/// <remarks>
	/// These methods are used to determine readiness and lifecycle state
	/// of Riot and Valorant components prior to initializing XMPP connections.
	/// </remarks>
	public static class InternalValorantMethods
	{
		/// <summary>
		/// Determines whether the Valorant client is fully initialized and ready
		/// for interaction.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the Valorant process is running, required configuration
		/// and log files exist, and the client has not yet shut down;
		/// otherwise, <c>false</c>.
		/// </returns>
		/// <remarks>
		/// Readiness is inferred by checking process state, file system artifacts,
		/// and the contents of the Valorant log file.
		/// </remarks>
		public static bool ClientIsReady() =>
			IsValorantProcessRunning() &&
			Directory.Exists(Path.GetDirectoryName(LogService.LogPath)) &&
			File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData",
				"Local", "Riot Games", "Riot Client", "Config", "lockfile")) &&
			File.Exists(LogService.LogPath) &&
			!LogService.ReadTextFile(LogService.LogPath)
				.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.RemoveEmptyEntries)
				.Last()
				.Contains("Log file closed", StringComparison.Ordinal);

		[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
		internal static bool IsValorantProcessRunning()
		{
			foreach (Process process in Process.GetProcesses())
				if (process.ProcessName.Equals("VALORANT", StringComparison.OrdinalIgnoreCase))
					return true;
			return false;
		}

		internal static bool IsRiotClientRunning()
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
