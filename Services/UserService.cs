using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using RadiantConnect.Network.PVPEndpoints.DataTypes;

namespace RadiantConnect.Services
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class UserService
    {
        // Get match details by id string? matchDetails = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/match-details/v1/matches/{match.MatchID}");
        // Get Match History: string? data = await Net.GetAsync(LogStats.ClientData.PdUrl, $"/mmr/v1/players/{UserId}/competitiveupdates?startIndex=0&endIndex=20&queue={queueType}");


        public async Task<string> GetCurrentSeasonId()
        {
            string? data = await Initiator.InternalSystem.Net.GetAsync(Initiator.InternalSystem.ClientData.SharedUrl, "/content-service/v3/content");

            if (string.IsNullOrEmpty(data)) return string.Empty;

            Content? seasonContent = JsonSerializer.Deserialize<Content>(data);

            Season? season = seasonContent?.Seasons.FirstOrDefault(s => s.IsActive && s.Name.Contains("ACT"));

            return season?.SeasonId ?? string.Empty;
        }

    }
}
