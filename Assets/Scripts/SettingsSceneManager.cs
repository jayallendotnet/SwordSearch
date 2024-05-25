using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsSceneManager : MonoBehaviour{

    public Text worldNameDisplay;
    public Text levelNumberDisplay;

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
                worldNameDisplay.text = "HOMETOWN";
                break;
            case (1):
                worldNameDisplay.text = "GRASSLANDS";
                break;
            default:
                worldNameDisplay.text = "HOMETOWN";
                break;
        }
        levelNumberDisplay.text = StaticVariables.highestUnlockedLevel + "";

    }

    public void WorldDown(){
        StaticVariables.highestUnlockedWorld --;
        StaticVariables.highestUnlockedLevel = 1;
        if (StaticVariables.highestUnlockedWorld < 0)
            StaticVariables.highestUnlockedWorld = 0; 
        DisplayProgress();
           
    }

    public void WorldUp(){
        StaticVariables.highestUnlockedWorld ++;
        StaticVariables.highestUnlockedLevel = 1;
        if (StaticVariables.highestUnlockedWorld > 1)
            StaticVariables.highestUnlockedWorld = 1;   
        DisplayProgress(); 
    }
    public void LevelDown(){
        StaticVariables.highestUnlockedLevel --;
        if (StaticVariables.highestUnlockedLevel < 0)
            StaticVariables.highestUnlockedLevel = 0; 
        DisplayProgress();

    }
    public void LevelUp(){
        StaticVariables.highestUnlockedLevel ++;
        switch (StaticVariables.highestUnlockedWorld){
            case (0):
                if(StaticVariables.highestUnlockedLevel > 6)
                    StaticVariables.highestUnlockedLevel = 6;
                break;
            case (1):
                if(StaticVariables.highestUnlockedLevel > 10)
                    StaticVariables.highestUnlockedLevel = 10;
                break;
        }
        DisplayProgress();
    }


/* 
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
    } */

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
