using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Runtime.InteropServices;
using RadiantConnect.Methods;

#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class CaptureHandler
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(nint hwnd, ref Rectangle rectangle);

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
            if (!InternalValorantMethods.IsValorantProcessRunning()) throw new RadiantConnectException("Valorant is not running");

            nint processHandle = Process.GetProcessesByName("VALORANT-Win64-Shipping")[0].MainWindowHandle;
            Rectangle captureRectangle = new();
            GetWindowRect(processHandle, ref captureRectangle);
            
            return captureRectangle;
        }

        internal static Bitmap GetSpikeBox()
        {
            Rectangle valorantRectangle = GetValorantRectangle();

            int valorantMiddle = (valorantRectangle.Width - SpikeBoxWidth) / 2;

            Bitmap croppedScreenshot = new (SpikeBoxWidth, SpikeBoxHeight);

            using Graphics graphics = Graphics.FromImage(croppedScreenshot);
            graphics.CopyFromScreen(valorantMiddle, HeightOffset, 0, -HeightOffset, croppedScreenshot.Size);

            return croppedScreenshot;
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

        internal static bool SpikePlantedResult(Bitmap spikeItem)
        {
            int middle = spikeItem.Width / 2;
            bool wasFound = false;
            
            for (int yIndex = 0; yIndex < spikeItem.Height; yIndex++)
            {
                Color pixelColor = spikeItem.GetPixel(middle, yIndex);
                if (!ColorHandler.IsSpikeRed(pixelColor)) continue;
                wasFound = true;
                break;
            }

            return wasFound;
        }

        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        internal static bool ActionDetected(Color[] colors)
        {
            bool wasFound = false;

            for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
            {
                if (!ColorHandler.IsActionColor(colors[colorIndex])) continue;

                wasFound = true;
                break;
            }

            return wasFound;
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
                Color[] pixelColors = [
                    killFeedItem.GetPixel(xIndex, borderBottom + 1),
                    killFeedItem.GetPixel(xIndex, borderBottom), 
                    killFeedItem.GetPixel(xIndex, borderBottom - 1)
                ];

                if (!ActionDetected(pixelColors)) continue;

                if (positions.RedPixel > positions.GreenPixel)
                    performedKill = true;

                break;
            }

            for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color[] pixelColors = [killFeedItem.GetPixel(xIndex, borderTop + 1), killFeedItem.GetPixel(xIndex, borderTop), killFeedItem.GetPixel(xIndex, borderTop - 1)];
                if (!ActionDetected(pixelColors)) continue;

                if (positions.RedPixel < positions.GreenPixel)
                    wasKilled = true;

                break;
            }

            if (!wasKilled && !performedKill)
            {
                for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
                {
                    Color[] pixelTopColors = [killFeedItem.GetPixel(xIndex, borderTop + 1), killFeedItem.GetPixel(xIndex, borderTop), killFeedItem.GetPixel(xIndex, borderTop - 1)];
                    Color[] pixelBottomColors = [killFeedItem.GetPixel(xIndex, borderBottom + 1), killFeedItem.GetPixel(xIndex, borderBottom), killFeedItem.GetPixel(xIndex, borderBottom - 1)];
                    if (!ActionDetected(pixelTopColors)) continue;
                    if (ActionDetected(pixelBottomColors)) continue;

                    didAssist = true;

                    break;
                }
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
