using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class performing all logic cheks, computations and actions in the game
/// </summary>
public class LogicUnit : MonoBehaviour {
    
    public Cell[,] grid; // Logical state of the grid
    
    [Header("Units' refs")]
    public GraphicsUnit gu;
    public InputUnit iu;
    public ParamUnit pu;
    
    [HideInInspector]
    public Vector2 gemSO = UNSELECTED;
    [HideInInspector]
    public Vector2 gemST = UNSELECTED;
    [HideInInspector]
    public bool readyToSwap = false;
    [HideInInspector]
    public int suboptimalMoves; // Number of energy available
    [HideInInspector]
    public float timeLeft = 0;
    [HideInInspector]
    public int movesLeft = 0;
    [HideInInspector]
    public bool checkTime = false;
    [HideInInspector]
    public bool checkMoves = false;
    [HideInInspector]
    public int score = 0;

    private static Vector2 UNSELECTED = new Vector2(-1, -1);

    private int gSizeX; // Width of the grid
    private int gSizeY; // Height of the grid

    private bool needToCheck = false; // Whether or not unit should check the grid for empty cells or gems to be destroyed
    private bool bonusIsWorking = false;

    private bool initialSpawnHappened = false;

    private int collectedMetal = 0;
    private int collectedFuel = 0;
    private int collectedEnergy = 0;

    private List<int> colorsMetal;
    private List<int> colorsFuel;

