using RadiantConnect.Services;
using RadiantConnect.SocketServices.XMPP.DataTypes;
using RadiantConnect.SocketServices.XMPP.XMPPManagement;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
#pragma warning disable CA2000
#pragma warning disable CA1849
#pragma warning disable CA5359
#pragma warning disable CA1031
#pragma warning disable SYSLIB0057

// Credit to https://github.com/molenzwiebel/Deceive for guide
// ReSharper disable CheckNamespace

namespace RadiantConnect.XMPP
{
	/// <summary>
	/// Represents a VAL-specific XMPP client responsible for intercepting,
	/// proxying, and handling Valorant XMPP communication.
	/// </summary>
	/// <remarks>
	/// This class manages lifecycle coordination between the Riot Client,
	/// a local proxy server, and the XMPP stream. It supports presence updates,
	/// internal messaging, and connection readiness notifications.
	/// </remarks>
	public partial class ValXMPP : IDisposable
	{
		internal delegate void SocketHandled(XMPPSocketHandle handle);
		internal XMPPSocketHandle? Handle { get; private set; }
		internal event SocketHandled? OnSocketCreated;

		internal X509Certificate2? Certificate;
		private InternalProxy? _proxyServer;
		private Process? _valorantProcess;

		private readonly CancellationTokenSource? _cancellationTokenSource = new();

		/// <summary>
		/// Gets the collection of client intercept delegates to be invoked during client operations.
		/// </summary>
		/// <remarks>Each delegate in the collection is called with an <see cref="InterceptContext"/> and can perform
		/// asynchronous processing. The order of invocation matches the order of delegates in the list.</remarks>
		public IReadOnlyList<Func<InterceptContext, Task>> ClientIntercepts { get; set; } = [];

		/// <summary>
		/// Gets or sets the collection of server-side interceptors to be invoked during request processing.
		/// </summary>
		/// <remarks>Each interceptor is represented as a delegate that receives an <see cref="InterceptContext"/> and
		/// returns a <see cref="Task"/>. Interceptors are executed in the order they appear in the list. Modifying this
		/// collection affects the set of interceptors applied to subsequent requests.</remarks>
		public IReadOnlyList<Func<InterceptContext, Task>> ServerIntercepts { get; set; } = [];
		
		/// <summary>
		/// Raised when the XMPP connection has fully initialized and is ready
		/// for communication.
		/// </summary>
		public event Action? OnReady;

		/// <summary>
		/// Gets a value indicating whether the XMPP connection is fully ready.
		/// </summary>
		/// <remarks>
		/// Transitioning from <c>false</c> to <c>true</c> will automatically
		/// raise the <see cref="OnReady"/> event.
		/// </remarks>
		public bool Ready
		{
			get;
			private set
			{
				if (!field && value)
					OnReady?.Invoke();
				field = value;
			}
		}

		internal string? StreamUrl { get; set; }
		
		/// <summary>
		/// Delegate invoked when an internal XMPP XML message is received.
		/// </summary>
		/// <param name="data">
		/// The raw XML message payload.
		/// </param>
		public delegate void InternalMessage(string data);

		/// <summary>
		/// Delegate invoked when the local player's Valorant presence is updated.
		/// </summary>
		/// <param name="presence">
		/// The updated presence information.
		/// </param>
		public delegate void PresenceUpdated(ValorantPresence presence);

		/// <summary>
		/// Delegate invoked when another player's presence is updated.
		/// </summary>
		/// <param name="presence">
		/// The updated player presence information.
		/// </param>
		public delegate void PlayerPresenceUpdated(PlayerPresence presence);

		/// <summary>
		/// Raised when an internal client-side XMPP message is received.
		/// </summary>
		public event InternalMessage? OnClientMessage;

		/// <summary>
		/// Raised when an internal server-side XMPP message is received.
		/// </summary>
		public event InternalMessage? OnServerMessage;

		/// <summary>
		/// Raised when the local Valorant presence information is updated.
		/// </summary>
		public event PresenceUpdated? OnValorantPresenceUpdated;

		/// <summary>
		/// Raised when another player's presence information is updated.
		/// </summary>
		public event PlayerPresenceUpdated? OnPlayerPresenceUpdated;

		/// <summary>
		/// Raised when an outbound XMPP message is intercepted by the proxy.
		/// </summary>
		public event InternalMessage? OnOutboundMessage;

		/// <summary>
		/// Raised when an inbound XMPP message is intercepted by the proxy.
		/// </summary>
		public event InternalMessage? OnInboundMessage;
		
		/// <summary>
		/// Disposes the socket connection and disconnects from the MITM XMPP server.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Disposes the socket connection and disconnects from the MITM XMPP server.
		/// </summary>
		protected virtual void Dispose(bool disposing)
		{
			if (!disposing) return;
			try { _valorantProcess?.Kill(); } catch { /**/ }
			_cancellationTokenSource?.Cancel();
			Handle?.Dispose();
			_proxyServer?.Dispose();
			Certificate?.Dispose();
			_cancellationTokenSource?.Dispose();
			_valorantProcess?.Dispose();
		}

