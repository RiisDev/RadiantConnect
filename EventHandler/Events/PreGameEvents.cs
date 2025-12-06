using RadiantConnect.Methods;

namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events that occur during the pre-game phase, including player load, match load,
	/// and agent selection/lock-in actions.
	/// </summary>
	/// <param name="initiator">The event initiator used to bind pre-game event triggers.</param>
	public class PreGameEvents(Initiator initiator)
	{
		/// <summary>
		/// Represents a callback for pre-game events that provide a string identifier.
		/// </summary>
		/// <param name="id">The identifier associated with the event.</param>
		public delegate void QueueEvent(string id);

		/// <summary>
		/// Occurs when a player has fully loaded into the pre-game session.
		/// </summary>
		public event QueueEvent? OnPreGamePlayerLoaded;

		/// <summary>
		/// Occurs when the pre-game match information has been loaded.
		/// </summary>
		public event QueueEvent? OnPreGameMatchLoaded;

		/// <summary>
		/// Occurs when a player selects an agent.
		/// </summary>
		public event QueueEvent? OnAgentSelected;

		/// <summary>
		/// Occurs when a player locks in their selected agent.
		/// </summary>
		public event QueueEvent? OnAgentLockedIn;

		private string _matchId = null!;

		internal void HandlePreGameEvents(string invoker, string logData)
		{
			string agentId;
			switch (invoker)
			{
				case "Pregame_GetPlayer":
					OnPreGamePlayerLoaded?.Invoke(initiator.ExternalSystem.ClientData.UserId);
					break;
				case "Pregame_GetMatch":
					string matchId = logData.ExtractValue(@"matches/([a-fA-F\d-]+)", 1);
					if (matchId.IsNullOrEmpty()) return;
					if (matchId == _matchId) return;
					_matchId = matchId;
					OnPreGameMatchLoaded?.Invoke(matchId);
					break;
				case "Pregame_LockCharacter":
					agentId = logData.ExtractValue(@"lock/([a-fA-F\d-]+)", 1);
					OnAgentLockedIn?.Invoke(ValorantTables.AgentIdToAgent[agentId]);
					break;
				case "Pregame_SelectCharacter":
					agentId = logData.ExtractValue(@"select/([a-fA-F\d-]+)", 1);
					OnAgentSelected?.Invoke(ValorantTables.AgentIdToAgent[agentId]);
					break;
			}
		}
	}
}