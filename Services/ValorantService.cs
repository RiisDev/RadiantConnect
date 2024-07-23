using System.Diagnostics;
using System.Text.Json;
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
            [property: JsonPropertyName("VanguardVersion")] string VanguardVersion,
            [property: JsonPropertyName("UserClientVersion")] string UserClientVersion,
            [property: JsonPropertyName("UserPlatform")] string UserPlatform
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

        public string GetOsVersion()
        {
            try
            {
                return $"{Environment.OSVersion.Version.Major}.{Environment.OSVersion.Version.Minor}.{Environment.OSVersion.Version.Build}.{GetValue($@"{LocalMachine}\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "256")}";
            }
            catch
            {
                return "10.0.19043.1.256";
            }
        }

        public string GetVersionHeader()
        {
            try
            {
                return $"Windows/{GetOsVersion()}";
            }
            catch
            {
               return "Windows/10.0.19043.1.256.64bit";
            }
        }

        public string GetArchitecture()
        {
            string? arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            return (arch is null || !arch.Contains("64")) ? "Unknown" : arch.ToLowerInvariant();
        }

        public string GetClientPlatform()
        {
            
            Dictionary<string, string> platform = new()
            {
                { "platformType", "PC" },
                { "platformOS", "Windows" },
                { "platformOSVersion", GetOsVersion() },
                { "platformChipset", GetArchitecture() },
            };

            return JsonSerializer.Serialize(platform, new JsonSerializerOptions
            {
                WriteIndented = true
            }).ToBase64();
        }

        public ValorantService()
        {
            string valorantPath = GetValorantPath();
            string userClientVersion = GetVersionHeader();
            string userPlatform = GetClientPlatform();

            string? engineVersion;
            string? branch;
            string? changelist;
            string? buildVersion;
            string? clientVersion;

            if (File.Exists(valorantPath))
            {
                GameVersionService.VersionData versionData = GameVersionService.GetClientVersion(valorantPath);
                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(valorantPath);

                engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
                branch = versionData.Branch;
                buildVersion = versionData.BuildVersion;
                changelist = versionData.VersionNumber.ToString();
                clientVersion = versionData.BuiltData;
            }
            else
            {
                Debug.WriteLine(message: "[RADIANTCONNECT] Failed to find valorant executable, using fallback...");
                string fileText = LogService.GetLogText();
                branch = fileText.ExtractValue(pattern: "Branch: (.+)", groupId: 1);
                changelist = fileText.ExtractValue(pattern: @"Changelist: (\d+)", groupId: 1);
                buildVersion = fileText.ExtractValue(pattern: @"Build version: (\d+)", groupId: 1);
                clientVersion = $"{branch}-shipping-{buildVersion}-{changelist}";
                engineVersion = "";
            }
            
            ValorantClientVersion = new Version(
                RiotClientVersion: clientVersion, 
                Branch: branch, 
                BuildVersion: buildVersion, 
                Changelist: changelist, 
                EngineVersion: engineVersion,
                VanguardVersion: GetVanguardVersion(), 
                UserClientVersion: userClientVersion,
                UserPlatform: userPlatform
            );
        }
    }
}
