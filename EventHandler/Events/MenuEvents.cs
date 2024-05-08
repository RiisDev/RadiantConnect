namespace RadiantConnect.EventHandler.Events
{
    public class MenuEvents()
    {
        public delegate void MenuEvent<in T>(T value);

        public event MenuEvent<object?>? OnBattlePassView;
        public event MenuEvent<object?>? OnAgentsView;
        public event MenuEvent<object?>? OnCareerView;
        public event MenuEvent<object?>? OnPlayScreen;
        public event MenuEvent<object?>? OnEsportView;
        public event MenuEvent<object?>? OnCollectionView;
        public event MenuEvent<object?>? OnStoreView;
        public event MenuEvent<object?>? OnPremierView;

        public void HandleMenuEvent(string invoker, string logData)
        {
            if (!logData.Contains("LogMenuStackManager")) return;

            switch (invoker)
            {
                case "BattlePassScreenV2_Opened":
                    OnBattlePassView?.Invoke(null);
                    break;
                case "CharactersScreenV2_Opened":
                    OnAgentsView?.Invoke(null);
                    break;
                case "MatchHistoryScreenWidgetV3_Opened":
                    OnCareerView?.Invoke(null);
                    break;
                case "PlayScreenV5_Opened":
                    OnPlayScreen?.Invoke(null);
                    break;
                case "Esports_MainScreen_Opened":
                    OnEsportView?.Invoke(null);
                    break;
                case "CollectionsScreen_Opened":
                    OnCollectionView?.Invoke(null);
                    break;
                case "TabbedStoreScreen_Opened":
                    OnStoreView?.Invoke(null);
                    break;
                case "TournamentsScreen_Opened":
                    OnPremierView?.Invoke(null);
                    break;
            }
        }
    }
}
