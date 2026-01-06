using RadiantConnect.Network;

namespace RadiantConnect.Services
{
	/// <summary>
	/// Provides methods for retrieving and representing Valorant client version information.
	/// </summary>
	public class ValorantService
	{
		/// <summary>
		/// Represents the detailed version information of the Valorant client.
		/// </summary>
		/// <param name="RiotClientVersion">The version of the Riot Client executable.</param>
		/// <param name="Branch">The branch of the game build (e.g., live, beta).</param>
		/// <param name="BuildVersion">The numeric build version of the client.</param>
		/// <param name="Changelist">The changelist identifier for this build.</param>
		/// <param name="EngineVersion">The engine version used by this build.</param>
		/// <param name="VanguardVersion">The version of Riot Vanguard anti-cheat.</param>
		/// <param name="UserClientVersion">The user-facing client version string.</param>
		/// <param name="UserPlatform">The platform details of the client (OS, architecture).</param>
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

		/// <summary>
		/// Gets the currently detected Valorant client version information.
		/// </summary>
		public Version ValorantClientVersion { get; init; }

		internal static Version GetVersionFromFile(string valorantPath)
		{
			GameVersionService.VersionData versionData = GameVersionService.GetClientVersion(valorantPath);
			FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(valorantPath);

			string userClientVersion = GameVersionService.GetVersionHeader();
			string userPlatform = GameVersionService.GetClientPlatform();
			string engineVersion = $"{fileInfo.FileMajorPart}.{fileInfo.FileMinorPart}.{fileInfo.FileBuildPart}.{fileInfo.FilePrivatePart}";
			string branch = versionData.Branch;
			string buildVersion = versionData.VersionNumber.ToString(StringExtensions.CultureInfo);
			string changelist = versionData.BuildVersion;
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

		/// <summary>
		/// Initializes a new instance of the <see cref="ValorantService"/> class,
		/// attempting to detect the Valorant client version from the local install, logs, or fallback API.
		/// </summary>
		/// <exception cref="RadiantConnectException">Thrown if no version information can be retrieved from any source.</exception>
		public ValorantService()
		{
			string valorantPath;
			try { valorantPath = RiotPathService.GetValorantPath(); }
			catch { valorantPath = ""; }

			if (File.Exists(valorantPath)) ValorantClientVersion = GetVersionFromFile(valorantPath);
			else if (File.Exists(LogService.LogPath)) ValorantClientVersion = GetVersionFromLog();
			else
			{
				ValorantNet.ValorantVersionApiRoot versionApiRoot = InternalHttp.GetAsync<ValorantNet.ValorantVersionApiRoot>("https://api.radiantconnect.ca", "/api/version/latest").Result!;
				ValorantNet.ValorantVersionApi data = versionApiRoot.Data;

				if (data == null || string.IsNullOrEmpty(data.Version))
					throw new RadiantConnectException("Failed to get fallback version from API.");

				ValorantClientVersion = new Version(
					RiotClientVersion: data.RiotClientVersion,
					Branch: data.Branch,
					BuildVersion: data.BuildVersion,
					Changelist: data.ManifestId,
					EngineVersion: data.EngineVersion,
					VanguardVersion: data.VanguardVersion,
					UserClientVersion: GameVersionService.GetVersionHeader(),
					UserPlatform: GameVersionService.GetClientPlatform()
				);
			}
		}
	}
}
