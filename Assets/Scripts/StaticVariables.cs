using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class StaticVariables
{
    static public Transform tweenDummy;
    static public System.Random rand = new();
    static public BattleData battleData = null;
    static private string sceneName = "";
    static public Image fadeImage;
    static public float sceneFadeDuration = 0.5f;
    static public bool healActive = true;
    static public bool waterActive = true;
    static public bool fireActive = false;
    static public bool earthActive = false;
    static public bool lightningActive = false;
    static public bool darkActive = false;
    static public bool swordActive = false;
    static public int powerupsPerPuzzle = 3;
    static public BattleManager.PowerupTypes buffedType = BattleManager.PowerupTypes.None;
    //static public int highestUnlockedWorld = 1; //0 for start of game
    //static public int highestUnlockedLevel = 10;
    //static public int currentBattleWorld = 0;
    //static public int currentBattleLevel = 1;
    //static public bool beatCurrentBattle = false;
    static public bool hasTalkedToNewestEnemy = false;
    static public CutsceneManager.Cutscene cutsceneID;
    static public bool useAutoSubmit = false;
    static public string playerName = "Rebecca";

    //static public StageData lastWorldStageVisited = new(2,5);
    //static public StageData highestWorldStageBeaten = new(2,4);
    //static public StageData lowestWorldStageUnbeaten = new(2,5);

    //stages
    static public List<StageData> allStages;
    //static public List<StageData> hometownStages;
    //static public List<StageData> grasslandsStages;

    //game progress
    static public StageData highestBeatenStage;
    static public StageData lastVisitedStage;
    static public bool hasCompletedStage = false;

    //scene names
    static public string mainMenuName = "Homepage";
    static public string battleSceneName = "Battle Scene";
    static public string world1Name = "World 1 - Hometown";
    static public string world2Name = "World 2 - Grasslands";
    //static public string world3Name = "World 3 - Magical Forest";
    //static public string world4Name = "World 4 - Desert";
    //static public string world5Name = "World 5 - Darklands";
    //static public string world6Name = "World 6 - Frostlands";
    //static public string world7Name = "World 7 - Dragonlands";



    static public void WaitTimeThenCallFunction(float delay, TweenCallback function) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(function);
    }    
    static public void WaitTimeThenCallFunction(float delay, TweenCallback<string> function, string param) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(()=>function(param));
    }
    static public void WaitTimeThenCallFunction(float delay, TweenCallback<GameObject> function, GameObject param){
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(()=>function(param));
    }


    static public void FadeOutThenLoadScene(string name){
        sceneName = name;
        StartFadeDarken(sceneFadeDuration);
        WaitTimeThenCallFunction(sceneFadeDuration, LoadScene);
    }

    static public void FadeIntoScene(){
        StartFadeLighten(sceneFadeDuration);
    }

    static public void StartFadeDarken(float duration){
        Color currentColor = Color.black;
        currentColor.a = 0;
        fadeImage.color = currentColor;
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(Color.black, duration);
    }

    static public void StartFadeLighten(float duration){
        Color nextColor = Color.black;
        nextColor.a = 0;
        fadeImage.color = Color.black;
        fadeImage.gameObject.SetActive(true);
        fadeImage.DOColor(nextColor, duration).OnComplete(HideFadeObject);
    }

    static private void HideFadeObject(){
        fadeImage.gameObject.SetActive(false);
    }

    static private void LoadScene(){
        SceneManager.LoadScene(sceneName);
    }

    /*
    static public string GetCurrentWorldName(){
        return lastWorldStageVisited.world switch{
            -2 => "Map Scene",
            1 => world1Name,
            2 => world2Name,
            //2 => world2Name,
            //3 => world3Name,
            //4 => world4Name,
            //5 => world5Name,
            _ => world1Name,
        };
    }
    */
    /*
    static public bool AdvanceGameIfAppropriate(int worldNum, int spacesInWorld){
        //returns if the next overworld space should be unlocked
        if ((worldNum == lastWorldStageVisited.world) && (worldNum == lowestWorldStageUnbeaten.world)){
            if (lowestWorldStageUnbeaten.stage == lastWorldStageVisited.stage){
                if (beatCurrentBattle){
                    hasTalkedToNewestEnemy = false;
                    AdvanceGameProgress(spacesInWorld);
                    ClearCurrentBattleStats();
                    return true;
                }
            }
        }
        ClearCurrentBattleStats();
        return false;
    }
    */
    /*
    static private void AdvanceGameProgress(int spacesInWorld){
        lowestWorldStageUnbeaten.stage ++;
        if (lowestWorldStageUnbeaten.stage > spacesInWorld){
            lowestWorldStageUnbeaten.world ++;
            lowestWorldStageUnbeaten.stage = 1;
            if (lowestWorldStageUnbeaten.world > 6)
                lowestWorldStageUnbeaten.world = 6;
        }


        //highestUnlockedLevel ++;
        //if (highestUnlockedLevel > spacesInWorld){
        //    highestUnlockedWorld ++;
        //    highestUnlockedLevel = 1;
        //    if (highestUnlockedWorld > 6)
        //        highestUnlockedWorld = 6;
        //}
    }
    */
    /*
    static public void ClearCurrentBattleStats(){
        lastWorldStageVisited = new(0,0);
        //currentBattleLevel = 0;
        //currentBattleWorld = 0;
        beatCurrentBattle = false;

    }
    */
    static public StageData GetStage(int worldNum, int stageNum){
        foreach (StageData stageData in allStages){
            if (stageData.world == worldNum && stageData.stage == stageNum)
                return stageData;
        }
        return allStages[0];
    }

}

public class StageData{
    public int world;
    public int stage;
    public GameObject enemyPrefab;
    public StageData previousStage = null;
    public StageData nextStage = null;
    public string worldName;

    public StageData (int worldNum, string worldName, int stageNum, GameObject enemyPrefab){
        this.world = worldNum;
        this.worldName = worldName;
        this.stage = stageNum;
        this.enemyPrefab = enemyPrefab;
    }

    /*
    public StageData(int worldNum, int stageNum){
        this.world = worldNum;
        if (this.world < 1)
            this.world = 1;
        if (this.world > 7)
            this.world = 7;
        this.stage = stageNum;
        if (this.stage < 1)
            this.stage = 1;
        //no max stageNum yet
    }
    */
}