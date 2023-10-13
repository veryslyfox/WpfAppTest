using System.Collections;
using System.Windows;
using System;
class MirrorSpace
{
    public MirrorSpace(float[,] space)
    {
        Space = space;
    }

    public float[,] Space { get; }
}
class Ball
{
    private float _vectorAngle;
    static Ball()
    {
        Image = new bool[16, 16];
        for (int y = 0; y < 16; y++)
        {
            for (int x = 0; x < 16; x++)
            {
                Image[x, y] = (x * x + y * y) < 256;
            }
        }
    }
    public Ball(Point position, float vectorAngle, float velocity)
    {
        Position = position;
        VectorAngle = vectorAngle;
        Velocity = velocity;
    }

    public Point Position { get; }
    static bool[,] Image { get; }
    public float VectorAngle
    {
        get => _vectorAngle;
        set
        {
            _vectorAngle = value;
        }
    }
    public float Velocity { get; }
    float X { get; set; }
    float Y { get; set; }
    void SyncVector()
    {
        X = MathF.Sin(VectorAngle) * Velocity;
        Y = MathF.Cos(VectorAngle) * Velocity;
    }
    void Tick(MirrorSpace space)
    {
        
    }
}