using RadiantConnect.Network.LocalEndpoints.DataTypes;

namespace RadiantConnect.SocketServices.InternalTcp
{
	public partial class TcpEvents : IDisposable
	{
		// User Variables to detect changes
		private string _matchmakingStatus = "unknown";
		private string _queueId = "unknown";
		private string _map = "unknown";
		private string _gamestate = "unknown";
		private string _score = "unknown";

		public delegate void PresenceUpdate(string base64);

		public delegate void CurrencyEvent(string value);
		public delegate void QueueEvent(string value);
		public delegate void MatchChange(string value);

		public delegate void XpEvent();
		public delegate void MmrEvent();
		public delegate void ContractEvent();
		public delegate void Heartbeat();

		public event MatchChange? OnScoreChanged;

		public event QueueEvent? OnQueueStateChanged;
		public event QueueEvent? OnQueueIdChanged;
		public event QueueEvent? OnMapChanged;
		public event QueueEvent? OnGameStateChanged;

		public event CurrencyEvent? OnCurrencyAdjusted;

		public event XpEvent? OnXpChanged;
		public event MmrEvent? OnMmrChanged;
		public event ContractEvent? OnContractChanged;
		public event Heartbeat? OnSessionHeartbeat;

		public event PresenceUpdate? OnPresenceUpdated;

		private readonly Initiator _initiator;
		private readonly ValSocket _socket;

		public TcpEvents(Initiator init, ValSocket socket, bool initiateSocket = false)
		{
			_initiator = init;
			_socket = socket;
			SetupVariables().Wait();
			socket.OnNewMessage += (e) => TcpMessage(e).Wait();
			if (initiateSocket) socket.InitializeConnection();
		}

		private async Task SetupVariables()
		{
			try
			{
				FriendPresences? friendPresences = await _initiator.Endpoints.LocalEndpoints.GetFriendsPresencesAsync().ConfigureAwait(false);
				Presence? localPresence = friendPresences?.Presences.FirstOrDefault(x => x.Puuid == _initiator.Client.UserId);

				try
				{
					_matchmakingStatus = JsonDocument
						.Parse(localPresence?.Private.FromBase64() ?? "[]").RootElement
						.GetProperty("partyState").ToString();
				}
				catch { /**/ }

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
			switch (value)
			{
				case var _ when value.Contains("\"eventType\":\"Update\",\"uri\":\"/chat/", StringComparison.Ordinal):
					HandlePresenceUpdate(value);
					break;
				case var _ when value.Contains("cap/v1/wallet", StringComparison.Ordinal):
					HandleCurrencyUpdate(value);
					break;
				case var _ when value.Contains("ares-account-xp/account-xp", StringComparison.Ordinal):
					OnXpChanged?.Invoke();
					break;
				case var _ when value.Contains("ares-mmr/mmr/v1/players/", StringComparison.Ordinal):
					OnMmrChanged?.Invoke();
					break;
				case var _ when value.Contains("ares-contracts/contracts/v1/players/", StringComparison.Ordinal):
					OnContractChanged?.Invoke();
					break;
				case var _ when value.Contains("product-session/v1/session-heartbeats", StringComparison.Ordinal):
					OnSessionHeartbeat?.Invoke();
					break;
			}

			await Task.CompletedTask.ConfigureAwait(false);
		}

		private void HandleCurrencyUpdate(string data)
		{
			Match match = GetCurrencyAmount().Match(data);
			if (!match.Success) return;
			OnCurrencyAdjusted?.Invoke(match.Groups[1].Value);
		}

		public void HandlePresenceUpdate(string data)
		{
			Match match = GetPresence64().Match(data);
			if (!match.Success) return;
			string presence64 = match.Groups[1].Value;
			string jsonData = presence64.FromBase64();

			OnPresenceUpdated?.Invoke(jsonData);
			
			Debug.WriteLine(jsonData);

			JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
			JsonElement root = jsonDocument.RootElement;
			if (!root.TryGetProperty("partyState", out JsonElement partyState)) return;
			if (!root.TryGetProperty("queueId", out JsonElement queueId)) return;
			if (!root.TryGetProperty("matchMap", out JsonElement matchMap)) return;
			if (!root.TryGetProperty("sessionLoopState", out JsonElement sessionState)) return;
			if (!root.TryGetProperty("partyOwnerMatchScoreAllyTeam", out JsonElement allyScore)) return;
			if (!root.TryGetProperty("partyOwnerMatchScoreEnemyTeam", out JsonElement enemyScore)) return;

			string matchmakingStatus = partyState.GetString() ?? "unknown";
			string queueIdOut = queueId.GetString() ?? "unknown";
			string map = matchMap.GetString() ?? "unknown";
			string gameState = sessionState.GetString() ?? "unknown";
			string score = $"{allyScore.GetInt32()} - {enemyScore.GetInt32()}";

			if (map.Contains('/', StringComparison.Ordinal))
				map = map[(map.LastIndexOf('/') + 1)..].Trim();

			if (gameState != _gamestate)
			{
				_gamestate = gameState;
				OnGameStateChanged?.Invoke(gameState);
			}

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

			if (map != _map && gameState == "PREGAME")
			{
				_map = map;
				OnMapChanged?.Invoke(map);
			}

			if (score != _score && gameState == "INGAME")
			{
				_score = score;
				OnScoreChanged?.Invoke(score);
			}
		}

		[GeneratedRegex("private\"\\s*:\\s*\"([^\"]+)", RegexOptions.Compiled)]
		private static partial Regex GetPresence64();

		[GeneratedRegex("amount\\\\\":(\\d+)", RegexOptions.Compiled)]
		private static partial Regex GetCurrencyAmount();

		public void Dispose()
		{
			_socket.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}