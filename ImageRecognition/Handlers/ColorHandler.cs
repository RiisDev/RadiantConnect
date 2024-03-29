using System.Drawing;

namespace RadiantConnect.ImageRecognition.Handlers
{
    internal static class ColorHandler
    {
        internal static bool IsAccentBarColour(Color color)
        {
            //return color is { R: < 220, G: < 220, B: < 220 } and { R: > 130, G: > 130, B: > 130 } or {R: 125, G: 125, B: 125};
            //return color is { R: >= 238 and <= 240, G: >= 238 and <= 240, B: >= 238 and <= 240 };
            return color is { R: 237, G: 237, B: 237 } or { R: 239, G: 239, B: 239 } or { R: < 220, G: < 220, B: < 220 } and { R: > 130, G: > 130, B: > 130 } or { R: 125, G: 125, B: 125 } or { R: >= 238 and <= 240, G: >= 238 and <= 240, B: >= 238 and <= 240 };
        }

        internal static bool IsValorantGreen(Color color)
        {
            return color is { R: < 103, G: < 197, B: < 170 } and { R: >= 99, G: > 190, B: > 165 };
        }

        internal static bool IsValorantRed(Color color)
        {
            return color is { R: >= 235 and <= 241, G: 90, B: 85 } or { R: 208, G: 99, B: 91 };
        }

        internal static bool IsActionColor(Color color)
        {
            return color is { R: >= 225 and <= 238, G: >= 231 and <= 238, B: >= 115 and <= 150 };
        }
    }
}
