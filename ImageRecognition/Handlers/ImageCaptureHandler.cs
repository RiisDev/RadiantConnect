using System.Diagnostics;
using System.Runtime.InteropServices;
using RadiantConnect.Utilities;

#if WINDOWS
using System.Drawing;
using System.Drawing.Drawing2D;
#endif

#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class ImageCaptureHandler
    {
#if WINDOWS
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hwnd, ref Rectangle rectangle);
#endif
        // Kill feed offsets
        internal const int KillFeedHeight = 700;
        internal const int KillFeedWidth = 900;
        internal const int KillFeedWidthOffset = 110;

        // Spike offsets
        internal const int SpikeBoxWidth = 200;
        internal const int SpikeBoxHeight = 80;
        internal const int HeightOffset = 10;

        internal static Rectangle GetValorantRectangle()
        {
#if WINDOWS
            if (!InternalValorantMethods.IsValorantProcessRunning()) throw new RadiantConnectException("Valorant is not running");

            nint processHandle = Process.GetProcessesByName("VALORANT-Win64-Shipping")[0].MainWindowHandle;
            Rectangle captureRectangle = new();
            GetWindowRect(processHandle, ref captureRectangle);
            
            return captureRectangle;
#else
            throw new PlatformNotSupportedException("This library is only supported on Windows.");
#endif
        }
#if WINDOWS
        internal static Bitmap GetSpikeBox()
#else
        internal static object GetSpikeBox()
#endif
        {
#if WINDOWS
            Rectangle valorantRectangle = GetValorantRectangle();

            int valorantMiddle = (valorantRectangle.Width - SpikeBoxWidth) / 2;

            Bitmap croppedScreenshot = new(SpikeBoxWidth, SpikeBoxHeight);

            using Graphics graphics = Graphics.FromImage(croppedScreenshot);
            graphics.CopyFromScreen(valorantMiddle, HeightOffset, 0, -HeightOffset, croppedScreenshot.Size);

            return croppedScreenshot;
#else
            throw new PlatformNotSupportedException("This library is only supported on Windows.");
#endif
        }

        internal static Bitmap GetKillFeedBox(Point captureLocation = default, Size captureSize = default)
        {
#if WINDOWS
            Rectangle valorantRectangle = GetValorantRectangle();

            captureLocation = captureLocation with { X = valorantRectangle.Width - KillFeedWidth + captureLocation.X + KillFeedWidthOffset };
            captureSize = new Size(KillFeedWidth - KillFeedWidthOffset, captureSize.Height > 0 ? captureSize.Height : KillFeedHeight);

            Bitmap bitmap = new(captureSize.Width, captureSize.Height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CopyFromScreen(captureLocation, Point.Empty, captureSize);

            return bitmap;
#else
            throw new PlatformNotSupportedException("This library is only supported on Windows.");
#endif
        }
    }
}
