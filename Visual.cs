global using System.Linq;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Objects.Data;

namespace Objects
{
    static class Rendering
    {
        public static void Render(this IPixelMap pixelMap, WriteableBitmap _bitmap)
        {
            _bitmap.Lock();
            for (int y = 0; y < _bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < _bitmap.PixelWidth; x++)
                {
                    if (pixelMap.IsColored(new Point(x, y)))
                    {
                        var color = pixelMap.GetColor(new Point(x, y));
                        var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                        unsafe
                        {
                            *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                        }
                    }
                }
            }
            _bitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            _bitmap.Unlock();
        }
        public static void Render(Bitmap map, WriteableBitmap bitmap)
        {

        }
    }
    struct Pixel
    {
        public Pixel(Point point, Color color)
        {
            Point = point;
            Color = color;
        }

        public Point Point { get; }
        public Color Color { get; }
    }
    public interface IPixelMap
    {
        bool IsColored(Point point);
        Color GetColor(Point point);
    }
    class Shape : IPixelMap
    {
        public Shape(Color color, params (int, int)[] points)
        {
            Points = points;
            Color = color;
        }
        public (int, int)[] Points { get; }
        public Color Color { get; }

        public bool IsColored(Point point)
        {
            var shape = new Vector[Points.Length];

            for (int i = 0; i < shape.Length - 1; i++)
            {
                shape[i] = new(Points[i].Item1, Points[i].Item2, Points[i + 1].Item1, Points[i + 1].Item2);
            }
            shape[shape.Length - 1] = new Vector(Points[shape.Length - 1].Item1, Points[shape.Length - 1].Item2, Points[0].Item1, Points[0].Item2);
            return Array.TrueForAll(shape, (Vector vector) => vector.DotRight(point));
        }
        public Color GetColor(Point point)
        {
            return Color;
        }
    }
    class Vector
    {
        public Vector(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }
        public Vector(Point begin, Point end)
        {
            X1 = ((int)begin.X);
            X2 = ((int)end.X);
            Y1 = ((int)begin.Y);
            Y2 = ((int)end.Y);
            Begin = begin;
            End = end;
        }
        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }
        public Point Begin { get; }
        public Point End { get; }
        public double Gradient { get => (Y2 - Y1) / (X2 - X1); }
        public static double Atan(Point point)
        {
            return Math.Atan2(point.Y, point.X);
        }
        public bool DotRight(Point point)
        {
            var vector = (System.Windows.Vector)Begin;
            var e = End - vector;
            var p = point - vector;
            return Atan(p) > Atan(e);
        }

    }
}
