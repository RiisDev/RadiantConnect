using RadiantConnect.Utilities;

namespace RadiantConnect.EventHandler.Events
{
    public class PartyEvents
    {
        public delegate void PartyEvent(string value);

        public event PartyEvent? OnChanged;
        public event PartyEvent? OnInviteSent;
        public event PartyEvent? OnInviteDeclined;
        public event PartyEvent? OnGamePodChanged;
        
        public void HandleMatchEvent(string invoker, string logData)
        {
            string partyId;
            switch (invoker)
            {
                case "Party_Updated":
                    string partyNegateData = logData[..(logData.LastIndexOf('/')+1)];
                    partyId = logData.Replace(partyNegateData, "");
                    OnChanged?.Invoke(partyId);
                    break;
                case "Party_InviteToParty":
                    partyId = logData.ExtractValue(@"\/parties\/([a-fA-F\d-]+)\/", 1);
                    OnInviteSent?.Invoke(partyId);
                    break;
                case "Party_DeclineRequest":
                    partyId = logData.ExtractValue(@"\/parties\/([a-fA-F\d-]+)\/", 1);
                    OnInviteDeclined?.Invoke(partyId);
                    break;
                case "Party_SetPreferredGamePods":
                    partyId = logData.ExtractValue(@"\/parties\/([a-fA-F\d-]+)\/", 1);
                    OnGamePodChanged?.Invoke(partyId);
                    break;
            }
        }
    }
}
