using System.Drawing;
using System.Runtime.InteropServices;

namespace RadiantConnect.Authentication.QRSignIn.Modules
{
    internal class Win32Form : IDisposable
    {
        private delegate nint WndProcDelegate(nint hWnd, uint uMsg, nint wParam, nint lParam);

        internal nint WindowHandle { get; set; }

        internal Win32Form(Bitmap bitmap)
        {
            Task.Run(() =>
            {
                const string className = "CustomBitmapWindowClass";
                nint hBitmap = bitmap.GetHbitmap();

                User32.WNDCLASSEX wndClass = new()
                {
                    cbSize = (uint)Marshal.SizeOf<User32.WNDCLASSEX>(),
                    lpfnWndProc = Marshal.GetFunctionPointerForDelegate((WndProcDelegate)WndProc),
                    lpszClassName = className,
                    hInstance = nint.Zero
                };

                User32.RegisterClassEx(ref wndClass);

                nint hWnd = User32.CreateWindowEx(
                    dwExStyle: 0,
                    lpClassName: className, 
                    lpWindowName: "RadiantConnect QR",
                    dwStyle: User32.WS_OVERLAPPED | User32.WS_SYSMENU | User32.WS_CAPTION | User32.WS_VISIBLE,
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

                User32.ShowWindow(hWnd, User32.SW_SHOWNORMAL);

                while (User32.GetMessage(out User32.MSG msg, nint.Zero, 0, 0) > 0)
                {
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }

                User32.DestroyWindow(hWnd);
                return;

                IntPtr WndProc(IntPtr wndhWnd, uint uMsg, IntPtr wParam, IntPtr lParam)
                {
                    switch (uMsg)
                    {
                        case User32.WM_PAINT:
                        {
                            User32.PAINTSTRUCT ps = new();
                            nint hdc = User32.BeginPaint(wndhWnd, ref ps);

                            nint hdcMem = User32.CreateCompatibleDC(hdc);
                            nint hBitmapOld = User32.SelectObject(hdcMem, hBitmap);

                            User32.BitBlt(hdc, 0, 0, bitmap.Width, bitmap.Height, hdcMem, 0, 0, User32.SRCCOPY);

                            User32.SelectObject(hdcMem, hBitmapOld);
                            User32.DeleteDC(hdcMem);

                            User32.EndPaint(wndhWnd, ref ps);
                            return nint.Zero;
                        }
                        case User32.WM_DESTROY:
                            User32.DeleteObject(hBitmap);
                            User32.PostQuitMessage(0);
                            return nint.Zero;
                        default:
                            return User32.DefWindowProc(wndhWnd, uMsg, wParam, lParam);
                    }
                }
            });

            while (WindowHandle == 0) Thread.Sleep(50);
        }

        private void CloseWindow() => User32.PostMessage(WindowHandle, User32.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

        public void Dispose() => CloseWindow();
    }
}
