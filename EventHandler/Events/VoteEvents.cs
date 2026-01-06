namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events related to player voting actions such as surrender,
	/// remake requests, and general vote declarations.
	/// </summary>
	public class VoteEvents
	{
		/// <summary>
		/// Represents a callback for vote-related events, indicating a yes/no response.
		/// </summary>
		/// <param name="yesNo">A boolean value representing the vote outcome or selection.</param>
		public delegate void VoteEvent(bool yesNo);

		/// <summary>
		/// Occurs when a vote is initially declared.
		/// </summary>
		public event VoteEvent? OnVoteDeclared;

		/// <summary>
		/// Occurs when a vote is actively invoked or cast.
		/// </summary>
		public event VoteEvent? OnVoteInvoked;

		/// <summary>
		/// Occurs when a surrender vote is initiated.
		/// </summary>
		public event VoteEvent? OnSurrenderCalled;

		/// <summary>
		/// Occurs when a remake vote is initiated.
		/// </summary>
		public event VoteEvent? OnRemakeCalled;

		/// <summary>
		/// Occurs when a timeout vote is initiated.
		/// </summary>
		public event VoteEvent? OnTimeoutCalled;

		internal void HandleVoteEvent(string invoker, string logData)
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
