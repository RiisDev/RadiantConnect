using RadiantConnect.Authentication.DriverRiotAuth.Records;
using RadiantConnect.EventHandler;
using RadiantConnect.Network;
using RadiantConnect.Network.ChatEndpoints;
using RadiantConnect.Network.ContractEndpoints;
using RadiantConnect.Network.CurrentGameEndpoints;
using RadiantConnect.Network.LocalEndpoints;
using RadiantConnect.Network.PartyEndpoints;
using RadiantConnect.Network.PreGameEndpoints;
using RadiantConnect.Network.PVPEndpoints;
using RadiantConnect.Network.StoreEndpoints;
using RadiantConnect.Services;
using RadiantConnect.SocketServices.InternalTcp;

namespace RadiantConnect
{
	/// <summary>
	/// Represents a collection of core internal services required
	/// for interacting with the Valorant client and its subsystems.
	/// </summary>
	/// <param name="ValorantClient">
	/// The primary service interface for interacting with the Valorant client.
	/// </param>
	/// <param name="Net">
	/// The networking layer used for HTTP and service communication.
	/// </param>
	/// <param name="LogService">
	/// The log service used to read and monitor Valorant client logs.
	/// </param>
	/// <param name="ClientData">
	/// Parsed client-specific metadata derived from log files.
	/// </param>
	public record InternalSystem(
		ValorantService ValorantClient,
		ValorantNet Net,
		LogService LogService,
		LogService.ClientData ClientData
	);

	/// <summary>
	/// Represents a grouped collection of API endpoint sets used
	/// to interact with various Valorant services.
	/// </summary>
	/// <param name="ChatEndpoints">
	/// Endpoints related to chat and messaging services.
	/// </param>
	/// <param name="ContractEndpoints">
	/// Endpoints related to player contracts and progression.
	/// </param>
	/// <param name="CurrentGameEndpoints">
	/// Endpoints related to the player's current game state.
	/// </param>
	/// <param name="LocalEndpoints">
	/// Endpoints exposed by the local Valorant client.
	/// </param>
	/// <param name="PartyEndpoints">
	/// Endpoints related to party and social features.
	/// </param>
	/// <param name="PreGameEndpoints">
	/// Endpoints related to pre-game configuration and agent selection.
	/// </param>
	/// <param name="PvpEndpoints">
	/// Endpoints related to player-vs-player matchmaking and games.
	/// </param>
	/// <param name="StoreEndpoints">
	/// Endpoints related to in-game store and purchases.
	/// </param>
	public record Endpoints(
		ChatEndpoints ChatEndpoints,
		ContractEndpoints ContractEndpoints,
		CurrentGameEndpoints CurrentGameEndpoints,
		LocalEndpoints LocalEndpoints,
		PartyEndpoints PartyEndpoints,
		PreGameEndpoints PreGameEndpoints,
		PVPEndpoints PvpEndpoints,
		StoreEndpoints StoreEndpoints
	);

	internal record GeoAffinities(
		[property: JsonPropertyName("pbe")] string Pbe,
		[property: JsonPropertyName("live")] string Live
	);
	internal record GeoRoot(
		[property: JsonPropertyName("token")] string Token,
		[property: JsonPropertyName("affinities")] GeoAffinities Affinities
	);

	/// <summary>
	/// Coordinates initialization of core systems, services, endpoints,
	/// and event pipelines required to interact with the Valorant client.
	/// </summary>
	/// <remarks>
	/// This class acts as the primary bootstrapper for the application
	/// and is responsible for lifecycle management and resource cleanup.
	/// </remarks>
	public class Initiator : IDisposable
	{
		private bool _disposed;

		private static string IsVpnDetected() => string.Join('|',
			Process.GetProcesses().Where(process =>
				process.ProcessName.Contains("vpn", StringComparison.CurrentCultureIgnoreCase)));

		/// <summary>
		/// Gets the initialized internal system services.
		/// </summary>
		public InternalSystem ExternalSystem { get; private set; } = null!;

		/// <summary>
		/// Returns the endpoint collections for various services
		/// </summary>
		public Endpoints Endpoints { get; private set; } = null!;

