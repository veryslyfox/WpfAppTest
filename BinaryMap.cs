using System;
using System.Windows;
using System.Windows.Media;

namespace Objects.Data;
class Bitmap
{
    public void OnRandom(double prob)
    {
        Random random = new Random();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (random.NextDouble() < prob)
                {
                    Map[x, y] = true;
                }
            }
        }
    }
    public Bitmap(Style style, bool[,] map)
    {
        Map = map;
        Style = style;
        Height = map.GetLength(0) - 1;
        Width = map.GetLength(1) - 1;
    }
    public bool this[int x, int y]
    {
        get => Map[x, y];
        set => Map[x, y] = value;
    }
    public Bitmap(Style style, int width, int height)
    {
        Map = new bool[width, height];
        Width = width;
        Height = height;
        Style = style;
    }
    public void Horse()
    {
        void Offset(int x, int y, int xOffset, int yOffset)
        {
            if (x + xOffset >= 0 && y + yOffset >= 0 && x + xOffset <= Height - 1 && y + yOffset <= Width - 1)
            {
                return;
            }
            Map[x + xOffset, y + yOffset] = true;
        }
        Bitmap bitmap = new Bitmap(Style, Width, Height);
        for (int y = 0; y < Width; y++)
        {
            for (int x = 0;  x < Height; x++)
            {
                if (Map[x, y])
                {
                    Offset(x, y, 1, 2);
                    Offset(x, y, -1, 2);
                    Offset(x, y, 1, -2);
                    Offset(x, y, -1, -2);
                    Offset(x, y, 2, 1);
                    Offset(x, y, -2, 1);
                    Offset(x, y, 2, -1);
                    Offset(x, y, -2, -1);
                }
            }
        }
    }
    public bool[,] Map { get; }
    public Style Style { get; }
    public int Width { get; }
    public int Height { get; }

    public bool IsColored(int x, int y)
    {
        return Map[x / (Style.CellX + Style.Border), y / (Style.CellY + Style.Border)];
    }

    public Color GetColor(Point point)
    {
        bool IsBorderedDotX(int value)
        {
            return value % (Style.CellX + Style.Border) > Style.CellX;
        }
        bool IsBorderedDotY(int value)
        {
            return value % (Style.CellY + Style.Border) > Style.CellY;
        }
        if (Style.Border == 0)
        {
            return Style.Color;
        }
        if (IsBorderedDotX((int)point.X) || IsBorderedDotY((int)point.Y))
        {
            return Style.BorderColor;
        }
        return Style.Color;
        throw new System.NotImplementedException();
    }
}
class Style
{
    public Style(int cell, Color color)
    {
        CellX = cell;
        CellY = cell;
        Color = color;
    }
    public Style(int cellX, int cellY, Color color)
    {
        CellX = cellX;
        CellY = cellY;
        Color = color;
    }
    public Style(int cellX, int cellY, Color cellColor, int border, Color borderColor)
    {
        CellX = cellX;
        CellY = cellY;
        Color = cellColor;
        Border = border;
        BorderColor = borderColor;
    }
    public int CellX { get; }
    public int CellY { get; }
    public Color Color { get; }
    public int Border { get; }
    public Color BorderColor { get; }
}