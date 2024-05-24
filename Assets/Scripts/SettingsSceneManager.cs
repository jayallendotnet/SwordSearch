using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsSceneManager : MonoBehaviour{

    //Debug Data
    public Text worldProgressText;
    public Text levelProgressText;
    public Text autoSubmitText;
    public BattleData JustBattleOpponent;

    void Start(){
        Setup();
    }

    public void Setup(){
        if (StaticVariables.currentBattleLevel < 0)
            StaticVariables.currentBattleLevel = 0;
        if (StaticVariables.currentBattleWorld < 0)
            StaticVariables.currentBattleWorld = 0;
        DisplayProgress();
        DisplayAutoSubmit();
    }

    public void HitBackButton(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }

    private void DisplayProgress(){
        switch (StaticVariables.highestUnlockedWorld){
            case (0):
                worldProgressText.text = "Hometown";
                break;
            default:
                worldProgressText.text = "Grasslands";
                break;
            //case (2):
            //    worldProgressText.text = "Enchanted Forest";
            //    coverUpGrasslands.SetActive(false);
            //    break;
        }
        levelProgressText.text = StaticVariables.highestUnlockedLevel + "";

    }

    public void ClearProgress(){
        StaticVariables.highestUnlockedWorld = 0;
        StaticVariables.highestUnlockedLevel = 1;
        StaticVariables.currentBattleWorld = 0;
        StaticVariables.currentBattleLevel = 1;
        DisplayProgress();
    }    
    
    public void CompleteHometown(){
        StaticVariables.highestUnlockedWorld = 1;
        StaticVariables.highestUnlockedLevel = 1;
        DisplayProgress();
    }    
    
    public void CompleteGrasslands(){
        StaticVariables.highestUnlockedWorld = 1;
        StaticVariables.highestUnlockedLevel = 10;
        DisplayProgress();
    }

    private void DisplayAutoSubmit(){
        if (StaticVariables.useAutoSubmit)
            autoSubmitText.text = "ON";
        else   
            autoSubmitText.text = "OFF";
    }

    public void ToggleAutoSubmit(){
        StaticVariables.useAutoSubmit = !StaticVariables.useAutoSubmit;
        DisplayAutoSubmit();
    }


    public void JustBattle(){
        StaticVariables.battleData = JustBattleOpponent;

        StaticVariables.powerupsPerPuzzle = 5;

        StaticVariables.healActive = true;
        StaticVariables.waterActive = true;
        StaticVariables.fireActive = true;
        StaticVariables.earthActive = true;
        StaticVariables.lightningActive = true;
        StaticVariables.darkActive = true;
        StaticVariables.swordActive = true;

        int worldNum = -2;
        int levelNum = -2;
        StaticVariables.currentBattleWorld = worldNum;
        StaticVariables.currentBattleLevel = levelNum;
        StaticVariables.beatCurrentBattle = false;

        StaticVariables.FadeOutThenLoadScene(StaticVariables.battleSceneName);
    }
}
