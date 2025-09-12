using RadiantConnect.Services;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.Network.PVPEndpoints.DataTypes;
using RadiantConnect.SocketServices.XMPP;

namespace RadiantConnect.Tests
{
    public class AuthenticationTest
    {
        /* These tests are to be only ran on a machine that has the Valorant client installed. */

        public static bool MachineEligible(bool valorantRequired = true) => Client.MachineReady(valorantRequired);

        [Fact]
        public async Task TestQr()
        {
            if (!MachineEligible(false)) return;

            Authentication.Authentication auth = new();
            
            RSOAuth? rso = await auth.AuthenticateWithQr(Authentication.Authentication.CountryCode.Na);

            Assert.NotNull(rso);

            List<string?> tokenProps =
            [
                rso.AccessToken,
                rso.Entitlement,
                rso.Ssid,
                rso.Affinity,
                rso.ChatAffinity,
                rso.IdToken,
                rso.PasToken,
                rso.RmsToken,
                rso.Clid,
                rso.Csid
            ];

            foreach (string? token in tokenProps)
            {
                Assert.False(string.IsNullOrWhiteSpace(token), "Expected token to be non-null and non-empty.");
            }

            Initiator ini = new(rso);

            AccountXP? accountXp = await ini.Endpoints.PvpEndpoints.FetchAccountXPAsync();

            Assert.NotNull(accountXp);
            Assert.True(accountXp.Progress.Level > -1, "Expected Progress to be non-empty.");

            // Test XMPP Connection

            RemoteXMPP remoteXMPP = new();
            await remoteXMPP.InitiateRemoteXMPP(rso);

            Assert.Equal(RemoteXMPP.XMPPStatus.Connected, remoteXMPP.Status);
        }

        [Fact]
        public async Task TestDriverAuth()
        {
            if (!MachineEligible(false)) return;

            Authentication.Authentication auth = new();
            
            string? username = Environment.GetEnvironmentVariable("RADIANTCONNECT_USERNAME");
            string? password = Environment.GetEnvironmentVariable("RADIANTCONNECT_PASSWORD");

            Assert.True(!string.IsNullOrWhiteSpace(username), "Username is not set. Please set the RADIANTCONNECT_USERNAME environment variable.");
            Assert.True(!string.IsNullOrWhiteSpace(password), "Password is not set. Please set the RADIANTCONNECT_PASSWORD environment variable.");

            RSOAuth? rso = await auth.AuthenticateWithDriver(username, password, null, true);

            Assert.NotNull(rso);

            List<string?> tokenProps =
            [
                rso.AccessToken,
                rso.Entitlement,
                rso.Ssid,
                rso.Affinity,
                rso.ChatAffinity,
                rso.IdToken,
                rso.PasToken,
                rso.RmsToken,
                rso.Clid,
                rso.Csid
            ];

            foreach (string? token in tokenProps)
            {
                Assert.False(string.IsNullOrWhiteSpace(token), "Expected token to be non-null and non-empty.");
            }

            Initiator ini = new(rso);

            AccountXP? accountXp = await ini.Endpoints.PvpEndpoints.FetchAccountXPAsync();

            Assert.NotNull(accountXp);
            Assert.True(accountXp.Progress.Level > -1, "Expected Progress to be non-empty.");
        }

        [Fact]
        public void TestVersionGrabbing()
        {
            if (!MachineEligible()) return;
            string valorantPath = RiotPathService.GetValorantPath();
            Assert.True(File.Exists(valorantPath), "Valorant executable not found.");
            Assert.NotNull(GameVersionService.GetClientVersion(valorantPath));
        }

        [Fact]
        public void TestClientPlatform()
        {
            if (!MachineEligible(false)) return;
            Assert.NotEmpty(GameVersionService.GetClientPlatform());
        }
    }
}
