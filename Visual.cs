using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
global using System.Linq
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
                        var color = pixelMap.GetColor(x, y);
                        var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                        unsafe
                        {
                            *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                        }
                    }
                }
                _bitmap.Unlock();
            }
        }

        public interface IPixelMap
        {
            bool IsColored(int x, int y);
            Color GetColor(int x, int y);
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

        public bool IsColored(int x, int y)
        {
            var shape = new Vector[Points.Length];
            
            for (int i = 0; i < shape.Length - 1; i++)
            {
                shape[i] = new(Points[i].Item1, Points[i].Item2, Points[i + 1].Item1, Points[i + 1].Item2);
            }
            shape[shape.Length - 1] = new Vector(Points[shape.Length - 1].Item1, Points[shape.Length - 1].Item2, Points[0].Item1, Points[0].Item2);
            return Array.TrueForAll(shape, (Vector vector) => vector.DotRight(x, y));
        }
        public Color GetColor(int x, int y)
        {
            return Color;
        } 
    }
    struct Vector
    {
        public Vector(int x1, int y1, int x2, int y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public int X1 { get; }
        public int Y1 { get; }
        public int X2 { get; }
        public int Y2 { get; }
        public double Angle { get => Math.Atan2(Y2 - Y1, X2 - X1); }
        public bool DotRight(int x, int y)
        {
            var angle = Math.Atan2(y - Y1, x - X1);
            return (angle > Angle) && (angle < Angle + 180);
        }
    }
}
