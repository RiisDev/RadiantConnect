using System.Diagnostics.CodeAnalysis;
using RadiantConnect.EventHandler;
using RadiantConnect.Methods;
using static System.Enum;
using Path = System.IO.Path;
#pragma warning disable IDE0079

namespace RadiantConnect.Services
{
    public class LogService
    {
        public record ClientData(ClientData.ShardType Shard, string UserId, string PdUrl, string GlzUrl, string SharedUrl)
        {
            [SuppressMessage("ReSharper", "InconsistentNaming")]
            public enum ShardType { na, latam, br, eu, ap, kr, }
        }

        internal static string GetLogPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Valorant", "Saved", "Logs", "ShooterGame.log");
        }

        internal static string GetLogText()
        {
            string logPath = GetLogPath();
            try
            {
                File.Copy(logPath, $"{logPath}.tmp", true);
                using StreamReader reader = File.OpenText($"{logPath}.tmp");
                return reader.ReadToEnd();
            }
            catch
            {
                return GetLogText();
            }
            finally
            {
                File.Delete($"{logPath}.tmp");
            }
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
