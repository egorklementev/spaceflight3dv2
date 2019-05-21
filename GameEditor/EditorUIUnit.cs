using UnityEngine;
using TMPro;

public class EditorUIUnit : MonoBehaviour {

    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI xSizeText;
    public TextMeshProUGUI ySizeText;
    public TextMeshProUGUI availableColorsText;
    public TextMeshProUGUI sequenceSizeText;
    public TextMeshProUGUI randomizeColors;
    public TextMeshProUGUI spawnNewGems;

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
        }
        else
        {
            spawnNewGems.text = "No";
        }
    }

    void Update () {

        fpsText.text = "FPS: " + (1f / Time.deltaTime).ToString("0");
        xSizeText.text = "Grid width: " + pu.gridSize.x.ToString("0");
        ySizeText.text = "Grid heigth: " + pu.gridSize.y.ToString("0");
        availableColorsText.text = "Colors number: " + pu.colorsAvailable.ToString();
        sequenceSizeText.text = "Sequence size: " + pu.sequenceSize.ToString();

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
        }
        else
        {
            spawnNewGems.text = "Yes";
        }
        pu.spawnNewGems = !pu.spawnNewGems;
    }

}
