// Major credit to: https://github.com/molenzwiebel/Deceive/blob/master/Deceive/ConfigProxy.cs
// It's pretty much ConfigProxy.cs but rewritten to my "standards" 

using System.Text.Json.Nodes;
using System.Net.Sockets;
using RadiantConnect.XMPP;


// ReSharper disable CheckNamespace

namespace RadiantConnect.SocketServices.XMPP.XMPPManagement
{
	internal class ChatServerEventArgs : EventArgs
	{
		internal string ChatHost { get; set; } = null!;
		internal int ChatPort { get; set; }
		internal string? ChatAffinity { get; set; }
	}

	internal class InternalProxy
	{
		internal event EventHandler<ChatServerEventArgs>? OnChatPatched;
		internal event ValXMPP.InternalMessage? OnOutboundMessage;
		internal event ValXMPP.InternalMessage? OnInboundMessage;


		private readonly HttpListener _proxyServer = new();
		private HttpClient Client { get; } = new();

		private const string ConfigUrl = "https://clientconfig.rpg.riotgames.com";
		private const string GeoPasUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat";

		internal int ConfigPort { get; }
		internal int ChatPort { get; }

		internal InternalProxy(int chatPort)
		{
			(TcpListener currentTcpListener, int currentPort) = ValXMPP.NewTcpListener();
			ChatPort = chatPort;
			ConfigPort = currentPort;
			currentTcpListener.Stop();

			_proxyServer.Prefixes.Add($"http://127.0.0.1:{ConfigPort}/");
			_proxyServer.Start();
			_proxyServer.BeginGetContext(DoProxy, _proxyServer);
		}

		private async void DoProxy(IAsyncResult result)
		{
			try
			{
				HttpListener listener = (HttpListener)result.AsyncState!;
				HttpListenerContext listenerContext = listener.EndGetContext(result);
				HttpListenerRequest listenerRequest = listenerContext.Request;
				HttpListenerResponse listenerResponse = listenerContext.Response;

				string rawUrl = ConfigUrl + listenerRequest.RawUrl;

				HttpRequestMessage message = new(HttpMethod.Get, rawUrl);
				message.Headers.TryAddWithoutValidation("User-Agent", listenerRequest.UserAgent);

				if (listenerRequest.Headers["x-riot-entitlements-jwt"] is not null)
					message.Headers.TryAddWithoutValidation("X-Riot-Entitlements-JWT", listenerRequest.Headers["x-riot-entitlements-jwt"]);

				if (listenerRequest.Headers["authorization"] is not null)
					message.Headers.TryAddWithoutValidation("Authorization", listenerRequest.Headers["authorization"]);

				OnOutboundMessage?.Invoke($"Url:{rawUrl}\nHeaders:\n{message.Headers}");

				HttpResponseMessage responseMessage = await Client.SendAsync(message).ConfigureAwait(false);
				string responseString = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);

				OnInboundMessage?.Invoke($"Url:{rawUrl}\nHeaders:\n{responseMessage.Headers}\nResponse:{responseString}");

				if (!responseMessage.IsSuccessStatusCode)
					goto DoResponse;
				try
				{
					JsonNode? riotClientConfig = JsonSerializer.Deserialize<JsonNode>(responseString);
					string? riotChatHost = null;
					int riotChatPort = 0;

					if (riotClientConfig?["chat.affinities"] is null) goto DoResponse;
					if (!(riotClientConfig["chat.affinity.enabled"]?.GetValue<bool>() ?? false)) goto DoResponse;

					if (riotClientConfig["chat.host"] is not null)
					{
						riotChatHost = riotClientConfig["chat.host"]!.GetValue<string>();
						riotClientConfig["chat.host"] = "127.0.0.1";
					}

					if (riotClientConfig["chat.port"] is not null)
					{
						riotChatPort = riotClientConfig["chat.port"]!.GetValue<int>();
						riotClientConfig["chat.port"] = ChatPort;
					}

					JsonNode? affinities = riotClientConfig["chat.affinities"];

					HttpRequestMessage pasRequest = new(HttpMethod.Get, GeoPasUrl);
					pasRequest.Headers.TryAddWithoutValidation("Authorization", listenerRequest.Headers["authorization"]);


					string? affinity = string.Empty;
					try
					{
						string pasJwt = await (await Client.SendAsync(pasRequest).ConfigureAwait(false)).Content.ReadAsStringAsync().ConfigureAwait(false);
						string pasJwtContent = pasJwt.Split('.')[1];
						string validBase64 = pasJwtContent.PadRight(pasJwtContent.Length / 4 * 4 + (pasJwtContent.Length % 4 == 0 ? 0 : 4), '=');
						string pasJwtString = validBase64.FromBase64();
						JsonNode? pasJwtJson = JsonSerializer.Deserialize<JsonNode>(pasJwtString);
						affinity = pasJwtJson?["affinity"]?.GetValue<string>();

						if (affinity is null) return;

						riotChatHost = affinities?[affinity]?.GetValue<string>();
					}
					catch {/**/}

					affinities?.AsObject().Select(pair => pair.Key).ToList().ForEach(s => affinities[s] = "127.0.0.1");

					if (riotClientConfig["chat.allow_bad_cert.enabled"] is not null)
						riotClientConfig["chat.allow_bad_cert.enabled"] = true;

					if (riotChatHost is not null && ChatPort != 0)
						OnChatPatched?.Invoke(this, new ChatServerEventArgs { ChatHost = riotChatHost, ChatPort = riotChatPort, ChatAffinity = affinity });

					responseString = JsonSerializer.Serialize(riotClientConfig);
				}
				catch {/**/}

				DoResponse:
				byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
				listenerResponse.StatusCode = (int)responseMessage.StatusCode;
				listenerResponse.SendChunked = false;
				listenerResponse.ContentLength64 = responseBytes.Length;
				listenerResponse.ContentType = "application/json";
				try
				{
					await listenerResponse.OutputStream
						.WriteAsync(responseBytes).ConfigureAwait(false); // The specified network name is no longer available.
				}
				catch {/**/}

				_proxyServer.BeginGetContext(DoProxy, _proxyServer);
				message.Dispose();
				responseMessage.Dispose();
				listenerResponse.Close();
			}
			catch {/**/} // Just ignore output
		}
	}
}
