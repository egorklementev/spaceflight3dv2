using UnityEngine;
using System.Collections.Generic;

public class EditorParams : MonoBehaviour
{

    [Header("Grid params")]
    public Vector2 gridSize;
    public float gemOffsetParam = .3f;
    public float gemMoveTime = 1f;
    public float gemScaleSpeed = 1f;
    public float destructionForce = 10f;
    public float dPartsLifetime = 3f;
    public Vector2 screenBound;
    [Space(10)]

    [Header("Gameplay params")]
    [Range(3, 8)]
    public int colorsAvailable = 5;
    public int sequenceSize = 3;
    public bool randomizeColors = false;
    public int maximumEnergy = 3;
    public bool spawnNewGems = true;
    [Space(10)]

    [Header("Bonus params")]
    [Range(0, 25)]
    public int bonusesPercentage = 5;
    [Range(0, 25)]
    public int energyPercentage = 5;
    public int[] permittedBonuses;
    [Space(10)]

    [Header("Units' refs")]
    public EditorGraphics gu;
    public EditorLogic lu;

    [HideInInspector]
    public float gemOffset;
    [HideInInspector]
    public float gemSize;
    [HideInInspector]
    public int[] colorVector;    

    public const int slotNumber = 8;
    public static int currentSlot = -1;

    public void InitializeOnStart()
    {
        #region Color vector for gem colors
        if (randomizeColors)
        {
            colorVector = new int[colorsAvailable];
            List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
            for (int i = 0; i < colorsAvailable; i++)
            {
                int index = Random.Range(0, list.Count);
                colorVector[i] = list[index];
                list.RemoveAt(index);
            }
        }
        else
        {
            colorVector = new int[colorsAvailable];
            for (int i = 0; i < colorsAvailable; i++)
            {
                colorVector[i] = i;
            }
        }
        #endregion        

        ComputeGemSizes();

        currentSlot = -1;

        gu.gameObject.SetActive(true);
        lu.gameObject.SetActive(true);
    }

    public int GetRandomColor()
    {
        return colorVector[Random.Range(0, colorsAvailable)];
    }

    // Bonus 1 - meteor
    // Bonus 2 - colorless
    // Bonus 3 - same color
    // Bonus 4 - obstacle
    // Bonus 5 - energy - not to be included to permitted bonuses
    public int GetRandomBonus()
    {
        int bonus = -1;

        int random = Random.Range(0, 100);

        bonus = random < bonusesPercentage ?
            permittedBonuses[Random.Range(0, permittedBonuses.Length)] : -1;

        bonus = (random > bonusesPercentage &&
            random < bonusesPercentage + energyPercentage) ? 5 : bonus; // Energy or leave unchanged

        return bonus;
    }

    // Calculates offset and gem size to fit the screen properly
    private void ComputeGemSizes()
    {
        // Size of the gems sides
        float pixelsInUnit = Screen.height / 10f; // Size of the camera is 5
        gemSize = Mathf.Min(
            screenBound.x * Screen.width / (gridSize.x + (gridSize.x - 1) * gemOffsetParam),
            screenBound.y * Screen.height / (gridSize.y + (gridSize.y - 1) * gemOffsetParam)
            );
        gemSize /= pixelsInUnit;
        gemOffset = gemOffsetParam * gemSize;
        
    }

    // Saves current state of the game in the persistent directory of the game. 
    // Uses 'currentSlot' variable to identify the file in which to save
    public void SaveLevel()
    {
        SaveUnit.SaveLevel(this, lu, currentSlot);
    }

