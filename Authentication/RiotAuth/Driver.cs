using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text.Json;
using System.Runtime.InteropServices;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.RiotAuth
{
    internal class Driver
    {
        // Hoping this will not throw AV issues :(
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        internal static async Task<string?> ExecuteOnPageWithResponse(string pageTitle, int port, Dictionary<string, object> dataToSend, string expectedOutput, bool output = false, bool skipCheck = false, bool executeOnAny = false, bool waitForPage = false)
        {
            using ClientWebSocket socket = new();
            string socketUrl = await GetSocketUrl(pageTitle, port, executeOnAny, waitForPage);
            
            if (string.IsNullOrEmpty(socketUrl)) throw new RadiantConnectAuthException("Page not found");

            await socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

            using MemoryStream memoryStream = new();
            byte[] buffer = new byte[8192];

            WebSocketReceiveResult result;
            do
            {
                result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                memoryStream.Write(buffer, 0, result.Count);
            }
            while (!result.EndOfMessage);

            string response = Encoding.UTF8.GetString(memoryStream.ToArray());

            if (output) Debug.WriteLine(response);
            if (!response.Contains(expectedOutput) && !skipCheck) throw new RadiantConnectAuthException("Expected output not found");

            return response;
        }

        internal static async Task NavigateTo(string url, int port)
        {
            Dictionary<string, object> dataToSend = new()
            {
                { "id",new Random((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()).Next() },
                { "method", "Runtime.evaluate" },
                { "params", new Dictionary<string, string> { {"expression", $"document.location.href = \"{url}\""} }
                }
            };

            using ClientWebSocket socket = new();
            string socketUrl = await GetSocketUrl("", port, true);

            if (string.IsNullOrEmpty(socketUrl)) throw new RadiantConnectAuthException("Page not found");

            await socket.ConnectAsync(new Uri(socketUrl), CancellationToken.None);

            await socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(dataToSend))), WebSocketMessageType.Text, true, CancellationToken.None);

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

        internal static async Task<string> GetSocketUrl(string pageTitle, int port, bool getAnyPage = false, bool waitForPage = false)
        {
            int retries = 0;
            string socketUrl = string.Empty;
            using HttpClient httpClient = new();
            do
            {
                if (!waitForPage)
                    retries++;
                else
                    retries = 0;

                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");

                if (debugResponse is null) continue;
                if (debugResponse.Count == 0) continue;
                if (debugResponse.Any(x => x.Title.Contains(pageTitle))) socketUrl = debugResponse.First(x => x.Title.Contains(pageTitle)).WebSocketDebuggerUrl;
                if (getAnyPage) socketUrl = debugResponse.First().WebSocketDebuggerUrl;
            } while (string.IsNullOrEmpty(socketUrl) && retries <= 250);
            
            return socketUrl;
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

        internal static async Task<Process?> StartDriver(string browserExecutable, int port)
        {
            using HttpClient httpClient = new();
            ProcessStartInfo processInfo = new()
            {
                FileName = browserExecutable,
                Arguments = $"--remote-debugging-port={port} --disable-gpu --disable-extensions --disable-hang-monitor --disable-breakpad --disable-client-side-phishing-detection --no-sandbox --disable-site-isolation-trials --disable-features=IsolateOrigins,SitePerProcess --disable-accelerated-2d-canvas --disable-accelerated-compositing --disable-smooth-scrolling --disable-application-cache --disable-background-networking --disable-site-engagement --disable-webgl --disable-predictive-service --disable-perf --disable-media-internals --disable-ppapi --disable-software-rasterizer --incognito https://www.google.com/",
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            Process? driverProcess = Process.Start(processInfo);

            Task.Run(() => HideDriver(driverProcess!)); // Todo make sure this isnt just spammed, find a way to detect if it's hidden already
            AppDomain.CurrentDomain.ProcessExit += (_, _) => driverProcess?.Kill(); // Make sure the engine is closed when the application is closed
            
            while (driverProcess?.MainWindowHandle == IntPtr.Zero) await Task.Delay(100);
            while (driverProcess is not null && !driverProcess.HasExited)
            {
                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");
                if (debugResponse is not null && debugResponse.Any(x => x.Url == "https://www.google.com/")) break;
            }

            Thread.Sleep(500);

            return driverProcess;
        }
    }
}
