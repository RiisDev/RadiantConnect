using RadiantConnect.EventHandler.Events;

namespace RadiantConnect.EventHandler
{
	public class GameEvents(Initiator initiator)
	{
		public QueueEvents Queue = new(initiator);
		public PreGameEvents PreGame = new(initiator);
		public MatchEvents Match = new();
		public RoundEvents Round = new();
		public VoteEvents Vote = new();
		public InGameEvents InGame = new();
		public MiscEvents Misc = new();
		public PartyEvents Party = new();
		public MenuEvents Menu = new();

		internal string LastEventCall = "";
		internal long LastLineRead;
		
		internal void HandleEvent(Action<string, string> eventAction, string logInvoker, string log, long lineIndex)
		{
			if (LastEventCall == logInvoker && !(logInvoker.Equals("Party_ChangeQueue") || logInvoker.Equals("Pregame_SelectCharacter"))) return;
			eventAction.Invoke(logInvoker, log);
			LastEventCall = logInvoker;
			LastLineRead = lineIndex;
		}

		[SuppressMessage("ReSharper", "StringLiteralTypo")]
		internal void ParseLogText(string logText)
		{
			List<(string Keyword, Action<string, long> Handler)> handlers =
			[
				("Party_ChangeQueue",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Party_ChangeQueue", line, index)),
				("Party_EnterMatchmakingQueue",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Party_EnterMatchmakingQueue", line, index)),
				("Party_LeaveMatchmakingQueue",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Party_LeaveMatchmakingQueue", line, index)),
				("Party_MakePartyIntoCustomGame",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Party_MakePartyIntoCustomGame", line, index)),
				("LogTravelManager: Beginning travel to /Game/Maps/Menu/MainMenuV2",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Travel_To_Menu", line, index)),
				("LogPlatformSessionManager: Loopstate changed from MENUS to PREGAME",
					(line, index) => HandleEvent(Queue.HandleQueueEvent, "Match_Found", line, index)),

				("Pregame_GetPlayer",
					(line, index) => HandleEvent(PreGame.HandlePreGameEvents, "Pregame_GetPlayer", line, index)),
				("Pregame_GetMatch",
					(line, index) => HandleEvent(PreGame.HandlePreGameEvents, "Pregame_GetMatch", line, index)),
				("Pregame_LockCharacter",
					(line, index) => HandleEvent(PreGame.HandlePreGameEvents, "Pregame_LockCharacter", line, index)),
				("Pregame_SelectCharacter",
					(line, index) => HandleEvent(PreGame.HandlePreGameEvents, "Pregame_SelectCharacter", line, index)),

				("LogMapLoadModel: Update: [Map Name: ", (line, index) =>
				{
					if (line.Contains(
							"| Changed: FALSE] [Local World: TRUE | Changed: FALSE] [Match Setup: TRUE | Changed: TRUE] [Map Ready: FALSE | Changed: FALSE] [Map Complete: FALSE | Changed: FALSE]"))
						HandleEvent(Match.HandleMatchEvent, "Map_Loaded", line, index);
				}),

				("Match Ended: Completion State:", (line, index) =>
				{
					Round.ResetRound();
					HandleEvent(Match.HandleMatchEvent, "Match_Ended", line, index);
				}),

				("LogPlatformSessionManager: Loopstate changed from PREGAME to INGAME", (line, index) =>
				{
					Round.ResetRound();
					HandleEvent(Match.HandleMatchEvent, "Match_Started", line, index);
				}),


				("AShooterGameState::OnRoundEnded",
					(line, index) => HandleEvent(Round.HandleRoundEvent, "Round_Ended", line, index)),
				("Gameplay started at local time",
					(line, index) => HandleEvent(Round.HandleRoundEvent, "Round_Started", line, index)),

				("LogVoteControllerComponent: Setting vote input bindings enabled to 1",
					(line, index) => HandleEvent(Vote.HandleVoteEvent, "Vote_Called", line, index)),
				("LogVoteControllerComponent: Making vote request for option ",
					(line, index) => HandleEvent(Vote.HandleVoteEvent, "Vote_Invoked", line, index)),
				("LogVoteControllerComponent: Requesting new vote SurrenderVote_C",
					(line, index) => HandleEvent(Vote.HandleVoteEvent, "Surrender_Called", line, index)),
				("LogVoteControllerComponent: Requesting new vote TimeoutVote_C",
					(line, index) => HandleEvent(Vote.HandleVoteEvent, "Timeout_Called", line, index)),
				("LogVoteControllerComponent: Requesting new vote RemakeVoteNew_C",
					(line, index) => HandleEvent(Vote.HandleVoteEvent, "Timeout_Called", line, index)),

				("LogMenuStackManager: Opening preRound",
					(line, index) => HandleEvent(InGame.HandleInGameEvent, "Buy_Menu_Opened", line, index)),
				("LogMenuStackManager: Closing preRound",
					(line, index) => HandleEvent(InGame.HandleInGameEvent, "Buy_Menu_Closed", line, index)),
				("LogNet: Warning: UNetDriver::ProcessRemoteFunction: No owning connection for actor",
					(line, index) => HandleEvent(InGame.HandleInGameEvent, "Util_Placed", line, index)),

				("Session_Heartbeat",
					(line, index) => HandleEvent(Misc.HandleInGameEvent, "Session_Heartbeat", line, index)),

				("LogRMSService: Received RMS update. URI: /riot-messaging-service/v1/messages/ares-parties/parties/v1/parties/",
					(line, index) => HandleEvent(Party.HandleMatchEvent, "Party_Updated", line, index)),
				("Party_DeclineRequest",
					(line, index) => HandleEvent(Party.HandleMatchEvent, "Party_DeclineRequest", line, index)),
				("Party_InviteToParty",
					(line, index) => HandleEvent(Party.HandleMatchEvent, "Party_InviteToParty", line, index)),
				("Party_SetPreferredGamePods",
					(line, index) => HandleEvent(Party.HandleMatchEvent, "Party_SetPreferredGamePods", line, index)),

				("LogMenuStackManager: Opening BattlepassScreenV2_PC_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "BattlePassScreenV2_Opened", line, index)),
				("LogMenuStackManager: Opening CharactersScreenV2_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "CharactersScreenV2_Opened", line, index)),
				("LogMenuStackManager: Opening MatchHistoryScreenWidgetV3_C",
					(line, index) =>
						HandleEvent(Menu.HandleMenuEvent, "MatchHistoryScreenWidgetV3_Opened", line, index)),
				("LogMenuStackManager: Opening PlayScreenV5_PC_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "PlayScreenV5_Opened", line, index)),
				("LogMenuStackManager: Opening Esports_MainScreen_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "Esports_MainScreen_Opened", line, index)),
				("LogMenuStackManager: Opening CollectionsScreen_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "CollectionsScreen_Opened", line, index)),
				("LogMenuStackManager: Opening TabbedStoreScreen_PC_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "TabbedStoreScreen_Opened", line, index)),
				("LogMenuStackManager: Opening TournamentsScreen_C",
					(line, index) => HandleEvent(Menu.HandleMenuEvent, "TournamentsScreen_Opened", line, index))

			];

			string[] fileLines = logText.Split('\n');
			for (long i = fileLines.Length - 1; i > LastLineRead; i--)
			{
				string line = fileLines[i].Trim();

				if (line.Contains("Log file closed"))
					break;

				foreach ((string keyword, Action<string, long> handler) in handlers)
				{
					if (!line.Contains(keyword)) continue;

					handler(line, i);
				}
			}
		}

	}
}
