using System.Diagnostics.CodeAnalysis;
using RadiantConnect.EventHandler;
using RadiantConnect.Methods;
using static System.Enum;
using Path = System.IO.Path;

namespace RadiantConnect.LogManager
{
    public class LogParser
    {
        internal static string GetLogPath()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(userProfile, "AppData", "Local", "Valorant", "Saved", "Logs", "ShooterGame.log");
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
            string currentLogText = GetLogText();
            string userId = currentLogText.ExtractValue("Logged in user changed: (.+)", 1);
            string pdUrl = currentLogText.ExtractValue(@"https://pd\.[^\s]+\.net/", 0);
            string glzUrl = currentLogText.ExtractValue(@"https://glz[^\s]+\.net/", 0);
            string regionData = currentLogText.ExtractValue(@"https://pd\.([^\.]+)\.a\.pvp\.net/", 1);
            if (!TryParse(regionData, out ClientData.ShardType region))
                region = ClientData.ShardType.na;
            string sharedUrl = $"https://shared.{regionData}.a.pvp.net";

            return new ClientData(region, userId, pdUrl, glzUrl, sharedUrl);
        }

#pragma warning disable IDE0079 // Remove unnecessary suppression
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
#pragma warning restore IDE0079 // Remove unnecessary suppression
        public async Task<GameEvents> InitiateEvents()
        {
            GameEvents events = new();
            long lastFileSize = 0;
            await Task.Run(async () =>
            {
                for (;;)
                {
                    await Task.Delay(100);
                    long currentFileSize = new FileInfo(GetLogPath()).Length;
                    if (currentFileSize == lastFileSize) continue;
                    lastFileSize = currentFileSize;
                    events?.ParseLogText(GetLogText());
                }
            });
            return events;
        }
    }
}
