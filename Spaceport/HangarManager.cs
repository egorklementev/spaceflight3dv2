using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HangarManager : MonoBehaviour {
    
    public GameObject rocketStand;
    public Transform rocketSpawnPoint;
    public GameObject[] rocketPrefabs;
    [Space(10)]

    [Header("UI")]
    public TextMeshProUGUI rocketNameText;
    public TextMeshProUGUI rocketPriceText;
    public TextMeshProUGUI rocketEnergyText;
    [Space(10)]
    public TextMeshProUGUI upgradeTitle;
    public TextMeshProUGUI upgradeLevel;    
    public TextMeshProUGUI upgradeDescription;
    public TextMeshProUGUI upgradePrice;
    public TextMeshProUGUI upgradeOldBuff;
    public TextMeshProUGUI upgradeNewBuff;
    public Image upgradeImage;
    public Sprite[] upgradeSprites;
    [Space(10)]
    public Button engineUpgradeBtn;    
    public Button sheathingUpgradeBtn;
    public Button frameUpgradeBtn;
    [Space(10)]
    public TextMeshProUGUI enginePercent;
    public TextMeshProUGUI sheathingPercent;
    public TextMeshProUGUI framePercent;
    [Space(10)]
    public Transform engineTitle;
    public Transform sheathingTitle;
    public Transform frameTitle;
    [Space(10)]
    public GameObject engineIcon;
    public GameObject sheathingIcon;
    public GameObject frameIcon;
    public Button buyButton;
    public Button selectButton;
    public TextMeshProUGUI selBtnText;
    public Button agreeBtn;
    public MessageText mt;
    [Space(10)]

    [Header("Metal bar")]
    public TextMeshProUGUI metalText;
    public RectTransform metalBar;
    public RectTransform underlay;
    private float underlayLength;
    [Space(10)]
    
    public int selectedRocket = 0;
    public float moveTime = 1f;
    
    private GameObject showedRocket;
    private GameObject hiddenRocket;
    private const int rocketNumber = 6;
    private bool isWorking = false;
    private int selectedUpgrade = 1;

    private void Awake()
    {
        underlayLength = underlay.sizeDelta.x;
        ShowRocket(selectedRocket);

        buyButton.interactable = !GameDataManager.instance.rocketData[selectedRocket].purchased;

        Invoke("UpdateBuffs", 0.1f);

        MusicManager.instance.Play("Spaceport theme");
    }       

    private void Update()
    {
        int metal = GameDataManager.instance.generalData.metal;
        int maxMetal = GameDataManager.instance.generalData.metalUpgrade * GameDataManager.instance.resPerStorageUpgMetal;
        metalText.text = metal + "/" + maxMetal;
        float metalBarLength = .925f * underlayLength * ((float)metal / maxMetal);
        metalBar.sizeDelta = new Vector2(metalBarLength, metalBar.sizeDelta.y);

        engineUpgradeBtn.interactable = GameDataManager.instance.rocketData[selectedRocket].engineLevel < GameDataManager.instance.maxRocketUpgradeLevel;
        sheathingUpgradeBtn.interactable = GameDataManager.instance.rocketData[selectedRocket].sheathingLevel < GameDataManager.instance.maxRocketUpgradeLevel;
        frameUpgradeBtn.interactable = GameDataManager.instance.rocketData[selectedRocket].frameLevel < GameDataManager.instance.maxRocketUpgradeLevel;
    }

    public void ShowNextRocket()
    {
        if (selectedRocket + 1 < rocketNumber && !isWorking) {
            HideRocket();
            selectedRocket++;
            ShowRocket(selectedRocket);
        }
    }
    public void ShowPrevRocket()
    {
        if (selectedRocket - 1 >= 0 && !isWorking)
        {
            HideRocket();
            selectedRocket--;
            ShowRocket(selectedRocket);
        }
    }

    public void BuyRocket()
    {
        RocketData rd = GameDataManager.instance.rocketData[selectedRocket];

        if (GameDataManager.instance.ConsumeMetal(rd.price))
        {
            mt.DisplayMessage(
                LocalizationManager.instance.GetLocalizedValue("hangar_rocket_purchase_1") +
                LocalizationManager.instance.GetLocalizedValue("rocket_" + (selectedRocket + 1).ToString() + "_title") +
                LocalizationManager.instance.GetLocalizedValue("hangar_rocket_purchase_2"),
                3f,
                MessageText.ScreenPosition.TOP
                );

            rd.purchased = true;

            buyButton.interactable = false;
            selectButton.interactable = true;
        }
        else
        {
            mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_not_enough_metal"), MessageText.ScreenPosition.TOP);
        }
    }

    public void SelectRocket()
    {
        GameDataManager.instance.generalData.selectedRocket = selectedRocket;
        selectButton.interactable = false;

        mt.DisplayMessage(
                LocalizationManager.instance.GetLocalizedValue("hangar_rocket_purchase_1") +
                LocalizationManager.instance.GetLocalizedValue("rocket_" + (selectedRocket + 1).ToString() + "_title") +
                LocalizationManager.instance.GetLocalizedValue("hangar_rocket_was_selected"),
                3f,
                MessageText.ScreenPosition.TOP
                );

    }

    public void UpgradeRocket()
    {
        RocketData rd = GameDataManager.instance.rocketData[selectedRocket];

        int cost = 0;

        switch (selectedUpgrade)
        {
            case 1: // Engine
                cost = (rd.engineLevel + selectedRocket + 1) * GameDataManager.instance.resPerEngineUpg;
                if (GameDataManager.instance.ConsumeMetal(cost))
                {
                    rd.engineLevel++;
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_engine_upgraded_successfully"), MessageText.ScreenPosition.TOP);
                    UpdateUpgradeInfo(selectedUpgrade);
                }
                else
                {
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_not_enough_metal"), MessageText.ScreenPosition.TOP);
                }
                break;
            case 2: // Sheathing
                cost = (rd.sheathingLevel + selectedRocket + 1) * GameDataManager.instance.resPerSheathingUpg;
                if (GameDataManager.instance.ConsumeMetal(cost))
                {
                    rd.sheathingLevel++;
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_sheathing_upgraded_successfully"), MessageText.ScreenPosition.TOP);
                    UpdateUpgradeInfo(selectedUpgrade);
                }
                else
                {
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_not_enough_metal"), MessageText.ScreenPosition.TOP);
                }
                break;
            case 3: // Frame
                cost = (rd.frameLevel + selectedRocket + 1) * GameDataManager.instance.resPerFrameUpg;
                if (GameDataManager.instance.ConsumeMetal(cost))
                {
                    rd.frameLevel++;
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_frame_upgraded_successfully"), MessageText.ScreenPosition.TOP);
                    UpdateUpgradeInfo(selectedUpgrade);
                }
                else
                {
                    mt.DisplayMessage(LocalizationManager.instance.GetLocalizedValue("hangar_not_enough_metal"), MessageText.ScreenPosition.TOP);
                }
                break;
        }

        UpdateBuffs();

    }

    public void UpdateUpgradeInfo(int upgID)
    {
        selectedUpgrade = upgID;

        int level = 0;
        float oldBonus = 0.0f;
        float newBonus = 0.0f;
        int resPerUpg = 0;

        RocketData rd = GameDataManager.instance.rocketData[selectedRocket];

        switch (upgID)
        {
            case 1: // Engine
                level = rd.engineLevel;
                oldBonus = GameDataManager.instance.GetEngineBonus(selectedRocket) * 100f;
                newBonus = GameDataManager.instance.GetEngineBonus(selectedRocket, rd.engineLevel + 1) * 100f;
                resPerUpg = GameDataManager.instance.resPerEngineUpg;

                upgradeTitle.text = LocalizationManager.instance.GetLocalizedValue("hangar_engine_upg_question");
                upgradeLevel.text = LocalizationManager.instance.GetLocalizedValue("hangar_current_upgrade") + 
                    (rd.engineLevel < 5 ? rd.engineLevel.ToString() : LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade"));
                upgradeDescription.text = LocalizationManager.instance.GetLocalizedValue("hangar_engine_upg_description");
                
                if (rd.engineLevel >= GameDataManager.instance.maxRocketUpgradeLevel)
                {
                    agreeBtn.interactable = false;
                    upgradePrice.text = LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade");
                }

                break;
            case 2: // Sheathing
                level = rd.sheathingLevel;
                oldBonus = GameDataManager.instance.GetSheathingBonus(selectedRocket) * 100f;
                newBonus = GameDataManager.instance.GetSheathingBonus(selectedRocket, rd.sheathingLevel + 1) * 100f;
                resPerUpg = GameDataManager.instance.resPerSheathingUpg;

                upgradeTitle.text = LocalizationManager.instance.GetLocalizedValue("hangar_sheathing_upg_question");
                upgradeLevel.text = LocalizationManager.instance.GetLocalizedValue("hangar_current_upgrade") +
                    (rd.sheathingLevel < 5 ? rd.sheathingLevel.ToString() : LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade"));
                upgradeDescription.text = LocalizationManager.instance.GetLocalizedValue("hangar_sheathing_upg_description");

                if (rd.sheathingLevel >= GameDataManager.instance.maxRocketUpgradeLevel)
                {
                    agreeBtn.interactable = false;
                    upgradePrice.text = LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade");
                }

                break;
            case 3: // Frame
                level = rd.frameLevel;
                oldBonus = GameDataManager.instance.GetFrameBonus(selectedRocket) * 100f;
                newBonus = GameDataManager.instance.GetFrameBonus(selectedRocket, rd.frameLevel + 1) * 100f;
                resPerUpg = GameDataManager.instance.resPerFrameUpg;

                upgradeTitle.text = LocalizationManager.instance.GetLocalizedValue("hangar_frame_upg_question");
                upgradeLevel.text = LocalizationManager.instance.GetLocalizedValue("hangar_current_upgrade") +
                    (rd.frameLevel < 5 ? rd.frameLevel.ToString() : LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade"));
                upgradeDescription.text = LocalizationManager.instance.GetLocalizedValue("hangar_frame_upg_description");

                if (rd.frameLevel >= GameDataManager.instance.maxRocketUpgradeLevel)
                {
                    agreeBtn.interactable = false;
                    upgradePrice.text = LocalizationManager.instance.GetLocalizedValue("hangar_maximum_upgrade");
                }

                break;
        }

        upgradeImage.sprite = upgradeSprites[upgID - 1];

        upgradePrice.text = ((level + selectedRocket + 1) * resPerUpg).ToString();
        upgradeOldBuff.text = (upgID == 1 ? "- " : "+ ") + oldBonus.ToString() + "%";
        upgradeNewBuff.text = (upgID == 1 ? "- " : "+ ") + newBonus.ToString() + "%";
    }

    private void UpdateBuffs()
    {
        RocketData rd = GameDataManager.instance.rocketData[selectedRocket];

        float engineBonus = GameDataManager.instance.GetEngineBonus(selectedRocket) * 100f;
        float sheathingBonus = GameDataManager.instance.GetSheathingBonus(selectedRocket) * 100f;
        float frameBonus = GameDataManager.instance.GetFrameBonus(selectedRocket) * 100f;

        enginePercent.text = "- " + engineBonus.ToString() + "%";
        sheathingPercent.text = "+ " + sheathingBonus.ToString() + "%";
        framePercent.text = "+ " + frameBonus.ToString() + "%";

        foreach (Transform child in engineTitle)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in sheathingTitle)
        {
            Destroy(child.gameObject);
        }

        foreach (Transform child in frameTitle)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < rd.engineLevel; i++)
        {
            GameObject icon = Instantiate(engineIcon, engineTitle);
            icon.GetComponent<RectTransform>().Translate(new Vector3(icon.GetComponent<RectTransform>().localScale.x * 1.25f * i, 0f, 0f));
        }
        for (int i = 0; i < rd.sheathingLevel; i++)
        {
            GameObject icon = Instantiate(sheathingIcon, sheathingTitle);
            icon.GetComponent<RectTransform>().Translate(new Vector3(icon.GetComponent<RectTransform>().localScale.x * 1.25f * i, 0f, 0f));
        }
        for (int i = 0; i < rd.frameLevel; i++)
        {
            GameObject icon = Instantiate(frameIcon, frameTitle);
            icon.GetComponent<RectTransform>().Translate(new Vector3(icon.GetComponent<RectTransform>().localScale.x * 1.25f * i, 0f, 0f));
        }
    }

    private void ShowRocket(int id)
    {
        RocketData rd = GameDataManager.instance.rocketData[id];

        rocketNameText.text = LocalizationManager.instance.GetLocalizedValue("hangar_rocket_name") +
            LocalizationManager.instance.GetLocalizedValue("rocket_" + (id + 1).ToString() + "_title");

        rocketPriceText.text = rd.price.ToString();
        rocketEnergyText.text = rd.maxEnergy.ToString();

        bool isPossibleToSelect = GameDataManager.instance.generalData.selectedRocket != selectedRocket && rd.purchased;
        selectButton.interactable = isPossibleToSelect;
        string selectButtonText = "";
        if (rd.purchased && !isPossibleToSelect)
        {
            selectButtonText = "hangar_selected_rocket";
        }
        else
        {
            selectButtonText = "hangar_select_rocket";

        }
        selBtnText.text = LocalizationManager.instance.GetLocalizedValue(selectButtonText);

        UpdateBuffs();
        
        StartCoroutine(ShowRocketCoroutine(rocketStand.transform.position + new Vector3(0f, .75f, 0f), id));

        buyButton.interactable = !rd.purchased;        
    }
    
    private void HideRocket()
    {        
        SwapRoles();
        StartCoroutine(HideRocketCoroutine());        
    }

    private void SwapRoles()
    {
        GameObject temp = hiddenRocket;
        hiddenRocket = showedRocket;
        showedRocket = temp;
    }

    private IEnumerator HideRocketCoroutine()
    {
        isWorking = true;
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        while (velocity.sqrMagnitude > .001f)
        {
            hiddenRocket.transform.position = Vector3.SmoothDamp(hiddenRocket.transform.position, rocketSpawnPoint.position, ref velocity, moveTime);
            yield return new WaitForEndOfFrame();
        }
        Destroy(hiddenRocket);       
        isWorking = false;
    }

    private IEnumerator ShowRocketCoroutine(Vector3 destination, int id)
    {
        isWorking = true;        
        showedRocket = Instantiate(rocketPrefabs[id]);
        Vector3 velocity = Vector3.zero; velocity.x += .1f;
        while (velocity.sqrMagnitude > .001f)
        {
            showedRocket.transform.position = Vector3.SmoothDamp(showedRocket.transform.position, destination, ref velocity, moveTime);
            yield return new WaitForEndOfFrame();
        }
        isWorking = false; 
    }    

    

}
