using System.Net.Http.Json;
using RadiantConnect.Network.ChatEndpoints.DataTypes;

namespace RadiantConnect.Network.ChatEndpoints;

public class ChatEndpoints(Initiator initiator)
{
    // TODO PARTY CHAT INFO, PRE GAME CHAT INFO, CURRENT GAME CHAT, SEND CHAT MESSAGE

    public async Task<ChatInfo?> GetChatInfo()
    {
        return await initiator.ExternalSystem.Net.GetAsync<ChatInfo>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/conversations");
    }

    public async Task<ChatParticipant?> GetChatParticipants()
    {
        return await initiator.ExternalSystem.Net.GetAsync<ChatParticipant>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v5/participants");
    }

    public async Task<InternalMessages?> GetMessageHistory()
    {
        return await initiator.ExternalSystem.Net.GetAsync<InternalMessages>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages");
    }

    public async Task<object?> SendChatMessage(NewMessage newMessage)
    {
        return await initiator.ExternalSystem.Net.PostAsync<object>($"https://127.0.0.1:{ValorantNet.GetAuthPort()}", "/chat/v6/messages", JsonContent.Create(newMessage));
    }
}