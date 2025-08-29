using static Microsoft.Win32.Registry;
#pragma warning disable CA1416
namespace RadiantConnect.Services
{
    public static class RiotPathService
    {
        public static string GetValorantPath()
        {
            string? installLocation;
            try
            {
                installLocation = GetValue($@"{CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game valorant.live",
                    "InstallLocation", "")?.ToString();
                installLocation += @"\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe";
            }
            catch
            {
                installLocation = @"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe";
            }

			if (!File.Exists(installLocation))
				throw new FileNotFoundException("Failed to find Valorant executable");

			return installLocation;
        }

        public static string GetRiotClientPath(bool service = true)
        {
            string installLocation;
            try
            {
                string? installString = GetValue($@"{CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game Riot_Client",
                    "InstallLocation", "")?.ToString();

                if (!Directory.Exists(installString))
	                throw new DirectoryNotFoundException("Failed to find Riot Client install path");

				installLocation = service ? Path.Combine(installString, "RiotClientServices.exe") : Path.Combine(installString, "Riot Client.exe");
			}
            catch
            {
                installLocation = @"C:\Riot Games\Riot Client\RiotClientServices.exe";
            }
			
			if (!File.Exists(installLocation))
				throw new FileNotFoundException($"Failed to find {(service ? "RiotClientServices" : "Riot Client")} executable");

			return installLocation;
        }
    }
}
