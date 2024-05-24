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
        StaticVariables.currentBattleWorld = 0;
        StaticVariables.currentBattleLevel = 1;
        StaticVariables.FadeOutThenLoadScene(StaticVariables.world0Name);
    }
    public void GoToGrasslands(){
        if (StaticVariables.highestUnlockedWorld >= 1){ 
            StaticVariables.currentBattleWorld = 1;
            StaticVariables.currentBattleLevel = 1;
            StaticVariables.FadeOutThenLoadScene(StaticVariables.world1Name);
        }
    }
}
