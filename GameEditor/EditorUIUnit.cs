using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditorUIUnit : MonoBehaviour {

    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI slotText;
    [Space(10)]

    public TextMeshProUGUI xSizeText;
    public TextMeshProUGUI ySizeText;
    public TextMeshProUGUI availableColorsText;
    public TextMeshProUGUI sequenceSizeText;
    public TextMeshProUGUI bonusChanceText;
    public TextMeshProUGUI energyChanceText;
    [Space(10)]

    public TextMeshProUGUI randomizeColors;
    public TextMeshProUGUI spawnNewGems;
    [Space(10)]

    // Loading options
    public TextMeshProUGUI levelToLoadText;
    public TextMeshProUGUI levelToSaveText;
    [Space(10)]

    public Button incrBonus;
    public Button decrBonus;
    public Button incrEnergy;
    public Button decrEnergy;
    [Space(10)]

    public GameObject preOptionsGroup;
    public FadeSwitcher loadingGroup;
    public FadeSwitcher saveGroup;
    [Space(10)]

    public EditorParams pu;

    private void Awake()
    {
        if (pu.randomizeColors)
        {
            randomizeColors.text = "Yes";
        }
        else
        {
            randomizeColors.text = "No";
        }

        if (pu.spawnNewGems)
        {
            spawnNewGems.text = "Yes";
            incrBonus.interactable = true;
            decrBonus.interactable = true;
            incrEnergy.interactable = true;
            decrEnergy.interactable = true;
        }
        else
        {
            spawnNewGems.text = "No";
            incrBonus.interactable = false;
            decrBonus.interactable = false;
            incrEnergy.interactable = false;
            decrEnergy.interactable = false;
        }

        //saveGroup.SwitchFade();
        //loadingGroup.SwitchFade();

        levelToLoadText.text = EditorParams.currentSlot.ToString();
        levelToSaveText.text = EditorParams.currentSlot.ToString();
    }

    void Update () {

        fpsText.text = "FPS: " + (1f / Time.deltaTime).ToString("0");
        slotText.text = "Slot " + EditorParams.currentSlot.ToString() + "/" + EditorParams.slotNumber.ToString();

        xSizeText.text = "Grid width: " + pu.gridSize.x.ToString("0");
        ySizeText.text = "Grid heigth: " + pu.gridSize.y.ToString("0");
        availableColorsText.text = "Colors number: " + pu.colorsAvailable.ToString();
        sequenceSizeText.text = "Sequence size: " + pu.sequenceSize.ToString();
        bonusChanceText.text = "Bonus chance: " + pu.bonusesPercentage.ToString();
        energyChanceText.text = "Energy chance: " + pu.energyPercentage.ToString();

        levelToLoadText.text = EditorParams.currentSlot.ToString();
        levelToSaveText.text = EditorParams.currentSlot.ToString();
    }

    public void SwitchColorRandomization()
    {
        if (pu.randomizeColors)
        {
            randomizeColors.text = "No";
        } else
        {
            randomizeColors.text = "Yes";
        }
        pu.randomizeColors = !pu.randomizeColors;
    }

    public void SwitchNewGemsSpawn()
    {
        if (pu.spawnNewGems)
        {
            spawnNewGems.text = "No";
            incrBonus.interactable = false;
            decrBonus.interactable = false;
            incrEnergy.interactable = false;
            decrEnergy.interactable = false;
        }
        else
        {
            spawnNewGems.text = "Yes";
            incrBonus.interactable = true;
            decrBonus.interactable = true;
            incrEnergy.interactable = true;
            decrEnergy.interactable = true;
        }
        pu.spawnNewGems = !pu.spawnNewGems;        
    }
    
    public void HidePreOptions()
    {
        preOptionsGroup.SetActive(false);
    }

    public void HideLoadingOptions()
    {
        loadingGroup.gameObject.SetActive(false);
    }
    public void ShowLoadingOptions()
    {
        loadingGroup.gameObject.SetActive(true);
    }

    public void HideSaveOptions()
    {
        saveGroup.gameObject.SetActive(false);
    }
    public void ShowSaveOptions()
    {
        saveGroup.gameObject.SetActive(true);
    }
}
