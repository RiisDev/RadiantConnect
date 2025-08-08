﻿using RadiantConnect.Methods;

namespace RadiantConnect.EventHandler.Events
{
    public class PreGameEvents(Initiator initiator)
    {
        private string _matchId = null!;
        public delegate void QueueEvent(string id);

        public event QueueEvent? OnPreGamePlayerLoaded;
        public event QueueEvent? OnPreGameMatchLoaded;
        public event QueueEvent? OnAgentSelected;
        public event QueueEvent? OnAgentLockedIn;

        public void HandlePreGameEvents(string invoker, string logData)
        {
            string agentId;
            switch (invoker)
            {
                case "Pregame_GetPlayer":
                    OnPreGamePlayerLoaded?.Invoke(initiator.ExternalSystem.ClientData.UserId);
                    break;
                case "Pregame_GetMatch":
                    string matchId = logData.ExtractValue(@"matches/([a-fA-F\d-]+)", 1);
                    if (string.IsNullOrEmpty(matchId)) return;
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