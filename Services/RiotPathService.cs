#pragma warning disable CA1416
#pragma warning disable IDE0046
namespace RadiantConnect.Services
{
	public static class RiotPathService
	{
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
