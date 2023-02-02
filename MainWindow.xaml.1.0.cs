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

namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
    private double _time;
    private List<Button> _buttons = new List<Button>();
    private Color _color;
    private int _f;
    private byte _hue;
    private byte _saturation;
    private byte _value;
    private Color[] _labs = new Color[] { Color.FromRgb(255, 0, 0), Color.FromRgb(0, 255, 0), Color.FromRgb(0, 0, 255) };
    private Color? _mouseColor;
    public MainWindow()
    {
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.1);
        _timer.Tick += Tick;
        _timer.Start();
        MouseLeftButtonDown += ButtonHandler;
    }

    private void ButtonHandler(object sender, MouseEventArgs args)
    {
        var position = args.GetPosition(this);
        int x = (int)position.X;
        int y = (int)position.Y;
        _f += 10;
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
        var v = value * 3;
        var b = (c * v / (c + 2)) * 256;
        var g = ((h * b + 2 * b - 2 * v) / (2 - h)) * 256;
        var r = (v - b - g) * 256;
        return Color.FromRgb(((byte)r), ((byte)g), ((byte)b));
    }
    public Color Interpolation(Color a, Color b, byte c)
    {
        return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
    }
    public Color HueToRgb(int h, byte s, byte v)
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
    private void Tick(object? sender, EventArgs e)
    {
        _bitmap.Lock();
        var vector = new Vector(0, 0, 100, 100);
        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                if((x ^ _f) == y)
                {
                    var color = FromRgb(255, 255, 255);
                    var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                    unsafe
                    {
                        *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                    }
                }
            }
        }
        _f++;
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }

    private byte Saturate(int value)
    => value >= 0
        ? value <= 255
            ? (byte)value
            : (byte)255
        : (byte)0;
}
