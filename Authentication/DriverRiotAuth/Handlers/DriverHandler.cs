using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using RadiantConnect.Authentication.DriverRiotAuth.Misc;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using static RadiantConnect.Authentication.DriverRiotAuth.Misc.Events;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
    internal class DriverHandler
    {
        internal static event RuntimeChanged? OnRuntimeChanged;

        internal static readonly Dictionary<int, TaskCompletionSource<string>> PendingRequests = new();

        internal static event FrameChangedEvent? OnFrameNavigation;
        internal static event FrameChangedEvent? OnFrameLoaded;
        internal static event FrameChangedEvent? OnDocumentNavigate;

        internal static event RadiantConsoleDetected? OnMfaDetected;
        internal static event RadiantConsoleDetected? OnAccessTokenFound;

        internal static readonly Regex FrameNavigatedRegex = new("\"id\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"", RegexOptions.Compiled);
        internal static readonly Regex NavigatedWithinDocument = new("\"frameId\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"", RegexOptions.Compiled);
        internal static readonly Regex FrameStoppedLoadingRegex = new("\"frameId\":\"([^\"]+)\"", RegexOptions.Compiled);
        internal static readonly Regex AccessTokenRegex = new("(access_token=[^}]*)", RegexOptions.Compiled);

        internal static void DoDriverCheck(string browserProcess, string browserExecutable, bool killBrowser)
        {
            List<Process> browserProcesses = Process.GetProcessesByName(browserProcess).ToList();

            if (browserProcesses.Any() && !killBrowser)
                throw new RadiantConnectException($"{browserProcesses.First().ProcessName} is currently running, it must be closed or Initialize must be started with 'true'");
            if (browserProcesses.Any() && killBrowser)
                browserProcesses.ToList().ForEach(x => x.Kill());

            if (!File.Exists(browserExecutable))
                throw new RadiantConnectException($"Browser executable not found at {browserExecutable}");
        }

        internal static Task CheckForEvent(string message)
        {
            Match match;
            switch (message)
            {
                case var _ when message.Contains("Page.frameNavigated"):
                    match = FrameNavigatedRegex.Match(message);
                    if (match.Success)
                        OnFrameNavigation?.Invoke(match.Groups[2].Value, match.Groups[1].Value);
                    break;
                case var _ when message.Contains("Page.frameStoppedLoading"):
                    match = FrameStoppedLoadingRegex.Match(message);
                    if (match.Success)
                        OnFrameLoaded?.Invoke(null, match.Groups[1].Value);
                    break;
                case var _ when message.Contains("navigatedWithinDocument"):
                    match = NavigatedWithinDocument.Match(message);
                    if (match.Success)
                        OnDocumentNavigate?.Invoke(match.Groups[2].Value, match.Groups[1].Value);
                    break;

                case var _ when message.Contains("[RADIANTCONNECT] MFA Detected"):
                    OnMfaDetected?.Invoke();
                    break;
                case var _ when message.Contains("[RADIANTCONNECT] Access Token"):
                    match = AccessTokenRegex.Match(message);
                    if (match.Success)
                        OnAccessTokenFound?.Invoke(match.Groups[1].Value);
                    break;
            }

            return Task.CompletedTask;
        }

        internal static async Task HandleMessage(string message)
        {
            await CheckForEvent(message);
            if (OnRuntimeChanged is not null && (message.Contains("\"result\":{}}") || message.Contains("\"result\":{\"identifier\":\"1\"}}"))) OnRuntimeChanged.Invoke();

            Dictionary<string, object>? json = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

            if (json == null || !json.ContainsKey("id")) return;

            int id = int.Parse(json["id"].ToString()!);
            if (!PendingRequests.TryGetValue(id, out TaskCompletionSource<string>? tcs)) return;

            tcs.SetResult(message);
            PendingRequests.Remove(id);
        }

        internal static async Task ListenAsync(ClientWebSocket? socket)
        {
            byte[] buffer = new byte[8192];

            while (socket is not null && socket.State == WebSocketState.Open)
            {
                using MemoryStream memoryStream = new();
                WebSocketReceiveResult result;
                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    memoryStream.Write(buffer, 0, result.Count);
                } while (!result.EndOfMessage);

                await HandleMessage(Encoding.UTF8.GetString(memoryStream.ToArray()));
            }
        }

        internal static async Task<string?> GetInitialSocket(int port)
        {
            using HttpClient httpClient = new();

            while (true)
            {
                string pageData = await httpClient.GetStringAsync($"http://localhost:{port}/json");
                if (pageData.IndexOf("\"title\": \"Google\"", StringComparison.OrdinalIgnoreCase) == -1) continue;
                int urlStartIndex = pageData.IndexOf("\"webSocketDebuggerUrl\": \"ws://", StringComparison.OrdinalIgnoreCase);
                if (urlStartIndex == -1) continue;
                int urlValueStart = pageData.IndexOf("ws://", urlStartIndex, StringComparison.OrdinalIgnoreCase);
                int urlValueEnd = pageData.IndexOf("\"", urlValueStart, StringComparison.OrdinalIgnoreCase);

                return pageData.Substring(urlValueStart, urlValueEnd - urlValueStart);
            }
        }

        internal static async Task<string?> WaitForPage(string title, int port, ClientWebSocket? socket, int maxRetries = 250, bool needsReturn = false)
        {
            using HttpClient httpClient = new();
            int retries = 0;
            string foundSocket = "";
            do
            {
                retries++;
                List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");

                switch (debugResponse)
                {
                    case null:
                        continue;
                    case var _ when debugResponse.Count == 0:
                        continue;
                    case var _ when title == "VALORANT_RSO" && debugResponse.Any(x => x.Title == "Sign in" || x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token=")):
                        break;
                    case var _ when title == "Verification Required" && debugResponse.Any(x => x.Title.Contains("playvalorant.com/en-us/opt_in/#access_token=")):
                        break;
                    case var _ when !debugResponse.Any(x => x.Title.Contains(title)):
                        continue;
                }

                if (needsReturn) foundSocket = debugResponse.First(x => x.Title.Contains(title)).WebSocketDebuggerUrl;

                break;

            } while (retries <= maxRetries);

            return foundSocket;
        }

        internal static async Task<string?> GetPageUrl(int port)
        {
            using HttpClient httpClient = new();
            List<EdgeDev>? debugResponse = await httpClient.GetFromJsonAsync<List<EdgeDev>>($"http://localhost:{port}/json");
            return debugResponse?.FirstOrDefault(x=> x.Type == "page")?.Url;
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
                if (pageTitle == "ACCESS_TOKEN_AUTH" && debugResponse.Any(x => x.Title.Contains("#access_token="))) break;
                if (debugResponse.Any(x => x.Title.Contains(pageTitle))) break;
            } while (retries <= 150);

            return retries < 150;
        }

        internal static async Task<(Process?, string?)> StartDriver(string browserExecutable, int port)
        {
            Debug.WriteLine($"{DateTime.Now} Starting driver");
            ProcessStartInfo processInfo = new()
            {
                FileName = browserExecutable,
                Arguments = $"--remote-debugging-port={port} --incognito --disable-gpu --disable-extensions --disable-hang-monitor --disable-breakpad --disable-client-side-phishing-detection --no-sandbox --disable-site-isolation-trials --disable-features=IsolateOrigins,SitePerProcess --disable-accelerated-2d-canvas --disable-accelerated-compositing --disable-smooth-scrolling --disable-application-cache --disable-background-networking --disable-site-engagement --disable-webgl --disable-predictive-service --disable-perf --disable-media-internals --disable-ppapi --disable-software-rasterizer https://www.google.com/",
                RedirectStandardOutput = true
            };

            Process? driverProcess = Process.Start(processInfo);
#if DEBUG
#else
            Task.Run(() => Win32.HideDriver(driverProcess!)); // Todo make sure this isn't just spammed, find a way to detect if it's hidden already
#endif
            driverProcess!.PriorityClass = ProcessPriorityClass.High;

            string? socketUrl = await GetInitialSocket(port);

            Debug.WriteLine($"Debug: http://localhost:{port}/json");

            AppDomain.CurrentDomain.ProcessExit += (_, _) => driverProcess?.Kill();

            Debug.WriteLine($"{DateTime.Now} Finished Driver");
            
            return (driverProcess, socketUrl);
        }
    }
}
