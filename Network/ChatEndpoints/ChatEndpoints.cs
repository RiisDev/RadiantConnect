using System.Net.Http.Json;
using RadiantConnect.Network.ChatEndpoints.DataTypes;

namespace RadiantConnect.Network.ChatEndpoints;

public class ChatEndpoints
{
    // TODO PARTY CHAT INFO, PRE GAME CHAT INFO, CURRENT GAME CHAT, SEND CHAT MESSAGE

    public static async Task<ChatInfo?> GetChatInfo()
    {
        return await ValorantNet.GetAsync<ChatInfo>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v6/conversations");
    }

    public static async Task<ChatParticipant?> GetChatParticipants()
    {
        return await ValorantNet.GetAsync<ChatParticipant>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v5/participants");
    }

    public static async Task<InternalMessages?> GetMessageHistory()
    {
        return await ValorantNet.GetAsync<InternalMessages>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v6/messages");
    }

    public static async Task<object?> SendChatMessage(NewMessage newMessage)
    {
        return await ValorantNet.PostAsync<object>($"https://127.0.0.1:{Initiator.InternalSystem.Net.GetAuthPort()}", "/chat/v6/messages", JsonContent.Create(newMessage));
    }
}