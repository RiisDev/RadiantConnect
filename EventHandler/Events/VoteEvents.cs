namespace RadiantConnect.EventHandler.Events
{
    public class VoteEvents
    {
        public delegate void VoteEvent(bool yesNo);

        public event VoteEvent? OnVoteDeclared;
        public event VoteEvent? OnVoteInvoked;

        public event VoteEvent? OnSurrenderCalled;
        public event VoteEvent? OnRemakeCalled;
        public event VoteEvent? OnTimeoutCalled;

        public void HandleVoteEvent(string invoker, string logData)
        {
            switch (invoker)
            {
                case "Vote_Called":
                    OnVoteDeclared?.Invoke(true);
                    break;
                case "Vote_Invoked":
                    OnVoteInvoked?.Invoke(logData[^1] == 's');
                    break;
                case "Surrender_Called":
                    OnSurrenderCalled?.Invoke(true);
                    break;
                case "Timeout_Called":
                    OnTimeoutCalled?.Invoke(true);
                    break;
                case "Remake_Called":
                    OnRemakeCalled?.Invoke(true);
                    break;
            }
        }
    }
}
