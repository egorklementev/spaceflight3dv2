using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SRIManager : MonoBehaviour {

    /// <summary>
    /// The object all facts are belong to
    /// </summary>
    public Transform factsParent;
    public GameObject dataFilePrefab;
    public FactAnimationController faController;

    public RectTransform underlay;
    public RectTransform energyBar;
    public RectTransform metalBar;
    public RectTransform fuelBar;

    public TextMeshProUGUI energyText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI fuelText;

    public TextMeshProUGUI energyCostText;
    public TextMeshProUGUI metalCostText;
    public TextMeshProUGUI fuelCostText;

    public MessageText mt;

    private Color[] dataFileColors;

    void Awake() {

        dataFileColors = new Color[] {
            new Color(1f, 1f, 1f),
            new Color(1f, 0f, 1f),
            new Color(0f, 1f, 1f),
            new Color(1f, 1f, 0f),
            new Color(1f, 0f, 0f),
            new Color(0f, 0f, 1f),
            new Color(0f, 1f, 0f),
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
            new Color(1f, 1f, 1f),
        };

        for (int i = 0; i < GameDataManager.instance.generalData.unlockedFacts; i++)
        {
            GameObject instance = Instantiate(dataFilePrefab, factsParent);
            instance.GetComponent<Image>().color = dataFileColors[i];
            instance.GetComponent<DataFile>().factId = i;
            instance.GetComponent<DataFile>().faController = faController;
        }

	}

    void Update()
    {
        float underlayLength = underlay.sizeDelta.x;

        float energy = GameDataManager.instance.generalData.energy;
        float metal = GameDataManager.instance.generalData.metal;
        float fuel = GameDataManager.instance.generalData.fuel;

        int maxEnergy = GameDataManager.instance.generalData.energyUpgrade * GameDataManager.instance.resPerStorageUpgEnergy;
        int maxMetal = GameDataManager.instance.generalData.metalUpgrade * GameDataManager.instance.resPerStorageUpgMetal;
        int maxFuel = GameDataManager.instance.generalData.fuelUpgrade * GameDataManager.instance.resPerStorageUpgFuel;

        energyText.text = energy + "/" + maxEnergy;
        metalText.text = metal + "/" + maxMetal;
        fuelText.text = fuel + "/" + maxFuel;

        float energyBarLength = .925f * underlayLength * Mathf.Min(1f, energy / maxEnergy);
        energyBar.sizeDelta = new Vector2(energyBarLength, energyBar.sizeDelta.y);

        float metalBarLength = .925f * underlayLength * Mathf.Min(1f, metal / maxMetal);
        metalBar.sizeDelta = new Vector2(metalBarLength, metalBar.sizeDelta.y);

        float fuelBarLength = .925f * underlayLength * Mathf.Min(1f, fuel / maxFuel);
        fuelBar.sizeDelta = new Vector2(fuelBarLength, fuelBar.sizeDelta.y);

        energyCostText.text = GameDataManager.instance.GetEnergyStorageUpgradeCost().ToString();
        metalCostText.text = GameDataManager.instance.GetMetalStorageUpgradeCost().ToString();
        fuelCostText.text = GameDataManager.instance.GetFuelStorageUpgradeCost().ToString();
    }

    public void UpgradeEnergy()
    {
        if (GameDataManager.instance.ConsumeMetal(GameDataManager.instance.GetEnergyStorageUpgradeCost()))
        {
            GameDataManager.instance.generalData.energyUpgrade++;
        }
        else
        {
            mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("sri_not_enough_metal"), MessageText.ScreenPosition.TOP);
        }
    }
    public void UpgradeMetal()
    {
        if (GameDataManager.instance.ConsumeFuel(GameDataManager.instance.GetMetalStorageUpgradeCost()))
        {
            GameDataManager.instance.generalData.metalUpgrade++;
        }
        else
        {
            mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("sri_not_enough_fuel"), MessageText.ScreenPosition.TOP);
        }
    }
    public void UpgradeFuel()
    {
        if (GameDataManager.instance.ConsumeEnergy(GameDataManager.instance.GetFuelStorageUpgradeCost()))
        {
            GameDataManager.instance.generalData.fuelUpgrade++;
        }
        else
        {
            mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("sri_not_enough_energy"), MessageText.ScreenPosition.TOP);
        }
    }

}
