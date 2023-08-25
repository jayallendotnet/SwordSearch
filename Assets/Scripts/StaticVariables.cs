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
    static public System.Random rand = new System.Random();
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
    static public int highestUnlockedWorld = 1; //0 for start of game
    static public int highestUnlockedLevel = 10;
    static public int currentBattleWorld = 1;
    static public int currentBattleLevel = 10;
    static public string battleSceneName = "Battle Scene";
    static public string world0Name = "World 0 - Hometown";
    static public string world1Name = "World 1 - Grasslands";
    static public string world2Name = "World 2 - Magical Forest";
    static public string world3Name = "World 3 - Desert";
    static public string world4Name = "World 4 - Darklands";
    static public string world5Name = "World 5 - Frostlands";
    static public string world6Name = "World 6 - Dragonlands";
    static public bool beatCurrentBattle = false;
    static public bool hasTalkedToNewestEnemy = false;
    static public CutsceneManager.Cutscene cutsceneID;
    static public bool useAutoSubmit = false;
    static public string playerName = "Rebecca";


    static public void WaitTimeThenCallFunction(float delay, TweenCallback function) {
        tweenDummy.DOLocalMove(tweenDummy.transform.localPosition, delay, false).OnComplete(function);
    }    
    static public void WaitTimeThenCallFunction(float delay, TweenCallback<string> function, string param) {
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

    static public string GetCurrentWorldName(){
        return currentBattleWorld switch{
            -2 => "Map Scene",
            0 => world0Name,
            1 => world1Name,
            2 => world2Name,
            3 => world3Name,
            4 => world4Name,
            5 => world5Name,
            _ => world6Name,
        };
    }

    static public bool AdvanceGameIfAppropriate(int worldNum, int spacesInWorld){
        //returns if the next overworld space should be unlocked
        if ((worldNum == currentBattleWorld) && (worldNum == highestUnlockedWorld)){
            if (highestUnlockedLevel == currentBattleLevel){
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

    
    static private void AdvanceGameProgress(int spacesInWorld){
        highestUnlockedLevel ++;
        if (highestUnlockedLevel > spacesInWorld){
            highestUnlockedWorld ++;
            highestUnlockedLevel = 1;
            if (highestUnlockedWorld > 6)
                highestUnlockedWorld = 6;
        }
    }

    static public void ClearCurrentBattleStats(){
        currentBattleLevel = 0;
        currentBattleWorld = 0;
        beatCurrentBattle = false;

    }

}