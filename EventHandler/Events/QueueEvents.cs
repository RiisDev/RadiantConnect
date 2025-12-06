using RadiantConnect.Network.PartyEndpoints.DataTypes;

namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events related to matchmaking queue transitions, custom game lobbies,
	/// and navigation between queue and menu states.
	/// </summary>
	/// <param name="initiator">The event initiator used for binding queue-related event triggers.</param>
	public class QueueEvents(Initiator initiator)
	{
		/// <summary>
		/// Represents a callback for queue-related events that pass a value of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of value associated with the event.</typeparam>
		/// <param name="value">The value passed by the event.</param>
		public delegate void QueueEvent<in T>(T value);

		/// <summary>
		/// Occurs when a custom game lobby is created.
		/// </summary>
		public event QueueEvent<CustomGameData?>? OnCustomGameLobbyCreated;

		/// <summary>
		/// Occurs when the current matchmaking queue changes.
		/// </summary>
		public event QueueEvent<string?>? OnQueueChanged;

		/// <summary>
		/// Occurs when a player enters a matchmaking queue.
		/// </summary>
		public event QueueEvent<string?>? OnEnteredQueue;

		/// <summary>
		/// Occurs when a player leaves a matchmaking queue.
		/// </summary>
		public event QueueEvent<string?>? OnLeftQueue;

		/// <summary>
		/// Occurs when the client transitions from gameplay or queue back to the main menu.
		/// </summary>
		public event QueueEvent<object?>? OnTravelToMenu;

		/// <summary>
		/// Occurs when a match has been found.
		/// </summary>
		public event QueueEvent<object?>? OnMatchFound;

		internal enum PartyDataReturn
		{
			CustomGame,
			ChangeQueue
		}

		private static string GetEndpoint(string prefix, string log) => log.TryExtractSubstring("https", ']', startIndex => startIndex != -1, prefix);

		private async Task<T?> GetPartyData<T>(PartyDataReturn dataReturn, string endPoint) where T : class?
		{
			string? data = await initiator.ExternalSystem.Net.GetAsync<string>(initiator.ExternalSystem.ClientData.GlzUrl, endPoint).ConfigureAwait(false);

			return data is null ? null : dataReturn switch
			{
				PartyDataReturn.CustomGame => (T?)Convert.ChangeType(JsonSerializer.Deserialize<Party>(data)?.CustomGameData, typeof(T), StringExtensions.CultureInfo),
				PartyDataReturn.ChangeQueue => (T?)Convert.ChangeType(JsonSerializer.Deserialize<Party>(data)?.MatchmakingData.QueueId, typeof(T), StringExtensions.CultureInfo),
				_ => throw new ArgumentOutOfRangeException(nameof(dataReturn), dataReturn, null)
			};
		}

		internal async void HandleQueueEvent(string invoker, string logData)
		{
			try
			{
				string parsedEndPoint = logData.Replace("/queue", "", StringComparison.Ordinal)
					.Replace("/matchmaking/join", "", StringComparison.Ordinal)
					.Replace("/matchmaking/leave", "", StringComparison.Ordinal)
					.Replace("/makecustomgame", "", StringComparison.Ordinal);
				if (!logData.Contains("https", StringComparison.Ordinal) && !logData.Contains("LogTravelManager", StringComparison.Ordinal)) return;
			
				switch (invoker)
				{
					case "Party_ChangeQueue":
						OnQueueChanged?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(initiator.ExternalSystem.ClientData.GlzUrl, parsedEndPoint)).ConfigureAwait(false));
						break;
					case "Party_EnterMatchmakingQueue":
						OnEnteredQueue?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(initiator.ExternalSystem.ClientData.GlzUrl, parsedEndPoint)).ConfigureAwait(false));
						break;
					case "Party_LeaveMatchmakingQueue":
						OnLeftQueue?.Invoke(await GetPartyData<string>(PartyDataReturn.ChangeQueue, GetEndpoint(initiator.ExternalSystem.ClientData.GlzUrl, parsedEndPoint)).ConfigureAwait(false));
						break;
					case "Party_MakePartyIntoCustomGame":
						OnCustomGameLobbyCreated?.Invoke(await GetPartyData<CustomGameData>(PartyDataReturn.CustomGame, GetEndpoint(initiator.ExternalSystem.ClientData.GlzUrl, parsedEndPoint)).ConfigureAwait(false));
						break;
					case "Travel_To_Menu":
						OnTravelToMenu?.Invoke(null);
						break;
					case "Match_Found":
						OnMatchFound?.Invoke(null);
						break;

				}
			}
			catch {/**/}
		}
	}
}
