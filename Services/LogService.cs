﻿using System.Diagnostics.CodeAnalysis;
using RadiantConnect.EventHandler;
using RadiantConnect.Utilities;
using static System.Enum;
using Path = System.IO.Path;

namespace RadiantConnect.Services
{
    public class LogService
    {
        internal static string ReadTextFile(string path)
        {
            string tempFileName = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}-radiant-{Path.GetExtension(path)}");
            const int maxAttempts = 5;

            for (int attempt = 0; attempt < maxAttempts; attempt++)
            {
                try
                {
                    File.Copy(path, tempFileName, true);
                    return File.ReadAllText(tempFileName);
                }
                catch { Thread.Sleep(150); }
                finally
                {
                    try { if (File.Exists(tempFileName)) File.Delete(tempFileName); }
                    catch { /**/ }
                }
            }

            return string.Empty;
        }

        public record ClientData(ClientData.ShardType Shard, string UserId, string PdUrl, string GlzUrl, string SharedUrl)
        {
            [SuppressMessage("ReSharper", "UnusedMember.Global")]
            // ReSharper disable InconsistentNaming
            public enum ShardType { na, latam, br, eu, ap, kr, }
        }

        internal static string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Valorant", "Saved", "Logs", "ShooterGame.log");
        
        public static ClientData GetClientData()
        {
            Restart:
            string currentLogText = ReadTextFile(LogPath);
            
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
                while (!Initiator.IsDisposed)
                {
                    await Task.Delay(100);
                    long currentFileSize = new FileInfo(LogPath).Length;
                    if (currentFileSize == lastFileSize) continue;
                    lastFileSize = currentFileSize;
                    events?.ParseLogText(ReadTextFile(LogPath));
                }
            });
        }
    }
}
