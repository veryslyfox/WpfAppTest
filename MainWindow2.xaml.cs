using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Windows.Input;
using System.IO;

namespace WpfApp1;

public partial class MainWindow : Window
{
    private readonly DispatcherTimer _timer = new();
    private readonly WriteableBitmap _bitmap;
    private readonly Random _rng = new();
    private int _f;
    private Color[,] _image;
    private Point3Collection _points = new(new Point3[10000]);
    public MainWindow()
    {
        for (int i = 0; i < _points.Points.Length; i++)
        {
            _points.Points[i] = new Point3(_rng.Next(-200, 200), _rng.Next(-200, 200), _rng.Next(-200, 200), _rng.Next(16777216));
        }
        _image = new Color[1000, 1000];
        InitializeComponent();
        _bitmap = new((int)image.Width, (int)image.Height, 96, 100, PixelFormats.Bgr32, null);
        image.Source = _bitmap;
        _timer.Interval = TimeSpan.FromSeconds(0.00001);
        _timer.Tick += Tick;
        _timer.Start();
    }
    private void AddRect()
    {
        byte Grad(int v1, int v2, int v3, int v4, double xPos, double yPos)
        {
            return
            (byte)
            ((v1 * xPos + v2 * (1 - xPos)) * yPos +
            (v3 * xPos + v4 * (1 - xPos)) * (1 - yPos));
        }
        var r1 = _rng.Next(256);
        var r2 = _rng.Next(256);
        var r3 = _rng.Next(256);
        var r4 = _rng.Next(256);
        var g1 = _rng.Next(256);
        var g2 = _rng.Next(256);
        var g3 = _rng.Next(256);
        var g4 = _rng.Next(256);
        var b1 = _rng.Next(256);
        var b2 = _rng.Next(256);
        var b3 = _rng.Next(256);
        var b4 = _rng.Next(256);
        var x = _rng.Next(900);
        var y = _rng.Next(900);
        var width = _rng.Next(100);
        var height = _rng.Next(100);
        for (int j = y; j < y + width; j++)
        {
            for (int i = x; i < x + height; i++)
            {
                var jValue = (j - y + 0.0) / width;
                var iValue = (i - x + 0.0) / height;
                _image[i, j] = FromRgb(Grad(r1, r2, r3, r4, iValue, jValue),
                Grad(g1, g2, g3, g4, iValue, jValue),
                Grad(b1, b2, b3, b4, iValue, jValue));
            }
        }
    }
    private void Tick(object? sender, EventArgs e)
    {
        _bitmap.Lock();
        // if (_f % 100 == 0)
        // {
        //     unsafe
        //     {
        //         System.Runtime.InteropServices.NativeMemory.Fill((void*)_bitmap.BackBuffer, (nuint)(_bitmap.BackBufferStride * _bitmap.PixelHeight), 0);
        //     }
        // }
        _points.Draw(_bitmap, Matrix3.GetRotateMatrix(0, _f / 1000.0, _f / 1000.0, 400, 400, 400), 255, 255, 255);
        // for (int y = 0; y < _bitmap.PixelHeight; y++)
        // {
        //     for (int x = 0; x < _bitmap.PixelWidth; x++)
        //     {
        //         var color = HsvToRgb(x, 255, 255) + HsvToRgb(y, 255, 255);
        //         var ptr = _bitmap.BackBuffer + x * 4 + _bitmap.BackBufferStride * y;
        //         unsafe
        //         {
        //             *((int*)ptr) = (color.R << 16) | (color.G << 8) | (color.B);
        //         }
        //     }
        // }
        _f++;
        _bitmap.AddDirtyRect(new Int32Rect(0, 0, _bitmap.PixelWidth, _bitmap.PixelHeight));
        _bitmap.Unlock();
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
    public Color Interpolation(Color a, Color b, byte c)
    {
        return FromRgb((a.R * c + b.R * (255 - c)) / 255, (a.G * c + b.G * (255 - c)) / 255, (a.B * c + b.B * (255 - c)) / 255);
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
        return Color.FromRgb((byte)(r * normalizer), ((byte)(g * normalizer)), ((byte)(b * normalizer)));

    }
}
class ParticleSystem
{
    public ParticleSystem(Particle2[] particles)
    {
        Particles = particles;
    }
    public static ParticleSystem Base;

