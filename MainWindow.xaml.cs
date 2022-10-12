using static System.Math;
using System;
using System.Media;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    struct Dot
    {
        public Dot(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }
    class PixelDraw
    {
        public PixelDraw(Dot dot, HsvaColor color)
        {
            X = dot.X;
            Y = dot.Y;
            H = color.Hue;

        }

        public int X { get; }
        public int Y { get; }
        public float H { get; }
        public float S { get; }
        public float V { get; }
        public float A { get; }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new();
        private readonly WriteableBitmap _bitmap;
        private readonly Random _rng = new();
        private int _f;

        public MainWindow()
        {
            SoundPlayer soundPlayer = new SoundPlayer();
            
            InitializeComponent();
            _bitmap = new(400, 400, 96, 100, PixelFormats.Bgr32, null);
            image.Source = _bitmap;
            _timer.Interval = TimeSpan.FromSeconds(0.00001);
            _timer.Tick += Tick;
            _timer.Start();
        }
        private void PixelsDraw(HsvaColor[,] hsvaColors)
        {
            _bitmap.Lock();
            for (int i = 0; i < _bitmap.PixelWidth; i++)
            {
                for (int j = 0; j < _bitmap.PixelHeight; j++)
                {
                    HsvaColor? item = hsvaColors[i, j];
                    var ptr = _bitmap.BackBuffer + i * 4 + _bitmap.BackBufferStride * j;
                    var color = item.ToRgba();
                    unsafe
                    {
                        *((int*)ptr) = (color.R << 16) | (color.G << 8) | color.B;
                    }

                }
            }
            _bitmap.Unlock();
        }
        private void Tick(object? sender, EventArgs e)
        {
            try
            {
                _bitmap.Lock();
                for (int i = 0; i < 100; i++)
                {
                    var x = _rng.Next(_bitmap.PixelWidth);
                    var y = _rng.Next(_bitmap.PixelHeight);
                    var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                    var (r, g, b) = (_f, 0, _f);
                    unsafe
                    {
                        *((int*)ptr) = (r << 16) | (g << 8) | b;
                    }

                    _bitmap.AddDirtyRect(new(x, y, 1, 1));
                }
            }
            finally
            {
                _bitmap.Unlock();
            }
            _f += 20;
        }
        private static void Tick2(object? sender, EventArgs e)
        {
            
        }
    }
}
class HsvaColor
{
    public HsvaColor(byte red, byte green, byte blue, byte alpha = 0)
    {
        float r = red / 255;
        float g = green / 255;
        float b = blue / 255;
        var max = Max(Max(r, g), b);
        var min = Min(Min(r, g), b);
        if (max == r)
        {
            Hue = 60 * (g - b) / (max - min)
            + g < b ? 360 : 0;
        }
        else if (max == g)
        {
            Hue = 60 * (b - r) / (max - min)
            + 120;
        }
        else if (max == b)
        {
            Hue = 60 * (r - g) / (max - min) + 240;
        }
        Saturation = max == 0 ? 0 : 1 - min / max;
        Value = max;
        Alpha = alpha;
    }
    public Color ToRgba()
    {
        byte red;
        byte green;
        byte blue;
        byte alpha;
        var s = Saturation * 100;
        var v = Value * 100;
        var hi = (int)Floor(Hue / 60) % 6;
        var vmin = (100 - s) * v / 100;
        var a = (v - vmin) * (Hue % 60) / 60;
        var vinc = vmin + a;
        var vdec = v - a;
        var indexArray = new int[] { 0, 3, 1, 1, 2, 0, 2, 0, 0, 3, 1, 1, 1, 1, 2, 0, 0, 3 };
#pragma warning disable
        byte ComponentValue(int component)
        {
            var array = new float[] { v, vmin, vinc, vdec };

            return (byte)(array[indexArray[hi + 6 * component]] * 255 / 100);
        }
#pragma warning restore
        red = ComponentValue(0);
        blue = ComponentValue(1);
        green = ComponentValue(2);
        alpha = Alpha;
        return Color.FromArgb(alpha, red, green, blue);
    }
    public float Hue;
    public float Saturation;
    public float Value;
    public byte Alpha;
}