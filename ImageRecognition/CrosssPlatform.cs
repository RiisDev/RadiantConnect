using System;
using System.Numerics; // Cross-platform alternative for color representation
// ReSharper disable CompareOfFloatsByEqualityOperator
#pragma warning disable CS9113 // Parameter is unread.

namespace RadiantConnect.ImageRecognition
{
#if !WINDOWS

    public readonly struct Size(int _, int __);

    public class Bitmap
    {
        public Bitmap()
        {
            throw new PlatformNotSupportedException("Bitmap is not supported on non-windows machines.");
        }
    }

    public readonly struct Color(byte r, byte g, byte b)
    {
        public byte R { get; } = r;
        public byte G { get; } = g;
        public byte B { get; } = b;

        public Vector3 ToVector3() => new(R, G, B);
    }

    public struct Point(int x, int y)
    {
        public int X { get; set; } = x;

        public int Y { get; set; } = y;
    }

    public readonly struct Rectangle(double x, double y, double width, double height)
    {
        public Rectangle(Point point, double width, double height) : this(point.X, point.Y, width, height)
        { }

        public double X => x;

        public double Y => y;

        public double Width => width;

        public double Height => height;

        public override bool Equals(object? obj)
        {
            if (obj is Rectangle rectangle)
                return this == rectangle;
            return false;
        }

        public override int GetHashCode()
        {
            return (int)(x + y + width + height);
        }

        public override string ToString()
        {
            return $"x:{x} y:{y} w:{width} h:{height}";
        }

        public static bool operator ==(Rectangle rectangle, Rectangle other)
        {
            return rectangle.X == other.X && rectangle.Y == other.Y && rectangle.Width == other.Width && rectangle.Height == other.Height;
        }

        public static bool operator !=(Rectangle rectangle, Rectangle other)
        {
            return !(rectangle == other);
        }
    }
#endif
}