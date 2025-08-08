using System.Drawing;
using RadiantConnect.ImageRecognition.Internals;
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers.KillFeed
{
    internal class ActionDetection
    {
        internal static KillFeedPositions GetKillHalfPosition(Bitmap killFeedItem, ColorConfig? colorConfig = null)
        {
            TimeOnly killTime = TimeOnly.FromDateTime(DateTime.Now);

            int middlePoint = 0;
            int firstGreenPixel = int.MaxValue;
            int firstRedPixel = int.MaxValue;
            int middleHeight = killFeedItem.Height / 2;

            for (int xIndex = 0; xIndex < killFeedItem.Width; xIndex++)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, middleHeight);

                if (ColorHandler.IsValorantGreen(pixelColor, colorConfig?.GreenConfig) && firstGreenPixel > xIndex)
                    firstGreenPixel = xIndex;
                if (ColorHandler.IsValorantRed(pixelColor, colorConfig?.RedConfig) && firstRedPixel > xIndex)
                    firstRedPixel = xIndex;
            }

            int step = firstGreenPixel > firstRedPixel ? -1 : 1;

            for (int xIndex = firstGreenPixel; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, middleHeight);
                if (!ColorHandler.IsValorantRed(pixelColor, colorConfig?.RedConfig)) continue;
                middlePoint = xIndex;
                break;
            }


            if (firstGreenPixel > firstRedPixel)
            {
                firstGreenPixel = middlePoint + 5;
                firstRedPixel = middlePoint - 5;
            }
            else
            {
                middlePoint -= 5;
                firstGreenPixel = middlePoint - 5;
                firstRedPixel = middlePoint + 5;
            }

            bool validPosition = firstGreenPixel > 15 && middlePoint > 15 && firstRedPixel > 15;

            return new KillFeedPositions(firstRedPixel, firstGreenPixel, middlePoint, validPosition, killTime);
        }

        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        internal static bool ActionDetected(Color[] colors, ColorConfig? colorConfig = null)
        {
            bool wasFound = false;

            for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
            {
                if (!ColorHandler.IsActionColor(colors[colorIndex], colorConfig?.ActionColorConfig)) continue;

                wasFound = true;
                break;
            }

            return wasFound;
        }

        internal static KillFeedAction ActionResult(Bitmap killFeedItem, KillFeedPositions positions, ColorConfig? colorConfig = null)
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
                
                if (!ActionDetected(pixelColors, colorConfig)) continue;

                if (positions.RedPixel > positions.GreenPixel)
                    performedKill = true;

                break;
            }

            for (int xIndex = positions.Middle; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color[] pixelColors = [killFeedItem.GetPixel(xIndex, borderTop + 1), killFeedItem.GetPixel(xIndex, borderTop), killFeedItem.GetPixel(xIndex, borderTop - 1)];
                if (!ActionDetected(pixelColors, colorConfig)) continue;

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
                    if (!ActionDetected(pixelTopColors, colorConfig)) continue;
                    if (ActionDetected(pixelBottomColors, colorConfig)) continue;

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
