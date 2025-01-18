using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Text.Json;
using RadiantConnect.Utilities;
using static Microsoft.Win32.Registry;

namespace RadiantConnect.Services
{
    internal class GameVersionService
    {
        internal static readonly char[] Separator = ['\0'];

        internal record VersionData(string Branch, string BuildVersion, int VersionNumber, string BuiltData);

        internal static VersionData GetClientVersion(string filePath)
        {
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fileStream, Encoding.Unicode);

            byte[] pattern = [43, 0, 43, 0, 65, 0, 114, 0, 101, 0, 115, 0, 45, 0, 67, 0, 111, 0, 114, 0, 101, 0, 43, 0]; // ++Ares-Core+
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);

            int pos = FindPattern(data, pattern) + pattern.Length;

            string?[] block = Encoding.Unicode.GetString(data, pos, 128).Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            string? branch = block[0];
            string? buildVersion = block[2];
            string? version = block[3];

            if (buildVersion != null && buildVersion.Contains('.'))
            {
                version = buildVersion;
                buildVersion = GetProductVersion(data);
            }

            int versionNumber = int.Parse(version?.Split('.').Last() ?? "-1");

            return new VersionData(branch!, buildVersion!, versionNumber, $"{branch}-shipping-{buildVersion}-{versionNumber}");
        }

        internal static int FindPattern(byte[] data, byte[] pattern)
        {
            for (int i = 0; i <= data.Length - pattern.Length; i++)
            {
                bool found = true;
                for (int j = 0; j < pattern.Length; j++) // Do not convert to linq, adds +5 seconds to runtime
                {
                    if (data[i + j] == pattern[j]) continue;

                    found = false;
                    break;
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        internal static string GetProductVersion(byte[] data)
        {
            Assembly assembly = Assembly.Load(data);

            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);

            string? productVersion = fileVersionInfo.ProductVersion;

            return !string.IsNullOrEmpty(productVersion) ? productVersion : "1";
        }

        internal static string GetOsVersion()
        {
            try
            {
                return $"{Environment.OSVersion.Version}.{GetValue($@"{LocalMachine}\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "256")}.64bit";
            }
            catch
            {
                return "10.0.19043.1.256";
            }
        }

        internal static string GetVersionHeader()
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

        internal static string GetArchitecture()
        {
            string? arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            return (arch is null || !arch.Contains("64")) ? "Unknown" : arch.ToLowerInvariant();
        }

        internal static string GetClientPlatform()
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
        
    }
}
