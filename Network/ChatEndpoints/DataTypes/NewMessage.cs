namespace RadiantConnect.Network.ChatEndpoints.DataTypes;
public record NewMessage(
    [property: JsonPropertyName("cid")] string ConversationId,
    [property: JsonPropertyName("message")] string Message,
    [property: JsonPropertyName("type")] string ChatType
);