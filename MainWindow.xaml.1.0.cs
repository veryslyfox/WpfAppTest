using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using Button = Objects.Button;
using Objects;
using Objects.SpecialMath;
using Objects.Data;
using Style = Objects.Data.Style;
using Vector = Objects.Vector;
using static System.Math;
namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private double _time;
    private List<Button> _buttons = new List<Button>();
    private int _f;
    private Roller[] _rollers = new Roller[] { new Roller(0), new Roller(2 * Math.PI / 3), new Roller(4 * Math.PI / 3) };
    private Color[] _labs = new Color[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255) };
    private Color? _mouseColor;
    private Bitmap _map = new Bitmap(new Style(800, 800, Color.FromRgb(255, 255, 255)), 800, 800);
    private int[,] _field = new int[50, 50];
    private int[,] _norm = new int[800, 800];
    private double _m;

    public MainWindow()
    {
        _map[0, 400] = true;
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.1);
        _f = 1;
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ButtonHandler;
    }


    private void Tick(object? sender, EventArgs e)
    {
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                var c = SpecialMath.DotFromMandelbrotSet(x / 200.0F, y / 200.0F, 0, 0) * 8;
                Color color = FromRgb(c, c, c);
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {

                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _f++;
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }
    private void New2D(int disperse)
    {
        var i = 400;
        var j = 400;
        for (int n = 0; n < disperse; n++)
        {
            switch (_rng.Next(4))
            {
                case 0:
                    i++;
                    break;
                case 1:
                    i--;
                    break;
                case 2:
                    j++;
                    break;
                case 3:
                    j--;
                    break;
            }
        }
        _norm[i, j] += 255;
    }
    private void ButtonHandler(object sender, MouseEventArgs args)
    {
        _m -= 0.1;
    }

    private Color FromRgb(int r, int g, int b)
    {
        return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
    }
    private Color FromSRgb(int r, int g, int b)
    {
        return Color.FromRgb(Saturate(r), Saturate(g), Saturate(b));
    }
    public Color Normalize(Color color, byte lightness)
    {
        var r = color.R;
        var g = color.G;
        var b = color.B;
        var max = Math.Max(r, Math.Max(g, b));
        if (max == 0)
        {
            return Color.FromRgb(lightness, lightness, lightness);
        }
        var normalizer = (double)lightness / max;
        return FromSRgb((int)(r * normalizer), ((int)(g * normalizer)), ((int)(b * normalizer)));

    }
    public Color CHVToRgb(int cold, int hot, double value)
    {
        var c = cold / 256.0;
        var h = hot / 256.0;
        var v = value;
        var b = (c * v / (c + 2)) * 256;
        var g = ((h * b + 2 * b - 2 * v) / (2 - h)) * 256;
        var r = (v - b - g) * 256;
        return Color.FromRgb(((byte)r), ((byte)g), ((byte)b));
    }
    public Color Interpolation(Color a, Color b, byte c)
    {
        return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
    }
    public Color HsvToRgb(int h, byte s, byte v)
    {
        var result = new Color();
        var hue = h % 360;
        var hv = (hue % 60) * 255 / 60;
        var a = 0;
        var b = hv;
        var c = 255 - hv;
        var d = 255;
        void DoubleInterval(int min, int max, int r, int g, int b)
        {
            if (min <= hue && max > hue)
            {
                result = Normalize(FromRgb(r, g, b), v);
            }
        }
        DoubleInterval(0, 60, d, b, a);
        DoubleInterval(60, 120, c, d, a);
        DoubleInterval(120, 180, a, d, b);
        DoubleInterval(180, 240, a, c, d);
        DoubleInterval(240, 300, b, a, d);
        DoubleInterval(300, 360, d, a, c);
        return Interpolation(result, Color.FromRgb(v, v, v), s);
    }
    public int New(int mean, int disperse, double pad = 0.5)
    {
        var i = mean;
        for (int j = 0; j < disperse; j++)
        {
            if (_rng.NextDouble() < pad)
            {
                i++;
                continue;
            }
            i--;
        }
        return i;
    }
    private int GetRNeighborCount(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] < 0)
        {
            count++;
        }

        if (row > 0 && _field[column, row - 1] < 0)
        {
            count++;
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] < 0)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] < 0)
        {
            count++;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] < 0)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] < 0)
        {
            count++;
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] < 0)
        {
            count++;
        }

        if (column > 0 && _field[column - 1, row] < 0)
        {
            count++;
        }

        return count;
    }
    private int GetGNeighborCount(int column, int row)
    {
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] > 0)
        {
            count++;
        }

        if (row > 0 && _field[column, row - 1] > 0)
        {
            count++;
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] > 0)
        {
            count++;
        }

        if (column < width - 1 && _field[column + 1, row] > 0)
        {
            count++;
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] > 0)
        {
            count++;
        }

        if (row < height - 1 && _field[column, row + 1] > 0)
        {
            count++;
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] > 0)
        {
            count++;
        }

        if (column > 0 && _field[column - 1, row] > 0)
        {
            count++;
        }

        return count;
    }
    private int NeighborhoodActive(int column, int row, Action<int, int> action)
    {
        void S(int columnOffset, int rowOffset)
        {
            action(column + columnOffset, row + rowOffset);
        }
        var width = _field.GetLength(0);
        var height = _field.GetLength(1);
        var count = 0;
        if (row > 0 && column > 0 && _field[column - 1, row - 1] > 0)
        {
            S(-1, -1);
        }

        if (row > 0 && _field[column, row - 1] > 0)
        {
            S(0, -1);
        }

        if (row > 0 && column < width - 1 && _field[column + 1, row - 1] > 0)
        {
            S(1, -1);
        }

        if (column < width - 1 && _field[column + 1, row] > 0)
        {
            S(1, 0);
        }

        if (column < width - 1 && row < height - 1 && _field[column + 1, row + 1] > 0)
        {
            S(1, 1);
        }

        if (row < height - 1 && _field[column, row + 1] > 0)
        {
            S(0, 1);
        }

        if (row < height - 1 && column > 0 && _field[column - 1, row + 1] > 0)
        {
            S(-1, 1);
        }

        if (column > 0 && _field[column - 1, row] > 0)
        {
            S(-1, 0);
        }

        return count;
    }
    private void Next(int column, int row)
    {
        var value = _field[column, row];
        if (value > 0)
        {
            _field[column, row]--;
            return;
        }
        if (value < 0)
        {
            NeighborhoodActive(column, row, (int a, int b) => { if (_field[a, b] > 0) { _field[a, b] = 0; _field[column, row]--; } });
            return;
        }
        if (value == 0)
        {
            if (GetRNeighborCount(column, row) > 0)
            {
                _field[column, row] = -3;
                return;
            }
            if (GetGNeighborCount(column, row) > 0)
            {
                _field[column, row] = 6;
                return;
            }
        }
    }
    private byte Saturate(int value)
    => value >= 0
        ? value <= 255
            ? (byte)value
            : (byte)255
        : (byte)0;
}
