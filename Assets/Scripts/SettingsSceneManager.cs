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
        DisplayProgress();
        DisplayAutoSubmit();
    }

    public void HitBackButton(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }

    private void DisplayProgress(){
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
        DisplayProgress();
    }

    public void WorldUp(){        
        int newWorld = StaticVariables.highestBeatenStage.nextStage.world + 1;
        if (newWorld > 2)
            newWorld = 2;
        int newStage = 1;
        StageData stage = StaticVariables.GetStage(newWorld, newStage);
        StaticVariables.highestBeatenStage = stage.previousStage;
        DisplayProgress(); 
    }
    public void StageDown(){
        if (StaticVariables.highestBeatenStage.previousStage != null)
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.previousStage;
        DisplayProgress();
    }

    public void StageUp(){
        if ((StaticVariables.highestBeatenStage.nextStage != null) && (StaticVariables.highestBeatenStage.nextStage.nextStage != null))
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.nextStage;
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
}
