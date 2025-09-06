using RadiantConnect.Network.LocalEndpoints.DataTypes;

namespace RadiantConnect.SocketServices.InternalTcp
{
    public partial class TcpEvents
    {
		// User Variables to detect changes
		private string _matchmakingStatus = "unknown";
		private string _queueId = "unknown";

		public delegate void CurrencyEvent(string value);
		public delegate void QueueEvent(string value);

		public delegate void XpEvent();
		public delegate void MmrEvent();
		public delegate void ContractEvent();
		public delegate void Heartbeat();

		public event QueueEvent? OnQueueStateChanged;
		public event QueueEvent? OnQueueIdChanged;

		public event CurrencyEvent? OnCurrencyAdjusted;

		public event XpEvent? OnXpChanged;
		public event MmrEvent? OnMmrChanged;
		public event ContractEvent? OnContractChanged;
		public event Heartbeat? OnSessionHeartbeat;

		private readonly Initiator _initiator;
		
		public TcpEvents(Initiator init, ValSocket socket, bool initiateSocket = false)
		{
			_initiator = init;
			SetupVariables().Wait();
	        socket.OnNewMessage += (e) => TcpMessage(e).Wait();
			if (initiateSocket) socket.InitializeConnection();
		}

		private async Task SetupVariables()
		{
			try
			{
				FriendPresences? friendPresences = await _initiator.Endpoints.LocalEndpoints.GetFriendsPresencesAsync();
				Presence? localPresence = friendPresences?.Presences.FirstOrDefault(x => x.Puuid == _initiator.Client.UserId);

				try
				{
					_matchmakingStatus = JsonDocument
						.Parse(localPresence?.Private.FromBase64() ?? "[]").RootElement
						.GetProperty("partyState").ToString();
				} catch { /**/ }

				try
				{
					_queueId = JsonDocument
						.Parse(localPresence?.Private.FromBase64() ?? "[]").RootElement
						.GetProperty("partyState").ToString();
				}
				catch { /**/ }
			}
			catch { /**/ }
		}

		private async Task TcpMessage(string value)
        {
			Debug.WriteLine(value);
	        switch (value)
	        {
				case var _ when value.Contains("\"eventType\":\"Update\",\"uri\":\"/chat/"):
					HandlePresenceUpdate(value);
					break;
				case var _ when value.Contains("cap/v1/wallet"):
					HandleCurrencyUpdate(value);
					break;
				case var _ when value.Contains("ares-account-xp/account-xp"):
					OnXpChanged?.Invoke();
					break;
				case var _ when value.Contains("ares-mmr/mmr/v1/players/"):
					OnMmrChanged?.Invoke();
					break;
				case var _ when value.Contains("ares-contracts/contracts/v1/players/"):
					OnContractChanged?.Invoke();
					break;
				case var _ when value.Contains("ares-contracts/contracts/v1/players/"):
					OnContractChanged?.Invoke();
					break;
				case var _ when value.Contains("product-session/v1/session-heartbeats"):
					OnSessionHeartbeat?.Invoke();
					break;
			}

			await Task.CompletedTask;
		}

		private void HandleCurrencyUpdate(string data)
		{
			Match match = GetCurrencyAmount().Match(data);
			if (!match.Success) return;
			OnCurrencyAdjusted?.Invoke(match.Groups[1].Value);
		}

        private void HandlePresenceUpdate(string data)
        {
			Match match = GetPresence64().Match(data);
			if (!match.Success) return;
			string presence64 = match.Groups[1].Value;
			string jsonData = presence64.FromBase64();

			JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
			JsonElement root = jsonDocument.RootElement;
			if (!root.TryGetProperty("partyState", out JsonElement partyState)) return;
			if (!root.TryGetProperty("queueId", out JsonElement queueId)) return;

			string matchmakingStatus = partyState.GetString() ?? "unknown";
			string queueIdOut = queueId.GetString() ?? "unknown";

			if (matchmakingStatus != _matchmakingStatus)
			{
				_matchmakingStatus = matchmakingStatus;
				OnQueueStateChanged?.Invoke(matchmakingStatus);
			}

			if (queueIdOut != _queueId)
			{
				_queueId = queueIdOut;
				OnQueueIdChanged?.Invoke(queueIdOut);
			}
		}

		[GeneratedRegex("private\"\\s*:\\s*\"([^\"]+)", RegexOptions.Compiled)]
		private static partial Regex GetPresence64();

		[GeneratedRegex("amount\\\\\":(\\d+)", RegexOptions.Compiled)]
		private static partial Regex GetCurrencyAmount();
	}

}