		/// <summary>
		/// Terminates all running Riot and Valorant-related processes.
		/// </summary>
		/// <remarks>
		/// This method forcefully kills the Riot Client, Riot Client Services,
		/// and Valorant game processes if they are running.
		/// </remarks>
		public static void KillRiot()
		{
			Process[] processes = Process.GetProcesses();
			foreach (Process process in processes.Where(proc => proc.ProcessName is "Riot Client" or "VALORANT-Win64-Shipping" or "RiotClientServices")) process.Kill();
		}

		/// <summary>
		/// Determines whether any Riot or Valorant processes are currently running.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the Riot Client or Valorant process is running;
		/// otherwise, <c>false</c>.
		/// </returns>
		public static bool IsRiotRunning() => InternalValorantMethods.IsRiotClientRunning() || InternalValorantMethods.IsValorantProcessRunning();

		internal static (TcpListener, int) NewTcpListener()
		{
			TcpListener listener = new(IPAddress.Loopback, 0);
			listener.Start();
			return (listener, ((IPEndPoint)listener.LocalEndpoint).Port);
		}
		
		internal static ValorantPresence? HandlePresenceObject(string data, Action<ValorantPresence>? presenceAction = null)
		{
			// Pull the <presence> XML out of the stream.
			// I do regex in case an error with the stream includes extra data
			Match match = ValorantPresenceRegex().Match(data);
			if (!match.Success) return null;

			string valorant64Data = match.Groups[1].Value;
			ValorantPresence? presenceData = JsonSerializer.Deserialize<ValorantPresence>(valorant64Data.FromBase64());

			if (presenceData is not null)
				presenceAction?.Invoke(presenceData);

			return presenceData;
		}

		internal static void HandlePlayerPresence(string data, Action<PlayerPresence>? action = null)
		{
			if (!data.Contains("<item jid=", StringComparison.Ordinal)) return;

			string[] presences = data.Split("<presence to=");

			foreach (string presence in presences)
			{
				if (string.IsNullOrEmpty(presence)) continue;
				if (!presence.Contains("tagline=", StringComparison.Ordinal)) continue;

				Match presenceMatch = XmlPresenceUpdateRegex().Match($"<presence to={presence}");
				if (!presenceMatch.Success) return;
				string newData = presenceMatch.Value;

				Dictionary<string, string> platforms = [];
				(string chatServer, string lobbyServer, string riotId, string tagLine) = ("", "", "", "");

				Match riotData = RiotDataRegex().Match(newData);
				Match platformsData = PlatformsDataRegex().Match(newData);
				Match chatServerData = ChatServerDataRegex().Match(newData);
				Match lobbyServerData = LobbyServerDataRegex().Match(newData);
				
				if (chatServerData.Success)
					chatServer = chatServerData.Groups[1].Value;
				if (lobbyServerData.Success)
					lobbyServer = lobbyServerData.Groups[1].Value;

				if (riotData.Success)
				{
					riotId = riotData.Groups[1].Value;
					tagLine = riotData.Groups[2].Value;
				}

				while (platformsData.Success)
				{
					platforms.Add(platformsData.Groups[1].Value, platformsData.Groups[2].Value);
					platformsData = platformsData.NextMatch();
				}

				action?.Invoke(new PlayerPresence(
					chatServer,
					lobbyServer,
					"riot",
					riotId,
					tagLine,
					lobbyServer[(lobbyServer.IndexOf('/', StringComparison.Ordinal) + 1)..],
					platforms,
					HandlePresenceObject(newData)!
				));
			}
		}

		internal void HandleValorantPresence(string data)
		{
			try
			{
				if (OnValorantPresenceUpdated is not null)
					HandlePresenceObject(data, presenceData => OnValorantPresenceUpdated?.Invoke(presenceData));
				if (OnPlayerPresenceUpdated is not null)
					HandlePlayerPresence(data, presenceData => OnPlayerPresenceUpdated?.Invoke(presenceData));
			}
			catch{/**/}
		}
		
