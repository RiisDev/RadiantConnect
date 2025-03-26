using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#pragma warning disable SYSLIB1054 // I cba to convert the DllImport to LibraryImport

namespace RadiantConnect.Utilities
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "IdentifierTypo")]
    internal static class Win32
    {
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(nint hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern ushort RegisterClassEx(ref WNDCLASSEX lpwcx);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        internal static extern nint CreateWindowEx(
            int dwExStyle, string lpClassName, string lpWindowName,
            uint dwStyle, int X, int Y, int nWidth, int nHeight,
            nint hWndParent, nint hMenu, nint hInstance, nint lpParam);

        [DllImport("user32.dll")]
        internal static extern bool IsWindow(nint hWnd);

        [DllImport("user32.dll")]
        internal static extern nint DefWindowProc(nint hWnd, uint uMsg, nint wParam, nint lParam);

        [DllImport("user32.dll")]
        internal static extern bool DestroyWindow(nint hWnd);

        [DllImport("user32.dll")]
        internal static extern void PostQuitMessage(int nExitCode);

        [DllImport("user32.dll")]
        internal static extern nint BeginPaint(nint hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("user32.dll")]
        internal static extern bool EndPaint(nint hWnd, ref PAINTSTRUCT lpPaint);

        [DllImport("gdi32.dll")]
        internal static extern nint SelectObject(nint hdc, nint hgdiobj);

        [DllImport("gdi32.dll")]
        internal static extern bool BitBlt(nint hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
            nint hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("user32.dll")]
        internal static extern nint GetDC(nint hWnd);

        [DllImport("user32.dll")]
        internal static extern int GetMessage(out MSG lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")]
        internal static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")]
        internal static extern nint DispatchMessage(ref MSG lpMsg);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteObject(nint hObject);


        [DllImport("gdi32.dll")]
        internal static extern nint CreateCompatibleDC(nint hdc);

        [DllImport("gdi32.dll")]
        internal static extern bool DeleteDC(nint hdc);

        [DllImport("user32.dll")]
        internal static extern bool PostMessage(nint hWnd, uint Msg, nint wParam, nint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern nint FindWindow(string lpClassName, string lpWindowName);

        // Constants
        internal const int WS_OVERLAPPED = 0x00000000;
        internal const int WS_SYSMENU = 0x00080000;
        internal const int WS_CAPTION = 0x00C00000; // Includes the close button
        internal const int WS_VISIBLE = 0x10000000;
        internal const int WM_PAINT = 0x000F;
        internal const int WM_DESTROY = 0x0002;
        internal const int SRCCOPY = 0x00CC0020;
        internal const int SW_SHOWNORMAL = 1;
        internal const uint WM_CLOSE = 0x0010;

        // Structs for window creation
        [StructLayout(LayoutKind.Sequential)]
        internal struct WNDCLASSEX
        {
            internal uint cbSize;
            internal uint style;
            internal nint lpfnWndProc;
            internal int cbClsExtra;
            internal int cbWndExtra;
            internal nint hInstance;
            internal nint hIcon;
            internal nint hCursor;
            internal nint hbrBackground;
            internal string lpszMenuName;
            internal string lpszClassName;
            internal nint hIconSm;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PAINTSTRUCT
        {
            internal nint hdc;
            internal bool fErase;
            internal RECT rcPaint;
            internal bool fRestore;
            internal bool fIncUpdate;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            internal byte[] rgbReserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MSG
        {
            internal nint hWnd;
            internal uint message;
            internal nint wParam;
            internal nint lParam;
            internal uint time;
            internal POINT pt;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            internal int x;
            internal int y;
        }


        // Yknow "thread safety" and all that
        private static int _captchaFound;
        internal static bool CaptchaFound
        {
            get => Interlocked.CompareExchange(ref _captchaFound, 1, 1) == 1;
            set => Interlocked.Exchange(ref _captchaFound, value ? 1 : 0);
        }

        internal static Task HideDriver(Process driver)
        {
            while (!driver.HasExited)
            {
                ShowWindow(driver.MainWindowHandle, CaptchaFound ? 1 : 0);
            }
            return Task.CompletedTask;
        }
    }
}
