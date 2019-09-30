using UnityEngine;
using TMPro;

public class ControlTowerManager : MonoBehaviour {

    public GameObject planet;
    public Texture[] planetTextures;
    public TextMeshProUGUI planetTitle;

    private int selectedPlanet = 0;

	private void Start () {
        UpdatePlanetTexture();
    }
	
	private void Update () {	

	}

    public void SelectNextPlanet()
    {
        if (selectedPlanet < planetTextures.Length - 1)
        {
            selectedPlanet++;

            UpdatePlanetTexture();
        }
    }

    public void SelectPrevPlanet()
    {
        if (selectedPlanet > 0)
        {
            selectedPlanet--;

            UpdatePlanetTexture();
        }
    }

    private void UpdatePlanetTexture()
    {
        planet.GetComponent<Renderer>().material.mainTexture = planetTextures[selectedPlanet];
        planetTitle.text = LocalizationManager.instance.GetLocalizedValue("planet_" + selectedPlanet.ToString() + "_title");
    }

}
