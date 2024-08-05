using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth
{
    internal class Driver
    {
        // Hoping this will not throw AV issues :(
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        internal static readonly Dictionary<int, TaskCompletionSource<string>> PendingRequests = new();

        internal static async Task<string?> ExecuteOnPageWithResponse(string pageTitle, int port, Dictionary<string, object> dataToSend, string expectedOutput, ClientWebSocket socket, bool output = false, bool skipCheck = false)
        {
            if (pageTitle != "") await WaitForPage(pageTitle, port);

            int id = (int)dataToSend["id"];
            TaskCompletionSource<string> tcs = new();
            PendingRequests[id] = tcs;

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            string response = await tcs.Task;

            if (output) Debug.WriteLine(response);
            if (!response.Contains(expectedOutput) && !skipCheck) throw new Exception("Expected output not found");

            return response;
        }

        // Lowkey thanks chatgpt <3
        internal static async Task ListenAsync(ClientWebSocket socket)
        {
            byte[] buffer = new byte[8192];

            while (socket.State == WebSocketState.Open)
            {
                using MemoryStream memoryStream = new();
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    memoryStream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                string message = Encoding.UTF8.GetString(memoryStream.ToArray());
                Dictionary<string, object>? json = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

                if (json == null || !json.ContainsKey("id")) continue;

                int id = int.Parse(json["id"].ToString()!);
                if (!PendingRequests.TryGetValue(id, out TaskCompletionSource<string>? tcs)) continue;

                tcs.SetResult(message);
                PendingRequests.Remove(id);
            }
        }

        internal static async Task NavigateTo(string url, string pageTitle, int port, ClientWebSocket socket)
        {
            Dictionary<string, object> dataToSend = new()
            {
                { "id",new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string> { {"expression", $"document.location.href = \"{url}\""} }
                }
            };
            
            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            await WaitForPage(pageTitle, port, 999999);
        }

        internal static int FreeTcpPort()
        {
            TcpListener l = new(IPAddress.Loopback, 0);
            l.Start();
            int port = ((IPEndPoint)l.LocalEndpoint).Port;
            l.Stop();
            l.Dispose();
            return port;
        }

        internal static async Task<string?> WaitForPage(string title, int port, int maxRetries = 250, bool needsReturn = false)
        {
            using HttpClient httpClient = new();
            int retries = 0;
            string foundSocket = "";
            do
            {
                retries++;
                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");

                if (debugResponse is null) continue;
                if (debugResponse.Count == 0) continue;
                if (!debugResponse.Any(x => x.Title.Contains(title))) continue;
                if (needsReturn) foundSocket = debugResponse.First(x => x.Title.Contains(title)).WebSocketDebuggerUrl;

                break;

            } while (retries <= maxRetries);

            return foundSocket;
        }

        internal static async Task<bool> PageExists(string pageTitle, int port)
        {
            using HttpClient httpClient = new();
            int retries = 0;
            do
            {
                retries++;
                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");

                if (debugResponse is null) continue;
                if (debugResponse.Count == 0) continue;
                if (debugResponse.Any(x => x.Title.Contains(pageTitle))) break;
            } while (retries <= 150);

            return retries < 150;
        }
        
        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        internal static Task HideDriver(Process driver)
        {
            while (!driver.HasExited)
            {
                ShowWindow(driver.MainWindowHandle, 0);
                ShowWindow(FindWindow("Chrome_WidgetWin_1", "Restore pages"), 0);
            }

            return Task.CompletedTask;
        }

        internal static async Task<(Process?, string)> StartDriver(string browserExecutable, int port)
        {
            ProcessStartInfo processInfo = new()
            {
                FileName = browserExecutable,
                Arguments = $"--remote-debugging-port={port} --disable-gpu --disable-extensions --disable-hang-monitor --disable-breakpad --disable-client-side-phishing-detection --no-sandbox --disable-site-isolation-trials --disable-features=IsolateOrigins,SitePerProcess --disable-accelerated-2d-canvas --disable-accelerated-compositing --disable-smooth-scrolling --disable-application-cache --disable-background-networking --disable-site-engagement --disable-webgl --disable-predictive-service --disable-perf --disable-media-internals --disable-ppapi --disable-software-rasterizer --incognito https://www.google.com/",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            Process? driverProcess = Process.Start(processInfo);

            Debug.WriteLine($"Debug: http://localhost:{port}/json");

            Task.Run(() => HideDriver(driverProcess!)); // Todo make sure this isnt just spammed, find a way to detect if it's hidden already
            AppDomain.CurrentDomain.ProcessExit += (_, _) => driverProcess?.Kill(); // Make sure the engine is closed when the application is closed

            string? socketUrl = await WaitForPage("Google", port, 999999, true);

            if (string.IsNullOrEmpty(socketUrl))
                throw new RadiantConnectAuthException("Failed to start driver");

            return (driverProcess, socketUrl);
        }
    }
}
