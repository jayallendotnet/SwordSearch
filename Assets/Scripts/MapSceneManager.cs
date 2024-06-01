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
    }
    public void GoToGrasslands(){
        PressedAnyworldButton(2);
    }

    private void PressedAnyworldButton(int worldNum){
        if (worldNum > StaticVariables.highestBeatenStage.nextStage.world)
            return;
        StaticVariables.lastVisitedStage = StaticVariables.GetStage(worldNum, 1);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
    }

}
