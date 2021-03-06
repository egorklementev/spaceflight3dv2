﻿using UnityEngine;
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
    public TextMeshProUGUI maxEnergyText;
    public TextMeshProUGUI timeAvailableText;
    public TextMeshProUGUI movesAvailableText;
    public TextMeshProUGUI winConditionText;
    public TextMeshProUGUI scoreToWinText;
    [Space(10)]

    public Toggle randomizeColors;
    public Toggle spawnNewGems;
    public Toggle energyToggle;
    public Toggle meteorToggle;
    public Toggle colorlessToggle;
    public Toggle sameColorToggle;
    public Toggle obstacleToggle;
    public Toggle frozenToggle1;
    public Toggle frozenToggle2;
    public Toggle frozenToggle3;
    public Toggle timeLimited;
    public Toggle movesLimited;
    [Space(10)]

    // Loading options
    public TextMeshProUGUI levelToLoadText;
    public TextMeshProUGUI levelToSaveText;
    [Space(10)]

    public Button incrBonus;
    public Button decrBonus;
    public Button incrEnergy;
    public Button decrEnergy;
    public Button incrTime;
    public Button decrTime;
    public Button incrMoves;
    public Button decrMoves;
    [Space(10)]    
    
    public EditorParams pu;
    
    private void Awake()
    {
        CorrectToggles();

        incrBonus.interactable = pu.spawnNewGems;
        decrBonus.interactable = pu.spawnNewGems;
        incrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;
        decrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;
        incrTime.interactable = pu.timeAvailable != 0;
        decrTime.interactable = pu.timeAvailable != 0;
        incrMoves.interactable = pu.movesAvailable != 0;
        decrMoves.interactable = pu.movesAvailable != 0;
    }

    private void Update () { 
        fpsText.text = LocalizationManager.instance.GetLocalizedValue("misc_fps_lable") + (1f / Time.deltaTime).ToString("0");
        slotText.text = EditorParams.currentSlot == -1 ?
            LocalizationManager.instance.GetLocalizedValue("editor_temp_slot_lable")
            :
            LocalizationManager.instance.GetLocalizedValue("editor_slot_lable") + 
            " " + EditorParams.currentSlot.ToString() + "/" + EditorParams.slotNumber.ToString() + 
            (EditorParams.isSaved ? "" : "*");

        xSizeText.text = pu.gridSize.x.ToString("0");
        ySizeText.text = pu.gridSize.y.ToString("0");
        availableColorsText.text = pu.colorsAvailable.ToString();
        sequenceSizeText.text = pu.sequenceSize.ToString();
        bonusChanceText.text = pu.bonusesPercentage.ToString() + "%";
        energyChanceText.text = pu.energyPercentage.ToString() + "%";
        maxEnergyText.text = pu.maximumEnergy.ToString();

        timeAvailableText.text = ParamUnit.GetParsedTime(pu.timeAvailable);
        movesAvailableText.text = pu.movesAvailable.ToString();
        incrTime.interactable = pu.timeAvailable != 0;
        decrTime.interactable = pu.timeAvailable != 0;
        incrMoves.interactable = pu.movesAvailable != 0;
        decrMoves.interactable = pu.movesAvailable != 0;

        winConditionText.text = LocalizationManager.instance.GetLocalizedValue("editor_win_condition_" + pu.winCondition.ToString());
        scoreToWinText.text = pu.scoreToWin.ToString();

        levelToLoadText.text = EditorParams.loadSlot.ToString();
        levelToSaveText.text = EditorParams.saveSlot.ToString();        
    }

    public void SwitchColorRandomization()
    {
        pu.randomizeColors = !pu.randomizeColors;
    }
    public void SwitchNewGemsSpawn()
    {
        pu.spawnNewGems = !pu.spawnNewGems;

        incrBonus.interactable = pu.spawnNewGems;
        decrBonus.interactable = pu.spawnNewGems;
        incrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;
        decrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;

        energyToggle.interactable = pu.spawnNewGems;
    }
    public void SwitchSpawnEnergy()
    {
        pu.spawnEnergy = !pu.spawnEnergy;

        incrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;
        decrEnergy.interactable = pu.spawnNewGems && pu.spawnEnergy;
    }    
    public void SwitchPermittedBonus(int bonusId)
    {
        bool wasPermitted = IsBonusPermitted(bonusId);

        int[] newArray;

        if (wasPermitted)
        {
            newArray = new int[pu.permittedBonuses.Length - 1];

            int iter = 0;
            foreach (int i in pu.permittedBonuses)
            {
                if (i != bonusId)
                {
                    newArray[iter] = i;
                    iter++;
                }                
            }
            pu.permittedBonuses = newArray;
        } else
        {
            newArray = new int[pu.permittedBonuses.Length + 1];
            pu.permittedBonuses.CopyTo(newArray, 0);
            newArray[pu.permittedBonuses.Length] = bonusId;
            pu.permittedBonuses = newArray;
        }
    }
    
    public void SwitchTimeLimited()
    {
        pu.timeAvailable = pu.timeAvailable == 0 ? 60 : 0;

        incrTime.interactable = pu.timeAvailable != 0;
        decrTime.interactable = pu.timeAvailable != 0;
    }
    public void SwitchMovesLimited()
    {
        pu.movesAvailable = pu.movesAvailable == 0 ? 5 : 0;

        incrMoves.interactable = pu.movesAvailable != 0;
        decrMoves.interactable = pu.movesAvailable != 0;
    }

    public void CorrectToggles()
    {
        randomizeColors.isOn = pu.randomizeColors;
        spawnNewGems.isOn = pu.spawnNewGems;
        energyToggle.isOn = pu.spawnEnergy;

        timeLimited.isOn = pu.timeAvailable != 0;
        movesLimited.isOn = pu.movesAvailable != 0;

        pu.randomizeColors = randomizeColors.isOn;
        pu.spawnNewGems = spawnNewGems.isOn;
        pu.spawnEnergy = energyToggle.isOn;
        pu.timeAvailable = timeLimited.isOn ? 60 : 0;
        pu.movesAvailable = movesLimited.isOn ? 5 : 0;

        meteorToggle.isOn = IsBonusPermitted((int)ParamUnit.Bonus.METEOR);
        colorlessToggle.isOn = IsBonusPermitted((int)ParamUnit.Bonus.COLORLESS);
        sameColorToggle.isOn = IsBonusPermitted((int)ParamUnit.Bonus.SAME_COLOR);
        obstacleToggle.isOn = IsBonusPermitted((int)ParamUnit.Bonus.OBSTACLE);
        frozenToggle1.isOn = IsBonusPermitted((int)ParamUnit.Bonus.ICE_1);
        frozenToggle2.isOn = IsBonusPermitted((int)ParamUnit.Bonus.ICE_2);
        frozenToggle3.isOn = IsBonusPermitted((int)ParamUnit.Bonus.ICE_3);

        if (meteorToggle.isOn != IsBonusPermitted((int)ParamUnit.Bonus.METEOR))
            SwitchPermittedBonus((int)ParamUnit.Bonus.METEOR);
        if (colorlessToggle.isOn != IsBonusPermitted((int)ParamUnit.Bonus.COLORLESS))
            SwitchPermittedBonus((int)ParamUnit.Bonus.COLORLESS);
        if (sameColorToggle.isOn != IsBonusPermitted((int)ParamUnit.Bonus.SAME_COLOR))
            SwitchPermittedBonus((int)ParamUnit.Bonus.SAME_COLOR);
        if (obstacleToggle.isOn != IsBonusPermitted((int)ParamUnit.Bonus.OBSTACLE))
            SwitchPermittedBonus((int)ParamUnit.Bonus.OBSTACLE);
        if (frozenToggle1.isOn != IsBonusPermitted((int)ParamUnit.Bonus.ICE_1))
            SwitchPermittedBonus((int)ParamUnit.Bonus.ICE_1);
        if (frozenToggle2.isOn != IsBonusPermitted((int)ParamUnit.Bonus.ICE_2))
            SwitchPermittedBonus((int)ParamUnit.Bonus.ICE_2);
        if (frozenToggle3.isOn != IsBonusPermitted((int)ParamUnit.Bonus.ICE_3))
            SwitchPermittedBonus((int)ParamUnit.Bonus.ICE_3);
    }

    private bool IsBonusPermitted(int bonusId)
    {
        bool contains = false;
        foreach (int i in pu.permittedBonuses)
        {
            if (i == bonusId)
            {
                contains = true;
            }
        }
        return contains;
    }    

}
