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
    [Space(10)]

    [Header("Bonus selection panel")]
    public GameObject bonusSelectionGroup;
    public BonusSelection bonusSelector;
    [Space(10)]

    [Header("Units' refs")]
    public EditorLogic lu;
    public EditorInput iu;
    public EditorParams pu;
    [Space(10)]

    [Header("Anchors")]
    public GameObject gemGridAnchor;
    public GameObject toolbarColorAnchor;
    public GameObject toolbarBonusAnchor;

    private GameObject[,] grid;

    private int gSizeX;
    private int gSizeY;

    private int workingObjs = 0;
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
        RecreateGrid(gSizeX, gSizeY);
        initialPos = transform.position;
        
        transform.position = initialPos;
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) + pu.gemSize / 2f,
            pu.gemSize,
            0);
        gemGridAnchor.transform.position = transform.position;
               
        colorSelector.CreatePanel(pu.colorVector, toolbarColorAnchor);        
        bonusSelector.CreatePanel(pu.permittedBonuses, toolbarBonusAnchor);
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

        grid[x, y] = Instantiate(gems[bonus == (int)ParamUnit.Bonus.NONE ? 0 : bonus], gemGridAnchor.transform);
        grid[x, y].transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        if (grid[x, y].GetComponent<Scale>() != null)
        {
            grid[x, y].GetComponent<Scale>().SetLocalScale(new Vector3(pu.gemSize, pu.gemSize, pu.gemSize));
        }
        grid[x, y].transform.position = position;
        grid[x, y].GetComponent<Renderer>().material.color = colors[color];
        grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[color];
        
        switch(bonus)
        {
            case (int)ParamUnit.Bonus.OBSTACLE:
            case (int)ParamUnit.Bonus.ICE_1:
            case (int)ParamUnit.Bonus.ICE_2:
            case (int)ParamUnit.Bonus.ICE_3:
                grid[x, y].tag = "Untouchable";
                break;
            case (int)ParamUnit.Bonus.SAME_COLOR:
                grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[8];
                break;
            default:
                break;
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

        EditorParams.isSaved = false;
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
        for (int i = 0; i < pu.permittedBonuses.Length + 2; ++i) // +2 since energy and deselector are always available 
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
                    EditorParams.isSaved = false;
                    DestroyGem(x, y, lu.grid[x, y].Gem.Color);
                    lu.grid[x, y].Gem.Color = pu.colorVector[colorSelector.currentSelected];
                    switch(lu.grid[x, y].Gem.Bonus)
                    {
                        case (int)ParamUnit.Bonus.COLORLESS:
                        case (int)ParamUnit.Bonus.OBSTACLE:
                            lu.grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.NONE;
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
                    bool wasBonused = false;

                    if (bonusSelector.currentSelected == 0)
                    {                        
                        if (lu.grid[x, y].Gem.Bonus != (int)ParamUnit.Bonus.NONE)
                        {
                            wasBonused = true;
                            if (lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.COLORLESS || lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.OBSTACLE)
                            {
                                lu.grid[x, y].Gem.Color = pu.GetRandomColor();
                            }
                            lu.grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.NONE;
                        }                                                              
                    }
                    else if (bonusSelector.currentSelected == 1)
                    {
                        if (lu.grid[x, y].Gem.Bonus != (int)ParamUnit.Bonus.ENERGY)
                        {
                            wasBonused = true;
                            if (lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.COLORLESS || lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.OBSTACLE)
                            {
                                lu.grid[x, y].Gem.Color = pu.GetRandomColor();
                            }
                            lu.grid[x, y].Gem.Bonus = (int)ParamUnit.Bonus.ENERGY;
                        }                        
                    }
                    else
                    {
                        switch (pu.permittedBonuses[bonusSelector.currentSelected - 2])
                        {
                            case (int)ParamUnit.Bonus.COLORLESS:
                                lu.grid[x, y].Gem.Color = 8;
                                break;
                            case (int)ParamUnit.Bonus.OBSTACLE:
                                lu.grid[x, y].Gem.Color = 9;
                                break;
                            default:
                                if (lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.COLORLESS || lu.grid[x, y].Gem.Bonus == (int)ParamUnit.Bonus.OBSTACLE)
                                {
                                    lu.grid[x, y].Gem.Color = pu.GetRandomColor();
                                }
                                break;
                        }

                        if (lu.grid[x, y].Gem.Bonus != pu.permittedBonuses[bonusSelector.currentSelected - 2])
                        {
                            wasBonused = true;
                            lu.grid[x, y].Gem.Bonus = pu.permittedBonuses[bonusSelector.currentSelected - 2];
                        }
                    }
                    if (wasBonused)
                    {
                        EditorParams.isSaved = false;
                        DestroyGem(x, y, lu.grid[x, y].Gem.Color);
                        SpawnGem(x, y, lu.grid[x, y].Gem.Color, lu.grid[x, y].Gem.Bonus, 0);
                    }
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
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) + pu.gemSize / 2f,
            pu.gemSize,
            0);
        gemGridAnchor.transform.position = transform.position;
        
        colorSelector.ReCreatePanel(pu.colorVector, toolbarColorAnchor);
        bonusSelector.ReCreatePanel(pu.permittedBonuses, toolbarBonusAnchor);
    }

    public bool IsWorking()
    {
        return workingObjs > 0;
    }

    // Moves given gem from it's current position to specific x-y position on the grid
    private IEnumerator MoveGem(GameObject gem, int x, int y)
    {
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        Vector3 newPosition = transform.position;
        newPosition.x += (pu.gemSize + pu.gemOffset) * x;
        newPosition.y += (pu.gemSize + pu.gemOffset) * y;
        workingObjs++;
        while (velocity.sqrMagnitude > .001f)
        {
            if (gem == null)
            {
                break;
            }
            gem.transform.position = Vector3.SmoothDamp(gem.transform.position, newPosition, ref velocity, pu.gemMoveTime);
            yield return new WaitForEndOfFrame();
        }
        workingObjs--;
    }

    // Scales given gem to some scale
    private IEnumerator ScaleGem(GameObject gem, Vector3 finalScale, float speed)
    {
        Vector3 currentScale = gem.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime * speed)
        {
            if (gem != null)
            {
                gem.transform.localScale = Vector3.Lerp(currentScale, finalScale, t);
                yield return new WaitForEndOfFrame();         
            }
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
