namespace Objects.SpecialMath;
using static System.Math;
using System.Windows;
using System;
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
        var radius = Sqrt(point.X * point.X + point.Y * point.Y);
        return new Point(radius * Sin(angle), radius * Cos(angle));
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
    public static Color ToRgb(int x, int y)
    {
        var d = 221.702;
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
}
class Matrix3
{
    public Matrix3(int m11, int m12, int m13, int m21, int m22, int m23, int m31, int m32, int m33)
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

    public int M11 { get; }
    public int M12 { get; }
    public int M13 { get; }
    public int M21 { get; }
    public int M22 { get; }
    public int M23 { get; }
    public int M31 { get; }
    public int M32 { get; }
    public int M33 { get; }
    /*
    public static Matrix RollMatrix(double α, double β, double γ)
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
          
    }
    */
}