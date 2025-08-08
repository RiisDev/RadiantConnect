namespace RadiantConnect.ValorantApi
{
    public static class Events
    {
        public static async Task<EventsData?> GetEventsAsync() => await ValorantApiClient.GetAsync<EventsData?>("https://valorant-api.com/v1", "events");

        public static async Task<EventData?> GetEventAsync(string uuid) => await ValorantApiClient.GetAsync<EventData?>("https://valorant-api.com/v1", $"events/{uuid}");

        public static async Task<EventData?> GetEventByUuidAsync(string uuid) => await GetEventAsync(uuid);
        
        public record EventDatum(
            [property: JsonPropertyName("uuid")] string Uuid,
            [property: JsonPropertyName("displayName")] string DisplayName,
            [property: JsonPropertyName("shortDisplayName")] string ShortDisplayName,
            [property: JsonPropertyName("startTime")] DateTime? StartTime,
            [property: JsonPropertyName("endTime")] DateTime? EndTime,
            [property: JsonPropertyName("assetPath")] string AssetPath
        );
        public record EventsData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] IReadOnlyList<EventDatum> Data
        );
        public record EventData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] EventDatum Data
        );
    }  
}
