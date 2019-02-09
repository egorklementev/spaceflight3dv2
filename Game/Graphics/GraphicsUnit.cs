using UnityEngine;
using System.Collections;

public class GraphicsUnit : MonoBehaviour {

    //public RectTransform transform;

    [Header("Prefabs")]
    public GameObject[] gems;
    [Space(10)]

    [Header("Units' refs")]
    public LogicUnit lu;
    public InputUnit iu;
    public ParamUnit pu;

    [HideInInspector]
    public int WorkingObjs { get; set; }

    private GameObject[,] grid;

    private int gSizeX;
    private int gSizeY;

    private float localSize;
    
    private void Awake()
    {
        WorkingObjs = 0;
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
        grid = new GameObject[gSizeX, gSizeY];
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) / 2f + pu.gemSize / 2f,
            pu.gemSize / 2f,
            0);
        transform.localScale *= 45f;
    }

    private void Update()
    {

    }
    
    public void SpawnGem(int x, int y)
    {
        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y + (pu.gemSize + pu.gemOffset) * (gSizeY + 1); // Higher than the grid         

        grid[x, y] = Instantiate(gems[lu.grid[x, y].Gem.Color]);
        grid[x, y].transform.parent = transform;
        grid[x, y].transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        grid[x, y].transform.position = position;
        
        StartCoroutine(MoveGem(grid[x, y], x, y));
    }

    public void SwapGems(Vector2 pos1, Vector2 pos2)
    {
        StartCoroutine(MoveGem(grid[(int)pos1.x, (int)pos1.y], (int)pos2.x, (int)pos2.y));
        StartCoroutine(MoveGem(grid[(int)pos2.x, (int)pos2.y], (int)pos1.x, (int)pos1.y));

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
                        } else if (lu.OneSelected())
                        {
                            lu.gemST = new Vector2(x, y);
                            
                            Vector3 bigScale = new Vector3(pu.gemSize * 1.25f, pu.gemSize * 1.25f, pu.gemSize * 1.25f);
                            StartCoroutine(ScaleGem(gem, bigScale, pu.gemScaleSpeed));
                        } else if (lu.TwoSelected())
                        {
                            if ((int)lu.gemSO.x == x && (int)lu.gemSO.y == y)
                            {
                                Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
                                StartCoroutine(ScaleGem(grid[(int)lu.gemST.x, (int)lu.gemST.y], normalScale, pu.gemScaleSpeed));

                                lu.gemST = new Vector2(-1, -1);
                            } else
                            {
                                Vector3 bigScale = new Vector3(pu.gemSize * 1.25f, pu.gemSize * 1.25f, pu.gemSize * 1.25f);
                                StartCoroutine(ScaleGem(gem, bigScale, pu.gemScaleSpeed));

                                Vector3 normalScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
                                StartCoroutine(ScaleGem(grid[(int)lu.gemST.x, (int)lu.gemST.y], normalScale, pu.gemScaleSpeed));

                                lu.gemST = new Vector2(x, y);
                            }
                        } else
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
    
    // Moves given gem from it's current position to specific x-y position on the grid
    private IEnumerator MoveGem(GameObject gem, int x, int y)
    {
        WorkingObjs++;
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        Vector3 newPosition = transform.position;
        newPosition.x += (pu.gemSize + pu.gemOffset) * x;
        newPosition.y += (pu.gemSize + pu.gemOffset) * y;
        while (velocity.sqrMagnitude > .001f)
        {
            gem.transform.position = Vector3.SmoothDamp(gem.transform.position, newPosition, ref velocity, pu.gemMoveTime);
            yield return new WaitForEndOfFrame();
        }
        WorkingObjs--;
    }

    // Scales given gem to some scale
    private IEnumerator ScaleGem(GameObject gem, Vector3 finalScale, float speed)
    {
        //WorkingObjs++;
        Vector3 currentScale = gem.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime * speed)
        {
            gem.transform.localScale = Vector3.Lerp(currentScale, finalScale, t);
            yield return new WaitForEndOfFrame();         
        }
        //WorkingObjs--;
    }
}
