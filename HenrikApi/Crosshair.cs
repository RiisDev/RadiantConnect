namespace RadiantConnect.HenrikApi
{
    public class Crosshair(HenrikClient henrikClient)
    {
        public async Task<string?> GenerateCrosshairAsync(string id) => await henrikClient.GetAsync<string?>($"/valorant/v1/crosshair/generate?id={id}");
    }
}  
