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
using static System.Math;
using static System.Numerics.Complex;
using System.Numerics;
using Objects.VolumeObjects;
using System.Threading;
using Vector = System.Windows.Vector;
using System.Windows.Controls;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private Color[] Colors = new Color[] { Color.FromRgb(0, 0, 0), Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255), Color.FromRgb(255, 255, 0), Color.FromRgb(255, 0, 255), Color.FromRgb(0, 255, 255), Color.FromRgb(0, 0, 0), Color.FromRgb(255, 255, 255), Color.FromRgb(127, 255, 127) };
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private int[,] _space = new int[800, 800];
    private int[] _ticks = new int[] { 128, 64, 32, 16, 8, 4, 2, 1 };
    private Point[] _points = new Point[] { new Point(400, 400) };
    private int _size = 100;
    Roller R1 = new Roller(PI / 3);
    Roller R2 = new Roller(-PI / 3);
    private double _f = 0;
    private int[] array = new int[] { 0, 1, 2, 3 };
    Dictionary<Direct, int> DirectX = new Dictionary<Direct, int>
    {
        {Direct.L, -1},
        {Direct.U, 0},
        {Direct.R, 1},
        {Direct.D, 0},
    };
    Dictionary<Direct, int> DirectY = new Dictionary<Direct, int>
    {
        {Direct.L, 0},
        {Direct.U, 1},
        {Direct.R, 0},
        {Direct.D, -1},
    };
    int[] Roll = new int[] { 1, 0, 1 };
    enum Direct
    {
        L,
        U,
        R,
        D,
    }
    List<LorenzDot> _attractor = new List<LorenzDot>();
    public MainWindow()
    {
        for (int i = 0; i < 100000; i++)
        {
            var r = 200;
            var a = _rng.NextDouble() * 2 * PI;
            var b = _rng.NextDouble() * 2 * PI;
            _attractor.Add(new((r * Sin(a) * Cos(b)), r * Sin(a) * Sin(b), r * Sin(a)));
        }
        
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.000001);
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ClickHandler;
        MouseLeftButtonUp += UpClickHandler;
        KeyDown += KeyHandler;
    }

    private void UpClickHandler(object sender, MouseButtonEventArgs e)
    {
    }
    private void KeyHandler(object sender, KeyEventArgs args)
    {
    }
    double F(double val)
    {
        if (val < 2)
        {
            return val;
        }
        else
        {
            return val * F(val - 1);
        }
    }
    public bool[,] Convolution(bool[,] image, int radius, int min)
    {

        var result = image;
        for (int row = radius; row < image.GetLength(1) - radius; row++)
        {
            for (int column = radius; column < image.GetLength(0) - radius; column++)
            {
                var sum = 0;
                for (int xMove = -radius; xMove <= radius; xMove++)
                {
                    for (int yMove = -radius; yMove <= radius; yMove++)
                    {
                        sum += image[column + xMove, row + yMove] ? 1 : 0;
                    }
                }
                result[column, row] = sum >= min;
            }
        }
        return result;
    }
    private void Tick(object? sender, EventArgs e)
    {
        foreach (var item in _attractor)
        {
            item.Next(_space);
        }
        _bitmap.Lock();
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                var c = 255 - _space[x, y];
                var color = FromRgb(c, c, c);
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelHeight));
        _bitmap.Unlock();
        _f += 1;
    }
    private void ClickHandler(object sender, MouseEventArgs args)
    {
    }
    private Color FromRgbSaturated(int r, int g, int b)
    {
        if (r < 0)
        {
            r = 0;
        }
        else if (r > 255)
        {
            r = 255;
        }

        if (g < 0)
        {
            g = 0;
        }
        else if (g > 255)
        {
            g = 255;
        }

        if (b < 0)
        {
            b = 0;
        }
        else if (b > 255)
        {
            b = 255;
        }

        return Color.FromRgb((byte)r, (byte)g, (byte)b);
    }
    private Color FromRgb(int r, int g, int b)
    {
        return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
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
        return FromRgb((int)(r * normalizer), ((int)(g * normalizer)), ((int)(b * normalizer)));
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
}
class LorenzDot
{
    public LorenzDot(double x, double y, double z, double dt = 0.001)
    {
        X = x;
        Y = y;
        Z = z;
        Dt = dt;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double Dt { get; }

    public void Next(int[,] proj)
    {
        proj[(int)(Y + 400), (int)(Abs(Z + 400))] = 0;
        X += 100 * (Y - X) * Dt;
        Y += (X * (28 - Z) - Y) * Dt;
        Z += (X * Y + Z * 8 / 3) * Dt;
        proj[(int)(Abs(Y + 400)), (int)(Abs(Z + 400))] = 255;
    }
}