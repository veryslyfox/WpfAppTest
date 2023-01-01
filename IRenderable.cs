using System;
using System.Windows.Media.Imaging;
namespace Objects
{
    namespace Visual
    {
        static class Rendering
        {
            public static void Render(WriteableBitmap bitmap, Shape shape)
            {

            }
        }
    }
    class Shape
    {
        public Shape(params (int, int)[] points)
        {
            Points = points;
            
        }
        public (int, int)[] Points { get; }
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
        public int DotRight(int x, int y)
        {
            var angle = Math.Atan2(y - Y1, x - X1);
        }
    }
}
