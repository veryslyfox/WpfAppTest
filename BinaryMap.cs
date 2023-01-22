using System;
using System.Windows.Media;

namespace Objects.Data;
class BinaryMap : IPixelMap
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
    public BinaryMap(Style style, bool[,] map)
    {
        Map = map;
        Style = style;
        Height = map.GetLength(0);
        Width = map.GetLength(1);
    }
    public BinaryMap(Style style, int width, int height)
    {
        Map = new bool[height, width];
        Width = width;
        Height = height;
        Style = style;
    }
    public bool[,] Map { get; }
    public Style Style { get; }
    public int Width { get; }
    public int Height { get; }

    public bool IsColored(int x, int y)
    {
        return true;
    }

    public Color GetColor(int x, int y)
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
        if (IsBorderedDotX(x) || IsBorderedDotY(y))
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