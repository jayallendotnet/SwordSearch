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
    static public bool hasTalkedToNewestEnemy = false;
    static public CutsceneManager.Cutscene cutsceneID;
    static public string playerName = "Rebecca";
    static public BookData[] readingWaterBooks;
    static public BookData[] readingHealBooks;
    static public BookData[] readingEarthBooks;
    static public BookData[] readingFireBooks;
    static public BookData[] readingLightningBooks;
    static public BookData[] readingDarkBooks;
    static public BookData[] readingSwordBooks;
    static public string[] wordLibraryForChecking;
    static public string[] wordLibraryForGeneration;
    static public char[] randomLetterPool;
    static public string[] wordLibraryForGeneratingSmallerPuzzles;

    //stages
    static public List<StageData> allStages;

    //game progress
    static public StageData highestBeatenStage;
    static public StageData lastVisitedStage;
    static public bool hasCompletedStage = false;

    //scene names
    static public string mainMenuName = "Homepage";
    static public string mapName = "Atlas";
    static public string battleSceneName = "Battle Scene";
    static public string world1Name = "World 1 - Hometown";
    static public string world2Name = "World 2 - Grasslands";
    static public string world3Name = "World 3 - Enchanted Forest";
    static public string world4Name = "World 4 - Sunscorched Desert";
    static public string world5Name = "World 5 - Fallen City";
    static public string world6Name = "World 6 - Frostlands";
    static public string world7Name = "World 7 - Caverns";
    static public string world8Name = "World 8 - Dragon's Den";



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

    static public StageData GetStage(int worldNum, int stageNum){
        foreach (StageData stageData in allStages){
            if (stageData.world == worldNum && stageData.stage == stageNum)
                return stageData;
        }
        return allStages[0];
    }

    static public StageData GetStage(EnemyData enemyData){
        //gets the stage which has enemyPrefab as its enemy
        foreach (StageData stageData in allStages){
            if (stageData.enemyPrefab  != null){
                EnemyData ed = stageData.enemyPrefab.GetComponent<EnemyData>();
                if ((ed != null) && (ed == enemyData))
                    return stageData;
            }
        }
        return allStages[0];
    }

    static public bool ShouldStageShowInfoButton(StageData stageData){
        return stageData.world switch {
            1 => false,
            2 => stageData.stage >= 4,
            _ => true,
        };
    }

    static public bool ShouldStageShowInfoButton(EnemyData enemyData){
        return ShouldStageShowInfoButton(GetStage(enemyData));
    }

    static public bool IsReadingEnabledForStage(StageData stageData){
        return stageData.world >= 4;
    }

    static public bool IsReadingEnabledForStage(EnemyData enemyData){
        return IsReadingEnabledForStage(GetStage(enemyData));
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
}

public class BookData{
    public string name;
    public string description;

    public BookData(string fullData){
        if (!fullData.Contains('\\'))
            fullData += "\\lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua";
        string[] data = fullData.Split('\\');
        name = data[0];
        description = data[1];
    }
}