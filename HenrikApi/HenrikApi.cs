
namespace RadiantConnect.HenrikApi
{
    public class HenrikApi
    {
        internal static HenrikClient Client;

        public HenrikApi(string apiKey) => Client = new HenrikClient(apiKey);

        public Accounts Accounts { get; } = new(Client);
        public Content Content { get; } = new(Client);
        public Crosshair Crosshair { get; } = new(Client);
        public Esports Esports { get; } = new(Client);
        public Leaderboard Leaderboard { get; } = new(Client);
        public Match Match { get; } = new(Client);
        public Matchlist Matchlist { get; } = new(Client);
        public MMR Mmr { get; } = new(Client);
        public MMRHistory MmrHistory { get; } = new(Client);
        public Premier Premier { get; } = new(Client);
        public QueueStatus QueueStatus { get; } = new(Client);
        public Raw Raw { get; } = new(Client);
        public Status Status { get; } = new(Client);
        public Store Store { get; } = new(Client);
        public StoredData StoredData { get; } = new(Client);
        public Website Website { get; } = new(Client);
    }
}
