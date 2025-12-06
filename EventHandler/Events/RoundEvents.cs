namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events related to the start and end of individual rounds during a match.
	/// </summary>
	public class RoundEvents
	{
		/// <summary>
		/// Represents a callback for round-related events, providing the affected round number.
		/// </summary>
		/// <param name="roundNumber">The round number associated with the event.</param>
		public delegate void RoundEvent(int roundNumber);

		/// <summary>
		/// Occurs when a new round begins.
		/// </summary>
		public event RoundEvent? OnRoundStarted;

		/// <summary>
		/// Occurs when a round has ended.
		/// </summary>
		public event RoundEvent? OnRoundEnded;

		private bool _roundEnded;
		private int _roundNumber;

		internal void ResetRound() => _roundNumber = 0;

		internal void HandleRoundEvent(string invoker, string logData)
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
					int roundNumber = int.Parse(logData.ExtractValue(@"round '([a-fA-F\d-]+)'", 1), StringExtensions.CultureInfo) + 1;
					if (_roundNumber < roundNumber) _roundNumber = roundNumber;
					OnRoundEnded?.Invoke(roundNumber);
					_roundEnded = true;
					break;
			}
		}
	}
}
