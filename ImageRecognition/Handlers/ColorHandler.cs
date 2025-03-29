#if WINDOWS
using System.Drawing;
#endif
using System.Globalization;
using RadiantConnect.ImageRecognition.Internals;

#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class ColorHandler
    {

        internal static bool IsSpikeRed(Color color, SpikeColorConfig? spikeConfig = null)
        {
#if WINDOWS
            if (spikeConfig is null) return color is { R: >= 160 and <= 230, G: < 5, B: < 5 };

            Color highestColor = spikeConfig.HighestColor;
            Color lowestColor = spikeConfig.LowestColor;

            return color.R >= lowestColor.R && color.R <= highestColor.R &&
                   color.G >= lowestColor.G && color.G <= highestColor.G &&
                   color.B >= lowestColor.B && color.B <= highestColor.B;
#else       
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }

        internal static bool IsValorantGreen(Color color, GreenConfig? greenConfig = null)
        {
#if WINDOWS
            if (greenConfig is null) return color is { R: < 105 and > 100, G: < 197 and > 192, B: < 171 and > 165 };

            Color highestColor = greenConfig.HighestColor;
            Color lowestColor = greenConfig.LowestColor;

            return color.R >= lowestColor.R && color.R <= highestColor.R &&
                   color.G >= lowestColor.G && color.G <= highestColor.G &&
                   color.B >= lowestColor.B && color.B <= highestColor.B;
#else       
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }

        internal static bool IsValorantRed(Color color, RedConfig? redConfig = null)
        {
#if WINDOWS
            if (redConfig is null) return color is {R: >= 239 and <= 245, G: >= 89 and <= 95, B: >= 80 and <= 88};

            Color highestColor = redConfig.HighestColor;
            Color lowestColor = redConfig.LowestColor;

            return color.R >= lowestColor.R && color.R <= highestColor.R &&
                   color.G >= lowestColor.G && color.G <= highestColor.G &&
                   color.B >= lowestColor.B && color.B <= highestColor.B;
#else       
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }

        internal static bool IsActionColor(Color color, ActionColorConfig? config = null)
        {
#if WINDOWS
            if (config is null) return color is { R: >= 220 and <= 238, G: >= 231 and <= 238, B: >= 115 and <= 130 };

            Color highestColor = config.HighestColor;
            Color lowestColor = config.LowestColor;

            return color.R >= lowestColor.R && color.R <= highestColor.R &&
                   color.G >= lowestColor.G && color.G <= highestColor.G &&
                   color.B >= lowestColor.B && color.B <= highestColor.B;
#else       
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }
#if WINDOWS
        public static Dictionary<Color, int> GetColourFrequencies(Bitmap image)
#else
        public static Dictionary<Color, int> GetColourFrequencies(object image)
#endif
        {
#if WINDOWS
            Dictionary<Color, int> colourFrequencies = [];

            for (int x = 0; x < image.Width; x++)
            {
                for (int y = 0; y < image.Height; y++)
                {
                    Color pixelColor = image.GetPixel(x, y);
                    if (colourFrequencies.TryAdd(pixelColor, 1)) colourFrequencies[pixelColor]++;
                }
            }

            return colourFrequencies;
#else
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }

        public static int CompareColorFrequencies(Dictionary<Color, int> frequencies1, Dictionary<Color, int> frequencies2)
        {
#if WINDOWS
            int totalFrequency1 = frequencies1.Values.Sum();
            int totalFrequency2 = frequencies2.Values.Sum();
            double commonFrequency = 0;

            foreach (Color color in frequencies1.Keys)
                if (frequencies2.TryGetValue(color, out int frequency2))
                    commonFrequency += Math.Min(frequencies1[color], frequency2);

            double similarityPercentage = (commonFrequency / Math.Min(totalFrequency1, totalFrequency2)) * 100;

            return int.Parse(Math.Floor(similarityPercentage).ToString(CultureInfo.InvariantCulture));
#else
            throw new PlatformNotSupportedException("This method is only supported on Windows.");
#endif
        }
    }
}
