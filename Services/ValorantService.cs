using System.Diagnostics;
using System.Text.Json.Serialization;
using RadiantConnect.LogManager;
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
            [property: JsonPropertyName("EngineVersion")] string EngineVersion
        );

        public Version ValorantClientVersion { get; init; }

        public ValorantService()
        {
            string fileText = LogParser.GetLogText();
            string ciServerVersion = fileText.ExtractValue("CI server version: (.+)", 1);
            string branch = fileText.ExtractValue("Branch: (.+)", 1);
            string changelist = fileText.ExtractValue(@"Changelist: (\d+)", 1);
            string buildVersion = fileText.ExtractValue(@"Build version: (\d+)", 1);

            string engineVersion = "";

            if (File.Exists(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe"))
            {
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(@"C:\Riot Games\VALORANT\live\ShooterGame\Binaries\Win64\VALORANT-Win64-Shipping.exe");
                engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
            }
            
            ValorantClientVersion = new Version(ciServerVersion, branch, buildVersion, changelist, engineVersion);
        }
    }
}
