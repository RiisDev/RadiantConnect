using System.Drawing;
using System.Globalization;
#pragma warning disable CA1416

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class ColorHandler
    {
        internal static bool IsValorantGreen(Color color)
        {
            return color is { R: < 103, G: < 197, B: < 170 } and { R: >= 99, G: > 190, B: > 165 };
        }

        internal static bool IsValorantRed(Color color)
        {
            return color is { R: >= 235 and <= 244, G: >= 88 and <= 92, B: >= 86 and <= 91 } or { R: 208, G: 99, B: 91 };
        }

        internal static bool IsActionColor(Color color)
        {
            return color is { R: >= 215 and <= 238, G: >= 231 and <= 238, B: >= 115 and <= 150 };
        }

        public static Dictionary<Color, int> GetColourFrequencies(Bitmap image)
        {
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
        }

        public static int CompareColorFrequencies(Dictionary<Color, int> frequencies1, Dictionary<Color, int> frequencies2)
        {
            int totalFrequency1 = frequencies1.Values.Sum();
            int totalFrequency2 = frequencies2.Values.Sum();
            double commonFrequency = 0;

            foreach (Color color in frequencies1.Keys)
                if (frequencies2.TryGetValue(color, out int frequency2))
                    commonFrequency += Math.Min(frequencies1[color], frequency2);

            double similarityPercentage = (commonFrequency / Math.Min(totalFrequency1, totalFrequency2)) * 100;

            return int.Parse(Math.Floor(similarityPercentage).ToString(CultureInfo.InvariantCulture));
        }
    }
}
