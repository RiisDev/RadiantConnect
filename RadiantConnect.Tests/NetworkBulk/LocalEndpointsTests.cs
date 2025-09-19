using RadiantConnect.Network.LocalEndpoints.DataTypes;

namespace RadiantConnect.Tests.NetworkBulk
{
	public class LocalEndpointsTests
	{
		
		private static readonly Initiator Initiator = new();

		[Fact]
		public async Task GetHelpTest()
		{
			if (!Client.MachineReady()) return;
			object? help = await Initiator.Endpoints.LocalEndpoints.GetHelpAsync();

			Assert.NotNull(help);
		}

		[Fact]
		public async Task GetLocalSessionsTest()
		{
			if (!Client.MachineReady()) return;
			object? localSessions = await Initiator.Endpoints.LocalEndpoints.GetLocalSessionsAsync();

			Assert.NotNull(localSessions);
		}

		[Fact]
		public async Task GetRSOInfoTest()
		{
			if (!Client.MachineReady()) return;
			object? rsoInfo = await Initiator.Endpoints.LocalEndpoints.GetRSOInfoAsync();

			Assert.NotNull(rsoInfo);
		}

		[Fact]
		public async Task GetLocalSwaggerTest()
		{
			if (!Client.MachineReady()) return;
			object? localSwagger = await Initiator.Endpoints.LocalEndpoints.GetLocalSwaggerDocsAsync();

			Assert.NotNull(localSwagger);
		}

		[Fact]
		public async Task GetLocaleInfoTest()
		{
			if (!Client.MachineReady()) return;
			LocaleInternal? localeInfo = await Initiator.Endpoints.LocalEndpoints.GetLocaleInfoAsync();

			Assert.NotNull(localeInfo);
			Assert.NotEmpty(localeInfo.Locale);
			Assert.NotEmpty(localeInfo.Region);
		}

		[Fact]
		public async Task GetAliasInfoTest()
		{
			if (!Client.MachineReady()) return;
			AliasInfo? aliasInfo = await Initiator.Endpoints.LocalEndpoints.GetAliasInfoAsync();

			Assert.NotNull(aliasInfo);
			Assert.NotEmpty(aliasInfo.GameName);
			Assert.NotEmpty(aliasInfo.TagLine);
		}

		[Fact]
		public async Task GetEntitlementTest()
		{
			if (!Client.MachineReady()) return;
			EntitlementTokens? entitlements = await Initiator.Endpoints.LocalEndpoints.GetEntitlementTokensAsync();

			Assert.NotNull(entitlements);
			Assert.NotEmpty(entitlements.AccessToken);			
		}

		[Fact]
		public async Task GetLocalChatTest()
		{
			if (!Client.MachineReady()) return;
			LocalChatSession? localChatSession = await Initiator.Endpoints.LocalEndpoints.GetLocalChatSessionAsync();

			Assert.NotNull(localChatSession);
			Assert.NotEmpty(localChatSession.GameName);
		}

		[Fact]
		public async Task GetLocalFriendsTest()
		{
			if (!Client.MachineReady()) return;
			InternalFriends? localFriends = await Initiator.Endpoints.LocalEndpoints.GetLocalFriendsAsync();

			Assert.NotNull(localFriends);
			Assert.NotEmpty(localFriends.Friends);
			Assert.NotEmpty(localFriends.Friends[0].Puuid);
		}

		[Fact]
		public async Task GetFriendsTest()
		{
			if (!Client.MachineReady()) return;
			FriendPresences? localFriends = await Initiator.Endpoints.LocalEndpoints.GetFriendsPresencesAsync();

			Assert.NotNull(localFriends);
			Assert.NotEmpty(localFriends.Presences);
		}
	}
}
