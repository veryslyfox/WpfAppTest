global using System;
enum MainEffect
{
    Damage,
    Fire,
    Freeze,
}
enum OptionalEffect
{
    None,
    Bomb,
    Time,
    Accumulation,
    Vulnerability,
}
class Effect
{
    public Effect(MainEffect main, int power, int time, OptionalEffect add, int value, int[] electricity)
    {
        Main = main;
        Power = power;
        Add = add;
        Value = value;
        Electricity = electricity;
    }

    public MainEffect Main { get; }
    public int Power { get; }
    public OptionalEffect Add { get; }
    public int Value { get; }
    public int[] Electricity { get; }
}

sealed class Weapon
{

    public Weapon(Resource[] create, Action effect, string ammutition, string name, int cartrige, bool isOneCartrige, string[] resources, RemontData remontData)
    {
        Effect = effect;
        Ammutition = ammutition;
    }

    public Action Effect { get; }
    public string Ammutition { get; }
}
sealed class Clothes
{
    public Clothes(Resource[] create, string name, Effect[] effects)
    {
        Effects = effects;
    }

    public Effect[] Effects { get; }
}
sealed class Resource
{
    public Resource(string name)
    {
    }
}
