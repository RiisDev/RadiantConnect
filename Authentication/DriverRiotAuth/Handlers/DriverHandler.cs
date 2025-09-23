using System.Net.WebSockets;
using static RadiantConnect.Authentication.DriverRiotAuth.Events;
using Match = System.Text.RegularExpressions.Match;

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace RadiantConnect.Authentication.DriverRiotAuth.Handlers
{
	internal partial class DriverHandler
	{
		internal static event RuntimeChanged? OnRuntimeChanged;

		internal static readonly Dictionary<int, TaskCompletionSource<string>> PendingRequests = [];

		internal static event FrameChangedEvent? OnFrameNavigation;
		internal static event FrameChangedEvent? OnFrameLoaded;
		internal static event FrameChangedEvent? OnDocumentNavigate;

		internal static event RadiantConsoleDetected? OnMfaDetected;
		internal static event RadiantConsoleDetected? OnAccessTokenFound;
		internal static event RadiantConsoleDetected? OnCaptchaFound;
		internal static event RadiantConsoleDetected? OnCaptchaRemoved;

		[GeneratedRegex("\"id\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"")]
		internal static partial Regex FrameNavigatedRegex();

		[GeneratedRegex("\"frameId\":\"([^\"]+)\".*?\"url\":\"([^\"]+)\"")]
		internal static partial Regex NavigatedWithinDocument();

		[GeneratedRegex("\"frameId\":\"([^\"]+)\"")]
		internal static partial Regex FrameStoppedLoadingRegex();

		internal static void DoDriverCheck(string browserProcess, string browserExecutable, bool killBrowser)
		{
			List<Process> browserProcesses = [];
			browserProcesses.AddRange(Process.GetProcessesByName(browserProcess));

			switch (browserProcesses.Count)
			{
				case 0 when !killBrowser:
					throw new RadiantConnectException($"{browserProcesses.First().ProcessName} is currently running, it must be closed or Initialize must be started with 'true'");
				case 0 when killBrowser:
					browserProcesses.ToList().ForEach(x => x.Kill());
					break;
			}

			if (!File.Exists(browserExecutable))
				throw new RadiantConnectException($"Browser executable not found at {browserExecutable}");
		}

		internal static Task CheckForEvent(string message)
		{
			Match match;
			switch (message)
			{
				case var _ when message.Contains("Page.frameNavigated"):
					match = FrameNavigatedRegex().Match(message);
					if (match.Success)
						OnFrameNavigation?.Invoke(match.Groups[2].Value, match.Groups[1].Value);
					break;
				case var _ when message.Contains("Page.frameStoppedLoading"):
					match = FrameStoppedLoadingRegex().Match(message);
					if (match.Success)
						OnFrameLoaded?.Invoke(null, match.Groups[1].Value);
					break;
				case var _ when message.Contains("navigatedWithinDocument"):
					match = NavigatedWithinDocument().Match(message);
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
			await CheckForEvent(message).ConfigureAwait(false);
			if (OnRuntimeChanged is not null && (
					message.Contains("frameScheduledNavigation") || 
					message.Contains("\"result\":{}}") || 
					message.Contains("\"result\":{\"identifier\":\"1\"}}"))
			) OnRuntimeChanged.Invoke();

			Dictionary<string, object>? json = JsonSerializer.Deserialize<Dictionary<string, object>>(message);

			if (json == null || !json.TryGetValue("id", out object? value)) return;

			int id = int.Parse(value.ToString() ?? "-1");
			if (id == -1) return;
			if (!PendingRequests.TryGetValue(id, out TaskCompletionSource<string>? tcs)) return;

			tcs.SetResult(message);
			PendingRequests.Remove(id);
		}

		internal static async Task ListenAsync(ClientWebSocket? socket)
		{
			try
			{
				byte[] buffer = new byte[8192];

				while (socket is not null && socket.State == WebSocketState.Open)
				{
					using MemoryStream memoryStream = new();
					WebSocketReceiveResult result;
					do
					{
						result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None).ConfigureAwait(false);
						memoryStream.Write(buffer, 0, result.Count);
					} while (!result.EndOfMessage);

					await HandleMessage(Encoding.UTF8.GetString(memoryStream.ToArray())).ConfigureAwait(false);
				}
			}
			catch (WebSocketException)
			{
				throw new RadiantConnectAuthException("WebSocket connection was closed unexpectedly. Please try again.");
			}
		}

		internal static async Task<string?> GetInitialSocket(int port)
		{
			while (true)
			{
				string? pageData = await InternalHttp.GetAsync<string>($"http://localhost:{port}", "/json").ConfigureAwait(false);
				if (pageData.IsNullOrEmpty()) return null;
				if (pageData.Contains("\"title\": \"Google\"")) continue;
				int urlStartIndex = pageData.IndexOf("\"webSocketDebuggerUrl\": \"ws://", StringComparison.OrdinalIgnoreCase);
				if (urlStartIndex == -1) continue;
				int urlValueStart = pageData.IndexOf("ws://", urlStartIndex, StringComparison.OrdinalIgnoreCase);
				int urlValueEnd = pageData.IndexOf("\"", urlValueStart, StringComparison.OrdinalIgnoreCase);
				
				return pageData[urlValueStart..urlValueEnd];
			}
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
				Task.Run(() => Win32.HideDriver(driverProcess)); // Todo make sure this isn't just spammed, find a way to detect if it's hidden already

			if (driverProcess is null) throw new RadiantConnectException("Failed to start the driver process.");

			driverProcess.PriorityClass = ProcessPriorityClass.High;

			string? socketUrl = await GetInitialSocket(port).ConfigureAwait(false);

			Debug.WriteLine($"Debug: http://localhost:{port}/json");

			AppDomain.CurrentDomain.ProcessExit += (_, _) => driverProcess.Kill();

			Debug.WriteLine($"{DateTime.Now} Finished Driver");
			
			return (driverProcess, socketUrl);
		}
	}
}
