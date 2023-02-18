using System.Windows;
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
}
class Point3
{
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