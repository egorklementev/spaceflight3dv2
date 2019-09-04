using UnityEngine;
using System.Collections;

public class EditorGraphics : MonoBehaviour
{

    [Header("Meshes")]
    public Mesh[] gridGemsMeshes;
    [Space(10)]

    [Header("Prefabs")]
    public GameObject[] gems; // Gems including bonuses
    [Space(10)]
    
    [Header("Color selection panel")]
    public GameObject colorSelectionGroup;
    public ColorSelection colorSelector;
    public float colorSize = 1f; 
    [Space(10)]

    [Header("Bonus selection panel")]
    public GameObject bonusSelectionGroup;
    public BonusSelection bonusSelector;
    public float bonusSize = 1f;
    [Space(10)]

    [Header("Units' refs")]
    public EditorLogic lu;
    public EditorInput iu;
    public EditorParams pu;
    public FadeManager fadeManager;
    
    private GameObject[,] grid;

    private int gSizeX;
    private int gSizeY;

    private Vector3 initialPos;

    public static Color[] colors;

    private void Awake()
    {
        colors = new Color[10];
        colors[0] = Color.blue;
        colors[1] = Color.green;
        colors[2] = Color.magenta;
        colors[3] = new Color(1, 165f / 255f, 0);
        colors[4] = Color.red;
        colors[5] = new Color(0, 1, 1);
        colors[6] = Color.white;
        colors[7] = Color.yellow;
        colors[8] = Color.gray;
        colors[9] = Color.white;

        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
        grid = new GameObject[gSizeX, gSizeY];
        RecreateGrid(gSizeX, gSizeY);
        initialPos = transform.position;
        
        transform.position = initialPos;
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) + pu.gemSize / 2f,
            pu.gemSize / 2f,
            0);

        transform.localScale *= 45f; // Magic number   
        
        colorSelector.CreatePanel(pu.gemSize * colorSize, pu.colorVector);        
        bonusSelector.CreatePanel(pu.gemSize * bonusSize, pu.permittedBonuses);
    }

    private void Update()
    {

    }

    public void SpawnGem(int x, int y, int color, int bonus)
    {
        SpawnGem(x, y, color, bonus, (pu.gemSize + pu.gemOffset) * (gSizeY + 1));        
    }

    public void SpawnGem(int x, int y, int color, int bonus, float v_offset)
    {
        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y + v_offset;

        grid[x, y] = Instantiate(gems[bonus == -1 ? 0 : bonus], transform);
        grid[x, y].transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        if (grid[x, y].GetComponent<Scale>() != null)
        {
            grid[x, y].GetComponent<Scale>().SetLocalScale(new Vector3(pu.gemSize, pu.gemSize, pu.gemSize));
        }
        grid[x, y].transform.position = position;
        grid[x, y].GetComponent<Renderer>().material.color = colors[color];
        grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[color];
        if (bonus == 3)
        {
            grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[8];
        }
        if (bonus == 4)
        {
            grid[x, y].tag = "Unbreakable";
        }

        StartCoroutine(MoveGem(grid[x, y], x, y));
    }

    public void DestroyGem(int x, int y, int color)
    {

        Destroy(grid[x, y]);
        grid[x, y] = null;        
    }

    public void SwapGems(Vector2 pos1, Vector2 pos2)
    {
        // * In case we perform swap with empty cell
        if (grid[(int)pos1.x, (int)pos1.y] != null) // *
        {
            StartCoroutine(MoveGem(grid[(int)pos1.x, (int)pos1.y], (int)pos2.x, (int)pos2.y));
        }
        if (grid[(int)pos2.x, (int)pos2.y] != null) // *
        {
            StartCoroutine(MoveGem(grid[(int)pos2.x, (int)pos2.y], (int)pos1.x, (int)pos1.y));
        }

        GameObject temp = grid[(int)pos1.x, (int)pos1.y];
        grid[(int)pos1.x, (int)pos1.y] = grid[(int)pos2.x, (int)pos2.y];
        grid[(int)pos2.x, (int)pos2.y] = temp;
    }

    // Operates with the selection of the gems
    public void SelectGem(GameObject gem)
    {
        // Search for gem inside grid array
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (gem.Equals(grid[x, y]))
                {
                    if (lu.IsGemValid(x, y))
                    { 
                        if (lu.NoOneSelected())
                        {
                            lu.gemSO = new Vector2(x, y);

                            Vector3 bigScale = new Vector3(pu.gemSize * 1.25f, pu.gemSize * 1.25f, pu.gemSize * 1.25f);
                            StartCoroutine(ScaleGem(gem, bigScale, pu.gemScaleSpeed));
                        }
                        else if (lu.OneSelected())
                        {
                            lu.gemST = new Vector2(x, y);

                            Vector3 bigScale = new Vector3(pu.gemSize * 1.25f, pu.gemSize * 1.25f, pu.gemSize * 1.25f);
                            StartCoroutine(ScaleGem(gem, bigScale, pu.gemScaleSpeed));
                        }
                        else if (lu.TwoSelected())
                        {
                            if ((int)lu.gemSO.x == x && (int)lu.gemSO.y == y)
                            {
                                Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
                                StartCoroutine(ScaleGem(grid[(int)lu.gemST.x, (int)lu.gemST.y], normalScale, pu.gemScaleSpeed));

                                lu.gemST = new Vector2(-1, -1);
                            }
                            else
                            {
                                Vector3 bigScale = new Vector3(pu.gemSize * 1.25f, pu.gemSize * 1.25f, pu.gemSize * 1.25f);
                                StartCoroutine(ScaleGem(gem, bigScale, pu.gemScaleSpeed));

                                Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
                                StartCoroutine(ScaleGem(grid[(int)lu.gemST.x, (int)lu.gemST.y], normalScale, pu.gemScaleSpeed));

                                lu.gemST = new Vector2(x, y);
                            }
                        }
                        else
                        {
                            float side = gem.transform.localScale.x;
                            Vector3 scale = new Vector3(side * 1.25f, side * 1.25f, side * 1.25f);
                            StartCoroutine(ScaleGem(gem, scale, pu.gemScaleSpeed));
                        }                   
                    }
                }
            }
        }
    }

    public void SelectColor(GameObject gem)
    {
        // Search if it is color panel
        for (int i = 0; i < pu.colorVector.Length; ++i)
        {
            if (gem.Equals(colorSelector.colorGrid[i]) && !colorSelector.isProcessing)
            {
                if (!lu.coloringMode && !lu.bonusingMode)
                {
                    lu.coloringMode = true;
                }
                else if (lu.bonusingMode)
                {
                    bonusSelector.SelectBonus(bonusSelector.currentSelected);
                    lu.bonusingMode = false;
                    lu.coloringMode = true;
                }
                else if (colorSelector.currentSelected == i)
                {
                    lu.coloringMode = false;
                }
                colorSelector.SelectColor(i);
            }
        }
    }

    public void SelectBonus(GameObject gem)
    {
        // Search if it is bonus panel
        for (int i = 0; i < pu.permittedBonuses.Length; ++i)
        {
            if (gem.Equals(bonusSelector.bonusGrid[i]) && !bonusSelector.isProcessing)
            {
                if (!lu.bonusingMode && !lu.coloringMode)
                {
                    lu.bonusingMode = true;
                }
                else if (lu.coloringMode)
                {
                    colorSelector.SelectColor(colorSelector.currentSelected);
                    lu.coloringMode = false;
                    lu.bonusingMode = true;
                }
                else if (bonusSelector.currentSelected == i)
                {
                    lu.bonusingMode = false;
                }
                bonusSelector.SelectBonus(i);
            }
        }
    }

    public void ColorGem(GameObject gem)
    {
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (gem.Equals(grid[x, y]))
                {
                    DestroyGem(x, y, lu.grid[x, y].Gem.Color);
                    lu.grid[x, y].Gem.Color = pu.colorVector[colorSelector.currentSelected];
                    switch(lu.grid[x, y].Gem.Bonus)
                    {
                        case 2:
                        case 4:
                            lu.grid[x, y].Gem.Bonus = -1;
                            break;
                    }
                    SpawnGem(x, y, lu.grid[x, y].Gem.Color, lu.grid[x, y].Gem.Bonus, 0);
                }
            }
        }
    }

    public void BonusGem(GameObject gem)
    {
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (gem.Equals(grid[x, y]))
                {
                    DestroyGem(x, y, lu.grid[x, y].Gem.Color);

                    switch (pu.permittedBonuses[bonusSelector.currentSelected])
                    {    
                        case 2:                       
                            lu.grid[x, y].Gem.Color = 8;
                            break;
                        case 4:
                            lu.grid[x, y].Gem.Color = 9;
                            break;
                        default:
                            if (lu.grid[x, y].Gem.Bonus == 2 || lu.grid[x, y].Gem.Bonus == 4)
                            {
                                lu.grid[x, y].Gem.Color = pu.GetRandomColor();
                            }
                            break;
                    }


                    lu.grid[x, y].Gem.Bonus = pu.permittedBonuses[bonusSelector.currentSelected];                    
                    SpawnGem(x, y, lu.grid[x, y].Gem.Color, lu.grid[x, y].Gem.Bonus, 0);
                }
            }
        }
    }    

    // If less than two gems were selected
    public void ResetSelection()
    {
        if (lu.gemSO != new Vector2(-1, -1))
        {
            Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
            StartCoroutine(ScaleGem(grid[(int)lu.gemSO.x, (int)lu.gemSO.y], normalScale, pu.gemScaleSpeed));
        }
        if (lu.gemST != new Vector2(-1, -1))
        {
            Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
            StartCoroutine(ScaleGem(grid[(int)lu.gemST.x, (int)lu.gemST.y], normalScale, pu.gemScaleSpeed));
        }
    }

    public void RecreateGrid(int newGSizeX, int newGSizeY)
    {
        grid = new GameObject[newGSizeX, newGSizeY];
    }

    public void UpdateDataAfterLoading()
    {
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
        transform.position = initialPos;
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) / 2f + pu.gemSize / 2f,
            pu.gemSize / 2f,
            0);
        
        colorSelector.ReCreatePanel(pu.gemSize * colorSize, pu.colorVector);
        bonusSelector.ReCreatePanel(pu.gemSize * bonusSize, pu.permittedBonuses);
    }

    // Moves given gem from it's current position to specific x-y position on the grid
    private IEnumerator MoveGem(GameObject gem, int x, int y)
    {
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        Vector3 newPosition = transform.position;
        newPosition.x += (pu.gemSize + pu.gemOffset) * x;
        newPosition.y += (pu.gemSize + pu.gemOffset) * y;
        while (velocity.sqrMagnitude > .001f)
        {
            if (gem == null)
            {
                break;
            }
            gem.transform.position = Vector3.SmoothDamp(gem.transform.position, newPosition, ref velocity, pu.gemMoveTime);
            yield return new WaitForEndOfFrame();
        }
    }

    // Scales given gem to some scale
    private IEnumerator ScaleGem(GameObject gem, Vector3 finalScale, float speed)
    {
        Vector3 currentScale = gem.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime * speed)
        {
            gem.transform.localScale = Vector3.Lerp(currentScale, finalScale, t);
            yield return new WaitForEndOfFrame();
        }
    }

    // Returns graphical position given x and y on the grid
    private Vector3 GetGraphPos(int x, int y)
    {
        return new Vector3(
            transform.position.x + (pu.gemSize + pu.gemOffset) * x,
            transform.position.y + (pu.gemSize + pu.gemOffset) * y,
            transform.position.z);
    }
}
