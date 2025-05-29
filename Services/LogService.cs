using System.Diagnostics.CodeAnalysis;
using RadiantConnect.EventHandler;
using RadiantConnect.Utilities;
using static System.Enum;
using Path = System.IO.Path;
//ReSharper disable InconsistentNaming

namespace RadiantConnect.Services
{
    public class LogService
    {
        public record ClientData(ClientData.ShardType Shard, string UserId, string PdUrl, string GlzUrl, string SharedUrl)
        {
            public enum ShardType { na, latam, br, eu, ap, kr, }
        }

        internal static string GetLogPath() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Valorant", "Saved", "Logs", "ShooterGame.log");

        internal static string GetLogText()
        {
            string logPath = GetLogPath();
            string tempPath = $"{logPath}.tmp";
            int attempt = 0;

            while (attempt < 15)
            {
                try
                {
                    File.Copy(logPath, tempPath, true);
                    using StreamReader reader = File.OpenText(tempPath);
                    return reader.ReadToEnd();
                }
                catch (IOException ex)
                {
                    attempt++;
                    if (attempt >= 16)
                        throw new Exception($"Failed to read log file after 15 attempts.", ex);
                }
                finally
                {
                    try { File.Delete(tempPath); }
                    catch { /**/ }
                }
            }

            throw new Exception("Unexpected failure while reading log file.");
        }


        public static ClientData GetClientData()
        {
            Restart:
            string currentLogText = GetLogText();
            
            string userId = currentLogText.ExtractValue("Logged in user changed: (.+)", 1);
            string pdUrl = currentLogText.ExtractValue(@"https://pd\.[^\s]+\.net/", 0);
            string glzUrl = currentLogText.ExtractValue(@"https://glz[^\s]+\.net/", 0);
            string regionData = currentLogText.ExtractValue(@"https://pd\.([^\.]+)\.a\.pvp\.net/", 1);
            if (!TryParse(regionData, out ClientData.ShardType region))
                region = ClientData.ShardType.na;
            string sharedUrl = $"https://shared.{regionData}.a.pvp.net/";

            if (string.IsNullOrEmpty(userId)) goto Restart;
            if (string.IsNullOrEmpty(pdUrl)) goto Restart;
            if (string.IsNullOrEmpty(glzUrl)) goto Restart;
            if (string.IsNullOrEmpty(regionData)) goto Restart;
            if (string.IsNullOrEmpty(sharedUrl)) goto Restart;

            return new ClientData(region, userId, pdUrl, glzUrl, sharedUrl);
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public static async Task InitiateEvents(Initiator initiator)
        {
            GameEvents events = new(initiator);
            initiator.GameEvents = events;
            long lastFileSize = 0;
            await Task.Run(async () =>
            {
                for (; ; )
                {
                    await Task.Delay(100);
                    long currentFileSize = new FileInfo(GetLogPath()).Length;
                    if (currentFileSize == lastFileSize) continue;
                    lastFileSize = currentFileSize;
                    events?.ParseLogText(GetLogText());
                }
            });
        }
    }
}
