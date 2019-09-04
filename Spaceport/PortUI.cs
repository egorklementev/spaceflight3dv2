using UnityEngine;
using TMPro;

public class PortUI : MonoBehaviour {

    public TextMeshProUGUI energyText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI fuelText;

    public RectTransform energyBar;
    public RectTransform metalBar;
    public RectTransform fuelBar;

    public RectTransform underlay;

    private float underlayLength;

    private void Awake()
    {
        underlayLength = underlay.sizeDelta.x;
    }

    private void Update () {
        
        // Resource variables
        int energy = GameDataManager.instance.generalData.energy;
        int maxEnergy = GameDataManager.instance.generalData.energyUpgrade * GameDataManager.instance.resPerStorageUpg;

        int metal = GameDataManager.instance.generalData.metal;        
        int maxMetal = GameDataManager.instance.generalData.metalUpgrade * GameDataManager.instance.resPerStorageUpg;

        int fuel = GameDataManager.instance.generalData.fuel;
        int maxFuel = GameDataManager.instance.generalData.fuelUpgrade * GameDataManager.instance.resPerStorageUpg;

        // Text update
        energyText.text = energy + "/" + maxEnergy;
        metalText.text = metal + "/" + maxMetal;
        fuelText.text = fuel + "/" + maxFuel;

        // Bar length update
        float energyBarLength = .925f * underlayLength * ((float)energy / maxEnergy);
        energyBar.sizeDelta = new Vector2(energyBarLength, energyBar.sizeDelta.y);

        float metalBarLength = .925f * underlayLength * ((float)metal / maxMetal);
        metalBar.sizeDelta = new Vector2(metalBarLength, metalBar.sizeDelta.y);

        float fuelBarLength = .925f * underlayLength * ((float)fuel / maxFuel);
        fuelBar.sizeDelta = new Vector2(fuelBarLength, fuelBar.sizeDelta.y);

    }
}