		/// <summary>
		/// Gets the game-related event dispatcher.
		/// </summary>
		public GameEvents GameEvents { get; internal set; } = null!;

		/// <summary>
		/// Gets the TCP-related event dispatcher.
		/// </summary>
		public TcpEvents TcpEvents { get; internal set; } = null!;

		/// <summary>
		/// Gets the parsed Valorant client data.
		/// </summary>
		public LogService.ClientData Client { get; private set; } = null!;

		private static async Task<LogService.ClientData> BuildClientData(ValorantNet net, RSOAuth rsoAuth)
		{
			if (net == null) throw new RadiantConnectException("Failed to build net data");
			using StringContent content = new ($"{{\"id_token\": \"{rsoAuth.IdToken}\"}}");
			GeoRoot? data = await net.PutAsync<GeoRoot>(
				"https://riot-geo.pas.si.riotgames.com",
				"/pas/v1/product/valorant",
				content
			).ConfigureAwait(false);

			Enum.TryParse(data?.Affinities.Live, true, out LogService.ClientData.ShardType shard);

			JsonWebToken token = new(data?.Token);

			return new LogService.ClientData(
				Shard: shard,
				UserId: token.Subject,
				PdUrl: $"https://pd.{shard}.a.pvp.net",
				GlzUrl: $"https://glz-{data?.Affinities.Live}-1.{shard}.a.pvp.net",
				SharedUrl: $"https://shared.{shard}.a.pvp.net"
			);
		}

		private void Initialize(ValorantNet net, RSOAuth rsoAuth)
		{
			Client = BuildClientData(net, rsoAuth).Result;

			ExternalSystem = new InternalSystem(
				null!,
				net,
				null!,
				Client
			);

			Endpoints = new Endpoints(
				new ChatEndpoints(this),
				new ContractEndpoints(this),
				new CurrentGameEndpoints(this),
				null!,
				new PartyEndpoints(this),
				new PreGameEndpoints(this),
				new PVPEndpoints(this),
				new StoreEndpoints(this)
			);
		}

		/// <summary>
		/// Initializes the system using an existing Riot OAuth session.
		/// </summary>
		/// <param name="rsoAuth">
		/// An authenticated Riot Sign-On (RSO) context containing access credentials.
		/// </param>
		/// <remarks>
		/// This constructor is intended for scenarios where authentication
		/// has already been performed externally.
		/// </remarks>
		public Initiator(RSOAuth rsoAuth)
		{
			ValorantNet net = new(rsoAuth);
			Initialize(net, rsoAuth);
		}

		/// <summary>
		/// Initializes the system by attaching to a running Valorant client instance.
		/// </summary>
		/// <param name="ignoreVpn">
		/// If set to <c>true</c>, VPN detection is skipped.  
		/// If <c>false</c>, initialization will fail if VPN processes are detected.
		/// </param>
		/// <exception cref="TimeoutException">
		/// Thrown when the Valorant client does not become ready within one minute.
		/// </exception>
		/// <exception cref="RadiantConnectException">
		/// Thrown when VPN usage is detected and <paramref name="ignoreVpn"/> is <c>false</c>.
		/// </exception>
		/// <remarks>
		/// This constructor waits for the Valorant client to fully initialize,
		/// reads client configuration from logs, and hooks into live services.
		/// </remarks>
		public Initiator(bool ignoreVpn = true)
		{
			DateTime startTime = DateTime.Now;
			TimeSpan timeout = TimeSpan.FromMinutes(1);

			while (!InternalValorantMethods.ClientIsReady())
			{
				if (DateTime.Now - startTime > timeout)
					throw new TimeoutException("Client did not become ready within 1 minute.");

				Task.Delay(2000);
			}

			ValorantService client = new ();

#pragma warning disable CA2000 // LogService is disposed in InternalSystem
			LogService logService = new();
#pragma warning restore CA2000
			LogService.ClientData cData = LogService.GetClientData();
			ValorantNet net = new(client);

			if (!ignoreVpn)
			{
				string vpnDetected = IsVpnDetected();

				if (!vpnDetected.IsNullOrEmpty())
					throw new RadiantConnectException($"Can not run with VPN running, found processes: {vpnDetected}. \n\nTo bypass this check launch Initiator with (true)");
			}

			ExternalSystem = new InternalSystem(
				client,
				net,
				logService,
				cData
			);

			Client = cData;

			Endpoints = new Endpoints(
				new ChatEndpoints(this),
				new ContractEndpoints(this),
				new CurrentGameEndpoints(this),
				new LocalEndpoints(this),
				new PartyEndpoints(this),
				new PreGameEndpoints(this),
				new PVPEndpoints(this),
				new StoreEndpoints(this)
			);

			TcpEvents = new TcpEvents(this, new ValSocket(this, false), true);
			_ = logService.InitiateEvents(this);
		}

