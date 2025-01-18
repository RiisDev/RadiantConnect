using RadiantConnect.Methods;
using RadiantConnect.Utilities;

// ReSharper disable RedundantDefaultMemberInitializer

namespace RadiantConnect.EventHandler.Events
{
    public class RoundEvents
    {
        private bool _roundEnded = false;
        private int _roundNumber = 0;
        public delegate void RoundEvent(int roundNumber);

        public event RoundEvent? OnRoundStarted;
        public event RoundEvent? OnRoundEnded;

        public void ResetRound() { _roundNumber = 0; }

        public void HandleRoundEvent(string invoker, string logData)
        {
            switch (invoker)
            {
                case "Round_Started":
                    if (!_roundEnded) return;
                    _roundNumber++;
                    OnRoundStarted?.Invoke(_roundNumber);
                    _roundEnded = false;
                    break;
                case "Round_Ended":
                    int roundNumber = int.Parse(logData.ExtractValue(@"round '([a-fA-F\d-]+)'", 1)) + 1;
                    if (_roundNumber < roundNumber) _roundNumber = roundNumber;
                    OnRoundEnded?.Invoke(roundNumber);
                    _roundEnded = true;
                    break;
            }
        }
    }
}
