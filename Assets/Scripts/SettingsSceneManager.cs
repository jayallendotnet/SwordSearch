using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsSceneManager : MonoBehaviour{

    public Text worldNameDisplay;
    public Text stageNumberDisplay;
    public InputField playerNameInput;
    //public Text storyModeDisplay;
    [Header("Difficulty Modes")]
    public Text difficultyModeDisplay;
    public Text difficultyModeDescription;
    [TextArea(2,5)]
    public string normalModeDescription;
    [TextArea(2,5)]
    public string storyModeDescription;
    [TextArea(2,5)]
    public string puzzleModeDescription;
    [TextArea(2,5)]
    public string easyModeDescription;
    [TextArea(2,5)]
    public string hardModeDescription;

    //public BattleData JustBattleOpponent;

    void Start(){
        Setup();
    }

    public void Setup(){
        DisplayProgress();
        DisplayPlayerName();
        DisplayDifficultyMode();
    }

    public void HitBackButton(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }

    private void DisplayProgress(){
        worldNameDisplay.text = StaticVariables.highestBeatenStage.nextStage.worldName.ToUpper();
        stageNumberDisplay.text = "STAGE  " + StaticVariables.highestBeatenStage.nextStage.stage;
    }

    public void WorldDown(){
        int newWorld = StaticVariables.highestBeatenStage.nextStage.world - 1;
        if (newWorld < 1)
            newWorld = 1;
        int newStage = 1;
        StageData stage = StaticVariables.GetStage(newWorld, newStage);
        StaticVariables.highestBeatenStage = stage.previousStage;
        DisplayProgress();
        SaveSystem.SaveGame();
    }

    public void WorldUp(){        
        int newWorld = StaticVariables.highestBeatenStage.nextStage.world + 1;
        if (newWorld > 8)
            newWorld = 8;
        int newStage = 1;
        StageData stage = StaticVariables.GetStage(newWorld, newStage);
        StaticVariables.highestBeatenStage = stage.previousStage;
        DisplayProgress(); 
        SaveSystem.SaveGame();
    }
    public void StageDown(){
        if (StaticVariables.highestBeatenStage.previousStage != null)
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.previousStage;
        DisplayProgress();
        SaveSystem.SaveGame();
    }

    public void StageUp(){
        if ((StaticVariables.highestBeatenStage.nextStage != null) && (StaticVariables.highestBeatenStage.nextStage.nextStage != null))
            StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.nextStage;
        DisplayProgress();
        SaveSystem.SaveGame();
    }

    private void DisplayPlayerName(){
        playerNameInput.text = StaticVariables.playerName;
    }

    public void UpdatePlayerName(){
        if (playerNameInput.text == "")
            DisplayPlayerName();
        else{
        StaticVariables.playerName = playerNameInput.text;
            DisplayPlayerName();
        }
        SaveSystem.SaveGame();
    }
    
    public void NextDifficultyMode(){
        //easy > normal > hard > puzzle > easy
        switch (StaticVariables.difficultyMode){
            case StaticVariables.DifficultyMode.Easy:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
                break;
            case StaticVariables.DifficultyMode.Normal:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Hard;
                break;
            case StaticVariables.DifficultyMode.Hard:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Puzzle;
                break;
            case StaticVariables.DifficultyMode.Puzzle:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Easy;
                break;
            default:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
                break;
            //case StaticVariables.DifficultyMode.Story:
            //    StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
            //    break;
        }
        DisplayDifficultyMode();
        SaveSystem.SaveGame();
    }
    
    public void PreviousDifficultyMode(){ 
        //same as above but reversed       
        switch (StaticVariables.difficultyMode){
            case StaticVariables.DifficultyMode.Easy:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Puzzle;
                break;
            case StaticVariables.DifficultyMode.Puzzle:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Hard;
                break;
            case StaticVariables.DifficultyMode.Hard:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
                break;
            case StaticVariables.DifficultyMode.Normal:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Easy;
                break;
            default:
                StaticVariables.difficultyMode = StaticVariables.DifficultyMode.Normal;
                break;
        }
        DisplayDifficultyMode();
        SaveSystem.SaveGame();
    }
    
    public void DisplayDifficultyMode(){
        switch (StaticVariables.difficultyMode){
            case StaticVariables.DifficultyMode.Normal:
                difficultyModeDisplay.text = "NORMAL";
                difficultyModeDescription.text = normalModeDescription;
                break;
            case StaticVariables.DifficultyMode.Story:
                difficultyModeDisplay.text = "STORY";
                difficultyModeDescription.text = storyModeDescription;
                break;
            case StaticVariables.DifficultyMode.Puzzle:
                difficultyModeDisplay.text = "PUZZLE";
                difficultyModeDescription.text = puzzleModeDescription;
                break;
            case StaticVariables.DifficultyMode.Easy:
                difficultyModeDisplay.text = "EASY";
                difficultyModeDescription.text = easyModeDescription;
                break;
            case StaticVariables.DifficultyMode.Hard:
                difficultyModeDisplay.text = "HARD";
                difficultyModeDescription.text = hardModeDescription;
                break;
            default:
                difficultyModeDisplay.text = "ERROR";
                difficultyModeDescription.text = "Attempting to use an invalid difficulty mode.";
                break;
        }
    }
}
