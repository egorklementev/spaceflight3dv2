using UnityEngine;
using TMPro;

public class EditorUIUnit : MonoBehaviour {

    public TextMeshProUGUI fpsText;
    public TextMeshProUGUI xSizeText;
    public TextMeshProUGUI ySizeText;
    public TextMeshProUGUI availableColorsText;
    public TextMeshProUGUI sequenceSizeText;

    public EditorParams pu;

	void Update () {

        fpsText.text = "FPS: " + (1f / Time.deltaTime).ToString("0");
        xSizeText.text = "Grid width: " + pu.gridSize.x.ToString("0");
        ySizeText.text = "Grid heigth: " + pu.gridSize.y.ToString("0");
        availableColorsText.text = "Colors number: " + pu.colorsAvailable.ToString();
        sequenceSizeText.text = "Sequence size: " + pu.sequenceSize.ToString();

    }
}
