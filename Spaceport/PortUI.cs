using UnityEngine;
using TMPro;
using System.Collections;

public class PortUI : MonoBehaviour {

    public TextMeshProUGUI energyText;
    public TextMeshProUGUI metalText;
    public TextMeshProUGUI fuelText;
    [Space(5)]

    public RectTransform energyBar;
    public RectTransform metalBar;
    public RectTransform fuelBar;
    public RectTransform underlay;
    [Space(5)]

    public float rocketFlightHeight = 30f;
    public float rocketFlightTime = 4f;
    public float rocketMinSpeed = 1f;
    [Space(5)]

    public Transform infrastructure;
    public GameObject[] rockets;    
    [Space(5)]

    public ParticleSystem landSmoke;
    public ParticleSystem trailSmoke;
    [Space(5)]

    public GameObject titleObj;
    public GameObject menuBtnObj;
    public GameObject energyBarObj;
    public GameObject metalBarObj;
    public GameObject fuelBarObj;
    [Space(5)]

    public float shakeTime = 5f;
    public float shakeInterval = .1f;
    public float shakeAmplitude = 1f;
    [Space(5)]

    public Camera cam;
    public FadeManager fm;

    public static bool launchingRocket = false;

    public static Vector3 camPos = new Vector3(-1, -1, -1);

    private float underlayLength;
    private GameObject standingRocket;
   
    private void Awake()
    {
        underlayLength = underlay.sizeDelta.x;
        int selectedRocket = GameDataManager.instance.generalData.selectedRocket;
        if (selectedRocket != -1)
        {
            standingRocket = Instantiate(rockets[selectedRocket], infrastructure);
        }

        if (!launchingRocket)
        {
            if (camPos != new Vector3(-1, -1, -1))
            {
                cam.transform.position = camPos;
            }
            else
            {
                camPos = cam.transform.position;
            }

            MusicManager.instance.Play("Spaceport theme");
        }
        else
        {
            titleObj.SetActive(false);
            menuBtnObj.SetActive(false);
            energyBarObj.SetActive(false);
            metalBarObj.SetActive(false);
            fuelBarObj.SetActive(false);
            cam.transform.localPosition = new Vector3(20, 38, 0);
            StartCoroutine(ShakeCamera(shakeTime));
            StartCoroutine(LaunchRocket());
            landSmoke.Play();
            trailSmoke.Play();

            MusicManager.instance.Stop("Spaceport theme");
            MusicManager.instance.PlaySound("Rocket launch sound");
        }
    }
    private void Update () {
        
        // Resource variables
        float energy = GameDataManager.instance.generalData.energy;
        int maxEnergy = GameDataManager.instance.generalData.energyUpgrade * GameDataManager.instance.resPerStorageUpgEnergy;

        float metal = GameDataManager.instance.generalData.metal;        
        int maxMetal = GameDataManager.instance.generalData.metalUpgrade * GameDataManager.instance.resPerStorageUpgMetal;

        float fuel = GameDataManager.instance.generalData.fuel;
        int maxFuel = GameDataManager.instance.generalData.fuelUpgrade * GameDataManager.instance.resPerStorageUpgFuel;

        // Text update
        energyText.text = energy + "/" + maxEnergy;
        metalText.text = metal + "/" + maxMetal;
        fuelText.text = fuel + "/" + maxFuel;

        // Bar length update
        float energyBarLength = .925f * underlayLength * Mathf.Min(1f, energy / maxEnergy);
        energyBar.sizeDelta = new Vector2(energyBarLength, energyBar.sizeDelta.y);

        float metalBarLength = .925f * underlayLength * Mathf.Min(1f, metal / maxMetal);
        metalBar.sizeDelta = new Vector2(metalBarLength, metalBar.sizeDelta.y);

        float fuelBarLength = .925f * underlayLength * Mathf.Min(1f, fuel / maxFuel);
        fuelBar.sizeDelta = new Vector2(fuelBarLength, fuelBar.sizeDelta.y);

    }

    private IEnumerator ShakeCamera(float time)
    {
        Vector3 initPos = cam.transform.position;
        float time_stamp = Time.realtimeSinceStartup;
        float timer = 0f;

        while (Time.realtimeSinceStartup - time_stamp < time)
        {
            if (timer > shakeInterval)
            {
                timer = 0f;
                float randX = Random.Range(0f, shakeAmplitude);
                float randZ = Random.Range(0f, shakeAmplitude);
                cam.transform.position = initPos + new Vector3(randX, 0f, randZ);
            }
            timer += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }
    }

    private IEnumerator LaunchRocket()
    {
        Vector3 offset = new Vector3(0f, rocketFlightHeight, 0f);
        Vector3 dest = standingRocket.transform.position + new Vector3(0f, rocketFlightHeight * 2f, 0f);
        Vector3 vel = new Vector3(0.1f, 0f, 0f);

        do
        {
            trailSmoke.transform.position = standingRocket.transform.position;
            standingRocket.transform.position = Vector3.SmoothDamp(standingRocket.transform.position, dest, ref vel, rocketFlightTime * Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        while (Vector3.SqrMagnitude(dest - standingRocket.transform.position - offset) > 1f);

        launchingRocket = false;
        fm.SetLevel(2); // To the game
    }

}
