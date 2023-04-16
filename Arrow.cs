// using System;
// using System.Diagnostics;
// using System.Windows;
// using System.Windows.Media;
// using System.Windows.Media.Imaging;
// using System.Windows.Threading;
// using System.Collections.Generic;
// using System.Windows.Input;
// using Button = Objects.Button;
// using Objects;
// using Objects.SpecialMath;
// using Objects.Data;
// using Style = Objects.Data.Style;
// using Vector = Objects.Vector;
// using static System.Math;
// using static System.Numerics.Complex;
// using System.Numerics;
// using Objects.VolumeObjects;
// using System.Threading;

// namespace WpfApp1;

// public partial class MainWindow : Window
// {
//     private readonly DispatcherTimer _timer = new();
//     private readonly WriteableBitmap _bitmap;
//     private readonly Random _rng = new();
//     private static readonly double TimeStep = 1.0 / Stopwatch.Frequency;
//     public MainWindow()
//     {
//         InitializeComponent();
//         _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
//         image.Source = _bitmap;
//         _timer.Interval = TimeSpan.FromSeconds(0.000001);
//         _timer.Tick += Tick;
//         _timer.Start();
//         MouseLeftButtonDown += ClickHandler;
//     }
//     private void Tick(object? sender, EventArgs e)
//     {
//         Thread.Sleep(1000);
//         _bitmap.Lock();
//         for (int y = 0; y < _bitmap.PixelHeight; y++)
//         {
//             for (int x = 0; x < _bitmap.PixelWidth; x++)
//             {
//                 Color color = FromRgb(0, 0, 0);
//                 var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
//                 unsafe
//                 {
//                     *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
//                 }
//             }
//         }
//         _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelHeight, _bitmap.PixelHeight));
//         _bitmap.Unlock();
//     }
//     private void ClickHandler(object sender, MouseEventArgs args)
//     {
//     }
//     private Color FromRgbSaturated(int r, int g, int b)
//     {
//         if (r < 0)
//         {
//             r = 0;
//         }
//         else if (r > 255)
//         {
//             r = 255;
//         }

//         if (g < 0)
//         {
//             g = 0;
//         }
//         else if (g > 255)
//         {
//             g = 255;
//         }

//         if (b < 0)
//         {
//             b = 0;
//         }
//         else if (b > 255)
//         {
//             b = 255;
//         }

//         return Color.FromRgb((byte)r, (byte)g, (byte)b);
//     }
//     private Color FromRgb(int r, int g, int b)
//     {
//         return Color.FromRgb(((byte)(r & 255)), ((byte)(g & 255)), ((byte)(b & 255)));
//     }
//     public Color Normalize(Color color, byte lightness)
//     {
//         var r = color.R;
//         var g = color.G;
//         var b = color.B;
//         var max = Math.Max(r, Math.Max(g, b));
//         if (max == 0)
//         {
//             return Color.FromRgb(lightness, lightness, lightness);
//         }
//         var normalizer = (double)lightness / max;
//         return FromRgb((int)(r * normalizer), ((int)(g * normalizer)), ((int)(b * normalizer)));
//     }
//     public Color Interpolation(Color a, Color b, byte c)
//     {
//         return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
//     }
//     public Color HsvToRgb(int h, byte s, byte v)
//     {
//         var result = new Color();
//         var hue = h % 360;
//         var hv = (hue % 60) * 255 / 60;
//         var a = 0;
//         var b = hv;
//         var c = 255 - hv;
//         var d = 255;
//         void DoubleInterval(int min, int max, int r, int g, int b)
//         {
//             if (min <= hue && max > hue)
//             {
//                 result = Normalize(FromRgb(r, g, b), v);
//             }
//         }
//         DoubleInterval(0, 60, d, b, a);
//         DoubleInterval(60, 120, c, d, a);
//         DoubleInterval(120, 180, a, d, b);
//         DoubleInterval(180, 240, a, c, d);
//         DoubleInterval(240, 300, b, a, d);
//         DoubleInterval(300, 360, d, a, c);
//         return Interpolation(result, Color.FromRgb(v, v, v), s);
//     }
// }
// class Arrow
// {
//     public Arrow(Point a, Point b) // A >> B
//     {
//         A = a;
//         B = b;
//     }
//     public Arrow[] Next(Arrow arrow)
//     {
//         return new Arrow[]{arrow * R90};
//     }
//     public static Arrow operator*(Arrow arrow, Matrix matrix)
//     {
//         return new(arrow.A * matrix, arrow.B * matrix);
//     }
//     public Point A { get; }
//     public Point B { get; }
//     static Matrix R90 = new Matrix();
// }