    public Particle2[] Particles { get; }
}
class Particle2
{
    public Particle2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; }
    public double Y { get; }
    public void GetLink()
    {
        foreach (var particle in ParticleSystem.Base.Particles)
        {
            var sqDist = Math.Pow(particle.X - X, 2) + Math.Pow(particle.Y - Y, 2);
        }
    }
}
class ParticleLink
{
    public ParticleLink(Particle2 a, Particle2 b)
    {
        A = a;
        B = b;
    }

    public Particle2 A { get; }
    public Particle2 B { get; }
}
class Point3Collection
{
    public Point3Collection(Point3[] points)
    {
        Points = points;
    }
    public unsafe void Draw(WriteableBitmap bitmap, Matrix3 matrix, byte r, byte g, byte b)
    {
        foreach (var point in Points)
        {
            var newPoint = point * matrix;
            var ptr = bitmap.BackBuffer + newPoint.X * 4 + bitmap.BackBufferStride * newPoint.Y;
            *((int*)ptr) = point.Color;
        }
    }
    public Point3[] Points { get; }
}
struct Point3
{
    public Point3(int x, int y, int z, int color = 0)
    {
        X = x;
        Y = y;
        Z = z;
        Color = color;
    }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    public int Color;
}
class Matrix3
{
    public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33, double v1, double v2, double v3)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M31 = m31;
        M32 = m32;
        M33 = m33;
        V1 = v1;
        V2 = v2;
        V3 = v3;
    }

    public double M11 { get; }
    public double M12 { get; }
    public double M13 { get; }
    public double M21 { get; }
    public double M22 { get; }
    public double M23 { get; }
    public double M31 { get; }
    public double M32 { get; }
    public double M33 { get; }
    public double V1 { get; }
    public double V2 { get; }
    public double V3 { get; }
    public static Point3 operator *(Point3 point, Matrix3 matrix)
    {
        return new((int)Math.Round(point.X * matrix.M11 + point.Y * matrix.M12 + point.Z * matrix.M13 + matrix.V1), (int)Math.Round(point.X * matrix.M21 + point.Y * matrix.M22 + point.Z * matrix.M23 + matrix.V2), (int)Math.Round(point.X * matrix.M31 + point.Y * matrix.M32 + point.Z * matrix.M33 + matrix.V3));
    }
    public static Matrix3 operator *(Matrix3 left, Matrix3 right)
    {
        return new(
left.M11 * right.M11 + left.M21 * right.M12 + left.M31 * right.M13,
left.M11 * right.M21 + left.M21 * right.M22 + left.M31 * right.M23,
left.M11 * right.M31 + left.M21 * right.M32 + left.M31 * right.M33,
left.M12 * right.M11 + left.M22 * right.M12 + left.M32 * right.M13,
left.M12 * right.M21 + left.M22 * right.M22 + left.M32 * right.M23,
left.M12 * right.M31 + left.M22 * right.M32 + left.M32 * right.M33,
left.M13 * right.M11 + left.M23 * right.M12 + left.M33 * right.M13,
left.M13 * right.M21 + left.M23 * right.M22 + left.M33 * right.M23,
left.M13 * right.M31 + left.M23 * right.M32 + left.M33 * right.M33,
left.V1 + right.V1,
left.V2 + right.V2,
left.V3 + right.V3
);

    }
    //     (
    //     x
    //     1 0 0
    //     0 cos a -sin a
    //     0 sin a cos a
    //     y
    //     cos y 0 sin y
    //     0 1 0
    //     -sin y 0 cos y
    //     z
    //     cos z -sin z 0
    //     sin z cos z 0
    //     0 0 1
    // )

    public static Matrix3 GetRotateMatrix(double x, double y, double z, double offsetX = 0, double offsetY = 0, double offsetZ = 0)
    {
        var sx = Math.Sin(x);
        var sy = Math.Sin(y);
        var sz = Math.Sin(z);
        var cx = Math.Cos(x);
        var cy = Math.Cos(y);
        var cz = Math.Cos(z);
        return new Matrix3(1, 0, 0, 0, cx, -sx, 0, sx, cx, offsetX, offsetY, offsetZ) * new Matrix3(cy, 0, sy, 0, 1, 0, -sy, 0, cy, 0, 0, 0) * new Matrix3(cz, -sz, 0, sz, cz, 0, 0, 0, 1, 0, 0, 0);
    }
}