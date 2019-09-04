using UnityEngine;

public class EditorLogic : MonoBehaviour
{

    [Header("Grid Instance")]
    public Cell[,] grid;
    [Space(10)]

    [Header("Units' refs")]
    public EditorGraphics gu;
    public EditorInput iu;
    public EditorParams pu;

    [HideInInspector]
    public Vector2 gemSO = UNSELECTED;
    [HideInInspector]
    public Vector2 gemST = UNSELECTED;
    [HideInInspector]
    public bool readyToSwap = false;
    [HideInInspector]
    public bool coloringMode = false;
    [HideInInspector]
    public bool bonusingMode = false;

    private static Vector2 UNSELECTED = new Vector2(-1, -1);

    private int gSizeX;
    private int gSizeY;  

    private void Awake()
    {
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

    public Gem GetRandomGem()
    {
        Gem gem = new Gem
        {
            Bonus = pu.GetRandomBonus()
        };

        // Some specific colors are needed for some bonuses
        if (gem.Bonus != (int) ParamUnit.Bonus.NONE)
        {
            switch (gem.Bonus)
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
        }
        else
        {
            gem.Color = pu.GetRandomColor();
        }

        return gem;
    }

    public void SwapGems(Vector2 pos1, Vector2 pos2)
    {
        Gem temp = grid[(int)pos1.x, (int)pos1.y].Gem;
        grid[(int)pos1.x, (int)pos1.y].Gem = grid[(int)pos2.x, (int)pos2.y].Gem;
        grid[(int)pos2.x, (int)pos2.y].Gem = temp;
    }

    public void DestroyGem(int x, int y)
    {        
        grid[x, y].Gem = null;
    }    

    // Checks for empty cells and fill it with the random gems
    public void FillGemGrid()
    {
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                grid[x, y].Gem = GetRandomGem();
                gu.SpawnGem(x, y, grid[x, y].Gem.Color, grid[x, y].Gem.Bonus);
            }
        }
    }

    // Fills the grid with the given grid preset
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

    // Checks if it is valid to select the gem or not
    public bool IsGemValid(int x, int y)
    {
        if (gemSO == UNSELECTED)
        {
            return true;
        }
        else
        {            
            return !((int)gemSO.x == x && (int)gemSO.y == y) && !((int)gemST.x == x && (int)gemST.y == y) ||
                (TwoSelected() && (int)gemSO.x == x && (int)gemSO.y == y);
        }
    }

    public void UpdateDataAfterLoading()
    {
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
        ResetSelection();
    }

    // If less than two gems were selected
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

}
