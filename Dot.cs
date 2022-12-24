struct Dot
{
    public Dot(int x, int y, Color color)
    {
        J = x;
        I = y;
        Color = color;
    }
    public void Offset(Direct direct)
    {
        switch (direct)
        {
            case Direct.Up:
                I--;
                break;
            case Direct.Down:
                I++;
                break;
            case Direct.Left:
                J--;
                break;
            case Direct.Right:
                J++;
                break;
        }
    }
    public void Offset(Direct direct, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Offset(direct);
        }
    }
    public int J { get; set; }
    public int I { get; set; }
    public Color Color { get; }
}
enum Direct
{
    Up,
    Down,
    Left,
    Right
}