		// ReSharper stinks.
		[SuppressMessage("ReSharper", "RemoveRedundantBraces")]
		[SuppressMessage("ReSharper", "MethodHasAsyncOverload")]
		[SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
		internal async Task HandleClients(TcpListener server, string chatHost, int chatPort)
		{
			if (Certificate is null)
				throw new RadiantConnectXMPPException("Certificate is null in handleClients.");

			while (_cancellationTokenSource is not null && !_cancellationTokenSource.IsCancellationRequested)
			{
				TcpClient? incomingClient = null!;
				SslStream? incomingStream = null!;
				TcpClient? outgoingClient = null!;
				SslStream? outgoingStream = null!;
				try
				{

					incomingClient = await server.AcceptTcpClientAsync(_cancellationTokenSource.Token)
						.ConfigureAwait(false);
					incomingStream = new SslStream(incomingClient.GetStream());
					await incomingStream.AuthenticateAsServerAsync(Certificate).ConfigureAwait(false);


					while (true)
					{
						try
						{
							outgoingClient = new TcpClient(chatHost, chatPort);
							break;
						}
						catch (Exception ex)
						{
							throw new RadiantConnectXMPPException($"Unable to communicate with chat client. {ex}");
						}
					}

					outgoingStream = new SslStream(outgoingClient.GetStream());
					await outgoingStream.AuthenticateAsClientAsync(chatHost).ConfigureAwait(false);

					XMPPSocketHandle handler = new(incomingStream, outgoingStream, ClientIntercepts, ServerIntercepts);
					OnSocketCreated?.Invoke(handler);
					Handle = handler;
					handler.OnClientMessage += (data) => OnClientMessage?.Invoke(data);
					handler.OnServerMessage += (data) =>
					{
						OnServerMessage?.Invoke(data);

						if (!data.Contains("<valorant>", StringComparison.Ordinal)) return;

						HandleValorantPresence(data);
					};
					handler.Initiate();
					Ready = true;
				}
				catch (Exception ex)
				{
					outgoingStream?.Dispose();
					incomingStream?.Dispose();
					incomingClient?.Dispose();
					outgoingClient?.Dispose();
					Debug.WriteLine($"[ValXMPP Log] Transient client handling error, continuing to listen. {ex}");
				}
			}
		}

		/// <summary>
		/// Initializes the Valorant XMPP connection by launching the Riot Client
		/// with a local proxy configuration.
		/// </summary>
		/// <param name="patchLine">
		/// The Valorant patch line to launch (default is <c>live</c>).
		/// </param>
		/// <returns>
		/// The started Riot Client process instance.
		/// </returns>
		/// <exception cref="RadiantConnectXMPPException">
		/// Thrown if Riot or Valorant is already running, required executables
		/// cannot be found, or the Riot Client fails to start.
		/// </exception>
		/// <remarks>
		/// This method starts a local proxy server, intercepts XMPP traffic,
		/// determines the chat server affinity, and establishes the XMPP stream.
		/// </remarks>
		public Process InitializeConnection(string patchLine = "live")
		{
			string riotClientPath = RiotPathService.GetRiotClientPath();
			string valorantPath = RiotPathService.GetValorantPath();
			if (IsRiotRunning()) throw new RadiantConnectXMPPException("Riot/Valorant cannot be running.");
			if (!File.Exists(riotClientPath)) throw new RadiantConnectXMPPException($"Riot Client executable not found: {riotClientPath}");
			if (!File.Exists(valorantPath)) throw new RadiantConnectXMPPException($"Valorant executable not found: {valorantPath}");

			(TcpListener currentTcpListener, int currentPort) = NewTcpListener();

			_proxyServer = new InternalProxy(currentPort, this);

			bool serverHooked = false;

			_proxyServer.OnChatPatched += async (_, args) =>
			{
				if (serverHooked) return;
				serverHooked = true;
				StreamUrl = args.ChatAffinity;
				await HandleClients(currentTcpListener, args.ChatHost, args.ChatPort).ConfigureAwait(false);
			};

			_proxyServer.OnOutboundMessage += data => OnOutboundMessage?.Invoke(data);
			_proxyServer.OnInboundMessage += data => OnInboundMessage?.Invoke(data);

			ProcessStartInfo riotClientStartArgs = new()
			{
				FileName = riotClientPath,
				Arguments = $"--client-config-url=\"http://{InternalProxy.LocalHostUrl}:{_proxyServer.ConfigPort}\" --launch-product=valorant --launch-patchline={patchLine}"
			};

			_valorantProcess = Process.Start(riotClientStartArgs);

			if (_valorantProcess is null)
				throw new RadiantConnectXMPPException("Failed to start Riot Client process.");

			_valorantProcess.EnableRaisingEvents = true;
			_valorantProcess.Exited += (_, _) => Dispose();

			return _valorantProcess;
		}
		
		[GeneratedRegex("<p>(.*?)<\\/p>")]
		private static partial Regex ValorantPresenceRegex();

		[GeneratedRegex("(<presence.*<\\/presence>)")]
		private static partial Regex XmlPresenceUpdateRegex();

		[GeneratedRegex("<id name='([^']*)' tagline='(.{0,6})'\\/><(p|l|\\/item)")]
		private static partial Regex RiotDataRegex();

		[GeneratedRegex("<riot name='([^']*)' tagline='(.{0,6})'\\/>")]
		private static partial Regex PlatformsDataRegex();

		[GeneratedRegex("to='(.*)' from")]
		private static partial Regex ChatServerDataRegex();

		[GeneratedRegex("from='(.*)' id")]
		private static partial Regex LobbyServerDataRegex();
	}
}
