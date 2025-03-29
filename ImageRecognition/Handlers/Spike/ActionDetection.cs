
#if WINDOWS
using System.Drawing;
#endif
using RadiantConnect.ImageRecognition.Internals;

#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers.Spike
{
    internal class ActionDetection
    {
#if WINDOWS
        internal static bool SpikePlantedResult(Bitmap spikeItem, ColorConfig? colorConfig = null)
#else
        internal static bool SpikePlantedResult(object spikeItem, ColorConfig? colorConfig = null)
#endif
        {
#if WINDOWS
            int middle = spikeItem.Width / 2;
            bool wasFound = false;

            for (int yIndex = 0; yIndex < spikeItem.Height; yIndex++)
            {
                Color pixelColor = spikeItem.GetPixel(middle, yIndex);
                if (!ColorHandler.IsSpikeRed(pixelColor, colorConfig?.SpikeColorConfig)) continue;
                wasFound = true;
                break;
            }

            return wasFound;
#else
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }
    }
}
