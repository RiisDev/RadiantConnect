#pragma warning disable CA1416
#pragma warning disable IDE0046
namespace RadiantConnect.Services
{
	/// <summary>
	/// Provides methods to locate the installation paths for Valorant and the Riot Client on the local system.
	/// </summary>
	public static class RiotPathService
	{
		/// <summary>
		/// Gets the full path to the Valorant game executable.
		/// </summary>
		/// <returns>The full file path to <c>VALORANT-Win64-Shipping.exe</c>.</returns>
		/// <exception cref="FileNotFoundException">Thrown if the Valorant executable cannot be found.</exception>
		public static string GetValorantPath()
		{
			string installLocation = string.Empty;
#if WINDOWS
			try
			{
				installLocation = Microsoft.Win32.Registry.GetValue($@"{Microsoft.Win32.Registry.CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game valorant.live",
					"InstallLocation", "")?.ToString() ?? "";

				if (installLocation.IsNullOrEmpty())
					throw new InvalidOperationException("Failed to access registry key for valorant.live");

				installLocation += @"\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe";
			}
			catch
			{
				installLocation = @"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe";
			}
#endif
			if (!File.Exists(installLocation))
				throw new FileNotFoundException("Failed to find Valorant executable");

			return installLocation;
		}

		/// <summary>
		/// Gets the full path to the Riot Client executable.
		/// </summary>
		/// <param name="service">
		/// If <c>true</c>, returns the path to <c>RiotClientServices.exe</c>;
		/// if <c>false</c>, returns the path to the main <c>Riot Client.exe</c>.
		/// </param>
		/// <returns>The full file path to the selected Riot Client executable.</returns>
		/// <exception cref="FileNotFoundException">Thrown if the Riot Client executable cannot be found.</exception>
		public static string GetRiotClientPath(bool service = true)
		{
			string installLocation = string.Empty;

#if WINDOWS
			try
			{
				string installString = Microsoft.Win32.Registry.GetValue($@"{Microsoft.Win32.Registry.CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game Riot_Client.",
					"InstallLocation", "")?.ToString() ?? "";

				if (installString.IsNullOrEmpty())
					throw new InvalidOperationException("Failed to access registry key for Riot_Client");

				if (!Directory.Exists(installString))
					throw new DirectoryNotFoundException("Failed to find Riot Client install path");

				installLocation = service ? Path.Combine(installString, "RiotClientServices.exe") : Path.Combine(installString, "RiotClientElectron", "Riot Client.exe");
			}
			catch
			{
				installLocation = @"C:\Riot Games\Riot Client\RiotClientServices.exe";
			}
#endif

			if (!File.Exists(installLocation))
				throw new FileNotFoundException($"Failed to find {(service ? "RiotClientServices" : "Riot Client")} executable");

			return installLocation;
		}
	}
}
