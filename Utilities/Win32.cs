#pragma warning disable SYSLIB1054

namespace RadiantConnect.Utilities
{
	internal static class Win32
	{
		
		[DllImport("user32.dll")]
		internal static extern bool ShowWindow(nint hWnd, int nCmdShow);

		// Yknow "thread safety" and all that
		private static int _captchaFound;
		internal static bool CaptchaFound
		{
			get => Interlocked.CompareExchange(ref _captchaFound, 1, 1) == 1;
			set => Interlocked.Exchange(ref _captchaFound, value ? 1 : 0);
		}

		internal static Task HideDriver(Process? driver)
		{

#if WINDOWS
			if (driver == null || driver.HasExited) return Task.CompletedTask;
			while (!driver.HasExited) ShowWindow(driver.MainWindowHandle, CaptchaFound ? 1 : 0);

			return Task.CompletedTask;
#else
			return Task.CompletedTask;
#endif
		}
	}
}
