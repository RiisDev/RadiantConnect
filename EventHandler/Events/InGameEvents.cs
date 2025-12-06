namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events that occur during active in-game interactions, such as buy menu usage or utility placement.
	/// </summary>
	public class InGameEvents
	{
		/// <summary>
		/// Represents a callback for in-game events that supply a value of type <typeparamref name="T"/>.
		/// </summary>
		/// <typeparam name="T">The type of value associated with the event.</typeparam>
		/// <param name="value">The value passed by the event.</param>
		public delegate void InGameEvent<in T>(T value);

		/// <summary>
		/// Occurs when the buy menu is opened during a round.
		/// </summary>
		public event InGameEvent<int?>? OnBuyMenuOpened;

		/// <summary>
		/// Occurs when the buy menu is closed during a round.
		/// </summary>
		public event InGameEvent<int?>? OnBuyMenuClosed;

		/// <summary>
		/// Occurs when a player places a piece of utility (e.g., smokes, abilities).
		/// </summary>
		public event InGameEvent<string?>? OnUtilPlaced;

		internal void HandleInGameEvent(string invoker, string logData)
		{
			switch (invoker)
			{
				case "Buy_Menu_Opened":
					OnBuyMenuOpened?.Invoke(1);
					break;
				case "Buy_Menu_Closed":
					OnBuyMenuClosed?.Invoke(0);
					break;
				case "Util_Placed":
					{
						string util = logData.ExtractValue(@"actor\s(\S+)(?=\.)", 1);
						OnUtilPlaced?.Invoke(util);
						break;
					}
			}
		}
	}
}
