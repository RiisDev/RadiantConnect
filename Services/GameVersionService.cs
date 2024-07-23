using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace RadiantConnect.Services
{
    public class GameVersionService
    {
        internal static readonly char[] Separator = ['\0'];

        public record VersionData(string Branch, string BuildVersion, int VersionNumber, string BuiltData);

        public static VersionData GetClientVersion(string filePath)
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
    }
}