    private void Awake() {        

        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;

        suboptimalMoves = pu.maximumEnergy;

        grid = new Cell[gSizeX, gSizeY];    
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                grid[x, y] = new Cell(new Vector2(x, y));
            }
        }

        FillGemGrid(); // Initial grid filling
    }

    private void Update ()
    {
        if (checkTime && !iu.IsGameOver())
            timeLeft -= Time.deltaTime;

        if (gu.WorkingObjs == 0)
        {
            if (needToCheck)
            {
                CheckGemGrid();
                FillGemGrid();
            }

            // Defeat
            if (suboptimalMoves <= 0)
            {
                if (!checkTime && !checkMoves)
                {
                    UpdateResults(true);
                    gu.DrawEndScreen(true);
                    iu.SetGameOver();
                }
                else
                {
                    UpdateResults(false);
                    gu.DrawEndScreen(false);
                    iu.SetGameOver();
                }

            }
            if (((checkTime && timeLeft <= 0) || (checkMoves && movesLeft <= 0)) && score < pu.scoreToWin)
            {
                UpdateResults(false);
                gu.DrawEndScreen(false);
                iu.SetGameOver();

            } else {
                // Victory
                if (pu.winCondition - 1 == (int)ParamUnit.WinCondition.FIELD_IS_EMPTY)
                {
                    int nonEmpty = 0;
                    foreach (Cell c in grid)
                    {
                        if (!c.IsEmpty())
                        {
                            nonEmpty++;
                        }
                    }
                    if (nonEmpty == 0)
                    {
                        UpdateResults(true);
                        gu.DrawEndScreen(true);
                        iu.SetGameOver();
                    }
                }

                if (pu.winCondition - 1 == (int)ParamUnit.WinCondition.SCORE_REACHED)
                {
                    if (score >= pu.scoreToWin && ((checkTime && timeLeft <= 0) || (checkMoves && movesLeft <= 0)))
                    {
                        UpdateResults(true);
                        gu.DrawEndScreen(true);
                        iu.SetGameOver();
                    }
                }
            }
        }
	}
    
    /// <summary>
    /// Returns randomly generated logical gem
    /// </summary>
    /// <returns></returns>
    public Gem GetRandomGem()
    {
        Gem gem = new Gem
        {
            Bonus = pu.GetRandomBonus()
        };

        // Some specific colors are needed for some bonuses
        if (gem.Bonus != -1)
        {
            switch(gem.Bonus)
            {
                case (int)ParamUnit.Bonus.COLORLESS:
                    gem.Color = 8;
                    break;
                case (int)ParamUnit.Bonus.OBSTACLE:
                    gem.Color = 9;
                    break;
                default:
                    gem.Color = pu.GetRandomColor();
                    break;
            }
        } else
        {
            gem.Color = pu.GetRandomColor();
        }

        return gem;
    }

    /// <summary>
    /// Destroys gem at the given location 
    /// and performs actions according to its bonus 
    /// </summary>
    /// <param name="x">X-pos of the gem to be destroyed</param>
    /// <param name="y">Y-pos of the gem to be destroyed</param>
    public int DestroyGem(int x, int y)
    {
        bool needToDestroy = true;
        int localScore = 0;

        if (grid[x, y].Gem.IsBonus())
        {
            switch (grid[x, y].Gem.Bonus)
            {
                case (int)ParamUnit.Bonus.METEOR:
                    gu.ActivateMeteorBonus(x, y);
                    localScore += pu.scoreUnit;
                    break;
                case (int)ParamUnit.Bonus.SAME_COLOR:
                    if (!bonusIsWorking)
                    {
                        bonusIsWorking = true;
                        // Search for gems with the same color
                        int color = grid[x, y].Gem.Color;
                        for (int _x = 0; _x < gSizeX; _x++)
                        {
                            for (int _y = 0; _y < gSizeY; _y++)
                            {
                                if (!grid[_x, _y].IsEmpty() && grid[_x, _y].Gem.Color == color)
                                {
                                    gu.DestroyGem(_x, _y, color);
                                    localScore += DestroyGem(_x, _y);
                                }
                            }
                        }
                    }
                    localScore += pu.scoreUnit;
                    bonusIsWorking = false;
                    break;
                case (int)ParamUnit.Bonus.ENERGY:
                    IncreaseSuboptimal();
                    localScore += pu.scoreUnit;
                    break;
                case (int)ParamUnit.Bonus.ICE_1:
                    needToDestroy = false;
                    grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.ICE_2;
                    gu.RespawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    localScore += pu.scoreUnit / 2;
                    break;
                case (int)ParamUnit.Bonus.ICE_2:
                    needToDestroy = false;
                    grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.ICE_3;
                    gu.RespawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    localScore += pu.scoreUnit / 2;
                    break;
                case (int)ParamUnit.Bonus.ICE_3:
                    needToDestroy = false;
                    grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.NONE;
                    gu.RespawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    localScore += pu.scoreUnit / 2;
                    break;
            }            
        }

        if (needToDestroy)
        {
            if (grid[x, y].Gem != null)
            {
                if (colorsMetal.Contains(grid[x, y].Gem.Color))
                {
                    collectedMetal++;
                }
                if (colorsFuel.Contains(grid[x, y].Gem.Color))
                {
                    collectedFuel++;
                }
                localScore += pu.scoreUnit;
            }

            grid[x, y].Gem = null;          
        }
        needToCheck = true;
        return localScore;
    }

    /// <summary>
    /// Checks for gems that should be destroyed and perform gravity of the gems
    /// </summary>
    public void CheckGemGrid()
    {
        needToCheck = false;

        bool[,] needToDestroy = new bool[gSizeX, gSizeY];
        for (int x = 0; x < gSizeX; ++x)
        {
            for (int y = 0; y < gSizeY; ++y)
            {
                needToDestroy[x, y] = false;
            }
        }

        // Check for destruction
        #region From left to right
        for (int y = 0; y < gSizeY; y++)
        {
            for (int x = 0; x <= gSizeX - pu.sequenceSize; x++)
            {
                int matchingGems = 0;
                for (int cx1 = x; cx1 < x + pu.sequenceSize; cx1++)
                {
                    for (int cx2 = x; cx2 < x + pu.sequenceSize; cx2++)
                    {
                        if (!grid[cx1, y].IsEmpty() && !grid[cx2, y].IsEmpty() && 
                            ColorsAreMatching(grid[cx1,y].Gem, grid[cx2, y].Gem))
                        {
                            matchingGems++;
                        }
                    }
                }
                if (matchingGems == pu.sequenceSize * pu.sequenceSize)
                {
                    for (int cx = x; cx < x + pu.sequenceSize; cx++)
                    {
                        needToDestroy[cx,y] = true;
                    }
                }
            }
        }
        #endregion
        #region From bottom to top
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y <= gSizeY - pu.sequenceSize; y++)
            {
                int matchingGems = 0;
                for (int cy1 = y; cy1 < y + pu.sequenceSize; cy1++)
                {
                    for (int cy2 = y; cy2 < y + pu.sequenceSize; cy2++)
                    {
                        if (!grid[x, cy1].IsEmpty() && !grid[x, cy2].IsEmpty() && 
                            ColorsAreMatching(grid[x, cy1].Gem, grid[x, cy2].Gem))
                        {
                            matchingGems++;
                        }
                    }
                }
                if (matchingGems == pu.sequenceSize * pu.sequenceSize)
                {
                    for (int cy = y; cy < y + pu.sequenceSize; cy++)
                    {
                        needToDestroy[x, cy] = true;
                    }
                }
            }
        }
        #endregion

        // Destruction
        bool wasDestroyed = false;
        int scoreToShow = 0;
        int avgX = 0, avgY = 0;
        int destrNum = 0;
        for (int x = 0; x < gSizeX; ++x)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (!grid[x, y].IsEmpty() && 
                    needToDestroy[x, y] &&
                    grid[x, y].Gem.Bonus != (int)ParamUnit.Bonus.OBSTACLE) 
                {
                    avgX += x;
                    avgY += y;
                    destrNum++;

                    wasDestroyed = true;
                    gu.DestroyGem(x, y, grid[x, y].Gem.Color);
                    scoreToShow += DestroyGem(x, y);                    
                }
            }
        }
        if (wasDestroyed)
        {
            avgX /= destrNum;
            avgY /= destrNum;
            gu.SpawnScoreMessage(avgX, avgY, scoreToShow);
            score += scoreToShow;
        }

        if (iu.wasSwap && !wasDestroyed)
        {
            DecreaseSubpotimal();            
        }
        iu.wasSwap = false;

        #region Gravity
        for (int x = 0; x < gSizeX; ++x)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (grid[x, y].IsEmpty())
                {
                    int offset = 1;
                    for (int i = y + 1; i < gSizeY; ++i)
                    {
                        if (!grid[x, i].IsEmpty())
                        {
                            gu.SwapGems(new Vector2(x, i), new Vector2(x, i - offset));
                            SwapGems(new Vector2(x, i), new Vector2(x, i - offset));
                        }
                        else
                        {
                            offset++;
                        }
                    }
                    break;
                }
            }
        }
        #endregion
    }    

    /// <summary>
    /// Swaps gems with the given positions on the grid
    /// </summary>
    /// <param name="pos1">The position of the first gem on the grid</param>
    /// <param name="pos2">The position of the second gem on the grid</param>
    public void SwapGems(Vector2 pos1, Vector2 pos2)
    {
        Gem temp = grid[(int)pos1.x, (int)pos1.y].Gem;
        grid[(int)pos1.x, (int)pos1.y].Gem = grid[(int)pos2.x, (int)pos2.y].Gem;
        grid[(int)pos2.x, (int)pos2.y].Gem = temp;
        needToCheck = true;
    }

    /// <summary>
    /// Check whether the gem with the given location is correct to be selected by user
    /// </summary>
    /// <param name="x">X-pos of the gem</param>
    /// <param name="y">Y-pos of the gem</param>
    /// <returns></returns>
    public bool IsGemValidToSelect(int x, int y)
    {
        if (gemSO == UNSELECTED)
        {
            return true;
        } else
        {
            bool _left = x == (int)gemSO.x - 1 && y == (int)gemSO.y;
            bool right = x == (int)gemSO.x + 1 && y == (int)gemSO.y;
            bool above = y == (int)gemSO.y + 1 && x == (int)gemSO.x;
            bool below = y == (int)gemSO.y - 1 && x == (int)gemSO.x;
            return (_left || right || above || below) && !((int)gemST.x == x && (int)gemST.y == y) ||
                (TwoSelected() && (int)gemSO.x == x && (int)gemSO.y == y);
        }
    }

    /// <summary>
    /// Checks for empty cells and fills it with the random gems
    /// </summary>
    public void FillGemGrid()
    {
        if (pu.spawnNewGems)
        {
            initialSpawnHappened = true;
            for (int x = 0; x < gSizeX; x++)
            {
                for (int y = 0; y < gSizeY; y++)
                {
                    if (grid[x, y].IsEmpty())
                    {
                        needToCheck = true;
                        grid[x, y].Gem = GetRandomGem();
                        gu.SpawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    }
                }
            }
        } else if (!initialSpawnHappened)
        {
            initialSpawnHappened = true;
            for (int x = 0; x < gSizeX; x++)
            {
                for (int y = 0; y < gSizeY; y++)
                {
                    if (grid[x, y].IsEmpty())
                    {
                        needToCheck = true;
                        grid[x, y].Gem = GetRandomGem();
                        gu.SpawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Fills the grid with the given grid preset
    /// </summary>
    /// <param name="givenGrid">The grid to be filled in</param>
    public void FillGemGrid(Cell[,] givenGrid)
    {
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                grid[x, y].Gem = givenGrid[x, y].Gem;
                gu.SpawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);           
            }
        }
    }    

    /// <summary>
    /// Updates some data after loading of the saved level
    /// </summary>
    public void UpdateDataAfterLoading()
    {
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;

        float timeBuff = GameDataManager.instance.GetSheathingBonus();

        timeLeft = pu.timeAvailable * (1f + (timeBuff) / 100f); 
        movesLeft = pu.movesAvailable;
        checkTime = timeLeft != 0;
        checkMoves = movesLeft != 0;
        if (checkTime)
        {
            timeLeft += 3f; // +3 for the initial animation
        }
    }

    public void ComputeResourceColors()
    {
        int metal = pu.colorsAvailable / 2;
        int fuel = pu.colorsAvailable - metal;

        colorsMetal = new List<int>();
        colorsFuel = new List<int>();

        List<int> list = new List<int>(pu.colorVector);

        for (int i = 0; i < metal; i++)
        {
            int index = Random.Range(0, list.Count);
            colorsMetal.Add(list[index]);
            list.RemoveAt(index);            
        }

        for (int i = 0; i < fuel; i++)
        {
            int index = Random.Range(0, list.Count);
            colorsFuel.Add(list[index]);
            list.RemoveAt(index);
        }
    }

    /// <summary>
    /// Resets the selected gems if less than two gems were selected
    /// </summary>
    public void ResetSelection()
    {
        gemSO = UNSELECTED;
        gemST = UNSELECTED;
    }

    public bool TwoSelected()
    {
        return gemSO != UNSELECTED && gemST != UNSELECTED;
    }
    public bool OneSelected()
    {
        return gemSO != UNSELECTED && gemST == UNSELECTED;
    }
    public bool NoOneSelected()
    {
        return gemSO == UNSELECTED && gemST == UNSELECTED;
    }

    private void IncreaseSuboptimal()
    {
        if (suboptimalMoves < pu.maximumEnergy)
        {
            gu.SwitchEnergy(suboptimalMoves);
            suboptimalMoves++;
        } else
        {
            collectedEnergy++;
        }
    }
    private void DecreaseSubpotimal()
    {
        suboptimalMoves--;
        if (suboptimalMoves >= 0)
            gu.SwitchEnergy(suboptimalMoves);
    }

    private bool ColorsAreMatching(Gem g1, Gem g2)
    {
        return 
            (
            g1.Color == g2.Color || 
            g1.Bonus == (int)ParamUnit.Bonus.COLORLESS || 
            g2.Bonus == (int)ParamUnit.Bonus.COLORLESS
            ) 
            &&
            (
            g1.Bonus != (int)ParamUnit.Bonus.OBSTACLE &&
            g2.Bonus != (int)ParamUnit.Bonus.OBSTACLE
            );
    }

    private void UpdateResults(bool isVictory)
    {
        ResultScreenManager.isVictory = isVictory;
        ResultScreenManager.scoreToShow = score;
        ResultScreenManager.timeToShow = (int) (pu.timeAvailable - timeLeft);
        ResultScreenManager.movesToShow = pu.movesAvailable - movesLeft;

        int planetId = GameDataManager.instance.generalData.selectedPlanet;
        int levelId = planetId != -1 ? GameDataManager.instance.planetData[planetId].levelsFinished + 1 : -1;
        bool isEndless = levelId > GameDataManager.instance.planetData[planetId].levelNum;

        // Update maxScore
        int maxScore = GameDataManager.instance.generalData.maxScore;
        if (score > maxScore)
        {
            GameDataManager.instance.generalData.maxScore = score;
            maxScore = score;
        }

        float generalBuff = maxScore == 0 ? 1f : 1f + (score / maxScore) / 2;
        if (checkTime)
        {
            generalBuff += timeLeft / pu.timeAvailable;
        }
        if (checkMoves)
        {
            generalBuff += (float) movesLeft / pu.movesAvailable + 5f;
        }

        float rocketBuff = 1f + GameDataManager.instance.GetFrameBonus();

        float planetBuff = 1f + (float) GameDataManager.instance.generalData.selectedPlanet / GameDataManager.instance.planetNumber / 5f;

        ResultScreenManager.collectedMetal = Mathf.RoundToInt(collectedMetal * rocketBuff * generalBuff * planetBuff);
        ResultScreenManager.collectedFuel = Mathf.RoundToInt(collectedFuel * rocketBuff * generalBuff * planetBuff);
        ResultScreenManager.collectedEnergy = Mathf.RoundToInt(collectedEnergy * rocketBuff * generalBuff * planetBuff);

        if (!isVictory)
        {
            ResultScreenManager.collectedMetal /= 5;
            ResultScreenManager.collectedFuel /= 5;
            ResultScreenManager.collectedEnergy /= 5;
        }

        if (isEndless)
        {
            ResultScreenManager.collectedMetal /= 10;
            ResultScreenManager.collectedFuel /= 10;
            ResultScreenManager.collectedEnergy /= 10;
        }

    }
}
