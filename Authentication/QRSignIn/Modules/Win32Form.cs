using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace RadiantConnect.Authentication.QRSignIn.Modules
{
    internal class Win32Form : IDisposable
    {
        private delegate nint WndProcDelegate(nint hWnd, uint uMsg, nint wParam, nint lParam);

        internal nint WindowHandle { get; set; } = 0;

        internal Win32Form(Bitmap bitmap)
        {
            Task.Run(() =>
            {
                const string className = "CustomBitmapWindowClass";
                nint hBitmap = bitmap.GetHbitmap();

                IntPtr WndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
                {
                    switch (uMsg)
                    {
                        case Win32.WM_PAINT:
                        {
                            Win32.PAINTSTRUCT ps = new();
                            nint hdc = Win32.BeginPaint(hWnd, ref ps);

                            nint hdcMem = Win32.CreateCompatibleDC(hdc);
                            nint hBitmapOld = Win32.SelectObject(hdcMem, hBitmap);

                            Win32.BitBlt(hdc, 0, 0, bitmap.Width, bitmap.Height, hdcMem, 0, 0, Win32.SRCCOPY);

                            Win32.SelectObject(hdcMem, hBitmapOld);
                            Win32.DeleteDC(hdcMem);

                            Win32.EndPaint(hWnd, ref ps);
                            return nint.Zero;
                        }
                        case Win32.WM_DESTROY:
                            Win32.DeleteObject(hBitmap);
                            Win32.PostQuitMessage(0);
                            return nint.Zero;
                        default:
                            return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                }

                Win32.WNDCLASSEX wndClass = new()
                {
                    cbSize = (uint)Marshal.SizeOf<Win32.WNDCLASSEX>(),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WndProc),
                    lpszClassName = className,
                    hInstance = nint.Zero
                };

                Win32.RegisterClassEx(ref wndClass);

                nint hWnd = Win32.CreateWindowEx(
                    0, className, "RadiantConnect QR",
                    Win32.WS_OVERLAPPED | Win32.WS_SYSMENU | Win32.WS_CAPTION | Win32.WS_VISIBLE,
                    100, 100, bitmap.Width + 16, bitmap.Height + 39, nint.Zero, nint.Zero, nint.Zero, nint.Zero);

                WindowHandle = hWnd;

                Debug.WriteLine(WindowHandle);

                Win32.ShowWindow(hWnd, Win32.SW_SHOWNORMAL);

                Win32.MSG msg;
                while (Win32.GetMessage(out msg, nint.Zero, 0, 0) > 0)
                {
                    Win32.TranslateMessage(ref msg);
                    Win32.DispatchMessage(ref msg);
                }

                Win32.DestroyWindow(hWnd);
            });

            while (WindowHandle == 0) Thread.Sleep(50);
        }

        private void CloseWindow() => Win32.PostMessage(WindowHandle, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

        public void Dispose() => CloseWindow();
    }
}
