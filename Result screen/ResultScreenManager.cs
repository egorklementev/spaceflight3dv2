using UnityEngine;
using TMPro;
using System.Collections;

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
    public TextMeshProUGUI metalBonusText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI fuelBonusText;
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI energyBonusText;
    [Space(10)]

    public GameObject newPlanetNotificationText;
    public GameObject newFactNotificationText;

    public static int scoreToShow = 0;
    public static int timeToShow = 0;
    public static int movesToShow = 0;

    public static int collectedMetal = 0;
    public static int collectedFuel = 0;
    public static int collectedEnergy = 0;

    public static int metalBonus = 0;
    public static int fuelBonus = 0;
    public static int energyBonus = 0;


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

                // Unlock next level
                if (levelId <= GameDataManager.instance.planetData[planetId].levelNum)
                    GameDataManager.instance.planetData[planetId].levelsFinished++;

                // New planet reached
                if (GameDataManager.instance.planetData[planetId].levelsFinished == GameDataManager.instance.planetData[planetId].levelNum
                    && planetId + 1 == GameDataManager.instance.generalData.planetsReached
                    && planetId + 1 < GameDataManager.instance.planetNumber)
                {
                    GameDataManager.instance.generalData.planetsReached++;
                    newPlanetNotificationText.SetActive(true);
                    newPlanetNotificationText.GetComponent<Animator>().Play("Change size up");
                }

                if (levelId - 4 % 10 == 0)
                {
                    GameDataManager.instance.generalData.unlockedFacts++;
                    newFactNotificationText.SetActive(true);
                    newFactNotificationText.GetComponent<Animator>().Play("Change size down");
                }
            }
        }
        else
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
            if (rocketId != 0) // First rocket is always available
                GameDataManager.instance.rocketData[rocketId].purchased = false;
            GameDataManager.instance.AddMetal(collectedMetal);
            GameDataManager.instance.AddFuel(collectedFuel);
            GameDataManager.instance.AddEnergy(collectedEnergy);

            metalText.text = (collectedMetal - metalBonus).ToString();
            fuelText.text = (collectedFuel - fuelBonus).ToString();
            energyText.text = (collectedEnergy - energyBonus).ToString();

            metalBonusText.text = "+" + metalBonus.ToString();
            fuelBonusText.text = "+" + fuelBonus.ToString();
            energyBonusText.text = "+" + energyBonus.ToString();
        }
        GameDataManager.instance.generalData.selectedRocket = -1;

        MusicManager.instance.Play("Spaceport theme");
        StartCoroutine(PlayRocketSound()); 
    }

    private IEnumerator PlayRocketSound()
    {
        float delay = 3f;
        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            timeElapsed += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        MusicManager.instance.PlaySound("Rocket flying sound");
    }

}
