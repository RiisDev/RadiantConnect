using RadiantConnect.Methods;
#if WINDOWS
using System.Drawing;
#endif
using System.Runtime.InteropServices;
using RadiantConnect.Utilities;

namespace RadiantConnect.Authentication.QRSignIn.Modules
{
    internal class Win32Form : IDisposable
    {
        private delegate nint WndProcDelegate(nint hWnd, uint uMsg, nint wParam, nint lParam);

        internal nint WindowHandle { get; set; }
#if WINDOWS
        internal Win32Form(Bitmap bitmap)
#else
        internal Win32Form(object bitmap)
#endif
        {
#if WINDOWS
            Task.Run(() =>
            {
                const string className = "CustomBitmapWindowClass";
                nint hBitmap = bitmap.GetHbitmap();

                Win32.WNDCLASSEX wndClass = new()
                {
                    cbSize = (uint)Marshal.SizeOf<Win32.WNDCLASSEX>(),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WndProc),
                    lpszClassName = className,
                    hInstance = nint.Zero
                };

                Win32.RegisterClassEx(ref wndClass);

                nint hWnd = Win32.CreateWindowEx(
                    dwExStyle: 0,
                    lpClassName: className, 
                    lpWindowName: "RadiantConnect QR",
                    dwStyle: Win32.WS_OVERLAPPED | Win32.WS_SYSMENU | Win32.WS_CAPTION | Win32.WS_VISIBLE,
                    X: 100,
                    Y: 100, 
                    nWidth: bitmap.Width + 16,
                    nHeight: bitmap.Height + 39, 
                    hWndParent: nint.Zero,
                    hMenu: nint.Zero, 
                    hInstance: nint.Zero, 
                    lpParam: nint.Zero
                );

                WindowHandle = hWnd;

                Win32.ShowWindow(hWnd, Win32.SW_SHOWNORMAL);

                while (Win32.GetMessage(out Win32.MSG msg, nint.Zero, 0, 0) > 0)
                {
                    Win32.TranslateMessage(ref msg);
                    Win32.DispatchMessage(ref msg);
                }

                Win32.DestroyWindow(hWnd);
                return;

                IntPtr WndProc(IntPtr wndhWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
                {
                    switch (uMsg)
                    {
                        case Win32.WM_PAINT:
                        {
                            Win32.PAINTSTRUCT ps = new();
                            nint hdc = Win32.BeginPaint(wndhWnd, ref ps);

                            nint hdcMem = Win32.CreateCompatibleDC(hdc);
                            nint hBitmapOld = Win32.SelectObject(hdcMem, hBitmap);

                            Win32.BitBlt(hdc, 0, 0, bitmap.Width, bitmap.Height, hdcMem, 0, 0, Win32.SRCCOPY);

                            Win32.SelectObject(hdcMem, hBitmapOld);
                            Win32.DeleteDC(hdcMem);

                            Win32.EndPaint(wndhWnd, ref ps);
                            return nint.Zero;
                        }
                        case Win32.WM_DESTROY:
                            Win32.DeleteObject(hBitmap);
                            Win32.PostQuitMessage(0);
                            return nint.Zero;
                        default:
                            return Win32.DefWindowProc(wndhWnd, uMsg, wParam, lParam);
                    }
                }
            });

            while (WindowHandle == 0) Thread.Sleep(50);
#else
            throw new PlatformNotSupportedException("Win32Form is only supported on Windows.");
#endif
        }

        private void CloseWindow()
        {
#if WINDOWS
            Win32.PostMessage(WindowHandle, Win32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
#else
            throw new PlatformNotSupportedException("Win32Form is only supported on Windows.");
#endif
        }

        public void Dispose()
        {
#if WINDOWS
            CloseWindow();
#else
            throw new PlatformNotSupportedException("Win32Form is only supported on Windows.");
#endif
        }
    }
}
