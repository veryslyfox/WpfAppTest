global using Vector3S = System.Numerics.Vector3;
using static System.Math;
using System.Windows;
using System;
using System.Windows.Media;
namespace Objects.VolumeObjects;
//ä
class Vector3
{
    public Point3 Begin { get; }
    public Point3 End { get; }

    public Vector3(Point3 begin, Point3 end)
    {
        Begin = begin;
        End = end;
    }
    public Vector Projection(Point3 observer, double displayDistance)
    {
        return new(Begin.Projection(observer, displayDistance), End.Projection(observer, displayDistance));
    }
    public static explicit operator Vector3S(Vector3 vector3)
    {
        return new(vector3.End.X - vector3.Begin.X, vector3.End.Y - vector3.Begin.Y, vector3.End.Z - vector3.Begin.Z);
    }
}
public class Point3
{
    public static explicit operator System.Numerics.Vector3(Point3 point)
    {
        return new(point.X, point.Y, point.Z);
    }
    public Point3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public Point Projection(Point3 observer, double displayDistance)
    {
        var x = observer.X;
        var y = observer.Y;
        var z = observer.Z;
        var xd = X - x;
        var yd = Y - y;
        var zd = Z - z;
        var proectionCooficent = displayDistance / zd;
        return new(xd * proectionCooficent, yd * proectionCooficent);
    }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
}

interface ITracedObject
{
    public Point3[] GetCollideDots(Vector3 vector);
    public Vector3 GetNormals(Vector3 vector);
}
class Plane
{
    public Plane(int a, int b, int c, int d)
    {
        A = a;
        B = b;
        C = c;
        D = d;
    }

    public int A { get; }
    public int B { get; }
    public int C { get; }
    public int D { get; }

    public bool Contains(int x, int y, int z)
    {
        return A * x + B * y + C * z + D == 0;
    }
}
class SchlafliSymbol
{
    public SchlafliSymbol(int p, int q)
    {
        P = p;
        Q = q;
    }

    public int P { get; }
    public int Q { get; }
}
struct Triangle
{
    public Triangle(Point3 a, Point3 b, Point3 c)
    {
        A = a;
        B = b;
        C = c;
        AB = new Vector3(a, b);
        AC = new Vector3(a, c);
        BC = new Vector3(b, c);
        Sign = 1;
    }
    public bool IsColored(int x, int y, double displayDistance, Point3 observer)
    {
        bool Right(Vector3 vector3)
        {
            return vector3.Projection(observer, displayDistance).DotRight(x, y);
        }
        return Right(AB) && Right(AC) && Right(BC);
    }
    public Vector3S Normal()
    {
        return ((Vector3S)A) * ((Vector3S)B);
    }
    public double Value(Point3 point3)
    {
        return Vector3S.Dot(Normal(), ((Vector3S)AB) - ((Vector3S)point3)) * Sign;
    }
    public Point3 A { get; set; }
    public Point3 B { get; set; }
    public Point3 C { get; set; }
    public Vector3 AB { get; set; }
    public Vector3 AC { get; set; }
    public Vector3 BC { get; set; }
    public int Sign;
}
class TriangleModel
{
    public TriangleModel(Triangle[] triangles)
    {
        Triangles = triangles;
    }
    public TriangleModel(Point3[] points, params (int, int, int)[] values)
    {
        Triangles = values.Select(n => new Triangle(points[n.Item1], points[n.Item2], points[n.Item3])).ToArray();
    }
    public Triangle[] Triangles { get; }
}