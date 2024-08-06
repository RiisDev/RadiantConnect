using System.Net.Sockets;
using System.Net;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace RadiantConnect.Authentication.DriverRiotAuth.Misc
{
    internal static class Win32
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern nint FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ShowWindow(nint hWnd, int nCmdShow);

        internal static Task HideDriver(Process driver)
        {
            while (!driver.HasExited)
            {
                ShowWindow(driver.MainWindowHandle, 0);
                ShowWindow(FindWindow("Chrome_WidgetWin_1", "Restore pages"), 0);
            }

            return Task.CompletedTask;
        }

        internal static int GetFreePort()
        {
            using TcpListener listener = new(IPAddress.Loopback, 0);
            listener.Start();
            int port = ((IPEndPoint)listener.LocalEndpoint).Port;
            return port;
        }
    }
}
