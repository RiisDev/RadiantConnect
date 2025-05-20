using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using RadiantConnect.Utilities;
using static Microsoft.Win32.Registry;

namespace RadiantConnect.Services
{
    public class GameVersionService
    {
        internal static readonly char[] Separator = ['\0'];

        public record VersionData(string Branch, string BuildVersion, int VersionNumber, string BuiltData);

        private static int GetBuildNumberFromLog() => int.Parse(LogService.GetLogText().ExtractValue(@"Build version: (\d+)", 1));

        public static VersionData GetClientVersion(string filePath)
        {
            using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read);
            using BinaryReader reader = new(fileStream, Encoding.Unicode);

            byte[] pattern = "+\0+\0A\0r\0e\0s\0-\0C\0o\0r\0e\0+\0"u8.ToArray();
            byte[] data = reader.ReadBytes((int)reader.BaseStream.Length);
            
            int pos = data.AsSpan().IndexOf(pattern);
            string?[] block = Encoding.Unicode.GetString(data, pos, 256).Split(Separator, StringSplitOptions.RemoveEmptyEntries);

            string? branch = block[0];
            string? buildVersion = block[2];
            string? buildNumber = block[3];

            if (!int.TryParse(buildNumber, out int parsedBuild) && 
                !int.TryParse((GetProductVersionString(filePath) ?? "ABC"), out parsedBuild))
            {
                parsedBuild = GetBuildNumberFromLog();
            }

            int versionNumber = int.Parse(buildVersion?.Split('.').Last() ?? "3");

            return new VersionData(branch!, buildVersion!, versionNumber, $"{branch}-shipping-{parsedBuild}-{versionNumber}");
        }

        internal static string GetOsVersion()
        {
            try
            {
                return $"{Environment.OSVersion.Version}.{GetValue($@"{LocalMachine}\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "UBR", "256")}.64bit";
            }
            catch { return "10.0.19043.1.256"; }
        }

        internal static string GetVersionHeader()
        {
            try { return $"Windows/{GetOsVersion()}"; }
            catch { return "Windows/10.0.19043.1.256.64bit"; }
        }

        internal static string GetArchitecture()
        {
            string? arch = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");

            return (arch is null || !arch.Contains("64")) ? "Unknown" : arch.ToLowerInvariant();
        }

        public static string GetClientPlatform()
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

        [DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern uint GetFileVersionInfoSize(string lptstrFilename, out uint lpdwHandle);

        [DllImport("version.dll", SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern bool GetFileVersionInfo(string lptstrFilename, uint dwHandle, uint dwLen, byte[] lpData);

        [DllImport("version.dll", CharSet = CharSet.Auto)]
        internal static extern bool VerQueryValue(byte[] pBlock, string lpSubBlock, out IntPtr lplpBuffer, out uint puLen);

        // Thanks Floxay
        // https://gist.github.com/floxay/a6bdacbd8db2298be602d330a43976da#file-get_client_version-py-L29-L32
        internal static string? GetProductVersionString(string filePath)
        {
            uint size = GetFileVersionInfoSize(filePath, out uint _);
            if (size == 0) return null;

            byte[] buffer = new byte[size];

            if (!GetFileVersionInfo(filePath, 0, size, buffer)) return null;
            if (!VerQueryValue(buffer, @"\VarFileInfo\Translation", out IntPtr transPtr, out uint _))return null;

            ushort langId = (ushort)Marshal.ReadInt16(transPtr);
            ushort codePage = (ushort)Marshal.ReadInt16(transPtr, 2);

            string subBlock = $@"\StringFileInfo\{langId:X4}{codePage:X4}\ProductVersion";

            if (VerQueryValue(buffer, subBlock, out IntPtr ptr, out uint len) && len > 0) return Marshal.PtrToStringAuto(ptr);

            return null;
        }

    }
}
