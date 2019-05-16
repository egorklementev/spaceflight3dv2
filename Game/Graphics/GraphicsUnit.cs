using UnityEngine;
using System.Collections;

public class GraphicsUnit : MonoBehaviour {
    
    [Header("Meshes")]
    public Mesh[] gridGemsMeshes;
    public Mesh[] gridGemsDParts;
    [Space(10)]

    [Header("Prefabs")]
    public GameObject[] gems; // Gems including bonuses
    public GameObject gemPart; // Part of the destroyed gem
    public GameObject meteorPrefab; // Meteor falling on the grid
    public GameObject energyAnchor; 
    public GameObject energyPrefab; 
    public ParticleSystem explosionPrefab;
    [Space(10)]

    [Header("UI")]
    public GameObject winText;
    public GameObject loseText;
    [Space(10)]

    [Header("Units' refs")]
    public LogicUnit lu;
    public InputUnit iu;
    public ParamUnit pu;
    public FadeManager fadeManager;

    [HideInInspector]
    public int WorkingObjs { get; set; }

    private GameObject[] energyBar;
    private GameObject[,] grid;

    private int gSizeX;
    private int gSizeY;

    private Color[] colors;

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
        transform.localScale *= 45f; // Magic number

        energyBar = new GameObject[pu.maximumEnergy];
        for (int i = 0; i < pu.maximumEnergy; i++) 
        {
            AddEnergy(i);
        }

        colors = new Color[10];
        colors[0] = Color.blue;
        colors[1] = Color.green;
        colors[2] = Color.magenta;
        colors[3] = new Color(1, 165f/255f, 0);
        colors[4] = Color.red;
        colors[5] = new Color(0, 1, 1);
        colors[6] = Color.white;
        colors[7] = Color.yellow;
        colors[8] = Color.gray;
        colors[9] = Color.white;
    }

    private void Update()
    {

    }
    
    public void SpawnGem(int x, int y, int color, int bonus)
    {
        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y + (pu.gemSize + pu.gemOffset) * (gSizeY + 1); // Higher than the grid         
      
        grid[x, y] = Instantiate(gems[bonus == -1 ? 0 : bonus], transform);
        grid[x, y].transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        grid[x, y].transform.position = position;
        grid[x, y].GetComponent<Renderer>().material.color = colors[color];
        grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[color];        
        if (bonus == 3)
        {
            grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[8];
        }

        StartCoroutine(MoveGem(grid[x, y], x, y));
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

    public void DestroyGem(int x, int y, int color)
    {
        Destroy(grid[x, y]);
        grid[x, y] = null;

        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y;

        int num = Random.Range(2, 4);
        for (int i = 0; i < num; i++)
        {
            GameObject dGemPart = Instantiate(gemPart);
            Destroy(dGemPart, pu.dPartsLifetime);
            dGemPart.transform.parent = transform;
            dGemPart.transform.localScale = new Vector3(.75f * pu.gemSize, .75f * pu.gemSize, .75f * pu.gemSize);
            dGemPart.transform.position = position;
            dGemPart.GetComponent<Renderer>().material.color = colors[color];
            dGemPart.GetComponent<MeshFilter>().mesh = gridGemsDParts[Random.Range(0, 3)];
            dGemPart.GetComponent<BoxCollider>().size = new Vector3(.75f * pu.gemSize, .75f * pu.gemSize, .75f * pu.gemSize);

            Vector3 forceDirection = new Vector3(
                Random.Range(-pu.destructionForce, pu.destructionForce),
                Random.Range(-pu.destructionForce, pu.destructionForce),
                Random.Range(-pu.destructionForce, pu.destructionForce)
            );
            dGemPart.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
        }        
    }

    public void ActivateBonus1(int x, int y)
    {
        int _x = Random.Range(0, gSizeX);
        int _y = Random.Range(0, gSizeY);

        GameObject meteor = Instantiate(meteorPrefab);
        meteor.transform.parent = transform;
        meteor.transform.position = GetGraphPos(x, y);
        meteor.transform.localScale = new Vector3(.75f * pu.gemSize, .75f * pu.gemSize, .75f * pu.gemSize);
        meteor.transform.Translate(0f, pu.meteorOffset, -1f);
        StartCoroutine(MoveMeteor(
            meteor, _x, _y,
            lu.grid[_x, _y].IsEmpty() ? 0 : lu.grid[_x, _y].Gem.Color));
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

    public void DrawEndScreen(bool isVictory)
    {
        if (isVictory)
        {
            winText.SetActive(true);
        } else
        {
            loseText.SetActive(true);
        }
        fadeManager.FadeToLevel(1);
    }

    public void SwitchEnergy(int i)
    {
        energyBar[i].GetComponent<Rotation>().enabled = !energyBar[i].GetComponent<Rotation>().enabled;

        foreach (Transform child in energyBar[i].transform)
        {
            child.gameObject.GetComponent<Renderer>().material.color *= 
                energyBar[i].GetComponent<Rotation>().enabled ? 5f : .2f;
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
        transform.position = new Vector3(0, transform.position.y, transform.position.z);
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) / 2f + pu.gemSize / 2f,
            pu.gemSize / 2f,
            0);        
    }

    // Creates an instance to the anchor
    private void AddEnergy(int i)
    {
        energyBar[i] = Instantiate(energyPrefab, energyAnchor.transform);
        energyBar[i].transform.localScale = new Vector3(1.5f * pu.gemSize, pu.gemSize, pu.gemSize);
        energyBar[i].transform.Translate(
            .75f * energyBar[i].transform.localScale.x * i, 0f, 0f
            );
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
            if (gem == null)
            {
                break;
            }
            gem.transform.position = Vector3.SmoothDamp(gem.transform.position, newPosition, ref velocity, pu.gemMoveTime);
            yield return new WaitForEndOfFrame();
        }
        WorkingObjs--;
    }

    private IEnumerator MoveMeteor(GameObject meteor, int x, int y, int color)
    {
        while(WorkingObjs > 0)
        {
            yield return new WaitForFixedUpdate();
        }
        WorkingObjs++;
        Vector3 start = meteor.transform.position;
        Vector3 newPosition = GetGraphPos(x, y);
        float t = 0f;
        while (t <= 1f)
        {
            t += (pu.meteorMoveSpeed / (start - newPosition).magnitude) * Time.fixedDeltaTime;
            meteor.transform.position = Vector3.Lerp(start, newPosition, t);
            yield return new WaitForFixedUpdate();
        }       
        if (!lu.grid[x, y].IsEmpty())
        {
            DestroyGem(x, y, color);
            lu.DestroyGem(x, y);
        }
            
        // Trail is destroying separately to prevent particles dissapearing
        GameObject trail = meteor.transform.Find("Trail").gameObject;
        trail.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmitting);
        trail.transform.parent = transform;
        trail.transform.localScale = new Vector3(1f, 1f, 1f);
        Destroy(trail, trail.GetComponent<ParticleSystem>().main.startLifetime.constant);
        ParticleSystem explosion = Instantiate(explosionPrefab);
        explosion.transform.parent = transform;
        explosion.transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        newPosition.y -= pu.gemSize;
        explosion.transform.position = newPosition;

        Destroy(meteor);
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
    
    // Returns graphical position given x and y on the grid
    private Vector3 GetGraphPos(int x, int y)
    {
        return new Vector3(
            transform.position.x + (pu.gemSize + pu.gemOffset) * x,
            transform.position.y + (pu.gemSize + pu.gemOffset) * y, 
            transform.position.z);
    }
}
