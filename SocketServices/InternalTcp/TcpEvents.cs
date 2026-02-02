using RadiantConnect.Network.LocalEndpoints.DataTypes;

namespace RadiantConnect.SocketServices.InternalTcp
{
	/// <summary>
	/// Handles internal TCP events from the Valorant client, including presence updates,
	/// currency changes, matchmaking state, scores, and other session-related events.
	/// </summary>
	public partial class TcpEvents : IDisposable
	{
		// User Variables to detect changes
		private string _matchmakingStatus = "unknown";
		private string _queueId = "unknown";
		private string _map = "unknown";
		private string _gameState = "unknown";
		private string _score = "unknown";

		/// <summary>
		/// Delegate for presence updates (base64-encoded presence data).
		/// </summary>
		/// <param name="base64">The base64-encoded presence string.</param>
		public delegate void PresenceUpdate(string base64);

		/// <summary>
		/// Delegate for currency change events.
		/// </summary>
		/// <param name="value">The new currency value.</param>
		public delegate void CurrencyEvent(string value);

		/// <summary>
		/// Delegate for queue or matchmaking status updates.
		/// </summary>
		/// <param name="value">The queue or matchmaking status.</param>
		public delegate void QueueEvent(string value);

		/// <summary>
		/// Delegate for match-related state changes, such as score updates.
		/// </summary>
		/// <param name="value">The new match-related value.</param>
		public delegate void MatchChange(string value);

		/// <summary> Delegate for experience points changes. </summary>
		public delegate void XpEvent();

		/// <summary> Delegate for MMR (matchmaking rating) changes. </summary>
		public delegate void MmrEvent();

		/// <summary> Delegate for contract-related updates. </summary>
		public delegate void ContractEvent();

		/// <summary> Delegate for session heartbeat events. </summary>
		public delegate void Heartbeat();

		/// <summary> Raised when the match score changes. </summary>
		public event MatchChange? OnScoreChanged;

		/// <summary> Raised when the queue state changes (e.g., matchmaking status). </summary>
		public event QueueEvent? OnQueueStateChanged;

		/// <summary> Raised when the queue ID changes. </summary>
		public event QueueEvent? OnQueueIdChanged;

		/// <summary> Raised when the match map changes during pregame. </summary>
		public event QueueEvent? OnMapChanged;

		/// <summary> Raised when the game session state changes. </summary>
		public event QueueEvent? OnGameStateChanged;

		/// <summary> Raised when a player's currency is updated. </summary>
		public event CurrencyEvent? OnCurrencyAdjusted;

		/// <summary> Raised when a player's XP changes. </summary>
		public event XpEvent? OnXpChanged;

		/// <summary> Raised when a player's MMR changes. </summary>
		public event MmrEvent? OnMmrChanged;

		/// <summary> Raised when a contract-related update occurs. </summary>
		public event ContractEvent? OnContractChanged;

		/// <summary> Raised on a session heartbeat. </summary>
		public event Heartbeat? OnSessionHeartbeat;

		/// <summary> Raised when the presence of the player is updated. </summary>
		public event PresenceUpdate? OnPresenceUpdated;

		private readonly Initiator _initiator;
		private readonly ValSocket _socket;

		/// <summary>
		/// Initializes a new instance of the <see cref="TcpEvents"/> class.
		/// </summary>
		/// <param name="init">The <see cref="Initiator"/> for API interaction.</param>
		/// <param name="socket">The socket connection for receiving TCP events.</param>
		/// <param name="initiateSocket">Whether to immediately initiate the socket connection.</param>

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

		[SuppressMessage("ReSharper", "InvertIf")]
		private void HandleMatchPresenceData(JsonElement root)
		{
			if (!root.TryGetProperty("sessionLoopState", out JsonElement queueState)) return;
			if (!root.TryGetProperty("queueId", out JsonElement queueId)) return;
			if (!root.TryGetProperty("matchMap", out JsonElement mapData)) return;

			string map = mapData.GetString() ?? "unknown";
			string queueIdOut = queueId.GetString() ?? "unknown";

			string gameState = queueState.GetString() ?? "unknown";
			gameState = gameState == "Invalid" ? "Idle" : gameState;
			
			if (queueIdOut != _queueId)
			{
				_queueId = queueIdOut;
				OnQueueIdChanged?.Invoke(queueIdOut);
			}

			if (gameState != _gameState)
			{
				_gameState = gameState;
				OnGameStateChanged?.Invoke(gameState);
			}

			if (map != _map && gameState == "PREGAME")
			{
				_map = map;
				OnMapChanged?.Invoke(map);
			}
		}

		[SuppressMessage("ReSharper", "InvertIf")]
		private void HandlePartyPresenceData(JsonElement root)
		{
			if (!root.TryGetProperty("partyState", out JsonElement matchmakingStatus)) return;

			if (matchmakingStatus.GetString() != _matchmakingStatus)
			{
				_matchmakingStatus = matchmakingStatus.GetString() ?? "unknown";
				OnQueueStateChanged?.Invoke(_matchmakingStatus);
			}
		}

		/// <summary>
		/// Processes a presence update received from the Valorant client.
		/// Parses the base64-encoded presence data and triggers events
		/// if relevant values have changed (e.g., matchmaking status, queue, map, score).
		/// </summary>
		/// <param name="data">The raw TCP message containing presence information.</param>
		[SuppressMessage("ReSharper", "InvertIf")]
		public void HandlePresenceUpdate(string data)
		{
			// We only want local player updates
			Match puuidMatch = GetPuuid().Match(data);
			if (!puuidMatch.Success) return;
			string puuid = puuidMatch.Groups[1].Value;
			if (puuid != _initiator.Client.UserId) return;

			Match match = GetPresence64().Match(data);
			if (!match.Success) return;
			string presence64 = match.Groups[1].Value;
			string jsonData = presence64.FromBase64();

			OnPresenceUpdated?.Invoke(jsonData);
			
			JsonDocument jsonDocument = JsonDocument.Parse(jsonData);
			JsonElement root = jsonDocument.RootElement;
			if (!root.TryGetProperty("partyOwnerMatchScoreAllyTeam", out JsonElement allyScore)) return;
			if (!root.TryGetProperty("partyOwnerMatchScoreEnemyTeam", out JsonElement enemyScore)) return;

			if (root.TryGetProperty("partyPresenceData", out JsonElement partyPresence)) HandlePartyPresenceData(partyPresence);
			if (root.TryGetProperty("matchPresenceData", out JsonElement matchPresence)) HandleMatchPresenceData(matchPresence);
			
			string score = $"{allyScore.GetInt32()} - {enemyScore.GetInt32()}";

			if (score != _score && _gameState == "INGAME")
			{
				_score = score;
				OnScoreChanged?.Invoke(score);
			}
		}

		[GeneratedRegex("private\"\\s*:\\s*\"([^\"]+)", RegexOptions.Compiled)]
		private static partial Regex GetPresence64();

		[GeneratedRegex("puuid\"\\s*:\\s*\"([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})", RegexOptions.Compiled)]
		private static partial Regex GetPuuid();

		[GeneratedRegex("amount\\\\\":(\\d+)", RegexOptions.Compiled)]
		private static partial Regex GetCurrencyAmount();

		/// <summary>
		/// Disposes the TCP event service and associated socket connection.
		/// </summary>
		public void Dispose()
		{
			_socket.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}