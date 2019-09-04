using UnityEngine;

/// <summary>
/// A class containing information about individual cell in the logical grid
/// </summary>
public class Cell {

    public Gem Gem { get; set; }
    public Vector2 Position { get; set; }

    public Cell (Vector2 position)
    {
        Position = position;
    }

    public void SetEmpty()
    {
        Gem = null;
    }

    public bool IsEmpty()
    {
        return Gem == null;
    }
}
