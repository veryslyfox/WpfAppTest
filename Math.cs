namespace Objects.SpecialMath;
using static System.Math;
using System.Windows;
using System;
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
        return new Point(Math.Cos(angle) * radius, Math.Sin(angle) * radius);
    }
    public static Point Roll(Point point, double angle)
    {
        var radius = Sqrt(point.X * point.X + point.Y * point.Y);
        return new Point(radius * Sin(angle), radius * Cos(angle));
    }
}