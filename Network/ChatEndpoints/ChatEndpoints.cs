using RadiantConnect.Network.ChatEndpoints.DataTypes;

namespace RadiantConnect.Network.ChatEndpoints
{
	/// <summary>
	/// Provides access to chat-related service endpoints.
	/// </summary>
	/// <remarks>
	/// This endpoint group exposes operations related to chat configuration,
	/// connection metadata, and messaging-related state.
	/// </remarks>
	/// <param name="initiator">
	/// The initialized <see cref="Initiator"/> instance providing
	/// authentication, networking, and configuration context.
	/// </param>
	public class ChatEndpoints(Initiator initiator)
	{
		/// <summary>
		/// Retrieves chat conversations.
		/// </summary>
		/// <returns>
		/// A <see cref="ChatInfo"/> instance containing chat-related information,
		/// or <c>null</c> if the request fails or no data is available.
		/// </returns>
		/// <exception cref="RadiantConnectException">
		/// Thrown when the request cannot be completed due to authentication
		/// or network errors.
		/// </exception>
		/// <remarks>
		/// This method is typically used to determine chat server affinity,
		/// routing configuration, and feature availability.
		/// </remarks>
		public async Task<ChatInfo?> GetChatInfo() => await initiator.ExternalSystem.Net.GetAsync<ChatInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/conversations").ConfigureAwait(false);

		internal async Task<ChatParticipant?> GetChatParticipants() => await initiator.ExternalSystem.Net.GetAsync<ChatParticipant>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v5/participants").ConfigureAwait(false);

		internal async Task<InternalMessages?> GetMessageHistory() => await initiator.ExternalSystem.Net.GetAsync<InternalMessages>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages").ConfigureAwait(false);

		internal async Task<object?> SendChatMessage(NewMessage newMessage) => await initiator.ExternalSystem.Net.PostAsync<object>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages", JsonContent.Create(newMessage)).ConfigureAwait(false);
	}
}