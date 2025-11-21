using System.Net.Security;
using System.Net.Sockets;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using IOException = System.IO.IOException;

namespace RadiantConnect.SocketServices.XMPP
{
	public class RemoteXMPP : IDisposable
	{
		public enum XMPPStatus
		{
			Connecting,
			InitiatingSslConnection,
			InitiatingSocketStream,
			SendingAuthorizationTokens,
			ReceivingFeatures,
			BindingStream,
			BindingSession,
			BindingEntitlement,
			Connected
		}
		#region XMPPRegions
		internal readonly Dictionary<string, string> ChatUrls = new() {
			{"asia", "jp1.chat.si.riotgames.com"},
			{"br1", "br.chat.si.riotgames.com"},
			{"eu", "euw1.chat.si.riotgames.com"},
			{"eun1", "eun1.chat.si.riotgames.com"},
			{"euw1", "euw1.chat.si.riotgames.com"},
			{"jp1", "jp1.chat.si.riotgames.com"},
			{"kr1", "kr1.chat.si.riotgames.com"},
			{"la1", "la1.chat.si.riotgames.com"},
			{"la2", "la2.chat.si.riotgames.com"},
			{"na1", "na2.chat.si.riotgames.com"},
			{"oc1", "kr1.chat.si.riotgames.com"},
			{"pbe1", "pbe1.chat.si.riotgames.com"},
			{"ru1", "euw1.chat.si.riotgames.com"},
			{"sea1", "sa1.chat.si.riotgames.com"},
			{"sea2", "sa2.chat.si.riotgames.com"},
			{"sea3", "sa3.chat.si.riotgames.com"},
			{"tr1", "euw1.chat.si.riotgames.com"},
			{"us", "la1.chat.si.riotgames.com"},
			{"us-br1", "br.chat.si.riotgames.com"},
			{"us-la2", "la2.chat.si.riotgames.com"}
		};

		internal readonly Dictionary<string, string?> ChatAffinity = new()
		{
			{ "asia", "jp1" },
			{ "br1", "br1" },
			{ "eu", "eu1" },
			{ "eun1", "eu2" },
			{ "euw1", "eu1" },
			{ "jp1", "jp1" },
			{ "kr1", "kr1" },
			{ "la1", "la1" },
			{ "la2", "la2" },
			{ "na1", "na1" },
			{ "oc1", "kr1" },
			{ "pbe1", "pb1" },
			{ "ru1", "eu1" },
			{ "sea1", "sa1" },
			{ "sea2", "sa2" },
			{ "sea3", "sa3" },
			{ "tr1", "eu1" },
			{ "us", "la1" },
			{ "us-br1", "br1" },
			{ "us-la2", "la2" }
		};
		#endregion
		public delegate void XMPPMessage(string data);
		public event XMPPMessage? OnMessage;

		public delegate void XMPPProgress(XMPPStatus status);
		public event XMPPProgress? OnXMPPProgress;

		internal SslStream SslStream = null!;

		internal XMPPStatus InternalStatus;

		internal string XmppBind { get; set; } = null!;

		internal RSOAuth AuthData { get; private set; } = null!;

		public XMPPStatus Status
		{
			get => InternalStatus;
			set
			{
				InternalStatus = value;
				OnXMPPProgress?.Invoke(InternalStatus);
			}
		}

		internal async Task AsyncSocketWrite(SslStream sslStream, string message)
		{
			try
			{

				byte[] buffer = Encoding.UTF8.GetBytes(message);
				await sslStream.WriteAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
				await sslStream.FlushAsync().ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				if (ex.InnerException?.Message.Contains("closed by the remote host", StringComparison.Ordinal) ?? false)
					throw new RadiantConnectXMPPException(
						"An issue occurred while messaging XMPP server, is the network stable?");

				throw new RadiantConnectXMPPException(
					$"An unknown error occured while connecting to riot XMPP servers: {ex}");
			}
		}

		// Super hacky way of continuous reading from the stream,
		// but it works and I apologize for the hackyness.
		internal string AsyncSocketRead(SslStream sslStream)
		{
			int byteCount;
			byte[] bytes = new byte[1024];
			StringBuilder contentBuilder = new();

			do
			{
				try { byteCount = sslStream.Read(bytes, 0, bytes.Length); }
				catch (IOException) { break; } // Timeout Occurred, aka no data left to read

				if (byteCount > 0) contentBuilder.Append(Encoding.UTF8.GetString(bytes, 0, byteCount));
				Debug.WriteLine(contentBuilder.ToString());
			} while (byteCount > 0);
			
			if (contentBuilder.Length > 0 && Status == XMPPStatus.Connected)
			{
				OnMessage?.Invoke(contentBuilder.ToString());
			}

			return contentBuilder.ToString();
		}

