using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OverworldEnemySpace : MonoBehaviour{

    [HideInInspector]
    public OverworldSceneManager overworldSceneManager;
    private Image enemyImage;
    private int pathFadeIndex = 0;
    private float timeBetweenPathFadeSteps = 0.5f;
    [Header("Scene references")]
    public GameObject playerDestination;
    public Transform pathFromLastSpace;
    public GameObject button;

    [Header("Gameplay Stuff")]
    public BattleData battleData;

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
        enemyImage = transform.GetChild(0).GetComponent<Image>();
        Color enemyImageColor = Color.white;
        enemyImageColor.a = 0;
        enemyImage.color = enemyImageColor;
        enemyImage.GetComponent<Animator>().enabled = false;

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
        enemyImage.DOColor(Color.white, timeBetweenPathFadeSteps).OnComplete(TurnEnemyAniamtionsOn);
    }

    private void TurnEnemyAniamtionsOn(){
        enemyImage.GetComponent<Animator>().enabled = true;
        button.SetActive(true);
    }
}


[System.Serializable]
public class BattleData{
    public GameObject enemyPrefab;
    public GameObject backgroundPrefab;
}