    // Loads previously saved state of the game and replaces current one
    // Uses 'currentSlot' variable to identify the file from which to load
    public void LoadLevel()
    {
        LevelData ld = SaveUnit.LoadLevel(currentSlot);

        if (ld == null)
        {
            SaveLevel();
        } else
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    if (!lu.grid[x, y].IsEmpty())
                    {
                        gu.DestroyGem(x, y, lu.grid[x, y].Gem.Color);
                    }
                }
            }

            colorsAvailable = ld.availableColors;
            ld.availableBonuses.CopyTo(permittedBonuses, 0);

            sequenceSize = ld.sequenceSize;
            maximumEnergy = ld.maximumEnergy;

            spawnNewGems = ld.spawnNewGems;
            randomizeColors = ld.randomizeColors;

            #region Color vector for gem colors
            if (randomizeColors)
            {
                colorVector = new int[colorsAvailable];
                List<int> list = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7 };
                for (int i = 0; i < colorsAvailable; i++)
                {
                    int index = Random.Range(0, list.Count);
                    colorVector[i] = list[index];
                    list.RemoveAt(index);
                }
            }
            else
            {
                colorVector = new int[colorsAvailable];
                for (int i = 0; i < colorsAvailable; i++)
                {
                    colorVector[i] = i;
                }
            }
            #endregion

            gridSize.x = ld.gridSizeX;
            gridSize.y = ld.gridSizeY;

            ComputeGemSizes();

            gu.UpdateDataAfterLoading();
            lu.UpdateDataAfterLoading();

            gu.RecreateGrid((int)gridSize.x, (int)gridSize.y);

            lu.grid = new Cell[(int)gridSize.x, (int)gridSize.y];
            for (int x = 0; x < (int)gridSize.x; x++)
            {
                for (int y = 0; y < (int)gridSize.y; y++)
                {
                    if (ld.gemColors[x * (int)gridSize.y + y] != -1)
                    {
                        lu.grid[x, y] = new Cell(new Vector2(x, y))
                        {
                            Gem = new Gem
                            {
                                Color = ld.gemColors[x * (int)gridSize.y + y],
                                Bonus = ld.gemBonuses[x * (int)gridSize.y + y]
                            }
                        };
                        gu.SpawnGem((int)lu.grid[x, y].Position.x, (int)lu.grid[x, y].Position.y, lu.grid[x, y].Gem.Color, lu.grid[x, y].Gem.Bonus);
                    }
                    else
                    {
                        lu.grid[x, y] = new Cell(new Vector2(x, y));
                        lu.grid[x, y].SetEmpty();
                    }
                }
            }
        }        
    }

    // Tells ParamsUnit to load level containing in 'currentSlot' variable
    public void SetLevelToLoad()
    {
        if (currentSlot == -1)
        {
            currentSlot = 0;
            SaveLevel();
            ParamUnit.slotToLoad = 0;
        } else
        {
            ParamUnit.slotToLoad = currentSlot;
        }
    }

    public void IncreaseGridSizeX()
    {
        gridSize.x += 1f;
    }
    public void DecreaseGridSizeX()
    {   
        if (gridSize.x > 1f)
            gridSize.x -= 1f;
    }

    public void IncreaseGridSizeY()
    {
        gridSize.y += 1f;
    }
    public void DecreaseGridSizeY()
    {
        if (gridSize.y > 1f)
            gridSize.y -= 1f;
    }

    public void IncreaseAvailableColors()
    {
        if (colorsAvailable < 8)
            colorsAvailable++;
    }
    public void DecreaseAvailableColors()
    {
        if (colorsAvailable > 3)
            colorsAvailable--;
    }

    public void IncreaseSequenceSize()
    {
        if (sequenceSize < Mathf.Min(gridSize.x, gridSize.y))
            sequenceSize++;
    }
    public void DecreaseSequenceSize()
    {
        if (sequenceSize > 3)
            sequenceSize--;
    }

    public void IncreaseBonusChance()
    {
        if (bonusesPercentage < 25)       
            bonusesPercentage++;        
    }
    public void DecreaseBonusChance()
    {
        if (bonusesPercentage > 0)
            bonusesPercentage--;
    }

    public void IncreaseEnergyChance()
    {
        if (energyPercentage < 25)
            energyPercentage++;
    }
    public void DecreaseEnergyChance()
    {
        if (energyPercentage > 0)
            energyPercentage--;
    }

    public void IncreaseSlot()
    {
        if (currentSlot < slotNumber)
            currentSlot++;
    }
    public void DecreaseSlot()
    {
        if (currentSlot > 1)
            currentSlot--;
    }
    public void InitializeSlot()
    {
        currentSlot = 1;
    }
    
}
