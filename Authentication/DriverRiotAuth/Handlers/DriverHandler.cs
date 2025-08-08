using System.Net.WebSockets;
using RadiantConnect.Authentication.DriverRiotAuth.Records;
using static RadiantConnect.Authentication.DriverRiotAuth.Events;
using Match = System.Text.RegularExpressions.Match;

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
        internal static event RadiantConsoleDetected? OnCaptchaFound;
        internal static event RadiantConsoleDetected? OnCaptchaRemoved;

        internal static readonly Regex FrameNavigatedRegex = new("\"id\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"", RegexOptions.Compiled);
        internal static readonly Regex NavigatedWithinDocument = new("\"frameId\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"", RegexOptions.Compiled);
        internal static readonly Regex FrameStoppedLoadingRegex = new("\"frameId\":\"([^\"]+)\"", RegexOptions.Compiled);

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
                    OnAccessTokenFound?.Invoke(AuthUtil.ParseAccessToken(message));
                    break;
                case var _ when message.Contains("[RADIANTCONNECT] CAPTCHAFOUND"):
                    OnCaptchaFound?.Invoke();
                    Win32.CaptchaFound = true;
                    break;
                case var _ when message.Contains("[RADIANTCONNECT] CAPTCHAREMOVED"):
                    OnCaptchaRemoved?.Invoke();
                    Win32.CaptchaFound = false;
                    break;
            }

            return Task.CompletedTask;
        }

        internal static async Task HandleMessage(string message)
        {
            await CheckForEvent(message);
            if (OnRuntimeChanged is not null && (
                    message.Contains("frameScheduledNavigation") || 
                    message.Contains("\"result\":{}}") || 
                    message.Contains("\"result\":{\"identifier\":\"1\"}}"))
            ) OnRuntimeChanged.Invoke();

            Dictionary<string, object>? json = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

            if (json == null || !json.TryGetValue("id", out object? value)) return;

            int id = int.Parse(value.ToString()!);
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
            while (true)
            {
                string? pageData = await InternalHttp.GetAsync<string>($"http://localhost:{port}", "/json");
                if (string.IsNullOrEmpty(pageData)) return null;
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
            int retries = 0;
            string foundSocket = "";
            do
            {
                retries++;
                List<EdgeDev>? debugResponse = await InternalHttp.GetAsync<List<EdgeDev>>($"http://localhost:{port}", "/json");

                switch (debugResponse)
                {
                    case null:
                        continue;
                    case var _ when debugResponse.Count == 0:
                        continue;
                    case var _ when title == "VALORANT_RSO" && debugResponse.Any(x => x.Title == "Sign in" || x.Title.Contains("/opt_in/#access_token=")):
                        break;
                    case var _ when title == "Verification Required" && debugResponse.Any(x => x.Title.Contains("/opt_in/#access_token=")):
                        break;
                    case var _ when !debugResponse.Any(x => x.Title.Contains(title)):
                        continue;
                }

                if (needsReturn) foundSocket = debugResponse.First(x => x.Title.Contains(title)).WebSocketDebuggerUrl;

                break;

            } while (retries <= maxRetries);

            return foundSocket;
        }
        
        internal static async Task<(Process?, string?)> StartDriver(string browserExecutable, int port, bool headless)
        {
            Debug.WriteLine($"{DateTime.Now} Starting driver");
            ProcessStartInfo processInfo = new()
            {
                FileName = browserExecutable,
                Arguments = $"--remote-debugging-port={port} --disable-session-crashed-bubble --hide-crash-restore-bubble --disable-gpu --no-first-run --disable-extensions --disable-notifications --disable-hang-monitor --disable-remote-fonts --disable-crashpad-forwarding --disable-breakpad --disable-crashpad-for-testing --disable-first-run-ui  --disable-dinosaur-easter-egg  --disable-crash-reporter  --disable-client-side-phishing-detection --no-sandbox --disable-site-isolation-trials --disable-features=IsolateOrigins,SitePerProcess --disable-accelerated-2d-canvas --disable-accelerated-compositing --disable-smooth-scrolling --disable-application-cache --disable-background-networking --disable-site-engagement --disable-webgl --disable-predictive-service --disable-perf --disable-media-internals --disable-ppapi --disable-software-rasterizer https://www.google.com/",
                RedirectStandardOutput = true
            };

            Process? driverProcess = Process.Start(processInfo);
            
            if (headless)
                Task.Run(() => Win32.HideDriver(driverProcess!)); // Todo make sure this isn't just spammed, find a way to detect if it's hidden already

            if (driverProcess is null) throw new RadiantConnectException("Failed to start the driver process.");

            driverProcess.PriorityClass = ProcessPriorityClass.High;

            string? socketUrl = await GetInitialSocket(port);

            Debug.WriteLine($"Debug: http://localhost:{port}/json");

            AppDomain.CurrentDomain.ProcessExit += (_, _) => driverProcess.Kill();

            Debug.WriteLine($"{DateTime.Now} Finished Driver");
            
            return (driverProcess, socketUrl);
        }
    }
}
