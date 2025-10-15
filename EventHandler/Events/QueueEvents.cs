﻿using RadiantConnect.Network.PartyEndpoints.DataTypes;

namespace RadiantConnect.EventHandler.Events
{
	public class QueueEvents(Initiator initiator)
	{
		internal enum PartyDataReturn
		{
			CustomGame,
			ChangeQueue
		}

		public delegate void QueueEvent<in T>(T value);

		public event QueueEvent<CustomGameData?>? OnCustomGameLobbyCreated;
		public event QueueEvent<string?>? OnQueueChanged;
		public event QueueEvent<string?>? OnEnteredQueue;
		public event QueueEvent<string?>? OnLeftQueue;
		public event QueueEvent<object?>? OnTravelToMenu;
		public event QueueEvent<object?>? OnMatchFound;

		private static string GetEndpoint(string prefix, string log) => log.TryExtractSubstring("https", ']', startIndex => startIndex != -1, prefix);

		private async Task<T?> GetPartyData<T>(PartyDataReturn dataReturn, string endPoint) where T : class?
		{
			string? data = await initiator.ExternalSystem.Net.GetAsync<string>(initiator.ExternalSystem.ClientData.GlzUrl, endPoint).ConfigureAwait(false);

			return data is null ? null : dataReturn switch
			{
				PartyDataReturn.CustomGame => (T?)Convert.ChangeType(JsonSerializer.Deserialize<Party>(data)?.CustomGameData, typeof(T)),
				PartyDataReturn.ChangeQueue => (T?)Convert.ChangeType(JsonSerializer.Deserialize<Party>(data)?.MatchmakingData.QueueId, typeof(T)),
				_ => throw new ArgumentOutOfRangeException(nameof(dataReturn), dataReturn, null)
			};
		}

		public async void HandleQueueEvent(string invoker, string logData)
		{
			try
			{
				string parsedEndPoint = logData.Replace("/queue", "")
					.Replace("/matchmaking/join", "")
					.Replace("/matchmaking/leave", "")
					.Replace("/makecustomgame", "");
				if (!logData.Contains("https") && !logData.Contains("LogTravelManager")) return;
			
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
