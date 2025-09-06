using RadiantConnect.Network.PVPEndpoints.DataTypes;

namespace RadiantConnect.Tests.NetworkBulk
{
    public class PvPEndpointsTests
    {
        private static readonly Initiator Initiator = new();

        [Fact]
        public async Task FetchContentTest()
        {
            if (!Client.MachineReady()) return;
            Content? content = await Initiator.Endpoints.PvpEndpoints.FetchContentAsync();
            Assert.NotNull(content);
            Assert.NotEmpty(content.Events);
        }

        [Fact]
        public async Task FetchAccountXpTest()
        {
            if (!Client.MachineReady()) return;

            AccountXP? accountXP = await Initiator.Endpoints.PvpEndpoints.FetchAccountXPAsync();
            Assert.NotNull(accountXP);
            Assert.NotEmpty(accountXP.Subject);
        }

        [Fact]
        public async Task FetchPlayerLoadoutTest()
        {
            if (!Client.MachineReady()) return;

            PlayerLoadout? loadout =
                await Initiator.Endpoints.PvpEndpoints.FetchPlayerLoadoutAsync();
            Assert.NotNull(loadout);
            Assert.NotEmpty(loadout.Guns);
        }

        [Fact]
        public async Task FetchPlayerMMRTest()
        {
            if (!Client.MachineReady()) return;

            PlayerMMR? mmr = await Initiator.Endpoints.PvpEndpoints.FetchPlayerMMRAsync(Initiator.Client.UserId);
            Assert.NotNull(mmr);
            Assert.NotEmpty(mmr.Subject);
        }

        [Fact]
        public async Task FetchPlayerMatchHistoryTest()
        {
            if (!Client.MachineReady()) return;

            MatchHistory? history =
                await Initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryAsync(Initiator.Client.UserId);
            Assert.NotNull(history);
            Assert.NotEmpty(history.Subject);
        }

        [Fact]
        public async Task FetchPlayerMatchHistoryByQueueIdTest()
        {
            if (!Client.MachineReady()) return;

            MatchHistory? history =
                await Initiator.Endpoints.PvpEndpoints.FetchPlayerMatchHistoryByQueueIdAsync(Initiator.Client.UserId,
                    "competitive");
            Assert.NotNull(history);
            Assert.NotEmpty(history.Subject);
        }
    }
}
