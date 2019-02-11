using UnityEngine;
using TMPro;

public class UIUnit : MonoBehaviour {

    public TextMeshProUGUI fpsText;
	
	void Update () {

        fpsText.text = "FPS: " + (1f / Time.deltaTime).ToString("0");

    }
}
