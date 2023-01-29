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
    public Color HueToRgb(int h)
    {
        var result = new Color();
        var hue = h % 360;
        var hv = (hue % 120) * 256 / 120;
        var a = 0;
        var b = hv;
        var c = 255 - hv;
        var d = 255;
        void DoubleInterval(int min, int max, int r, int g, int b)
        {
            if (min < hue && max < hue)
            {
                result = FromRgb(r, g, b);
            }
        }
        DoubleInterval(0, 60, d, b, a);
        DoubleInterval(60, 120, c, d, a);
        DoubleInterval(120, 180, a, d, b);
        DoubleInterval(180, 240, a, c, b);
        DoubleInterval(240, 300, b, a, d);
        DoubleInterval(300, 360, d, a, c);
        return result;
    }
    private void Tick(object? sender, EventArgs e)
    {

        _bitmap.Lock();

        for (int y = 0; y < _bitmap.PixelHeight; y++)
        {
            for (int x = 0; x < _bitmap.PixelWidth; x++)
            {
                
                var color = Normalize(HueToRgb(x), 255);
                var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                unsafe
                {
                    *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                }
            }
        }
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        _bitmap.Unlock();
    }

    private byte Saturate(int value)
    => value >= 0
        ? value <= 255
            ? (byte)value
            : (byte)255
        : (byte)0;
    private class ColorElement
    {
        public ColorElement()
        {
            
        }
    }
}
