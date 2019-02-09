public class Gem {
    
    public int Color { get; set; }
    public int Bonus { get; set; }

    public Gem()
    {
        Color = -1;
        Bonus = -1;
    }

    public bool IsBonus()
    {
        return Bonus != -1;
    }

}
