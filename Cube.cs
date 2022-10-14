class Cube
{
    public Cube(int argbVisual, string name, Scane? scane = null)
    {
        ArgbVisual = argbVisual;
        Name = name;
        Scane = scane;
    }

    public int ArgbVisual { get; }
    public string Name { get; }
    public Scane? Scane { get; }
}
class Scane
{
    public Scane(string majorText, string minorText)
    {

    }   
}