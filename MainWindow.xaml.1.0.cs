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
    public MainWindow()
    {
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.00001);
        _timer.Tick += Tick;
        _timer.Start();
        
    }

    // private void ButtonHandler(object sender, MouseEventArgs args)
    // {
    //     var position = args.GetPosition(this);
    //     int x = (int)position.X;
    //     int y = (int)position.Y;
    //     foreach (var button in _buttons)
    //     {
    //         if (button.OnPoint(x, y))
    //         {
    //             _color = FromRgb(x, 0, y);
    //         }
    //     }
    // }

    private Color FromRgb(int r, int g, int b)
    {
        return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
    }

    private void Tick(object? sender, EventArgs e)
    {
        _bitmap.Lock();
        var color = FromRgb(255, 255, 255);
        var vector = new Objects.Vector(0, 0, 200, 200);
        foreach (var item in _buttons)
        {
            for (int y = 0; y < _bitmap.PixelHeight; y++)
            {
                for (int x = 0; x < _bitmap.PixelWidth; x++)
                {
                    if (vector.DotRight(x, y))
                    {
                        var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                        unsafe
                        {
                            *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
                        }
                    }
                }
            }
        }
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelWidth));
        _bitmap.Unlock();
    }

    private byte Saturate(int value)
    => value >= 0
        ? value <= 255
            ? (byte)value
            : (byte)255
        : (byte)0;
}
