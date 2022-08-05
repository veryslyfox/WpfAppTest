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
            _bitmap = new(400, 400, 96, 96, PixelFormats.Bgr32, null);
            image.Source = _bitmap;
            _timer.Interval = TimeSpan.FromSeconds(0.01);
            _timer.Tick += Tick;
            _timer.Start();
        }

        private void Tick(object? sender, EventArgs e)
        {
            try
            {
                _bitmap.Lock();
                for (int i = 0; i < 100000; i++)
                {
                    var x = _rng.Next(_bitmap.PixelWidth);
                    var y = _rng.Next(_bitmap.PixelHeight);
                    var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
                    var (r, g, b) = (_f % 256, _f % 256, _f % 256);
                    unsafe
                    {
                        *((int*)ptr) = (r << 16) | (g << 8) | b;
                    }
                }

                _bitmap.AddDirtyRect(new(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
            }
            finally 
            { 
                _bitmap.Unlock();
            }
            ++_f;
        }
    }
}
