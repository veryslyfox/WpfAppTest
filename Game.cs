using System.Collections.Generic;
using System.Windows.Media;

static class Params
{
    public static int CellSize = 42;
}
class Rect
{
    public Rect(int x, int y, int x2, int y2)
    {
        X1 = x;
        Y1 = y;
        X2 = x2;
        Y2 = y2;
    }

    public int X1 { get; }
    public int Y1 { get; }
    public int X2 { get; }
    public int Y2 { get; }
    static bool IsLineCollide(int start1, int end1, int start2, int end2)
    {
        return !((start1 < start2) ^ (end1 < end2));
    }
    static bool GetLineCollision(int start1, int end1, int start2, int end2)
    {
        return !((start1 < start2) ^ (end1 < end2));
    }
    public static bool IsCollide(Rect a, Rect b)
    {
        return IsLineCollide(a.X1, a.X2, b.X1, b.X2) | IsLineCollide(a.Y1, a.Y2, b.Y1, b.Y2);
    }
}
class Level
{
    public Level(LevelElement[] field)
    {
        Field = field;
    }

    public LevelElement[] Field { get; }
    public List<LevelElement> GetImageElementList(Rect cameraPosition)
    {
        var result = new List<LevelElement>();
        foreach (var item in Field)
        {
            if (Rect.IsCollide(item.GetHitbox(), cameraPosition))
            {
                result.Add(item);
            }
        }
        return result;
    }
}
enum LevelElementType
{
    Block,
    Spike
}
class LevelElement
{
    public LevelElement(LevelElementType type, int x, int y)
    {
        Type = type;
        X = x;
        Y = y;
    }
    public Rect GetHitbox()
    {
        return new Rect(X, Y, X + Params.CellSize, Y + Params.CellSize);
    }
    public Color GetColor()
    {
        switch (Type)
        {
            case LevelElementType.Block:
                return Color.FromRgb(255, 255, 255);
            case LevelElementType.Spike:
                return Color.FromRgb(255, 0, 0);
        }
        Test.Log("Error 000: invalid object");
        return Color.FromRgb(255, 255, 0);
    }
    public LevelElementType Type { get; }
    public int X { get; }
    public int Y { get; }
}
static class Drawing
{
    public static Color[,] Image;
    public static void DrawRect(Rect rect, Color color)
    {
        var height = Image.GetLength(0);
        var width = Image.GetLength(1);
        for (int y = rect.Y1; y <= rect.Y2; y++)
        {
            for (int x = rect.X1; x <= rect.X2; x++)
            {
                Image[x, y] = color;
            }
        }
    }
}
