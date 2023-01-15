using System.Windows;
namespace Objects.VolumeObjects;
class Line3
{
    public Point3 Begin { get; }
    public Point3 End { get; }

    public Line3(Point3 begin, Point3 end)
    {
        Begin = begin;
        End = end;
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
    public Point Proection(Point3 observationPoint, double displayDistance)
    {
        var x = observationPoint.X;
        var y = observationPoint.Y;
        var z = observationPoint.Z;
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
class VolumeObject
{
    
}