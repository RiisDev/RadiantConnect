namespace RadiantConnect.EventHandler.Events
{
	/// <summary>
	/// Provides events related to party state changes, invitations, and pod (server) transitions.
	/// </summary>
	public class PartyEvents
	{
		/// <summary>
		/// Represents a callback for party-related events that provide a string value.
		/// </summary>
		/// <param name="value">The value passed by the event.</param>
		public delegate void PartyEvent(string value);

		/// <summary>
		/// Occurs when the party state has changed.
		/// </summary>
		public event PartyEvent? OnChanged;

		/// <summary>
		/// Occurs when a party invite is sent.
		/// </summary>
		public event PartyEvent? OnInviteSent;

		/// <summary>
		/// Occurs when a party invite is declined.
		/// </summary>
		public event PartyEvent? OnInviteDeclined;

		/// <summary>
		/// Occurs when the party's game pod (server region) changes.
		/// </summary>
		public event PartyEvent? OnGamePodChanged;

		internal void HandleMatchEvent(string invoker, string logData)
		{
			string partyId;
			switch (invoker)
			{
				case "Party_Updated":
					string partyNegateData = logData[..(logData.LastIndexOf('/')+1)];
					partyId = logData.Replace(partyNegateData, "", StringComparison.Ordinal);
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
