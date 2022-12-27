using System.Windows.Media;

namespace WpfApp1;

internal sealed class Particle
{
    public Vector2 Position { get; set; }

    public Vector2 Velocity { get; set; }

    public Color Color { get; set; }

    public double Life { get; set; }
}

internal struct Vector2
{
    public Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public static Vector2 operator+(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator*(Vector2 left, double right) => new(left.X * right, left.Y * right);
}