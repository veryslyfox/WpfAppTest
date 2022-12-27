global using System.Windows.Media;
global using System;
using static System.Math;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace WpfApp1
{


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly DispatcherTimer _timer = new();
        private readonly WriteableBitmap _bitmap;
        private readonly Random _rng = new();
        private Dot[,] _dots = new Dot[100, 100];
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
            // var redX = _f;
            // var redY = _f;
            // var blueX = 1000 - _f;
            // var blueY = 1000 - _f;
            // var greenX = _f;
            // var greenY = 1000 - _f;
            var redX = 500;
            var redY = 300;
            var blueX = 500;
            var blueY = 600;
            var greenX = 450;
            var greenY = 700;
            _bitmap.Lock();

            for (int i = 0; i < _bitmap.PixelWidth; i++)
            {
                for (int j = 0; j < _bitmap.PixelHeight; j++)
                {
                    var ptr = _bitmap.BackBuffer + i * 4 + _bitmap.BackBufferStride * j;
                    var red = (byte)(((i - redX) * (i - redX) + (j - redY) * (j - redY)) & 255);
                    var blue = (byte)(((i - blueX) * (i - blueX) + (j - blueY) * (j - blueY)) & 255);
                    var green = (byte)(((i - greenX) * (i - greenX) + (j - greenY) * (j - greenY)) & 255);
                    var color = FromRgb(red, red, red);
                    unsafe
                    {
                        *((int*)ptr) = (color.R << 16) | (color.G << 8) | color.B;
                    }
                }
            }
            _f++;
            _bitmap.AddDirtyRect(new(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            _bitmap.Unlock();
        }
        private void DrawRect(int x, int y, int height, int width, Color color)
        {
            _bitmap.Lock();
            for (int i = x; i < x + height; i++)
            {
                for (int j = y; j < y + width; j++)
                {
                    var ptr = _bitmap.BackBuffer + i * 4 + _bitmap.BackBufferStride * j;
                    unsafe
                    {
                        *((int*)ptr) = (color.R << 16) | (color.G << 8) | color.B;
                    }
                }
            }
            _bitmap.Unlock();
        }
        private void Tick2(object? sender, EventArgs e)
        {
            _bitmap.Lock();
            var aX = 250;
            var aY = 500;
            var bX = 625;
            var bY = 216;
            var cX = 625;
            var cY = 784;
            for (int k = 0; k < 1000; k++)
            {
                var i = 500;
                var j = 500;
                var ptr = _bitmap.BackBuffer + i * 4 + _bitmap.BackBufferStride * j;
                var dotX = 0;
                var dotY = 0;
                switch (new Random().Next(3))
                {
                    case 0:
                        dotX = aX;
                        dotY = aY;
                        break;
                    case 1:
                        dotX = bX;
                        dotY = bY;
                        break;
                    case 2:
                        dotX = cX;
                        dotY = cY;
                        break;
                }
                unsafe
                {
                    *((int*)ptr) = 1 << 16;
                }
                i = (i + dotX) / 2;
                j = (j + dotY) / 2;
            }
            _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            _bitmap.Unlock();
        }
        public Color FromRgb(int r, int g, int b, bool isMonoChromed = false)
        {
            return Color.FromRgb((byte)(r & 255), (byte)(g & 255), (byte)(b & 255));
        }
    }
}
