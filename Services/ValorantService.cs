﻿using System.Diagnostics;
using RadiantConnect.Utilities;

namespace RadiantConnect.Services
{
    public class ValorantService
    {
        public record Version(
            string RiotClientVersion,
            string Branch,
            string BuildVersion,
            string Changelist,
            string EngineVersion,
            string VanguardVersion,
            string UserClientVersion,
            string UserPlatform
        );

        public Version ValorantClientVersion { get; init; }

        internal static Version GetVersionFromFile(string valorantPath)
        {
            GameVersionService.VersionData versionData = GameVersionService.GetClientVersion(valorantPath);
            FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(valorantPath);

            string userClientVersion = GameVersionService.GetVersionHeader();
            string userPlatform = GameVersionService.GetClientPlatform();
            string engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
            string branch = versionData.Branch;
            string buildVersion = versionData.BuildVersion;
            string changelist = versionData.VersionNumber.ToString();
            string clientVersion = versionData.BuiltData;

            return new Version(
                RiotClientVersion: clientVersion,
                Branch: branch,
                BuildVersion: buildVersion,
                Changelist: changelist,
                EngineVersion: engineVersion,
                VanguardVersion: GameVersionService.GetVanguardVersion(),
                UserClientVersion: userClientVersion,
                UserPlatform: userPlatform
            );
        }

        internal static Version GetVersionFromLog()
        {
            string userClientVersion = GameVersionService.GetVersionHeader();
            string userPlatform = GameVersionService.GetClientPlatform();
            string fileText = LogService.ReadTextFile(LogService.LogPath);

            string branch = fileText.ExtractValue("Branch: (.+)", 1);
            string changelist = fileText.ExtractValue(@"Changelist: (\d+)", 1);
            string buildVersion = fileText.ExtractValue(@"Build version: (\d+)", 1);
            string clientVersion = $"{branch}-shipping-{buildVersion}-{changelist}";

            return new Version(
                RiotClientVersion: clientVersion, 
                Branch: branch, 
                BuildVersion: buildVersion, 
                Changelist: changelist, 
                EngineVersion: string.Empty,
                VanguardVersion: GameVersionService.GetVanguardVersion(), 
                UserClientVersion: userClientVersion,
                UserPlatform: userPlatform
            );
        }

        public ValorantService()
        {
            string valorantPath = RiotPathService.GetValorantPath();
            
            ValorantClientVersion = File.Exists(valorantPath) ? GetVersionFromFile(valorantPath) : GetVersionFromLog();
        }
    }
}
