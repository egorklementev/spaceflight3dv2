using UnityEngine;
using TMPro;

public class ControlTowerManager : MonoBehaviour {

    public GameObject planet;
    public GameObject _lock;
    public Texture[] planetTextures;
    [Space(10)]

    public TextMeshProUGUI planetTitle;
    public TextMeshProUGUI planetFuel;
    public TextMeshProUGUI planetLevels;
    public TextMeshProUGUI planetCurLevel;
    public TextMeshProUGUI levelGoal;
    public TextMeshProUGUI savedFuel;
    public TextMeshProUGUI savedFuelNumber;
    [Space(10)]

    public TextMeshProUGUI fuelBarText;
    public RectTransform fuelBarUnderline;
    public RectTransform fuelBarLine;
    [Space(10)]

    public Material planetMaterial;
    public GameObject fuelIcon;
    public GameObject fuelSavedIcon;
    public MessageText mt;
    public FadeManager fm;

    private static int selectedPlanet = 0;

	private void Awake () {
        planetMaterial = planet.GetComponent<Renderer>().material;

        UpdatePlanetUI();

        MusicManager.instance.Play("Spaceport theme");
    }
	
	private void Update () {

        // Update of the fuel bar
        int fuel = GameDataManager.instance.generalData.fuel;
        int maxFuel = GameDataManager.instance.generalData.fuelUpgrade * GameDataManager.instance.resPerStorageUpgFuel;
        fuelBarText.text = fuel + "/" + maxFuel;
        float fuelBarLength = .925f * fuelBarUnderline.sizeDelta.x * ((float)fuel / maxFuel);
        fuelBarLine.sizeDelta = new Vector2(fuelBarLength, fuelBarLine.sizeDelta.y);

    }

    public void SelectNextPlanet()
    {
        if (selectedPlanet < planetTextures.Length - 1)
        {
            selectedPlanet++;

            UpdatePlanetUI();
        }
    }

    public void SelectPrevPlanet()
    {
        if (selectedPlanet > 0)
        {
            selectedPlanet--;

            UpdatePlanetUI();
        }
    }

    public void LaunchRocket()
    {
        if (GameDataManager.instance.generalData.selectedRocket != -1)
        {
            float fuelToReach = CalculateFuelConsumption();
            if (GameDataManager.instance.generalData.fuel >= fuelToReach)
            {
                // Go to the spaceport, play animation, go to the game level
                GameDataManager.instance.ConsumeFuel(Mathf.RoundToInt(fuelToReach));
                GameDataManager.instance.generalData.selectedPlanet = selectedPlanet;
                PortUI.launchingRocket = true;
                fm.SetLevel(4);
            }
            else
            {
                mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("control_tower_insufficient_fuel"), 2f, MessageText.ScreenPosition.TOP);
            }
        }
        else
        {
            mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("control_tower_rocket_unselected"), 2f, MessageText.ScreenPosition.TOP);
        }
    }

    private void UpdatePlanetUI()
    {
        if (GameDataManager.instance.generalData.planetsReached > selectedPlanet)
        {
            PlanetData pd = GameDataManager.instance.planetData[selectedPlanet];            
            int fuelToReach = CalculateFuelConsumption();
            int fuelSaved = CalculateSavedFuel();

            planetMaterial.SetColor(Shader.PropertyToID("_ColorTint"), new Color(1, 1, 1, 1));
            planetMaterial.mainTexture = planetTextures[selectedPlanet];
            planetTitle.text = LocalizationManager.instance.GetLocalizedValue("control_tower_planet_title") +
                LocalizationManager.instance.GetLocalizedValue("planet_" + selectedPlanet.ToString() + "_title");
            planetFuel.text = LocalizationManager.instance.GetLocalizedValue("control_tower_fuel_to_reach") + fuelToReach.ToString();
            planetLevels.text = LocalizationManager.instance.GetLocalizedValue("control_tower_levels_to_play") + pd.levelNum.ToString();
            savedFuel.text = LocalizationManager.instance.GetLocalizedValue("control_tower_fuel_saved");
            savedFuelNumber.text = selectedPlanet == 0 ? "0" : "-" + fuelSaved.ToString();

            if (selectedPlanet + 1 == GameDataManager.instance.generalData.planetsReached)
            {
                int nextLevel = GameDataManager.instance.planetData[selectedPlanet].levelsFinished + 1;
                planetCurLevel.text = LocalizationManager.instance.GetLocalizedValue("control_tower_next_level") + nextLevel.ToString();
                LevelData ld = SaveUnit.LoadLevel(nextLevel, "/Levels/Planet_" + (selectedPlanet + 1).ToString() + "/");
                switch (ld.winCondition)
                {
                    case 1:
                        levelGoal.text = LocalizationManager.instance.GetLocalizedValue("control_tower_goal_reach_score");
                        break;
                    case 2:
                        levelGoal.text = LocalizationManager.instance.GetLocalizedValue("control_tower_goal_clear_board");
                        break;
                    default:
                        break;
                }
            }
            else
            {
                planetCurLevel.text = LocalizationManager.instance.GetLocalizedValue("control_tower_endless_level");
                levelGoal.text = LocalizationManager.instance.GetLocalizedValue("control_tower_goal_endless");
            }

            fuelIcon.SetActive(true);
            fuelIcon.GetComponent<RectTransform>().localPosition = new Vector3(planetFuel.preferredWidth * 1.05f, 0f, 0f);
            fuelSavedIcon.SetActive(true);
            savedFuelNumber.GetComponent<RectTransform>().localPosition = new Vector3(savedFuel.preferredWidth * 1.05f, 0f, 0f);
            fuelSavedIcon.GetComponent<RectTransform>().localPosition = new Vector3((savedFuel.preferredWidth + savedFuelNumber.preferredWidth) * 1.05f, 0f, 0f);
            _lock.SetActive(false);
        }
        else
        {
            planetMaterial.mainTexture = planetTextures[selectedPlanet];
            planetMaterial.SetColor(Shader.PropertyToID("_ColorTint"), new Color(40f / 255f, 40f / 255f, 40f / 255f, 1));
            planetTitle.text = LocalizationManager.instance.GetLocalizedValue("control_tower_unavailable");
            planetFuel.text = "";
            planetLevels.text = "";
            planetCurLevel.text = "";
            levelGoal.text = "";
            savedFuel.text = "";
            savedFuelNumber.text = "";
            fuelIcon.SetActive(false);
            fuelSavedIcon.SetActive(false);
            _lock.SetActive(true);
        }
    }

    //public void DEBUGPlanetSave()
    //{
    //    GameDataManager.instance.SaveToDataFile("/PlanetData/planet_" + localData.index.ToString() + ".json", localData);
    //}

    private int CalculateFuelConsumption()
    {
        return (int)(GameDataManager.instance.planetData[selectedPlanet].fuelToReach * (1f - GameDataManager.instance.GetEngineBonus()));
    }

    private int CalculateSavedFuel()
    {
        return (int)(GameDataManager.instance.planetData[selectedPlanet].fuelToReach * GameDataManager.instance.GetEngineBonus()) + 1;
    }

}
