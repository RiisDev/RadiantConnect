namespace RadiantConnect.ValorantApi
{
    public static class Versions
    {
        public static async Task<VersionData?> GetVersionAsync(string uuid) => await ValorantApiClient.GetAsync<VersionData?>("https://valorant-api.com/v1", "version");
        
        public record VersionDatum(
            [property: JsonPropertyName("manifestId")] string ManifestId,
            [property: JsonPropertyName("branch")] string Branch,
            [property: JsonPropertyName("version")] string Version,
            [property: JsonPropertyName("buildVersion")] string BuildVersion,
            [property: JsonPropertyName("engineVersion")] string EngineVersion,
            [property: JsonPropertyName("riotClientVersion")] string RiotClientVersion,
            [property: JsonPropertyName("riotClientBuild")] string RiotClientBuild,
            [property: JsonPropertyName("buildDate")] DateTime? BuildDate
        );

        public record VersionData(
            [property: JsonPropertyName("status")] int? Status,
            [property: JsonPropertyName("data")] VersionDatum Data
        );
    }  
}
