global using System.Windows.Media;
using System.IO;

static class Pictures
{
    public static Sprite ReadFile(string file, int width, int height)
    {
        var result = new Sprite(height, width);
        var reader = new StreamReader(File.Open(file, FileMode.Open));
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                var p = reader.Peek();
                var a = p % 256;
                var r = p / 256 % 256;
                var g = p / 65536 % 256;
                var b = p / 16777216;
                result[i, j] = Color.FromArgb((byte)a, (byte)r, (byte)g, (byte)b);
            }
        }
        return result;
    }
}
class Sprite
{
    public Sprite(int width, int height)
    {
        Colors = new Color[width, height];
    }
    public Sprite(Color[,] colors)
    {
        Colors = colors;
    }
    public Color this[int x, int y]
    {
        get => Colors[x, y];
        set => Colors[x, y] = value;
    }
    public Color[,] Colors { get; }
}