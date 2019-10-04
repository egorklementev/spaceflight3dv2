using UnityEngine;
using TMPro;

public class ControlTowerManager : MonoBehaviour {

    public GameObject planet;
    public GameObject _lock;
    public Texture[] planetTextures;
    public TextMeshProUGUI planetTitle;

    public Material planetMaterial;

    private int selectedPlanet = 0;

	private void Start () {
        planetMaterial = planet.GetComponent<Renderer>().material;

        UpdatePlanetUI();
    }
	
	private void Update () {	

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

    private void UpdatePlanetUI()
    {
        if (GameDataManager.instance.generalData.planetsReached >= selectedPlanet)
        {
            planetMaterial.SetColor(Shader.PropertyToID("_ColorTint"), new Color(1, 1, 1, 1));
            planetMaterial.mainTexture = planetTextures[selectedPlanet];
            planetTitle.text = LocalizationManager.instance.GetLocalizedValue("planet_" + selectedPlanet.ToString() + "_title");
            _lock.SetActive(false);
        }
        else
        {
            planetMaterial.mainTexture = planetTextures[selectedPlanet];
            planetMaterial.SetColor(Shader.PropertyToID("_ColorTint"), new Color(40f / 255f, 40f / 255f, 40f / 255f, 1));
            planetTitle.text = LocalizationManager.instance.GetLocalizedValue("control_tower_unavailable");
            _lock.SetActive(true);
        }
    }

}
