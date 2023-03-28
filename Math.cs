namespace Objects.SpecialMath;
using static System.Math;
using static System.MathF;
using static System.Numerics.Complex;
using System.Windows;
using System;
using System.Numerics;
using System.Windows.Media;
using VolumeObjects;
static class SpecialMath
{
    public static Point XYToPolar(Point point)
    {
        return new Point(point.X * point.X + point.Y * point.Y, Math.Atan2(point.Y, point.X));
    }
    public static Point PolarToXY(Point point)
    {
        double radius = point.X;
        double angle = point.Y;
        return new Point(Math.Sin(angle) * radius, Math.Cos(angle) * radius);
    }
    public static Point Roll(Point point, double angle)
    {
        return point * GetRollMatrix(angle);
    }

    public static Matrix GetRollMatrix(double angle)
    {
        var sin = Math.Sin(angle);
        var cos = Math.Cos(angle);
        return new Matrix(cos, -sin, sin, cos, 0, 0);
    }

    public static Color Chrome(double hue)
    {
        if (0 < hue && hue < 85)
        {
            return Color.FromRgb(0, 0, ((byte)(hue + 170)));
        }
        if (85 < hue && hue < 170)
        {
            return Color.FromRgb(0, ((byte)(hue + 85)), 0);
        }
        if (170 < hue && hue < 255)
        {
            return Color.FromRgb((byte)hue, 0, 0);
        }
        return Color.FromRgb(0, 0, 0);
    }
    public static double Distance(Point begin, Point end)
    {
        var x = end.X - begin.X;
        var y = end.Y - begin.Y;
        return Math.Sqrt(x * x + y * y);
    }

    public static Color ToRgb(byte hue, byte saturation, byte value)
    {
        if (value == 0)
        {
            return new Color();
        }
        var d = 221.702;
        var pointR = new Point(0, 0);
        var pointG = new Point(-128, d);
        var pointB = new Point(128, d);
        var point = PolarToXY(new Point(saturation, hue));
        var r = 256 - Distance(point, pointR);
        var g = 256 - Distance(point, pointG);
        var b = 256 - Distance(point, pointB);
        var value2 = (byte)(value / Max(r, Max(g, b)));
        return Color.FromRgb(((byte)(value2 * ((byte)r))), ((byte)(value2 * ((byte)g))), ((byte)(value2 * ((byte)b))));
    }
    public static Color ToRgb(int x, int y, int f)
    {
        var d = f;
        var pointR = new Point(0, 0);
        var pointG = new Point(-128, d);
        var pointB = new Point(128, d);
        var point = new Point(x, y);
        var r = 256 - Distance(point, pointR);
        var g = 256 - Distance(point, pointG);
        var b = 256 - Distance(point, pointB);
        var value2 = (byte)(255 / Max(r, Max(g, b)));
        return Color.FromRgb(((byte)(value2 * ((byte)r))), ((byte)(value2 * ((byte)g))), ((byte)(value2 * ((byte)b))));
    }
    public static byte Wave(double x, double y, int n)
    {
        var a = 0.0;
        for (int i = 0; i < n; i++)
        {
            a += Wave(x, y, i, n);
        }
        return (byte)(255 * (n - a));
    }
    static double Wave(double x, double y, int k, int n)
    {
        var a = k / n * 2 * Math.PI;
        return (x - Sin(a)) * (x - Sin(a)) + (y - Cos(a)) * (y - Cos(a));
    }
    public static int DotFromMandelbrotSet(double x, double y, double k)
    {
        var c = new Complex(0, 0);
        var a = new Complex(x, y);
        for (int i = 0; i < 30; i++)
        {
            c = Pow(c, k) + a;
            if (c.Magnitude > k)
            {
                return 30 - i;
            }
        }
        return 0;
    }
    public static Complex[] NewtonFractalRoots(int n)
    {
        Complex[] roots = new Complex[n];
        for (int i = 0; i < n; i++)
        {
            roots[i] = Sin(2 * Math.PI / n) + Cos(2 * Math.PI / n) * ImaginaryOne;
        }
        return roots;
    }
    public static (int Energy, int Color) NewtonFractal(double x, double y, double tolerant, Complex a, Complex b)
    {
        var r1 = 1;
        var r2 = a;
        var r3 = b;
        if(F(r1) != 0 || F(r2) != 0 || F(r3) != 0)
        {
            throw new Exception("Not root exception");
        }
        Complex F(Complex x)
        {
            return x * x * x - 1;
        }
        Complex dF(Complex x)
        {
            return 3 * x * x;
        }
        Complex z = new Complex(x, y);
        Complex znext = 0;
        for (int i = 0; i < 255; i++)
        {
            znext = z - F(z) / dF(z);
            z = znext;
            if (Abs(z - r1) < tolerant)
            {
                return (255, 0);
            }
            if (Abs(z - r2) < tolerant)
            {
                return (255, 1);
            }
            if (Abs(z - r3) < tolerant)
            {
                return (255, 2);
            }
        }
        return (0, 0);
    }
    public static int JuliaSet(double x, double y, int repeat, double bold, Func<Complex, Complex> func, double dy)
    {
        var c = new Complex(x, y);
        var dc = new Complex(x, y + dy);
        for (int i = 0; i < repeat; i++)
        {
            c = func(c);
            dc = func(dc);
            if (Abs(dc.Magnitude - c.Magnitude) > bold)
            {
                return i;
            }
        }
        return 0;
    }
    public static int DotFromMandelbrotSet(double r, double i, double j, double k)
    {
        var c = new Quaternion(0, 0, 0, 0);
        var a = new Quaternion(((float)i), ((float)j), ((float)k), ((float)r));
        for (int q = 0; q < 30; q++)
        {
            c = c * c + a;
            if (c.LengthSquared() > 4)
            {
                return 30 - q;
            }
        }
        return 0;
    }
    public static Func<double, double> Derivative(Func<double, double> func, double dx = 0.01)
    {
        return (double d) => (func(d + dx) - func(d)) / dx;
    }
    public static Func<double, double> Int(Func<double, double> func, double dx = 0.01)
    {
        return (double d) =>
        {
            var acc = 0.0;
            for (double i = 0; i < d; i += dx)
            {
                acc += func(i);
            }
            return acc;
        };
    }
    static int _m1 = 100, _m2 = 200, _m3 = 300;
    static double g = 9.8;
    // public static double X1(double time)
    // {
    //     return Int(Int((double d) => _m2 * ));
    // }
    // public static double X2(double time)
    // {

