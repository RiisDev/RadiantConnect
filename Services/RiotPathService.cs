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
            return installLocation;
        }

        public static string GetRiotClientPath()
        {
            string installLocation = string.Empty;
            try
            {
                string? uninstallString = GetValue($@"{CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game valorant.live",
                    "UninstallString", "")?.ToString();

                if (uninstallString is not null)
                    installLocation = uninstallString[1..(uninstallString.IndexOf(".exe", StringComparison.Ordinal) + 4)];
            }
            catch
            {
                installLocation = @"C:\Riot Games\Riot Client\RiotClientServices.exe";
            }
            return installLocation;
        }
    }
}
