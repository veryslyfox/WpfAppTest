using System;
using System.Windows.Input;

class ElementaryCA
{
    public ElementaryCA(byte rule, int height, int width, bool isCyclic)
    {
        Rule = rule;
        Row = new int[height];
        IsCyclic = isCyclic;
        Memory = new int[width][];
        var filler = Row;
        for (int i = 0; i < width; i++)
        {
            Memory[i] = filler;
            filler = filler.ToArray();
        }
    }
    public int Combine(int left, int center, int right)
    {
        var position = left * 4 + center * 2 + right;
        return (Rule >> position) & 1;
    }
    public void Next()
    {
        if (Layer == 799)
        {
            return;
        }
        int[] next = new int[Row.Length];
        for (int i = 0; i < Row.Length; i++)
        {
            if (i == 0)
            {
                next[i] = Combine(0, Row[0], Row[1]);
                continue;
            }
            if (i == Row.Length - 1)
            {
                next[i] = Combine(Row[i - 1], Row[i], 0);
                continue;
            }
            next[i] = Combine(Row[i - 1], Row[i], Row[i + 1]);
        }
        Layer++;
        Memory[Layer] = Row;
        Row = next;
    }
    public void Left()
    {
        Cursor = (Cursor - 1) % Row.Length;
    }
    public void Right()
    {
        Cursor = (Cursor + 1) % Row.Length;
    }
    public void Inv()
    {
        Row[Cursor] = 1 - Row[Cursor];
    }
    public void Tick(Key c)
    {
        switch (c)
        {
            case Key.Right:
                Right();
                break;
            case Key.Left:
                Left();
                break;
            case Key.Space:
                Inv();
                break;
            case Key.S:
                Next();
                break;
        }
    }

    public int[] Row;
    public byte Rule { get; }
    public bool IsCyclic { get; }
    public int[][] Memory { get; }
    public int Layer { get; set; }
    public int Cursor { get; set; }
}