    // }
    // public static double X3(double time)
    // {

    // }
    // public static double Y1(double time)
    // {

    // }
    // public static double Y2(double time)
    // {

    // }
    // public static double Y3(double time)
    // {

    // }
}
class Matrix3
{
    public Matrix3(double m11, double m12, double m13, double m21, double m22, double m23, double m31, double m32, double m33)
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

    public static Matrix3 RollMatrix(double α, double β, double γ)
    {
        var a = α;
        var b = β;
        var y = γ;
        var asin = Sin(a);
        var bs = Sin(b);
        var ys = Sin(y);
        var ac = Cos(a);
        var bc = Cos(b);
        var yc = Cos(y);
        var m11 = ac * yc - bc * asin * ys;
        var m12 = -yc * asin - ac * bc * ys;
        var m13 = bs * ys;
        var m21 = bc * yc * asin + ac * ys;
        var m22 = ac * bc * yc - asin * ys;
        var m23 = -yc * bs;
        var m31 = asin * bs;
        var m32 = ac * bs;
        var m33 = bc;
        return new Matrix3(m11, m12, m13, m21, m22, m23, m31, m32, m33);
    }

}
class Roller
{
    public Roller(double angle)
    {
        (Sin, Cos) = SinCos(angle);
    }
    public int RollX(int x, int y)
    {
        return (int)(x * Cos + y * Sin);
    }
    public double Sin { get; }
    public double Cos { get; }
}
class Point4
{
    public Point4(double x, double y, double z, double w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    public static Matrix4x4 Roll(double alpha, double beta, double gamma, double theta, double psi, double phi)
    {
        var a = ((float)alpha);
        var b = ((float)beta);
        var c = ((float)gamma);
        var d = ((float)theta);
        var e = ((float)psi);
        var f = ((float)phi);
        return new
         Matrix4x4(
         Cos(a), Sin(a), 0, 0,
         -Sin(a), Cos(a), 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1) *
         new Matrix4x4(
         Cos(b), Sin(b), 0, 0,
         0, 0, 1, 0,
         -Sin(b), Cos(b), 0, 0,
         0, 0, 0, 1) *
         new Matrix4x4(
         Cos(a), Sin(a), 0, 0,
         -Sin(a), Cos(a), 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1) *
         new Matrix4x4(
         Cos(a), Sin(a), 0, 0,
         -Sin(a), Cos(a), 0, 0,
         0, 0, 1, 0,
         0, 0, 0, 1);
    }
    public double X { get; }
    public double Y { get; }
    public double Z { get; }
    public double W { get; }
}