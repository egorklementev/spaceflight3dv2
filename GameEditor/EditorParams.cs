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
    public bool spawnEnergy = true;
    [Space(10)]

    [Header("Bonus params")]
    [Range(0, 25)]
    public int bonusesPercentage = 5;
    [Range(0, 25)]
    public int energyPercentage = 5;
    public int[] permittedBonuses;
    [Space(10)]

    [Header("Level type and params")]
    public int winCondition = 1;
    public int timeAvailable = 60;
    public int scoreToWin = 100;
    public int movesAvailable = 0;
    [Space(10)]

    [Header("Units' refs")]
    public EditorGraphics gu;
    public EditorLogic lu;
    public FadeManager fm;
    [Space(10)]

    [Header("Pop-up message")]
    public MessageText mt;
    [Space(10)]

    public Camera mainCamera;

    [HideInInspector]
    public float gemOffset;
    [HideInInspector]
    public float gemSize;
    [HideInInspector]
    public int[] colorVector;
    [HideInInspector]
    public string[] colorNames;
    [HideInInspector]
    public string[] bonusNames;

    public const int slotNumber = 8;
    public static int currentSlot = -1;
    public static int loadSlot = 1;
    public static int saveSlot = 1;
    public static bool isSaved = false;

    private bool firstTimeLaunched = true;

    private void Awake()
    {
        colorNames = new string[10];
        colorNames[0] = LocalizationManager.instance.GetLocalizedValue("editor_color_blue");
        colorNames[1] = LocalizationManager.instance.GetLocalizedValue("editor_color_green");
        colorNames[2] = LocalizationManager.instance.GetLocalizedValue("editor_color_magenta");
        colorNames[3] = LocalizationManager.instance.GetLocalizedValue("editor_color_orange");
        colorNames[4] = LocalizationManager.instance.GetLocalizedValue("editor_color_red");
        colorNames[5] = LocalizationManager.instance.GetLocalizedValue("editor_color_diamond");
        colorNames[6] = LocalizationManager.instance.GetLocalizedValue("editor_color_white");
        colorNames[7] = LocalizationManager.instance.GetLocalizedValue("editor_color_yellow");
        colorNames[8] = LocalizationManager.instance.GetLocalizedValue("editor_color_gray");
        colorNames[9] = LocalizationManager.instance.GetLocalizedValue("editor_color_white");

        bonusNames = new string[9];
        bonusNames[0] = LocalizationManager.instance.GetLocalizedValue("editor_deselector");
        bonusNames[1] = LocalizationManager.instance.GetLocalizedValue("editor_energy_bonus");
        bonusNames[2] = LocalizationManager.instance.GetLocalizedValue("editor_meteor_bonus");
        bonusNames[3] = LocalizationManager.instance.GetLocalizedValue("editor_colorless_bonus");
        bonusNames[4] = LocalizationManager.instance.GetLocalizedValue("editor_same_color_bonus");
        bonusNames[5] = LocalizationManager.instance.GetLocalizedValue("editor_obstacle_bonus");
        bonusNames[6] = LocalizationManager.instance.GetLocalizedValue("editor_ice_1");
        bonusNames[7] = LocalizationManager.instance.GetLocalizedValue("editor_ice_2");
        bonusNames[8] = LocalizationManager.instance.GetLocalizedValue("editor_ice_3");

        MusicManager.instance.Play("Menu theme");
    }

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

        if (!firstTimeLaunched)
        {
            UpdateGUandLU();
        }

        firstTimeLaunched = false;
    }

    public int GetRandomColor()
    {
        return colorVector[Random.Range(0, colorsAvailable)];
    }

    public int GetRandomBonus()
    {
        int bonus = -1;

        int random = Random.Range(0, 100);

        if (permittedBonuses.Length > 0)
            bonus = random < bonusesPercentage ?
                permittedBonuses[Random.Range(0, permittedBonuses.Length)] : -1;

        bonus = spawnEnergy && (random > bonusesPercentage &&
            random < bonusesPercentage + energyPercentage) ? (int) ParamUnit.Bonus.ENERGY : bonus; // Energy or leave unchanged

        return bonus;
    }

    // Calculates offset and gem size to fit the screen properly
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

    // Saves current state of the game in the persistent directory of the game. 
    // Uses 'currentSlot' variable to identify the file in which to save
    public void SaveLevel()
    {
        SaveLevel(false);
    }
    public void SaveLevel(bool toTheTemporarySlot)
    {
        if (!toTheTemporarySlot)
        {
            currentSlot = saveSlot;
        }
        else
        {
            currentSlot = 0;
        }
        SaveUnit.SaveLevel(this, lu, currentSlot, "/Levels/Editor/");

        string levelSaved = LocalizationManager.instance.GetLocalizedValue("editor_level_saved");
        mt.DisplayMessage(levelSaved + " " + currentSlot.ToString() + "!", 2.5f);

        isSaved = true;
    }

    // Loads previously saved state of the game and replaces current one
    // Uses 'currentSlot' variable to identify the file from which to load
    public void LoadLevel()
    {
        currentSlot = loadSlot;
        saveSlot = loadSlot;

        LevelData ld = SaveUnit.LoadLevel(currentSlot, "/Levels/Editor/");

        if (ld == null)
        {
            string slot = LocalizationManager.instance.GetLocalizedValue("editor_slot_lable");
            string empty = LocalizationManager.instance.GetLocalizedValue("editor_empty_slot");
            mt.DisplayMessage(slot + " " + currentSlot.ToString() + " " + empty + "!", 2.0f);
        }
        else
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

            winCondition = ld.winCondition;
            timeAvailable = ld.timeAvailable;
            movesAvailable = ld.movesAvailable;
            scoreToWin = ld.scoreToWin;

            colorsAvailable = ld.colorVector.Length;
            permittedBonuses = ld.availableBonuses;

            sequenceSize = ld.sequenceSize;
            maximumEnergy = ld.maximumEnergy;

            spawnNewGems = ld.spawnNewGems;
            spawnEnergy = ld.spawnEnergy;
            randomizeColors = ld.randomizeColors;

            colorVector = ld.colorVector;

            gridSize.x = ld.gridSizeX;
            gridSize.y = ld.gridSizeY;

            ComputeGemSizes();

            lu.UpdateDataAfterLoading();
            gu.UpdateDataAfterLoading();

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

        isSaved = true;
    }

    // Tells ParamsUnit to load level containing in 'currentSlot' variable
    public void SetLevelToLoad()
    {
        if (currentSlot == -1)
        {
            currentSlot = 0;
            SaveLevel(true);
            ParamUnit.editorSlotToLoad = 0;
        } else
        {
            ParamUnit.editorSlotToLoad = currentSlot;
        }
    }

    public void ResetGrid()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                gu.DestroyGem(x, y, lu.grid[x, y].Gem.Color);

                lu.grid[x, y].Gem = lu.GetRandomGem();

                gu.SpawnGem(x, y, lu.grid[x, y].Gem.Color, lu.grid[x, y].Gem.Bonus, 0);
            }
        }
    }

    public void IncreaseGridSizeX()
    {
        if (IsSizeValueValid(gridSize.x + 1f, gridSize.y))
            gridSize.x += 1f;
    }
    public void DecreaseGridSizeX()
    {   
        if (gridSize.x > 1f && IsSizeValueValid(gridSize.x - 1f, gridSize.y))
            gridSize.x -= 1f;
    }

    public void IncreaseGridSizeY()
    {
        if (IsSizeValueValid(gridSize.x, gridSize.y + 1f))
            gridSize.y += 1f;
    }
    public void DecreaseGridSizeY()
    {
        if (gridSize.y > 1f && IsSizeValueValid(gridSize.x, gridSize.y - 1f))
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

    public void IncreaseMaxEnergy()
    {
        if (maximumEnergy < 10)
        {
            maximumEnergy++;
        }
    }
    public void DecreaseMaxEnergy()
    {
        if (maximumEnergy > 1)
        {
            maximumEnergy--;
        }
    }

    public void IncreaseWinCondition()
    {
        if (winCondition < 2)
        {
            winCondition++;
        }
    }
    public void DecreaseWinCondition()
    {
        if (winCondition > 1)
        {
            winCondition--;
        }
    }

    public void IncreaseTimeAvailable()
    {
        timeAvailable += 10;
    }
    public void DecreaseTimeAvailable()
    {
        if (timeAvailable > 0)
        {
            timeAvailable -= 10;
        }
    }

    public void IncreaseScoreToWin()
    {
        scoreToWin += 100;
    }
    public void DecreaseScoreToWin()
    {
        if (scoreToWin > 0)
        {
            scoreToWin -= 100;
        }
    }

    public void IncreaseMovesAvailable()
    {
        movesAvailable++;
    }
    public void DecreaseMovesAvailable()
    {
        if (movesAvailable > 0)
        {
            movesAvailable--;
        }
    }

    public void IncreaseLoadSlot()
    {
        if (loadSlot < slotNumber)
            loadSlot++;
    }
    public void DecreaseLoadSlot()
    {
        if (loadSlot > 1)
            loadSlot--;
    }

    public void IncreaseSaveSlot()
    {
        if (saveSlot < slotNumber)
            saveSlot++;
    }
    public void DecreaseSaveSlot()
    {
        if (saveSlot > 1)
            saveSlot--;
    }

    private bool IsSizeValueValid(float width, float height)
    {
        return 
            width < 16f && 
            height < 16f && 
            (width / height) <= (2f) && 
            (width / height) >= (2f / 3f) && 
            (width * height) <= 100f &&
            (width * height) >= 20f;
    }

    // Check with the purpose of not showing save group every time
    public void UILevelSave()
    {
        if (currentSlot == -1)
        {
            fm.ShowGroup("Panel group");
            fm.ShowGroup("Save group");
        }
        else
        {
            SaveLevel();
        }
    }

    // For the purpose of loading level from the start
    public void UILevelLoad()
    {
        InitializeOnStart();
        LoadLevel();
    }

    private void UpdateGUandLU()
    {
        for (int x = 0; x < lu.grid.GetLength(0); x++)
        {
            for (int y = 0; y < lu.grid.GetLength(1); y++)
            {               
                gu.DestroyGem(x, y, lu.grid[x, y].Gem.Color);
            }
        }

        lu.UpdateDataAfterLoading();
        gu.UpdateDataAfterLoading();

        gu.RecreateGrid((int)gridSize.x, (int)gridSize.y);

        lu.grid = new Cell[(int)gridSize.x, (int)gridSize.y];
        for (int x = 0; x < (int)gridSize.x; x++)
        {
            for (int y = 0; y < (int)gridSize.y; y++)
            {
                lu.grid[x, y] = new Cell(new Vector2(x, y));                        
            }
        }
        lu.FillGemGrid();
    }
}
