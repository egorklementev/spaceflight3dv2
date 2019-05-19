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

    private static Vector2 UNSELECTED = new Vector2(-1, -1);

    private int gSizeX;
    private int gSizeY;

    private bool needToCheck = false;
    private bool bonusIsWorking = false;    

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

    private void Update()
    {
    }

    public Gem GetRandomGem()
    {
        Gem gem = new Gem
        {
            Bonus = pu.GetRandomBonus()
        };

        // Some specific colors are needed for some bonuses
        if (gem.Bonus != -1)
        {
            switch (gem.Bonus)
            {
                case 2:
                    gem.Color = 8;
                    break;
                case 4:
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

    public void UpdateDataAfterLoading()
    {
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
    }

}
