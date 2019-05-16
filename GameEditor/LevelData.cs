
public class LevelData {

    int gridSizeX;
    int gridSizeY;

    int[] gemColors;
    int[] gemBonuses;

    int availableColors;
    int[] availableBonuses;

    int sequenceSize;
    int maximumEnergy;

    bool spawnNewGems;
    bool randomizeColors;

    int bonusPercentage;
    int energyPercentage;

    public LevelData(ParamUnit pu, LogicUnit lu)
    {
        gridSizeX = (int) pu.gridSize.x;
        gridSizeY = (int) pu.gridSize.y;

        gemColors = new int[gridSizeX * gridSizeY];
        gemBonuses = new int[gridSizeX * gridSizeY];

        int i = 0;
        foreach (Cell c in lu.grid)
        {
            gemColors[i] = c.Gem.Color;
            gemBonuses[i] = c.Gem.Bonus;
            i++;
        }

        availableColors = pu.colorsAvailable;
        availableBonuses = new int[pu.permittedBonuses.Length];
        pu.permittedBonuses.CopyTo(availableBonuses, 0);

        sequenceSize = pu.sequenceSize;
        maximumEnergy = pu.maximumEnergy;

        spawnNewGems = pu.spawnNewGems;
        randomizeColors = pu.randomizeColors;

        bonusPercentage = pu.bonusesPercentage;
        energyPercentage = pu.energyPercentage;
    }
}
