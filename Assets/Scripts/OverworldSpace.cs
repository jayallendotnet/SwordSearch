using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using MyBox;

public class OverworldSpace : MonoBehaviour{

    [HideInInspector]
    public OverworldSceneManager overworldSceneManager;
    private List<Image> enemyImages = new List<Image>();
    private int pathFadeIndex = 0;
    private float timeBetweenPathFadeSteps = 0.5f;
    [Header("Scene references")]
    public GameObject playerDestination;
    public Transform pathFromLastSpace;
    public GameObject button;


    public enum OverworldSpaceType{Battle, Cutscene, Tutorial}
    [Header("Gameplay Stuff")]
    public OverworldSpaceType type = OverworldSpaceType.Battle;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Battle, OverworldSpaceType.Tutorial)]
    public BattleData battleData;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public CutsceneData cutsceneData;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public string cutsceneDescription = "";
    [ConditionalField(nameof(type), false, OverworldSpaceType.Tutorial)]
    public string tutorialName;

    public void MovePlayerToThisSpace(){
        overworldSceneManager.currentPlayerSpace = this;
        overworldSceneManager.currentEnemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
        overworldSceneManager.MovePlayerToPosition(playerDestination);
    }

    public void ClickedSpace(){
        if (overworldSceneManager.interactOverlayManager.isInteractOverlayShowing)
            return;        
        if (overworldSceneManager.isPlayerMoving)
            return;
        if (overworldSceneManager.currentPlayerSpace != this)
            MovePlayerToThisSpace();
        else
            overworldSceneManager.interactOverlayManager.ShowInteractOverlay();
    }

    public void FadeInVisuals(){
        if (battleData.enemyPrefab.GetComponent<EnemyData>().isHorde){
            foreach (Transform t in transform.GetChild(0))
                enemyImages.Add(t.GetChild(0).GetComponent<Image>());
        }
        else
            enemyImages.Add(transform.GetChild(0).GetComponent<Image>());

        foreach (Image im in enemyImages){
            Color enemyImageColor = Color.white;
            enemyImageColor.a = 0;
            im.color = enemyImageColor;
            im.GetComponent<Animator>().enabled = false;
        }


        button.SetActive(false);

        foreach (Transform t in pathFromLastSpace){
            Image im = t.GetComponent<Image>();
            Color pathColor = im.color;
            pathColor.a = 0;
            im.color = pathColor;
        }
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, FadeNextStepOfPath);
    }

    private void FadeNextStepOfPath(){
        pathFadeIndex ++;
        if (pathFadeIndex >= pathFromLastSpace.childCount){
            FadeInEnemy();
            return;
        }
        Image im = pathFromLastSpace.GetChild(pathFadeIndex).GetComponent<Image>();
        Color c = im.color;
        c.a = 1;
        im.DOColor(c, timeBetweenPathFadeSteps).OnComplete(FadeNextStepOfPath);
    }

    private void FadeInEnemy(){
        foreach (Image im in enemyImages)
            im.DOColor(Color.white, timeBetweenPathFadeSteps).OnComplete(TurnEnemyAniamtionsOn);
    }

    private void TurnEnemyAniamtionsOn(){
        foreach (Image im in enemyImages)
            im.GetComponent<Animator>().enabled = true;
        button.SetActive(true);
    }
}


[System.Serializable]
public class BattleData{
    public GameObject enemyPrefab;
    public GameObject backgroundPrefab;
}

