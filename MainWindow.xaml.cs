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
        private byte _f;

        public MainWindow()
        {
            InitializeComponent();
            _bitmap = new(1000, 1000, 96, 100, PixelFormats.Bgr32, null);
            image.Source = _bitmap;
            _timer.Interval = TimeSpan.FromSeconds(0.00001);
            _timer.Tick += Tick;
            _timer.Start();
        }
        private void Tick(object? sender, EventArgs e)
        {
            var redX = 300;
            var redY = 500;
            var blueX = 600;
            var blueY = 500;
            _bitmap.Lock();
            for (int i = 0; i < _bitmap.PixelWidth; i++)
            {
                for (int j = 0; j < _bitmap.PixelHeight; j++)
                {
                    var ptr = _bitmap.BackBuffer + i * 4 + _bitmap.BackBufferStride * j;
                    var red = ((i - redX) * (i - redX) + (j - redY) * (j - redY)) % 256;
                    var blue = ((i - blueX) * (i - blueX) + (j - blueY) * (j - blueY)) % 256;
                    var color = Color.FromRgb((byte)red, 0, (byte)blue);
                    unsafe
                    {
                        *((int*)ptr) = (color.R << 16) | (color.G << 8) | color.B;
                    }

                }
            }
            _bitmap.Unlock();
        }
    }
}
