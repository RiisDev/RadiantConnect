namespace RadiantConnect.Network.ChatEndpoints.DataTypes
{
	public record Conversation(
		[property: JsonPropertyName("cid")] string Cid,
		[property: JsonPropertyName("direct_messages")] bool DirectMessages,
		[property: JsonPropertyName("global_readership")] bool GlobalReadership,
		[property: JsonPropertyName("message_history")] bool MessageHistory,
		[property: JsonPropertyName("mid")] string Mid,
		[property: JsonPropertyName("muted")] bool Muted,
		[property: JsonPropertyName("mutedRestriction")] bool MutedRestriction,
		[property: JsonPropertyName("type")] string Type,
		[property: JsonPropertyName("uiState")] UiState UiState,
		[property: JsonPropertyName("unread_count")] int UnreadCount
	);

	public record ChatInfo(
		[property: JsonPropertyName("conversations")] IReadOnlyList<Conversation> Conversations
	);

	public record UiState(
		[property: JsonPropertyName("changedSinceHidden")] bool ChangedSinceHidden,
		[property: JsonPropertyName("hidden")] bool Hidden
	);
}

