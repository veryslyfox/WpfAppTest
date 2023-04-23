using System.Windows.Media;

namespace WpfApp1;

public sealed class Particle
{
    public Particle()
    {

    }
    public Particle(Vector2 position, Vector2 velocity, Color color, double life)
    {
        Position = position;
        Velocity = velocity;
        Color = color;
        Life = life;
    }
    public Vector2 Position { get; set; }

    public Vector2 Velocity { get; set; }

    public Color Color { get; set; }

    public double Life { get; set; }
}

public struct Vector2
{
    public Vector2(double x, double y)
    {
        X = x;
        Y = y;
    }

    public double X { get; set; }

    public double Y { get; set; }

    public static Vector2 operator +(Vector2 left, Vector2 right) => new(left.X + right.X, left.Y + right.Y);

    public static Vector2 operator *(Vector2 left, double right) => new(left.X * right, left.Y * right);
}
sealed class Particle2
{
    public Particle2(double x, double y, double velocityX, double velocityY)
    {
        X = x;
        Y = y;
        VelocityX = velocityX;
        VelocityY = velocityY;
    }

    public double X { get; set; }
    public double Y { get; set; }
    public double VelocityX { get; }
    public double VelocityY { get; }

    public void Next()
    {
        X += VelocityX;
        Y += VelocityY;
    }
    public static Particle2 operator *(Particle2 left, Matrix right)
    {
        return new(left.X, left.Y, left.VelocityX * right.M11 + left.VelocityY * right.M12, left.VelocityX * right.M21 + left.VelocityY * right.M22);
    }
}