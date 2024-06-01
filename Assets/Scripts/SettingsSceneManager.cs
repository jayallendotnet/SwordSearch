using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsSceneManager : MonoBehaviour{

    public Text worldNameDisplay;
    public Text stageNumberDisplay;

    public Text autoSubmitText;
    public BattleData JustBattleOpponent;

    void Start(){
        Setup();
    }

    public void Setup(){
        //if (StaticVariables.currentBattleLevel < 0)
        //    StaticVariables.currentBattleLevel = 0;
        //if (StaticVariables.currentBattleWorld < 0)
        //    StaticVariables.currentBattleWorld = 0;
        DisplayProgress();
        DisplayAutoSubmit();
    }

    public void HitBackButton(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }

    private void DisplayProgress(){
        /*
        switch (StaticVariables.lowestWorldStageUnbeaten.world){
            case (1):
                worldNameDisplay.text = "HOMETOWN";
                break;
            case (2):
                worldNameDisplay.text = "GRASSLANDS";
                break;
            default:
                break;
        }
        
        levelNumberDisplay.text = StaticVariables.lowestWorldStageUnbeaten.stage + "";
        */

        worldNameDisplay.text = StaticVariables.highestBeatenStage.nextStage.worldName;
        stageNumberDisplay.text = StaticVariables.highestBeatenStage.nextStage.stage + "";

    }

    public void WorldDown(){
        int newWorld = StaticVariables.highestBeatenStage.nextStage.world - 1;
        if (newWorld < 1)
            newWorld = 1;
        int newStage = 1;
        StageData stage = StaticVariables.GetStage(newWorld, newStage);
        StaticVariables.highestBeatenStage = stage.previousStage;



        //if (StaticVariables.lowestWorldStageUnbeaten.world <= 1)
        //    return;
        //StaticVariables.lowestWorldStageUnbeaten.world --;
        //StaticVariables.lowestWorldStageUnbeaten.stage = 1;
        //StaticVariables.highestUnlockedWorld --;
        //StaticVariables.highestUnlockedLevel = 1;
        //if (StaticVariables.highestUnlockedWorld < 0)
        //    StaticVariables.highestUnlockedWorld = 0; 
        DisplayProgress();
        //SetHighestWorldStageBeaten();
    }

    public void WorldUp(){        
        int newWorld = StaticVariables.highestBeatenStage.nextStage.world + 1;
        if (newWorld > 2)
            newWorld = 2;
        int newStage = 1;
        StageData stage = StaticVariables.GetStage(newWorld, newStage);
        StaticVariables.highestBeatenStage = stage.previousStage;

        //if (StaticVariables.lowestWorldStageUnbeaten.world >= 2)
        //    return;
        //StaticVariables.lowestWorldStageUnbeaten.world ++;
        //StaticVariables.lowestWorldStageUnbeaten.stage = 1;

        //StaticVariables.highestUnlockedWorld ++;
        //StaticVariables.highestUnlockedLevel = 1;
        //if (StaticVariables.highestUnlockedWorld > 1)
        //    StaticVariables.highestUnlockedWorld = 1;   
        DisplayProgress(); 
        //SetHighestWorldStageBeaten();
    }
    public void StageDown(){
        if (StaticVariables.highestBeatenStage.previousStage != null)
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.previousStage;


        //if (StaticVariables.lowestWorldStageUnbeaten.stage <= 1)
        //    return;
        //StaticVariables.lowestWorldStageUnbeaten.stage --;
        //StaticVariables.highestUnlockedLevel --;
        //if (StaticVariables.highestUnlockedLevel < 0)
        //    StaticVariables.highestUnlockedLevel = 0; 
        DisplayProgress();
        //SetHighestWorldStageBeaten();
    }

    public void StageUp(){
        //print(StaticVariables.highestBeatenStage.stage);
        if ((StaticVariables.highestBeatenStage.nextStage != null) && (StaticVariables.highestBeatenStage.nextStage.nextStage != null))
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.nextStage;

        //StaticVariables.lowestWorldStageUnbeaten.stage ++;
        //switch (StaticVariables.lowestWorldStageUnbeaten.world){
        //    case (1):
        //        if(StaticVariables.lowestWorldStageUnbeaten.stage > 7)
        //            StaticVariables.lowestWorldStageUnbeaten.stage = 7;
        //        break;
        //    case (2):
        //        if(StaticVariables.lowestWorldStageUnbeaten.stage > 10)
        //            StaticVariables.lowestWorldStageUnbeaten.stage = 10;
        //        break;
        //}

        /*
        StaticVariables.highestUnlockedLevel ++;
        switch (StaticVariables.highestUnlockedWorld){
            case (0):
                if(StaticVariables.highestUnlockedLevel > 7)
                    StaticVariables.highestUnlockedLevel = 7;
                break;
            case (1):
                if(StaticVariables.highestUnlockedLevel > 10)
                    StaticVariables.highestUnlockedLevel = 10;
                break;
        }
        */
        DisplayProgress();
        //SetHighestWorldStageBeaten();
    }

    /*
    private void SetHighestWorldStageBeaten(){
        print(StaticVariables.highestWorldStageBeaten.world + ", " + StaticVariables.highestWorldStageBeaten.stage);
        StaticVariables.highestWorldStageBeaten.world = StaticVariables.lowestWorldStageUnbeaten.world;
        StaticVariables.highestWorldStageBeaten.stage = StaticVariables.lowestWorldStageUnbeaten.stage - 1;
        if (StaticVariables.highestWorldStageBeaten.stage <= 0){
            if (StaticVariables.highestWorldStageBeaten.world == 2)
                StaticVariables.highestWorldStageBeaten = new(1,7);
        }
        print(StaticVariables.highestWorldStageBeaten.world + ", " + StaticVariables.highestWorldStageBeaten.stage);
    }
    */

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


    /*
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
    */
}