		 /// <summary>
		 /// Initializes the system using the Riot Client for authentication,
		 /// without requiring the Valorant game process to be running.
		 /// </summary>
		 /// <param name="riotClient">
		 /// Must be set to <c>true</c>.  
		 /// This parameter exists solely to disambiguate the constructor signature.
		 /// </param>
		 /// <param name="shard">
		 /// The regional shard used to construct service endpoints.
		 /// </param>
		 /// <param name="ignoreVpn">
		 /// If set to <c>true</c>, VPN detection is skipped.
		 /// </param>
		 /// <exception cref="RadiantConnectException">
		 /// Thrown if the Riot Client is not running or VPN usage is detected.
		 /// </exception>
		 /// <exception cref="ArgumentException">
		 /// Thrown if <paramref name="riotClient"/> is <c>false</c>.
		 /// </exception>
		 /// <remarks>
		 /// This constructor is useful for lightweight authentication-only scenarios
		 /// or tooling that does not require full client attachment.
		 /// </remarks>
		public Initiator(bool riotClient, LogService.ClientData.ShardType shard, bool ignoreVpn = true)
		{
			if (!InternalValorantMethods.IsRiotClientRunning())
				throw new RadiantConnectException("Riot Client must be open and running.");

			if (!riotClient)
				throw new ArgumentException("This constructor is only to be used when using the Riot Client for authentication.");
			
			ValorantService client = new();
			ValorantNet net = new(client);

			(string accessToken, string _) = net.GetAuthorizationToken().Result;

			LogService.ClientData cData = new(
				Shard: shard,
				UserId: new JsonWebToken(accessToken).Subject,
				PdUrl: $"https://pd.{shard}.a.pvp.net",
				GlzUrl: $"https://glz-{shard}-1.{shard}.a.pvp.net",
				SharedUrl: $"https://shared.{shard}.a.pvp.net"
			);
			
			if (!ignoreVpn)
			{
				string vpnDetected = IsVpnDetected();

				if (!vpnDetected.IsNullOrEmpty())
					throw new RadiantConnectException($"Can not run with VPN running, found processes: {vpnDetected}. \n\nTo bypass this check launch Initiator with (true)");
			}

			ExternalSystem = new InternalSystem(
				client,
				net,
				null!,
				cData
			);

			Client = cData;

			Endpoints = new Endpoints(
				new ChatEndpoints(this),
				new ContractEndpoints(this),
				new CurrentGameEndpoints(this),
				new LocalEndpoints(this),
				new PartyEndpoints(this),
				new PreGameEndpoints(this),
				new PVPEndpoints(this),
				new StoreEndpoints(this)
			);
		}

		/// <summary>
		/// Releases all managed resources held by the initiator.
		/// </summary>
		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs the actual resource cleanup.
		/// </summary>
		/// <param name="disposing">
		/// Indicates whether the method was called from <see cref="Dispose()"/>.
		/// </param>
		protected virtual void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				try { ExternalSystem.LogService.Dispose(); } catch { /**/ }
				try { TcpEvents.Dispose(); } catch { /**/ }
			}

			ExternalSystem = null!;
			Endpoints = null!;
			GameEvents = null!;
			TcpEvents = null!;
			Client = null!;

			_disposed = true;
		}
	}
}
