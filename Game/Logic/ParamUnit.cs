using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A class containing all parameters of the level
/// </summary>
public class ParamUnit : MonoBehaviour {

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
    [Range(3, 5)]
    public int sequenceSize = 3;
    public bool randomizeColors = false;
    public int maximumEnergy = 3;
    public bool spawnNewGems = true;
    public bool spawnEnergy = true;
    [Space(10)]

    [Header("Bonus params")]
    [Range(0, 25)]
    public int bonusesPercentage = 5;
    [Range(0, 25)]
    public int energyPercentage = 5;
    [Space(10)]
    public int[] permittedBonuses;
    [Space(10)]
    public float meteorMoveSpeed = 1f;
    public float meteorOffset = 10f;
    [Space(10)]

    [Header("Units' refs")]
    public GraphicsUnit gu;
    public LogicUnit lu;

    public Camera mainCamera;

    [HideInInspector]
    public float gemOffset;
    [HideInInspector]
    public float gemSize;

    public static int slotToLoad = -1;

    public enum Bonus
    {
        NONE = -1,
        DESELECTOR = 0,
        ENERGY = 1,
        METEOR = 2,
        COLORLESS = 3,
        SAME_COLOR = 4,
        OBSTACLE = 5,
        ICE_1 = 6,
        ICE_2 = 7,
        ICE_3 = 8
    }

    private int[] colorVector;
    
    private void Start()
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
        } else
        {
            colorVector = new int[colorsAvailable];
            for (int i = 0; i < colorsAvailable; i++)
            {
                colorVector[i] = i;
            }
        }
        #endregion
        
        ComputeGemSizes();

        gu.gameObject.SetActive(true);
        lu.gameObject.SetActive(true);

        if (slotToLoad != -1)
        {
            LoadLevel();     
        }
    }
        
    /// <summary>
    /// Returns random color from the allowed color list
    /// </summary>
    /// <returns></returns>
    public int GetRandomColor()
    {
        return colorVector[Random.Range(0, colorsAvailable)];
    }
    
    /// <summary>
    /// Returns random bonus from the allowed bonus list
    /// </summary>
    /// <remarks>
    /// Can return energy gem. It has its own chance of appearance
    /// </remarks>
    /// <returns></returns>
    public int GetRandomBonus()
    {
        int bonus = (int) Bonus.NONE;

        int random = Random.Range(0, 100);

        bonus = random < bonusesPercentage ?
            permittedBonuses[Random.Range(0, permittedBonuses.Length)] : (int) Bonus.NONE;
        
        bonus = spawnEnergy && (random > bonusesPercentage && 
            random < bonusesPercentage + energyPercentage) ? (int) Bonus.ENERGY : bonus; // Energy or leave unchanged

        return bonus;
    }

    /// <summary>
    /// Calculates the offset and the size of gems to fit the screen properly
    /// </summary>
    private void ComputeGemSizes()
    {
        // Size of the gems sides
        float height = 2f * mainCamera.orthographicSize;
        float width = height * mainCamera.aspect;
        gemSize = Mathf.Min(
            screenBound.x * width / (gridSize.x + (gridSize.x - 1) * gemOffsetParam),
            screenBound.y * height / (gridSize.y + (gridSize.y - 1) * gemOffsetParam)
            );
        gemOffset = gemOffsetParam * gemSize;        
    }
    
    /// <summary>
    /// Loads previously saved state of the game and replaces current one
    /// Uses 'slotToLoad' variable to identify the file from which to load
    /// </summary>
    public void LoadLevel()
    {
        LevelData ld = SaveUnit.LoadLevel(slotToLoad);
        
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                if (!lu.grid[x, y].IsEmpty())
                {
                    gu.DestroyGem(x, y, lu.grid[x, y].Gem.Color, false);
                }
            }
        }

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

        colorsAvailable = ld.colorVector.Length;
        colorVector = ld.colorVector;
        permittedBonuses = ld.availableBonuses;

        sequenceSize = ld.sequenceSize;
        maximumEnergy = ld.maximumEnergy;

        lu.suboptimalMoves = maximumEnergy;
        gu.RecreateEnergyBar(maximumEnergy);

        spawnNewGems = ld.spawnNewGems;
        spawnEnergy = ld.spawnEnergy;
        randomizeColors = ld.randomizeColors;

        slotToLoad = -1;
    }

}
