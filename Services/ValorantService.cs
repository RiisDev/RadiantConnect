using System.Diagnostics;
using System.Text.Json.Serialization;
using RadiantConnect.Methods;

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

        public ValorantService()
        {
            string? engineVersion = null;
            string fileText = LogService.GetLogText();
            string ciServerVersion = fileText.ExtractValue("CI server version: (.+)", 1);
            string branch = fileText.ExtractValue("Branch: (.+)", 1);
            string changelist = fileText.ExtractValue(@"Changelist: (\d+)", 1);
            string buildVersion = fileText.ExtractValue(@"Build version: (\d+)", 1);

            if (File.Exists(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe"))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe");
                engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
            }
            
            ValorantClientVersion = new Version(ciServerVersion, branch, buildVersion, changelist, engineVersion ?? "", GetVanguardVersion());
        }
    }
}
