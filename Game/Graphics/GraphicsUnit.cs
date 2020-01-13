using UnityEngine;
using TMPro;
using System.Collections;

/// <summary>
/// A class performing all graphics on the screen, e.g. gem movement, scaling, animations and etc.
/// </summary>
public class GraphicsUnit : MonoBehaviour {
    
    [Header("Meshes")]
    public Mesh[] gridGemsMeshes; // Meshes of regular gems
    public Mesh[] gridGemsDParts; // Meshed of destroyed parts
    [Space(10)]

    [Header("Prefabs")]
    public GameObject[] gems; // Gems including bonuses
    public GameObject gemPart; // Part of the destroyed gem
    public GameObject meteorPrefab; 
    public GameObject energyBarObj; // Initial position of the energy bar
    public GameObject energyPrefab; 
    public ParticleSystem explosionPrefab;
    public Material iceMaterial;
    [Space(10)]

    [Header("UI")]
    public Animator winTextAnim;
    public Animator loseTextAnim;
    public GameObject timeIcon;
    public TextMeshProUGUI timeText;
    public GameObject movesIcon;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI initialMessage;
    public GameObject popUpPrefab;
    [Space(10)]

    [Header("Anchors")]
    public GameObject energyBarAnchor;
    public GameObject gemGridAnchor;
    [Space(10)]

    [Header("Units' refs")]
    public LogicUnit lu;
    public InputUnit iu;
    public ParamUnit pu;
    public FadeManager fadeManager;
    public Camera mainCamera;

    [HideInInspector]
    public int WorkingObjs { get; set; } // The number of objects that are in a working state, e.g. we should wait for them

    private GameObject[] energyBar;
    private GameObject[,] grid; // Instances of gems prefabs

    private int gSizeX; // Width of the gem grid
    private int gSizeY; // Height of the gem grid

    private Vector3 initialPos; // Initial position of the graphics unit to properly calculate its new position

    private Color[] colors;
    private float energyPrefabSize = 1f;

