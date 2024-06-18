using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneHeader : MonoBehaviour{

    public void GoToHomepage(){
        StaticVariables.FadeOutThenLoadScene("Homepage");
    }
    public void GoToAtlas(){
        StaticVariables.FadeOutThenLoadScene(StaticVariables.mapName);
    }
}