		public async Task SendMessage([StringSyntax(StringSyntaxAttribute.Xml)] string message) => await AsyncSocketWrite(SslStream, message).ConfigureAwait(false);

		public async Task InitiateRemoteXMPP(RSOAuth auth)
		{
			try
			{
				AuthData = auth;

				Status = XMPPStatus.Connecting;

				string? pasToken = auth.PasToken;
				string desiredAffinity = new JsonWebToken(pasToken).GetRequiredPayloadValue<string>("affinity");
				string? affinityDomain = ChatAffinity[desiredAffinity];
				string? token = auth.AccessToken;
				string? entitlement = auth.Entitlement;
				string chatHost = ChatUrls[desiredAffinity];

				Status = XMPPStatus.InitiatingSslConnection;
				TcpClient tcpClient = new(chatHost, 5223);
				SslStream = new(tcpClient.GetStream(), true, (_, _, _, _) => true);
				await SslStream.AuthenticateAsClientAsync(chatHost).ConfigureAwait(false);

				SslStream.ReadTimeout = 500;

				Status = XMPPStatus.InitiatingSocketStream;
				await AsyncSocketWrite(SslStream,
					$"<?xml version=\"1.0\"?><stream:stream to=\"{affinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\">").ConfigureAwait(false);
				string incomingData;
				do
				{
					incomingData = AsyncSocketRead(SslStream);
				} while (!incomingData.Contains("X-Riot-RSO-PAS", StringComparison.Ordinal));

				Status = XMPPStatus.SendingAuthorizationTokens;
				await AsyncSocketWrite(SslStream, $"<auth mechanism=\"X-Riot-RSO-PAS\" xmlns=\"urn:ietf:params:xml:ns:xmpp-sasl\"><rso_token>{token}</rso_token><pas_token>{pasToken}</pas_token></auth>").ConfigureAwait(false);
				AsyncSocketRead(SslStream);

				Status = XMPPStatus.ReceivingFeatures;
				await AsyncSocketWrite(SslStream, $"<?xml version=\"1.0\"?><stream:stream to=\"{affinityDomain}.pvp.net\" version=\"1.0\" xmlns:stream=\"http://etherx.jabber.org/streams\">").ConfigureAwait(false);
				do
				{
					incomingData = AsyncSocketRead(SslStream);
				} while (!incomingData.Contains("stream:features", StringComparison.Ordinal));

				Status = XMPPStatus.BindingStream;
				await AsyncSocketWrite(SslStream,
					"<iq id=\"_xmpp_bind1\" type=\"set\"><bind xmlns=\"urn:ietf:params:xml:ns:xmpp-bind\"><puuid-mode enabled=\"true\"/></bind></iq>").ConfigureAwait(false);
				incomingData = AsyncSocketRead(SslStream);

				if (incomingData.Contains("jid", StringComparison.Ordinal))
				{
					XmppBind = incomingData.ExtractValue("<jid>(.*?)</jid>", 1);
				}

				Status = XMPPStatus.BindingSession;
				await AsyncSocketWrite(SslStream,
					"<iq id=\"_xmpp_session1\" type=\"set\"><session xmlns=\"urn:ietf:params:xml:ns:xmpp-session\"/></iq>").ConfigureAwait(false);
				AsyncSocketRead(SslStream);

				Status = XMPPStatus.BindingEntitlement;
				await AsyncSocketWrite(SslStream,
					$"<iq id=\"xmpp_entitlements_0\" type=\"set\"><entitlements xmlns=\"urn:riotgames:entitlements\"><token xmlns=\"\">{entitlement}</token></entitlements></iq>").ConfigureAwait(false);
				AsyncSocketRead(SslStream);

				Status = XMPPStatus.Connected;

				_ = Task.Run(() =>
				{
					while (tcpClient.Connected)
					{
						AsyncSocketRead(SslStream);
					}
				});

				_ = Task.Run(async () => // Keep the stream active
				{
					while (tcpClient.Connected)
					{
						await AsyncSocketWrite(SslStream, "").ConfigureAwait(false);
						await Task.Delay(150000).ConfigureAwait(false);
					}
				});
			}
			catch (Exception ex)
			{
				if (ex.InnerException?.Message.Contains("closed by the remote host", StringComparison.Ordinal) ?? false)
					throw new RadiantConnectXMPPException(
						$"An issue occurred while connecting to riot's XMPP server, is the network stable? STEP_{Status.ToString().ToUpperInvariant()}");

				throw new RadiantConnectXMPPException(
					$"An unknown error occured while connecting to riot XMPP servers STEP_{Status.ToString().ToUpperInvariant()}: {ex}");
			}
		}

		public void Dispose()
		{
			SslStream.Dispose();
			GC.SuppressFinalize(this);
		}

	}
}
