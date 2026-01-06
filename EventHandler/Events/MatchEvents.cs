namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events related to match lifecycle stages, such as map loading, match start, and match end.
	/// </summary>
	public class MatchEvents
	{
		/// <summary>
		/// Represents a callback for match-related events that provide a value of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of value supplied by the event.</typeparam>
		/// <param name="value">The value passed by the event.</param>
		public delegate void MatchEvent<in T>(T value);

		/// <summary>
		/// Occurs when the match map has finished loading.
		/// </summary>
		public event MatchEvent<string?>? OnMapLoaded;

		/// <summary>
		/// Occurs when the match has ended and a final match result is available.
		/// </summary>
		public event MatchEvent<string>? OnMatchEnded;

		/// <summary>
		/// Occurs when the match officially begins.
		/// </summary>
		public event MatchEvent<string?>? OnMatchStarted;

		private string? _mapName;
		private static string GetWinningTeam(string log) => log.TryExtractSubstring("Team: ", '(', startIndex => startIndex >= 0).Trim().Replace("Team:'", "", StringComparison.InvariantCultureIgnoreCase)[..^1];
		private static string GetMapName(string log) => log.TryExtractSubstring("Map Name:", '|', startIndex => startIndex >= 0, "Map Name: ").Trim();
		
		internal void HandleMatchEvent(string invoker, string logData)
		{
			switch (invoker)
			{
				case "Map_Loaded":
					_mapName = GetMapName(logData);
					OnMapLoaded?.Invoke(_mapName);
					break;
				case "Match_Ended":
					OnMatchEnded?.Invoke(GetWinningTeam(logData));
					_mapName = null;
					break;
				case "Match_Started":
					OnMatchStarted?.Invoke(null);
					break;
			}
		}
	}
}