    private void Awake()
    {
        WorkingObjs = 0;

        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;

        grid = new GameObject[gSizeX, gSizeY];

        initialPos = transform.position;

        // Translation of the grid anchor (graphics unit) from the center of the screen
        // to the minus half of the length of the gems + offsets between them
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) / 2f + pu.gemSize / 2f,
            pu.gemSize,
            0);
        gemGridAnchor.transform.position = transform.position;

        // Position and look of energy bar
        energyBar = new GameObject[pu.maximumEnergy];
        ComputeEnergyPrefabSize();
        energyBarObj.transform.Translate(.5f * energyPrefabSize, -.75f * energyPrefabSize, 0f);
        energyBarAnchor.transform.position = energyBarObj.transform.position;
        for (int i = 0; i < pu.maximumEnergy; i++) 
        {
            AddEnergy(i);
        }

        int planetId = GameDataManager.instance.generalData.selectedPlanet;
        int levelId = planetId != -1 ? GameDataManager.instance.planetData[planetId].levelsFinished + 1 : -1;

        // Initial message
        if (levelId == -1)
        {
            initialMessage.text = LocalizationManager.instance.GetLocalizedValue("game_level") + "E";
        }
        else if (levelId > GameDataManager.instance.planetData[planetId].levelNum)
        {
            initialMessage.text = LocalizationManager.instance.GetLocalizedValue("game_endless_level");
        }
        else
        {
            initialMessage.text = LocalizationManager.instance.GetLocalizedValue("game_level") + levelId.ToString();
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
        timeText.text = ParamUnit.GetParsedTime((int) lu.timeLeft);
        movesText.text = lu.movesLeft.ToString();
        scoreText.text = LocalizationManager.instance.GetLocalizedValue("game_score") + lu.score.ToString() + "/" + pu.scoreToWin.ToString();
    }

    /// <summary>
    /// Spawns a gem on the grid with the default initial height of the spawn
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="x">X-pos on the grid</param>
    /// <param name="y">Y-pos on the grid</param>
    /// <param name="color">Color of the gem</param>
    /// <param name="bonus">Bonus of the gem</param>
    public void SpawnGem(int x, int y, int color, int bonus)
    {
        SpawnGem(x, y, color, bonus, (pu.gemSize + pu.gemOffset) * (gSizeY + 1));        
    }

    /// <summary>
    /// Spawns a gem on the grid with the parameterized initial height of the spawn
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="x">X-pos on the grid</param>
    /// <param name="y">Y-pos on the grid</param>
    /// <param name="color">Color of the gem</param>
    /// <param name="bonus">Bonus of the gem</param>
    /// <param name="v_offset">Y-distance from the spawn positon of the gem where it will appear initially </param>
    public void SpawnGem(int x, int y, int color, int bonus, float v_offset)
    {
        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y + v_offset; // Higher than the grid         

        grid[x, y] = Instantiate(gems[bonus == (int)ParamUnit.Bonus.NONE ? 0 : bonus], gemGridAnchor.transform);
        grid[x, y].transform.localScale = new Vector3(pu.gemSize, pu.gemSize, pu.gemSize);
        if (grid[x, y].GetComponent<Scale>() != null)
        {
            grid[x, y].GetComponent<Scale>().SetLocalScale(new Vector3(pu.gemSize, pu.gemSize, pu.gemSize));
        }
        grid[x, y].transform.position = position;
        grid[x, y].GetComponent<Renderer>().material.color = colors[color];
        grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[color];
        if (bonus == (int)ParamUnit.Bonus.SAME_COLOR)
        {
            grid[x, y].GetComponent<MeshFilter>().mesh = gridGemsMeshes[8];
        }

        if (v_offset != 0f)
            StartCoroutine(MoveGem(grid[x, y], x, y));
    }

    /// <summary>
    /// Swaps chosen gems
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="pos1">Vector position of the first gem</param>
    /// <param name="pos2">Vector position of the second gem</param>
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

    /// <summary>
    /// Destroys chosen gem
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="x">X-pos of the gem</param>
    /// <param name="y">Y-pos of the gem</param>
    /// <param name="color">Color of the gem</param>
    /// <param name="doSpawnParts">Do spawn destroyed parts or don't</param>
    public void DestroyGem(int x, int y, int color, bool doSpawnParts)
    {
        Destroy(grid[x, y]);
        grid[x, y] = null;

        Vector3 position = transform.position;
        position.x += (pu.gemSize + pu.gemOffset) * x;
        position.y += (pu.gemSize + pu.gemOffset) * y;

        if (doSpawnParts)
        {
            int num = Random.Range(2, 4);
            for (int i = 0; i < num; i++)
            {
                GameObject dGemPart = Instantiate(gemPart, gemGridAnchor.transform);
                Destroy(dGemPart, pu.dPartsLifetime);
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
    }
    public void DestroyGem(int x, int y, int color)
    {
        DestroyGem(x, y, color, true);
    }

    /// <summary>
    /// Spawns a pop-up text object showing the number of scores a player has got
    /// </summary>
    /// <param name="x">X spawn coordinate</param>
    /// <param name="y">Y spawn coordinate</param>
    /// <param name="score">A number to be shown</param>
    public void SpawnScoreMessage(int x, int y, int score)
    {
        GameObject popUpObj = Instantiate(popUpPrefab, transform);
        popUpObj.transform.localPosition = mainCamera.WorldToScreenPoint(GetGraphPos(x - 3, y + 1));

        GameObject popUpText = popUpObj.transform.Find("Pop-up score").gameObject;
        TextMeshProUGUI textComp = popUpText.GetComponent<TextMeshProUGUI>();
        textComp.text = score.ToString();
        float scoreEffect = Mathf.Max((float)score / (pu.sequenceSize * pu.scoreUnit) * .9f, 1f);
        textComp.fontSize *= scoreEffect;
        textComp.color = new Color(textComp.color.r, textComp.color.b / scoreEffect, textComp.color.g / scoreEffect);
        if (Random.Range(0f, 1f) > .5f)
        {
            popUpText.GetComponent<Animator>().Play("Pop-up left up");
        }
        else
        {
            popUpText.GetComponent<Animator>().Play("Pop-up right up");
        }
    }

    /// <summary>
    /// Sends meteor to the chosen position on the grid
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="x">X-pos of the destination</param>
    /// <param name="y">Y-pos of the destination</param>
    public void ActivateMeteorBonus(int x, int y)
    {
        int _x = Random.Range(0, gSizeX);
        int _y = Random.Range(0, gSizeY);

        GameObject meteor = Instantiate(meteorPrefab, gemGridAnchor.transform);
        meteor.transform.position = GetGraphPos(x, y);
        meteor.transform.localScale = new Vector3(.75f * pu.gemSize, .75f * pu.gemSize, .75f * pu.gemSize);
        meteor.transform.Translate(0f, pu.meteorOffset, -1f);
        StartCoroutine(MoveMeteor(
            meteor, _x, _y,
            lu.grid[_x, _y].IsEmpty() ? 0 : lu.grid[_x, _y].Gem.Color));
    }

    /// <summary>
    /// Performs scaling (selecting) of the chosen gem after checking with the logic unit and already selected gems
    /// </summary>
    /// <remarks>
    /// Changes graphics and logic
    /// </remarks>
    /// <param name="gem">A gem from the input unit that was selected by user</param>
    public void SelectGem(GameObject gem)
    {
        // Search for gem inside grid array
        for (int x = 0; x < gSizeX; x++)
        {
            for (int y = 0; y < gSizeY; y++)
            {
                if (gem.Equals(grid[x, y]))
                {
                    if (lu.IsGemValidToSelect(x, y))
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

    /// <summary>
    /// Resets selection if not enough conditions are satisfied
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
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

    /// <summary>
    /// Draws end screen with the corresponding message
    /// </summary>
    /// <param name="isVictory">Whether or not user won in this round</param>
    public void DrawEndScreen(bool isVictory)
    {
        if (isVictory)
        {
            winTextAnim.gameObject.SetActive(true);
            winTextAnim.Play("FadeOut");
        } else
        {
            loseTextAnim.gameObject.SetActive(true);
            loseTextAnim.Play("FadeOut");
        }      
    }

    /// <summary>
    /// Enables or disables (according to the previous state) chosen energy object on the energy bar
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="i">Index of the energy object in the energy bar</param>
    public void SwitchEnergy(int i)
    {
        energyBar[i].GetComponent<Rotation>().enabled = !energyBar[i].GetComponent<Rotation>().enabled;

        foreach (Transform child in energyBar[i].transform)
        {
            child.gameObject.GetComponent<Renderer>().material.color *= 
                energyBar[i].GetComponent<Rotation>().enabled ? 5f : .2f;
            child.localScale *= energyBar[i].GetComponent<Rotation>().enabled ? 1.17647f : 0.85f;
        }
    }

    /// <summary>
    /// Recreates grid with the new size
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="newGSizeX">New width of the grid</param>
    /// <param name="newGSizeY">New height of the grid</param>
    public void RecreateGrid(int newGSizeX, int newGSizeY)
    {
        grid = new GameObject[newGSizeX, newGSizeY];
    }

    /// <summary>
    /// Recreates energy bar
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="maxEnergy">New value of the maximum energy</param>
    public void RecreateEnergyBar(int maxEnergy)
    {
        foreach (GameObject g in energyBar)
        {
            Destroy(g);
        }

        energyBar = new GameObject[maxEnergy];

        for (int i = 0; i < pu.maximumEnergy; i++)
        {
            AddEnergy(i);
        }
    }

    /// <summary>
    /// Respawns gem with the ice destruction animation
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="x">X-pos of the gem</param>
    /// <param name="y">Y-pos of the gem</param>
    /// <param name="newColor">New color of the gem</param>
    /// <param name="newBonus">New bonus of the gem</param>
    public void RespawnGem(int x, int y, int newColor, int newBonus)
    {
        Destroy(grid[x, y]);

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
            dGemPart.GetComponent<Renderer>().material = iceMaterial;
            dGemPart.GetComponent<MeshFilter>().mesh = gridGemsDParts[Random.Range(0, 3)];
            dGemPart.GetComponent<BoxCollider>().size = new Vector3(.75f * pu.gemSize, .75f * pu.gemSize, .75f * pu.gemSize);

            Vector3 forceDirection = new Vector3(
                Random.Range(-pu.destructionForce, pu.destructionForce),
                Random.Range(-pu.destructionForce, pu.destructionForce),
                Random.Range(-pu.destructionForce, pu.destructionForce)
            );
            dGemPart.GetComponent<Rigidbody>().AddForce(forceDirection, ForceMode.Impulse);
        }

        SpawnGem(x, y, newColor, newBonus, 0f);
    }

    /// <summary>
    /// Updates needed variables after loading of the saved level
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    public void UpdateDataAfterLoading()
    {
        gSizeX = (int)pu.gridSize.x;
        gSizeY = (int)pu.gridSize.y;
        transform.position = initialPos;
        transform.Translate(
            -(gSizeX * pu.gemSize + (gSizeX - 1) * pu.gemOffset) / 2f + pu.gemSize / 2f,
            pu.gemSize,
            0);

        timeIcon.SetActive(pu.timeAvailable != 0);
        timeText.gameObject.SetActive(pu.timeAvailable != 0);
        movesIcon.SetActive(pu.movesAvailable != 0);
        movesText.gameObject.SetActive(pu.movesAvailable != 0);
    }

    /// <summary>
    /// Adds new prefab of the energy object to the energy bar
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// </remarks>
    /// <param name="i">Index of the energy object to be added</param>
    private void AddEnergy(int i)
    {
        energyBar[i] = Instantiate(energyPrefab, energyBarAnchor.transform);
        energyBar[i].transform.localScale = new Vector3(energyPrefabSize, .66f * energyPrefabSize, .66f * energyPrefabSize);
        energyBar[i].transform.Translate(
            .5f * energyBar[i].transform.localScale.x * i, 0f, 0f
            );
    }

    /// <summary>
    /// Computes proper size of the energy bat items
    /// </summary>
    private void ComputeEnergyPrefabSize()
    {
        // Size of the gems sides
        float height = 2f * pu.mainCamera.orthographicSize;
        float width = height * pu.mainCamera.aspect;
        energyPrefabSize = 0.5f * width / (10f - 9f * .25f);
    }

    /// <summary>
    /// Moves gem from its initial position to the specified position
    /// </summary>
    /// <remarks>
    /// Changes only graphics
    /// Some functions will wait until this coroutine is over
    /// </remarks>
    /// <param name="gem">Gem to be moved</param>
    /// <param name="x">X-pos of the destination</param>
    /// <param name="y">Y-pos of the destination</param>
    /// <returns></returns>
    private IEnumerator MoveGem(GameObject gem, int x, int y)
    {
        WorkingObjs++;
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        Vector3 newPosition = GetGraphPos(x, y);
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

    /// <summary>
    /// Moves meteor to the specified location on the grid and at the end destroys a gem with given position and color
    /// </summary>
    /// <remarks>
    /// Changes graphics and logic
    /// Will wait for some functions
    /// Some functions will wait until this coroutine is over
    /// </remarks>
    /// <param name="meteor">Instance of the meteor prefab with predefined location and scale</param>
    /// <param name="x">X-pos of the meteor's destination</param>
    /// <param name="y">Y-pos of the meteor's destination</param>
    /// <param name="color">The color of the gem to be destroyed</param>
    /// <returns></returns>
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
            int scoreToShow = lu.DestroyGem(x, y);
            SpawnScoreMessage(x, y, scoreToShow);
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

    /// <summary>
    /// Scales given gem to the given scale
    /// </summary>
    /// <param name="gem">The gem to be scaled</param>
    /// <param name="finalScale">The final scale of the gem</param>
    /// <param name="speed">Speed of the scaling</param>
    /// <returns></returns>
    private IEnumerator ScaleGem(GameObject gem, Vector3 finalScale, float speed)
    {
        Vector3 currentScale = gem.transform.localScale;
        for (float t = 0; t < 1f; t += Time.deltaTime * speed)
        {
            gem.transform.localScale = Vector3.Lerp(currentScale, finalScale, t);
            yield return new WaitForEndOfFrame();         
        }
    }

    /// <summary>
    /// Returns the position of the cell of the grid with the given location
    /// </summary>
    /// <param name="x">X-pos on the grid</param>
    /// <param name="y">Y-pos on the grid</param>
    /// <returns>
    /// Vector3 with position of the gem on the grid
    /// </returns>
    private Vector3 GetGraphPos(int x, int y)
    {
        return new Vector3(
            transform.position.x + (pu.gemSize + pu.gemOffset) * x,
            transform.position.y + (pu.gemSize + pu.gemOffset) * y, 
            transform.position.z);
    }
}
