using UnityEngine;
using TMPro;

public class ResultScreenManager : MonoBehaviour {

    [Header("Environment")]
    public Material planetMaterial;
    public Texture[] planetTextures;
    public GameObject[] rocketPrefabs;
    [Space(10)]

    [Header("Text lines")]
    public TextMeshProUGUI levelTitle;
    public TextMeshProUGUI planetTitle;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI movesText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI energyText;

    public static int scoreToShow = 0;
    public static int timeToShow = 0;
    public static int movesToShow = 0;

    public static int collectedMetal = 0;
    public static int collectedFuel = 0;
    public static int collectedEnergy = 0;

    public static bool isVictory = true;

	void Start () {

        int planetId = GameDataManager.instance.generalData.selectedPlanet;
        int rocketId = GameDataManager.instance.generalData.selectedRocket;
        int levelId = planetId != -1 ? GameDataManager.instance.planetData[planetId].levelsFinished + 1 : -1;

        planetMaterial.mainTexture = planetTextures[planetId == -1 ? 0 : planetId];

        Instantiate(rocketPrefabs[rocketId == -1 ? 0 : rocketId]);

        string curLevel = levelId != -1 ?
            levelId.ToString() : 
            "E";

        if (levelId > GameDataManager.instance.planetData[planetId].levelNum)
        {
            curLevel = "";
        }

        if (isVictory)
        {
            levelTitle.text = LocalizationManager.instance.GetLocalizedValue("results_level") + curLevel +
                LocalizationManager.instance.GetLocalizedValue("results_level_complete");
            if (rocketId != -1) {

                if (levelId <= GameDataManager.instance.planetData[planetId].levelNum)
                    GameDataManager.instance.planetData[planetId].levelsFinished++;

                if (GameDataManager.instance.planetData[planetId].levelsFinished == GameDataManager.instance.planetData[planetId].levelNum
                    && planetId + 1 == GameDataManager.instance.generalData.planetsReached
                    && planetId + 1 < GameDataManager.instance.planetNumber)
                {
                    GameDataManager.instance.generalData.planetsReached++;
                    // TODO: Show notification of new planet reaching
                }
            }
        } else
        {
            levelTitle.text = LocalizationManager.instance.GetLocalizedValue("results_level") + curLevel +
                LocalizationManager.instance.GetLocalizedValue("results_level_failed");
        }
        
        planetTitle.text = LocalizationManager.instance.GetLocalizedValue("results_planet_name") +
                (planetId == -1 ? "-" : LocalizationManager.instance.GetLocalizedValue("planet_" + planetId.ToString() + "_title"));
        scoreText.text = LocalizationManager.instance.GetLocalizedValue("results_score") + scoreToShow.ToString();
        timeText.text = LocalizationManager.instance.GetLocalizedValue("results_time") + timeToShow.ToString();
        movesText.text = LocalizationManager.instance.GetLocalizedValue("results_moves") + movesToShow.ToString();

        metalText.text = "-";
        fuelText.text = "-";
        energyText.text = "-";

        if (rocketId != -1)
        {
            GameDataManager.instance.rocketData[rocketId].purchased = false;
            GameDataManager.instance.AddMetal(collectedMetal);
            GameDataManager.instance.AddFuel(collectedFuel);
            GameDataManager.instance.AddEnergy(collectedEnergy);

            metalText.text = collectedMetal.ToString();
            fuelText.text = collectedFuel.ToString();
            energyText.text = collectedEnergy.ToString();
        }
        GameDataManager.instance.generalData.selectedRocket = -1;
    }

}
