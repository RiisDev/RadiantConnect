using System.Diagnostics;
using System.Text.Json.Serialization;
using RadiantConnect.Methods;
using static Microsoft.Win32.Registry;
#pragma warning disable CA1416

namespace RadiantConnect.Services
{
    public class ValorantService
    {
        public record Version(
            [property: JsonPropertyName("RiotClientVersion")] string RiotClientVersion,
            [property: JsonPropertyName("Branch")] string Branch,
            [property: JsonPropertyName("BuildVersion")] string BuildVersion,
            [property: JsonPropertyName("Changelist")] string Changelist,
            [property: JsonPropertyName("EngineVersion")] string EngineVersion,
            [property: JsonPropertyName("VanguardVersion")] string VanguardVersion
        );

        public Version ValorantClientVersion { get; init; }

        internal static string GetVanguardVersion()
        {
            string clientConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Riot Games", "VALORANT", "Config", "ClientConfiguration.json");
            string? fileText;
            try
            {
                File.Copy(clientConfigPath, $"{clientConfigPath}.tmp", true);
                fileText = File.ReadAllText($"{clientConfigPath}.tmp");
            }
            finally
            {
                File.Delete($"{clientConfigPath}.tmp");
            }

            return fileText.ExtractValue("anticheat\\.vanguard\\.version\": \"(.*)\"", 1);
        }

        // Thank you: https://github.com/techchrism/valorant-xmpp-logger/blob/trunk/src/riotClientUtils.ts
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
            string? installLocation;
            try
            {
                //"C:\Riot Games\Riot Client\RiotClientServices.exe" --uninstall-product=valorant --uninstall-patchline=live
                string? uninstallString = GetValue($@"{CurrentUser}\Software\Microsoft\Windows\CurrentVersion\Uninstall\Riot Game valorant.live",
                    "UninstallString", "")?.ToString();
                installLocation = uninstallString![1..(uninstallString.IndexOf(".exe", StringComparison.Ordinal) + 4)];
            }
            catch
            {
                installLocation = @"C:\Riot Games\Riot Client\RiotClientServices.exe";
            }
            return installLocation;
        }

        public ValorantService()
        {
            string valorantPath = GetValorantPath();
            string? engineVersion = null;
            string fileText = LogService.GetLogText();
            string ciServerVersion = fileText.ExtractValue("CI server version: (.+)", 1);
            string branch = fileText.ExtractValue("Branch: (.+)", 1);
            string changelist = fileText.ExtractValue(@"Changelist: (\d+)", 1);
            string buildVersion = fileText.ExtractValue(@"Build version: (\d+)", 1);

            if (File.Exists(valorantPath))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(valorantPath);
                engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
            }
            
            ValorantClientVersion = new Version(ciServerVersion, branch, buildVersion, changelist, engineVersion ?? "", GetVanguardVersion());
        }
    }
}
