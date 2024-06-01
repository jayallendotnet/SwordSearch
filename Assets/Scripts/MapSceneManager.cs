using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSceneManager : MonoBehaviour{

    //Debug Data
    public Text worldProgressText;
    public Text levelProgressText;
    public Text autoSubmitText;
    public BattleData JustBattleOpponent;
    public GameObject coverUpGrasslands;

    void Start(){
    }

    public void HitHomepageButton(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }

    public void GoToHometown(){

        PressedAnyworldButton(1);

        //StaticVariables.currentBattleWorld = 0;
        //StaticVariables.currentBattleLevel = 1;
        /*
        if (StaticVariables.lowestWorldStageUnbeaten.worldNum < 1)
            return;
        if (StaticVariables.lastWorldStageVisited.worldNum == 1)
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
        else{
            StaticVariables.lastWorldStageVisited = new (1,1);
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
        }
        */
        /*
        if (StaticVariables.lowestWorldStageUnbeaten)
        if (StaticVariables.lastWorldStageVisited.worldNum == 1)
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
        else if (StaticVariables.lowestWorldStageUnbeaten.worldNum == 1)
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
        else {
            StaticVariables.lastWorldStageVisited.worldNum == 1
        }
        */
    }
    public void GoToGrasslands(){

        PressedAnyworldButton(2);
        /*
        if (StaticVariables.lowestWorldStageUnbeaten.worldNum < 2)
            return;
        if (StaticVariables.lastWorldStageVisited.worldNum == 2)
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world2Name);
        else{
            StaticVariables.lastWorldStageVisited = new (2,1);
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world2Name);
        }
        */
    }

    private void PressedAnyworldButton(int worldNum){
        if (worldNum > StaticVariables.highestBeatenStage.nextStage.world)
            return;
        StaticVariables.lastVisitedStage = StaticVariables.GetStage(worldNum, 1);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        

        //if (StaticVariables.lowestWorldStageUnbeaten.world < worldNum)
        //    return;
        //if (StaticVariables.lastWorldStageVisited.world == worldNum)
        //    StaticVariables.FadeOutThenLoadScene(worldName);
        //else{
        //    StaticVariables.lastWorldStageVisited = new (worldNum,1);
        //    StaticVariables.FadeOutThenLoadScene(worldName);
        //}
    }

}
