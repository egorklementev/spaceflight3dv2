using UnityEngine;

public class LogicUnit : MonoBehaviour {
    
    [Header("Grid Instance")]
    public Cell[,] grid;
    [Space(10)]

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

    private static Vector2 UNSELECTED = new Vector2(-1, -1);

    private int gSizeX;
    private int gSizeY;

    private bool needToCheck = false;

    private void Awake() {        
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;

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
        if (gu.WorkingObjs == 0)
        {
            if (needToCheck)
            {
                CheckGemGrid();
                FillGemGrid();
            }
        }
	}
    
    public Gem GetRandomGem()
    {
        Gem gem = new Gem
        {
            Bonus = pu.GetRandomBonus()
        };
        if (gem.Bonus == 2)
        {
            gem.Color = 0;
        } else
        {
            gem.Color = pu.GetRandomColor();
        }

        return gem;
    }

    public void DestroyGem(int x, int y)
    {
        if (grid[x, y].Gem.Bonus != -1)
        {
            switch (grid[x, y].Gem.Bonus)
            {
                case 1:
                    gu.ActivateBonus1(x, y);
                    break;
                case 2:
                    break;
            }            
        }
        grid[x, y].Gem = null;
    }

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

        // From left to rigth
        for (int y = 0; y < gSizeY; ++y)
        {
            int curX = 0;
            int currentEqual = 1;
            while (curX < (gSizeX - 1))
            {
                while (curX < (gSizeX - 1) && 
                    !grid[curX, y].IsEmpty() && // No need to check empty cell
                    !grid[curX + 1, y].IsEmpty() &&
                    (grid[curX, y].Gem.Color == grid[curX + 1, y].Gem.Color))
                {
                    currentEqual++;
                    curX++;
                }
                
                if (currentEqual >= pu.sequenceSize)
                {
                    for (int x = curX - currentEqual + 1; x <= curX; ++x)
                    {
                        needToDestroy[x, y] = true;
                    }
                }

                currentEqual = 1;
                curX++;
            }
        }

        // From top to bottom
        for (int x = 0; x < gSizeX; ++x)
        {
            int curY = 0;
            int currentEqual = 1;
            while (curY < (gSizeY - 1))
            {
                while (curY < (gSizeY - 1) && 
                    !grid[x, curY].IsEmpty() && // No need to check empty cell
                    !grid[x, curY + 1].IsEmpty() &&
                    (grid[x, curY].Gem.Color == grid[x, curY + 1].Gem.Color))
                {
                    currentEqual++;
                    curY++;
                }
                
                if (currentEqual >= pu.sequenceSize)
                {
                    for (int y = curY - currentEqual + 1; y <= curY; ++y)
                    {
                        needToDestroy[x, y] = true;
                    }
                }

                currentEqual = 1;
                curY++;
            }
        }

