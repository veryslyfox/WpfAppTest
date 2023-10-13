namespace Objects.SpecialMath;
using static System.Math;
using static System.MathF;
using static System.Numerics.Complex;
using System.Windows;
using System;
using System.Numerics;
using System.Windows.Media;
using VolumeObjects;
using Vector = Objects.Vector;
using System.Collections.Generic;

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
        for (int i = 0; i < 256; i++)
        {
            c = c * c + a;
            if (c.Magnitude > 2)
            {
                return (int)(255 - (i * i + 0.0) / 255);
            }
        }
        return 0;
    }
    static Complex Absolute(Complex c)
    {
        return new(Abs(c.Real), Abs(c.Imaginary));
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
    public static int NewtonFractal(double x, double y, double tolerance)
    {
        var r1 = 2;
        var r2 = Exp(2 / 3.0 * Math.PI * ImaginaryOne) * 2;
        var r3 = r2 * r2 * 2;
        Complex F(Complex x)
        {
            return 8 * x * x * x - 1;
        }
        Complex dF(Complex x)
        {
            return 24 * x * x;
        }
        Complex z = new Complex(x, y);
        Complex znext = 0;
        for (int i = 0; i < 255; i++)
        {
            znext = z - F(z) / dF(z);
            z = znext;
            if (Abs(z - r1) < tolerance)
            {
                return 0;
            }
            if (Abs(z - r2) < tolerance)
            {
                return 1;
            }
            if (Abs(z - r3) < tolerance)
            {
                return 2;
            }
        }
        return 0;
    }
    public static double Oscillation(double value, double axis, double ampl, double dx = 0.00125)
    {
        if (value == 0 && ((axis / dx) - (int)(axis / dx)) != 0)
        {
            return 0;
        }
        var prev = Oscillation(value - dx, axis, ampl, dx);
        return prev + dx * Abs(axis - ampl * prev);
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
    public static IEnumerable<Vector> PythagorasTree(int depth, Matrix matrix, Matrix matrix2)
    {
        return PythagorasTree(depth - 1, matrix, matrix2).Select(v => Tree(v, matrix)).Concat(PythagorasTree(depth - 1, matrix, matrix2).Select(v => Tree(v, matrix2)));
    }
    public static Vector Tree(Vector v, Matrix matrix)
    {
        return new Vector(v.End, (Point)(v.End - v.Begin) * matrix);
    }

    public static Func<double, double> Derivative(Func<double, double> func, double dx = 0.01)
    {
        return (double d) => (func(d + dx) - func(d)) / dx;
    }
    public static Func<double, double, double> Int(Func<double, double> func, double dx = 0.01)
    {
        return (double a, double b) =>
        {
            var acc = 0.0;
            for (double i = a; i < b; i += dx)
            {
                acc += func(i);
            }
            return acc;
        };
    }
    public static Func<double, double, Complex> Int(Func<Complex, Complex> func, double dx = 0.01)
    {
        return (double a, double b) =>
        {
            Complex acc = 0;
            for (double i = a; i < b; i += dx)
            {
                acc += func(i);
            }
            return acc;
        };
    }
    public static Complex PolyLog(Complex value, int order)
    {
        var sum = Complex.Zero;
        for (var i = 0.0; i < 100; i += 0.1)
        {
            sum += Pow(i, order) / (Exp(i - value) + 1);
        }
        return sum;
    }
    public static bool IsFractal(Complex complex, int precision)
    {
        return (Fractal(complex, precision).Phase - Fractal(complex - 0.01, precision).Phase) > 1;
    }
    public static Point BezierCurve(double t, Point a, Point b, Point c, Point d)
    {
        var ti = 1 - t;
        var c1 = t * t * t;
        var c2 = 3 * t * t * ti;
        var c3 = 3 * t * ti * ti;
        var c4 = ti * ti * ti;
        return new(c1 * a.X + c2 * b.X + c3 * c.X + c4 * d.X, c1 * a.Y + c2 * b.Y + c3 * c.Y + c4 * d.Y);
    }
    public static Point BezierCurve(double t, List<Point> controls)
    {
        var ti = 1 - t;
        var c = new double[Binoms.Length];
        var value = Pow(t, c.Length);
        for (int i = 0; i < c.Length; i++)
        {
            c[i] = value;
            value /= t;
            value *= (1 - t);
            if (value == double.NaN)
            {
                value = 0;
            }
        }
        Point result = new();
        for (int i = 0; i < controls.Count; i++)
        {
            result += ((System.Windows.Vector)new Point(controls[i].X * Binoms[i] * c[i], controls[i].Y * Binoms[i] * c[i]));
        }
        return result;
    }
    public static int[] Binoms = new int[] { };
    public static Complex Fractal(Complex complex, int precision)
    {
        var result = complex;
        for (int i = 0; i < precision; i++)
        {
            result = result - (result * result * result - 1) / (result * result);
        }
        return result;
    }
}
class LSystem
{
    public LSystem(int[][] rules)
    {
        Rules = rules;
    }

    public int[][] Rules { get; }

    public int[] Next(int[] array)
    {
        return array.SelectMany(i => Rules[i]).ToArray();
    }
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
    public Matrix Matrix { get => new(Cos, Sin, -Sin, Cos, 0, 0); }
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