using static System.Math;
using System;
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
                    var (r, g, b) = ((byte)_f, (byte)0, (byte)_f);
                    var hsv = new MyColor(r, g, b).ToHsva();
                    
                    unsafe
                    {
                        *((int*)ptr) = (hsv.R << 16) | (hsv.G << 8) | hsv.B;
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
            var width = 100;
            var height = 100;
            var x = 0;
            var y = 0;
        }
    }
}