        #region Colorless gems
        for (int x = 0; x < gSizeX; ++x)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (!grid[x, y].IsEmpty() && 
                    grid[x, y].Gem.Bonus == 2)
                {
                    // Count equal ones to the left
                    int curX = x - 1;
                    int equalLeft = 1;
                    if (curX < 0)
                    {
                        equalLeft = 0;
                    } else
                    {
                        while (curX > 0 &&
                        !grid[curX, y].IsEmpty() &&
                        !grid[curX - 1, y].IsEmpty() &&
                        (grid[curX, y].Gem.Bonus == 2 || 
                        grid[curX, y].Gem.Color == grid[curX - 1, y].Gem.Color))
                        {
                            equalLeft++;
                            curX--;
                        }
                    }

                    // Count equal ones to the right
                    curX = x + 1;
                    int equalRight = 1;
                    if (curX >= gSizeX)
                    {
                        equalRight = 0;
                    }
                    else
                    {
                        while (curX < (gSizeX - 1) &&
                        !grid[curX, y].IsEmpty() &&
                        !grid[curX + 1, y].IsEmpty() &&
                        (grid[curX, y].Gem.Bonus == 2 || 
                        grid[curX, y].Gem.Color == grid[curX + 1, y].Gem.Color))
                        {
                            equalRight++;
                            curX++;
                        }
                    }

                    // If sufficient - destroy
                    if (equalLeft != 0 && 
                        equalRight != 0 &&
                        x - 1 >= 0 &&
                        x + 1 < gSizeX &&
                        !grid[x - 1, y].IsEmpty() &&
                        !grid[x + 1, y].IsEmpty() &&
                        grid[x - 1, y].Gem.Color == grid[x + 1, y].Gem.Color &&
                        equalLeft + equalRight + 1 >= pu.sequenceSize)
                    {
                        // Left
                        for (int i = x; i >= x - equalLeft; i--)
                        {
                            needToDestroy[i, y] = true;
                        }
                        // Right
                        for (int i = x; i <= x + equalRight; i++)
                        {
                            needToDestroy[i, y] = true;
                        }
                    }
                    if (equalLeft != 0 &&                        
                        equalLeft + 1 >= pu.sequenceSize)
                    {
                        for (int i = x; i >= x - equalLeft; i--)
                        {
                            needToDestroy[i, y] = true;
                        }
                    }
                    if (equalRight != 0 &&
                        equalRight + 1 >= pu.sequenceSize)
                    {
                        for (int i = x; i <= x + equalRight; i++)
                        {
                            needToDestroy[i, y] = true;
                        }
                    }

                    // Count equal ones to the top
                    int curY = y + 1;
                    int equalTop = 1;
                    if (curY >= gSizeY)
                    {
                        equalTop = 0;
                    }
                    else
                    {
                        while (curY < (gSizeY - 1) &&
                        !grid[x, curY].IsEmpty() &&
                        !grid[x, curY + 1].IsEmpty() &&
                        (grid[x, curY].Gem.Bonus == 2 || grid[x, curY].Gem.Color == grid[x, curY + 1].Gem.Color))
                        {
                            equalTop++;
                            curY++;
                        }
                    }

                    // Count equal ones to the bottom
                    curY = y - 1;
                    int equalBottom = 1;
                    if (curY < 0)
                    {
                        equalBottom = 0;
                    }
                    else
                    {
                        while (curY > 0 &&
                        !grid[x, curY].IsEmpty() &&
                        !grid[x, curY - 1].IsEmpty() &&
                        (grid[x, curY].Gem.Bonus == 2 || 
                        grid[x, curY].Gem.Color == grid[x, curY - 1].Gem.Color))
                        {
                            equalBottom++;
                            curY--;
                        }
                    }

                    // If sufficient - destroy
                    if (equalBottom != 0 &&
                        equalTop != 0 &&
                        y - 1 >= 0 && 
                        y + 1 < gSizeY &&
                        !grid[x, y - 1].IsEmpty() &&
                        !grid[x, y + 1].IsEmpty() &&
                        grid[x, y - 1].Gem.Color == grid[x, y + 1].Gem.Color &&
                        equalBottom + equalTop + 1 >= pu.sequenceSize)
                    {
                        // Bottom
                        for (int i = y; i >= y - equalBottom; i--)
                        {
                            needToDestroy[x, i] = true;
                        }
                        // Right
                        for (int i = y; i <= y + equalTop; i++)
                        {
                            needToDestroy[x, i] = true;
                        }
                    }
                    if (equalBottom != 0 &&
                        equalBottom + 1 >= pu.sequenceSize)
                    {
                        for (int i = y; i >= y - equalBottom; i--)
                        {
                            needToDestroy[x, i] = true;
                        }
                    }
                    if (equalTop != 0 &&
                        equalTop + 1 >= pu.sequenceSize)
                    {
                        for (int i = y; i <= y + equalTop; i++)
                        {
                            needToDestroy[x, i] = true;
                        }
                    }
                }
            }
        }
        #endregion

        // Destruction
        for (int x = 0; x < gSizeX; ++x)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (needToDestroy[x, y])
                {
                    gu.DestroyGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
                    DestroyGem(x, y);                    
                }
            }
        }

        // Gravity
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
    }

    // Checks if it is valid to select the gem or not
    public bool IsGemValid(int x, int y)
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

    // Checks for empty cells and fill it with the random gems
    public void FillGemGrid()
    {
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
    
    public void SwapGems(Vector2 pos1, Vector2 pos2)
    {
        Gem temp = grid[(int)pos1.x, (int)pos1.y].Gem;
        grid[(int)pos1.x, (int)pos1.y].Gem = grid[(int)pos2.x, (int)pos2.y].Gem;
        grid[(int)pos2.x, (int)pos2.y].Gem = temp;
        needToCheck = true;
    }

    // If less than two gems were selected
    public void ResetSelection()
    {
        gemSO = UNSELECTED;
        gemST = UNSELECTED;
    }

    // Conditions to swap
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
    
}
