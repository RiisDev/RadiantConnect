namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events triggered when the player navigates to various menu screens within the client.
	/// </summary>
	public class MenuEvents
	{
		/// <summary>
		/// Represents a callback for menu navigation events />.
		/// </summary>
		public delegate void MenuEvent();

		/// <summary>
		/// Occurs when the player opens the Battle Pass view.
		/// </summary>
		public event MenuEvent? OnBattlePassView;

		/// <summary>
		/// Occurs when the player opens the Agents view.
		/// </summary>
		public event MenuEvent? OnAgentsView;

		/// <summary>
		/// Occurs when the player opens the Career view.
		/// </summary>
		public event MenuEvent? OnCareerView;

		/// <summary>
		/// Occurs when the player navigates to the Play screen.
		/// </summary>
		public event MenuEvent? OnPlayScreen;

		/// <summary>
		/// Occurs when the player opens the Esports view.
		/// </summary>
		public event MenuEvent? OnEsportView;

		/// <summary>
		/// Occurs when the player opens the Collection view.
		/// </summary>
		public event MenuEvent? OnCollectionView;

		/// <summary>
		/// Occurs when the player opens the Store view.
		/// </summary>
		public event MenuEvent? OnStoreView;

		/// <summary>
		/// Occurs when the player opens the Premier view.
		/// </summary>
		public event MenuEvent? OnPremierView;

		internal void HandleMenuEvent(string invoker, string logData)
		{
			if (!logData.Contains("LogMenuStackManager", StringComparison.InvariantCultureIgnoreCase)) return;

			switch (invoker)
			{
				case "BattlePassScreenV2_Opened":
					OnBattlePassView?.Invoke();
					break;
				case "CharactersScreenV2_Opened":
					OnAgentsView?.Invoke();
					break;
				case "MatchHistoryScreenWidgetV3_Opened":
					OnCareerView?.Invoke();
					break;
				case "PlayScreenV5_Opened":
					OnPlayScreen?.Invoke();
					break;
				case "Esports_MainScreen_Opened":
					OnEsportView?.Invoke();
					break;
				case "CollectionsScreen_Opened":
					OnCollectionView?.Invoke();
					break;
				case "TabbedStoreScreen_Opened":
					OnStoreView?.Invoke();
					break;
				case "TournamentsScreen_Opened":
					OnPremierView?.Invoke();
					break;
			}
		}
	}
}
