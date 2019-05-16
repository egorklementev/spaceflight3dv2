﻿
[System.Serializable]
public class LevelData {

    public int gridSizeX;
    public int gridSizeY;

    public int[] gemColors;
    public int[] gemBonuses;

    public int availableColors;
    public int[] availableBonuses;

    public int sequenceSize;
    public int maximumEnergy;

    public bool spawnNewGems;
    public bool randomizeColors;

    public int bonusPercentage;
    public int energyPercentage;

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
