// Major credit to: https://github.com/molenzwiebel/Deceive/blob/master/Deceive/ConfigProxy.cs
// It's pretty much ConfigProxy.cs but rewritten to my coding standard 

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json.Nodes;
using System.Text.Json;

namespace RadiantConnect.XMPP
{
    internal class InternalProxy : IDisposable
    {
        internal HttpListener ProxyServer = new();
        internal HttpClient Client { get; } = new();
        internal readonly string ConfigUrl = "https://clientconfig.rpg.riotgames.com";
        internal readonly string GeoPasUrl = "https://riot-geo.pas.si.riotgames.com/pas/v1/service/chat";
        internal int ConfigPort { get; }
        internal int ChatPort { get; }

        internal InternalProxy(int chatPort)
        {
            (TcpListener currentTcpListener, int currentPort) = XMPP.NewTcpListener();
            ChatPort = chatPort;
            ConfigPort = currentPort;
            currentTcpListener.Stop();

            ProxyServer.Prefixes.Add($"http://*:{ConfigPort}/");
            ProxyServer.Start();
            ProxyServer.BeginGetContext(DoProxy, ProxyServer);
        }

        internal async void DoProxy(IAsyncResult result)
        {
            HttpListener? listener = (HttpListener)result.AsyncState!;
            HttpListenerContext listenerContext = listener.EndGetContext(result);
            HttpListenerRequest listenerRequest = listenerContext.Request;
            HttpListenerResponse listenerResponse = listenerContext.Response;

            string? rawUrl = listenerRequest.RawUrl;

            if (rawUrl is null) return;

            HttpRequestMessage message = new(HttpMethod.Get, rawUrl);
            message.Headers.TryAddWithoutValidation("User-Agent", listenerRequest.UserAgent);

            if (listenerRequest.Headers["x-riot-entitlements-jwt"] is not null)
                message.Headers.TryAddWithoutValidation("X-Riot-Entitlements-JWT", listenerRequest.Headers["x-riot-entitlements-jwt"]);

            if (listenerRequest.Headers["authorization"] is not null)
                message.Headers.TryAddWithoutValidation("Authorization", listenerRequest.Headers["authorization"]);

            HttpResponseMessage responseMessage = await Client.SendAsync(message);
            string responseString = await responseMessage.Content.ReadAsStringAsync();

            if (!responseMessage.IsSuccessStatusCode)
                goto DoResponse;

            try
            {
                JsonNode? riotClientConfig = JsonSerializer.Deserialize<JsonNode>(responseString);
                string? riotChatHost = null;

                if (riotClientConfig?["chat.host"] is not null)
                {
                    riotChatHost = riotClientConfig["chat.host"]!.GetValue<string>();
                    riotClientConfig["chat.host"] = "127.0.0.1";
                }

                if (riotClientConfig?["chat.port"] is not null)
                {
                    riotClientConfig["chat.port"]!.GetValue<int>();
                    riotClientConfig["chat.port"] = ChatPort;
                }

                if (riotClientConfig?["chat.affinities"] is null) goto DoResponse;

                JsonNode? affinities = riotClientConfig["chat.affinities"];
                if (riotClientConfig["chat.affinity.enabled"]?.GetValue<bool>() ?? false)
                {
                    HttpRequestMessage pasRequest = new(HttpMethod.Get, GeoPasUrl);
                    pasRequest.Headers.TryAddWithoutValidation("Authorization",
                        listenerRequest.Headers["authorization"]);

                    try
                    {
                        string pasJwt = await (await Client.SendAsync(pasRequest)).Content.ReadAsStringAsync();
                        Trace.WriteLine("PAS JWT:" + pasJwt);
                        string pasJwtContent = pasJwt.Split('.')[1];
                        string validBase64 = pasJwtContent.PadRight(
                            (pasJwtContent.Length / 4 * 4) + (pasJwtContent.Length % 4 == 0 ? 0 : 4), '=');
                        string pasJwtString = Encoding.UTF8.GetString(Convert.FromBase64String(validBase64));
                        JsonNode? pasJwtJson = JsonSerializer.Deserialize<JsonNode>(pasJwtString);
                        string? affinity = pasJwtJson?["affinity"]?.GetValue<string>();

                        // replace fallback host with host by player affinity
                        if (affinity is not null)
                        {
                            riotChatHost = affinities?[affinity]?.GetValue<string>();
                            Trace.WriteLine($"AFFINITY: {affinity} -> {riotChatHost}");
                        }
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Error getting player affinity token, using default chat server.");
                        Trace.WriteLine(e);
                    }
                }

                affinities?.AsObject().Select(pair => pair.Key).ToList().ForEach(s => affinities[s] = "127.0.0.1");

                // Allow an invalid cert.
                if (riotClientConfig?["chat.allow_bad_cert.enabled"] is not null)
                    riotClientConfig["chat.allow_bad_cert.enabled"] = true;

                //modifiedContent = JsonSerializer.Serialize(riotClientConfig);
               // Trace.WriteLine("MODIFIED CLIENTCONFIG: " + modifiedContent);

                //if (riotChatHost is not null && riotChatPort != 0)
                //    PatchedChatServer?.Invoke(this, new ChatServerEventArgs { ChatHost = riotChatHost, ChatPort = riotChatPort });
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

        DoResponse:
            byte[] responseBytes = Encoding.UTF8.GetBytes(responseString);
            listenerResponse.StatusCode = (int)responseMessage.StatusCode;
            listenerResponse.SendChunked = false;
            listenerResponse.ContentLength64 = responseBytes.Length;
            listenerResponse.ContentType = "application/json";
            await listenerResponse.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
            listenerResponse.OutputStream.Close();
            message.Dispose();
            responseMessage.Dispose();
        }

        public void Dispose()
        {
            ProxyServer.Stop();
            Client.Dispose();
        }
    }
}
