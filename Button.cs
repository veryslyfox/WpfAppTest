using System;
using System.Windows;
using System.Windows.Media;

namespace Objects
{
    class Button : IPixelMap
    {
        public Button(Int32Rect rect, Color color)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
            Color = color;
        }
        public Button(Int32Rect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
        public bool IsColored(int x, int y)
        {
            return !(x < X || x > X + Height || y < Y || y > Y + Width);
        }

        public Color GetColor(int x, int y)
        {
            return Color;
        }

        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Int32Rect Rect { get => new Int32Rect(X, Y, Width, Height); }
        public Color Color { get; }
    }
}