using System.Drawing;
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers.Spike
{
    internal class ActionDetection
    {
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
    }
}
