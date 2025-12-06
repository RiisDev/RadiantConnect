namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides miscellaneous game-related events that do not belong to a specific gameplay phase.
	/// </summary>
	public class MiscEvents
	{
		/// <summary>
		/// Represents a callback for miscellaneous events that do not require parameters.
		/// </summary>
		public delegate void MiscEvent();

		/// <summary>
		/// Occurs when the client heartbeat signal is detected or triggered.
		/// </summary>
		public event MiscEvent? OnHeartbeat;

		internal void HandleInGameEvent(string invoker, string _)
		{
			switch (invoker)
			{
				case "Session_Heartbeat":
					OnHeartbeat?.Invoke();
					break;
			}
		}
	}
}