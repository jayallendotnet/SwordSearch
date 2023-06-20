using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GeneralSceneManager : MonoBehaviour{

    public Image fadeImage;
    private bool hasStarted = false;

    public BattleData temp;
    public CutsceneData temp2;

    void Start(){
        Setup();
    }

    public void Setup(){
        if (!hasStarted){
            StaticVariables.tweenDummy = transform;
            StaticVariables.fadeImage = fadeImage;
            StaticVariables.FadeIntoScene();
            hasStarted = true;
        }
    }


    //temp for map scene start
    public void OpeningCutscene(){
        StaticVariables.cutsceneToPlay = temp2;
        StaticVariables.FadeOutThenLoadScene("Cutscene");
    }

    public void Grasslands(){


        int worldNum = 1;
        int levelNum = 1;
        StaticVariables.currentBattleWorld = worldNum;
        StaticVariables.currentBattleLevel = levelNum;
        StaticVariables.highestUnlockedWorld = 1;
        StaticVariables.highestUnlockedLevel = 1;
        StaticVariables.beatCurrentBattle = false;

        StaticVariables.healActive = true;
        StaticVariables.waterActive = true;
        StaticVariables.fireActive = false;
        StaticVariables.earthActive = false;
        StaticVariables.lightningActive = false;
        StaticVariables.darkActive = false;
        StaticVariables.swordActive = false;
        StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
    }

    public void JustBattle(){
        StaticVariables.battleData = temp;

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
