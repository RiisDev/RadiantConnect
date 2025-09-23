using RadiantConnect.Network.ChatEndpoints.DataTypes;

namespace RadiantConnect.Network.ChatEndpoints
{
	public class ChatEndpoints(Initiator initiator)
	{
		public async Task<ChatInfo?> GetChatInfo() => await initiator.ExternalSystem.Net.GetAsync<ChatInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/conversations").ConfigureAwait(false);

		internal async Task<ChatParticipant?> GetChatParticipants() => await initiator.ExternalSystem.Net.GetAsync<ChatParticipant>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v5/participants").ConfigureAwait(false);

		internal async Task<InternalMessages?> GetMessageHistory() => await initiator.ExternalSystem.Net.GetAsync<InternalMessages>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages").ConfigureAwait(false);

		internal async Task<object?> SendChatMessage(NewMessage newMessage) => await initiator.ExternalSystem.Net.PostAsync<object>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages", JsonContent.Create(newMessage)).ConfigureAwait(false);
	}
}