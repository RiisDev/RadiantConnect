using RadiantConnect.EventHandler;
using static System.Enum;
using Path = System.IO.Path;

namespace RadiantConnect.Services
{
	/// <summary>
	/// Service for reading Valorant log files and raising game-related events.
	/// </summary>
	public class LogService : IDisposable
	{
		private readonly CancellationTokenSource _shutdownLog = new();

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

		/// <summary>
		/// Represents client-related data extracted from the Valorant log file.
		/// </summary>
		/// <param name="Shard">The server shard/region of the client.</param>
		/// <param name="UserId">The user ID of the logged-in player.</param>
		/// <param name="PdUrl">The PD endpoint URL.</param>
		/// <param name="GlzUrl">The GLZ endpoint URL.</param>
		/// <param name="SharedUrl">The shared endpoint URL.</param>
		public record ClientData(ClientData.ShardType Shard, string UserId, string PdUrl, string GlzUrl, string SharedUrl)
		{
			/// <summary>
			/// Represents server regions/shards.
			/// </summary>
			public enum ShardType
			{
				/// <summary>North America shard.</summary>
				Na,

				/// <summary>Latin America shard.</summary>
				Latam,

				/// <summary>Brazil shard.</summary>
				Br,

				/// <summary>Europe shard.</summary>
				Eu,

				/// <summary>Asia Pacific shard.</summary>
				Ap,

				/// <summary>Korea shard.</summary>
				Kr
			}
		}

		internal static string LogPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Valorant", "Saved", "Logs", "ShooterGame.log");
		
		/// <summary>
		/// Extracts the client data (user ID, URLs, and region) from the Valorant log file.
		/// </summary>
		/// <returns>A <see cref="ClientData"/> object containing extracted log information.</returns>
		public static ClientData GetClientData()
		{
			Restart:
			string currentLogText = ReadTextFile(LogPath);
			
			string userId = currentLogText.ExtractValue("Logged in user changed: (.+)", 1);
			string pdUrl = currentLogText.ExtractValue(@"https://pd\.[^\s]+\.net/", 0);
			string glzUrl = currentLogText.ExtractValue(@"https://glz[^\s]+\.net/", 0);
			string regionData = currentLogText.ExtractValue(@"https://pd\.([^\.]+)\.a\.pvp\.net/", 1);
			if (!TryParse(regionData, out ClientData.ShardType region))
				region = ClientData.ShardType.Na;
			string sharedUrl = $"https://shared.{regionData.ToLowerInvariant()}.a.pvp.net/";

			if (string.IsNullOrEmpty(userId)) goto Restart;
			if (string.IsNullOrEmpty(pdUrl)) goto Restart;
			if (string.IsNullOrEmpty(glzUrl)) goto Restart;
			if (string.IsNullOrEmpty(regionData)) goto Restart;
			if (string.IsNullOrEmpty(sharedUrl)) goto Restart;

			return new ClientData(region, userId, pdUrl, glzUrl, sharedUrl);
		}

		[SuppressMessage("ReSharper", "FunctionNeverReturns")]
		internal async Task InitiateEvents(Initiator initiator)
		{
			GameEvents events = new(initiator);
			initiator.GameEvents = events;
			long lastFileSize = 0;
			await Task.Run(async () =>
			{
				try
				{
					while (true)
					{
						await Task.Delay(100, _shutdownLog.Token).ConfigureAwait(false);
						long currentFileSize = new FileInfo(LogPath).Length;
						if (currentFileSize == lastFileSize) continue;
						lastFileSize = currentFileSize;
						events?.ParseLogText(ReadTextFile(LogPath));
					}
				}
				catch (OperationCanceledException) { /* expected on shutdown */ }
			}, _shutdownLog.Token).ConfigureAwait(false);
		}

		/// <summary>
		/// Disposes of the log service, stopping any active log monitoring tasks.
		/// </summary>
		public void Dispose()
		{
			_shutdownLog.Cancel();
			_shutdownLog.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
