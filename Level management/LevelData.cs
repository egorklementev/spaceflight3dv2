/// <summary>
/// Serializable class that contains information about individual level
/// with all gems and bonuses and parameters of the level
/// </summary>
[System.Serializable]
public class LevelData {

    public int gridSizeX;
    public int gridSizeY;

    public int[] gemColors;
    public int[] gemBonuses;

    public int[] colorVector;
    public int[] availableBonuses;

    public int sequenceSize;
    public int maximumEnergy;

    public bool spawnNewGems;
    public bool spawnEnergy;
    public bool randomizeColors;

    public int bonusPercentage;
    public int energyPercentage;

    public LevelData(EditorParams pu, EditorLogic lu)
    {
        gridSizeX = (int) pu.gridSize.x;
        gridSizeY = (int) pu.gridSize.y;

        gemColors = new int[gridSizeX * gridSizeY];
        gemBonuses = new int[gridSizeX * gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                if (!lu.grid[x,y].IsEmpty())
                {
                    gemColors[x * gridSizeY + y] = lu.grid[x, y].Gem.Color;
                    gemBonuses[x * gridSizeY + y] = lu.grid[x, y].Gem.Bonus;
                } else
                {
                    gemColors[x * gridSizeY + y] = -1;
                }             
            }
        }    

        colorVector = pu.colorVector;
        availableBonuses = pu.permittedBonuses;

        sequenceSize = pu.sequenceSize;
        maximumEnergy = pu.maximumEnergy;

        spawnNewGems = pu.spawnNewGems;
        spawnEnergy = pu.spawnEnergy;
        randomizeColors = pu.randomizeColors;

        bonusPercentage = pu.bonusesPercentage;
        energyPercentage = pu.energyPercentage;
    }
}
