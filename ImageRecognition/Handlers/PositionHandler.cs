using System.Drawing;
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class PositionHandler
    {

        internal static KillFeedPositions GetKillHalfPosition(Bitmap killFeedItem)
        {
            TimeOnly killTime = TimeOnly.FromDateTime(DateTime.Now);

            int middlePoint = 0;
            int firstGreenPixel = int.MaxValue;
            int firstRedPixel = int.MaxValue;
            int middleHeight = killFeedItem.Height / 2;

            for (int xIndex = 0; xIndex < killFeedItem.Width; xIndex++)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, middleHeight);

                if (ColorHandler.IsValorantGreen(pixelColor) && firstGreenPixel > xIndex)
                    firstGreenPixel = xIndex;
                if (ColorHandler.IsValorantRed(pixelColor) && firstRedPixel > xIndex)
                    firstRedPixel = xIndex;
            }

            int step = firstGreenPixel > firstRedPixel ? -1 : 1;

            for (int xIndex = firstGreenPixel; xIndex >= 0 && xIndex < killFeedItem.Width; xIndex += step)
            {
                Color pixelColor = killFeedItem.GetPixel(xIndex, middleHeight);
                if (!ColorHandler.IsValorantRed(pixelColor)) continue;
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
    }
}
