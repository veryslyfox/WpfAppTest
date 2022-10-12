class PersonageInfo
{
    public PersonageInfo(string visualize, string name, Effect[] effects, int liveMax, int live,
    int[] weaponry, int[] clothes, int[] elements)
    {  
        Effects = effects;
        LiveMax = liveMax;
        Live = live;
        Weaponry = weaponry;
        Clothes = clothes;
        Elements = elements;
    }
    public Effect[] Effects { get; }
    public int LiveMax { get; set; }
    public int Live { get; set; }
    public int[] Weaponry { get; set; }
    public int[] Clothes { get; set; }
    public int[] Elements { get; }
}