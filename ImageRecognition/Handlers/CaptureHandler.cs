using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.InteropServices;
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class CaptureHandler
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hwnd, ref Rectangle rectangle);

        internal const int KillFeedHeight = 700;
        internal const int KillFeedWidth = 900;
        internal const int KillFeedWidthOffset = 110;

        internal static Rectangle GetValorantRectangle()
        {
            nint processHandle = Process.GetProcessesByName("vlc")[0].MainWindowHandle;
            Rectangle captureRectangle = new();
            GetWindowRect(processHandle, ref captureRectangle);

            return captureRectangle;
        }

        internal static Bitmap GetKillFeedBox(Point captureLocation = default, Size captureSize = default)
        {
            Rectangle valorantRectangle = GetValorantRectangle();

            captureLocation = captureLocation with { X = valorantRectangle.Width - KillFeedWidth + captureLocation.X + KillFeedWidthOffset };
            captureSize = new Size(KillFeedWidth - KillFeedWidthOffset, captureSize.Height > 0 ? captureSize.Height : KillFeedHeight);

            Bitmap bitmap = new(captureSize.Width, captureSize.Height);
            using Graphics graphics = Graphics.FromImage(bitmap);
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.CopyFromScreen(captureLocation, Point.Empty, captureSize);

            return bitmap;
        }

        internal static KillFeedAction ActionResult(Bitmap killFeedItem, KillFeedPositions positions)
        {
            const int borderTop = 4;
            bool wasKilled = false;
            bool performedKill = false;
            bool didAssist = false;

            int step = positions.RedPixel > positions.GreenPixel ? -1 : 1;

            int borderBottom = killFeedItem.Height - 4;

            for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, borderBottom);
                if (!ColorHandler.IsActionColor(pixelColor)) continue;

                if (positions.RedPixel > positions.GreenPixel)
                    performedKill = true;

                break;
            }

            for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, borderTop);
                if (!ColorHandler.IsActionColor(pixelColor)) continue;

                if (positions.RedPixel < positions.GreenPixel)
                    wasKilled = true;

                break;
            }

            for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color pixelTopColor = killFeedItem.GetPixel(xIndex, borderTop);
                Color pixelBottomColor = killFeedItem.GetPixel(xIndex, borderBottom);
                if (!ColorHandler.IsActionColor(pixelTopColor)) continue;
                if (ColorHandler.IsActionColor(pixelBottomColor)) continue;

                didAssist = true;

                break;
            }

            bool wasInFeed = performedKill || wasKilled || didAssist;

            return new KillFeedAction(
                performedKill,
                wasKilled,
                didAssist,
                wasInFeed,
                positions
            );
        }

    }
}
