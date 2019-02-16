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
        return new Gem
        {
            Color = pu.GetRandomColor(),
            Bonus = pu.GetRandomBonus()
        };
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
                    grid[curX, y].Gem.Color == grid[curX + 1, y].Gem.Color)
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
                    grid[x, curY].Gem.Color == grid[x, curY + 1].Gem.Color)
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
