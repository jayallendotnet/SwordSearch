using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSceneManager : MonoBehaviour{

    //Debug Data

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
    public void GoToForest(){
        PressedAnyworldButton(3);
    }
    public void GoToDesert(){
        PressedAnyworldButton(4);
    }
    public void GoToDark(){
        PressedAnyworldButton(5);
    }
    public void GoToFrostlands(){
        PressedAnyworldButton(6);
    }
    public void GoToDragonRealm(){
        PressedAnyworldButton(7);
    }


    private void PressedAnyworldButton(int worldNum){
        //if (worldNum > StaticVariables.highestBeatenStage.nextStage.world)
        //    return;
        StaticVariables.lastVisitedStage = StaticVariables.GetStage(worldNum, 1);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
    }

}
