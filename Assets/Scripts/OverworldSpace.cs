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
    public GameObject overworldPlayerSpaceIcon;
    


    public enum OverworldSpaceType{Battle, Cutscene, Tutorial}
    [Header("Gameplay Stuff")]
    public OverworldSpaceType type = OverworldSpaceType.Battle;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Battle, OverworldSpaceType.Tutorial)]
    public BattleData battleData;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public CutsceneManager.Cutscene cutsceneID;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public string cutsceneDescription = "";

    void Start(){
        //overworldPlayerSpaceIcon = playerDestination.transform.GetChild(1).gameObject;
        Destroy(playerDestination.transform.GetChild(0).gameObject);
        TurnStepArrows();
    }

    private void TurnStepArrows(){
        for (int i = 0; i < pathFromLastSpace.childCount; i++){
            if (i == pathFromLastSpace.childCount - 1)
                TurnStepArrow(pathFromLastSpace.GetChild(i), transform.GetChild(2).GetChild(1).position);
            else
                TurnStepArrow(pathFromLastSpace.GetChild(i), pathFromLastSpace.GetChild(i + 1).position);
        }
    }

    private void TurnStepArrow(Transform parent, Vector2 nextStep){
        Transform arrow = parent.GetChild(0).GetChild(0);
        Vector2 start = arrow.position;

        Vector2 diff = (nextStep - start).normalized;
        //print(diff);
        //Vector2 diff = (start - nextStep).normalized;
        //float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        float angle = Vector2.Angle(Vector2.down, diff);
        if (diff.x < 0)
            angle = 360 - angle;
        arrow.rotation = Quaternion.Euler(0,0, angle);
        //Vector3 diff2 = new Vector3 (diff.x, diff.y, 0);

        //arrow.rotation = Quaternion.LookRotation(Vector3.right);
        //parent.GetChild(1).LookAt(nextStep);
        //Vector2 startPos = arrow.transform.position;
        //Vector2 facing = (startPos - nextStep).normalized;
    }

    public void MovePlayerToThisSpace(){
        overworldSceneManager.StartMovingPlayerToSpace(this);
        overworldSceneManager.currentPlayerSpace = this;
        overworldSceneManager.currentEnemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
        //overworldSceneManager.MovePlayerToPosition(playerDestination);
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
            enemyImages.Add(transform.GetChild(0).GetChild(0).GetComponent<Image>());

        foreach (Image im in enemyImages){
            Color enemyImageColor = Color.white;
            enemyImageColor.a = 0;
            im.color = enemyImageColor;
            im.GetComponent<Animator>().enabled = false;
        }

        overworldPlayerSpaceIcon.GetComponent<PathStep>().HideStep(0);

        //Image playerSpotIm = playerDestination.transform.Find("Overworld Player Space Icon").GetComponent<Image>();
        //Color spotColor = playerSpotIm.color;
        //spotColor.a = 0;
        //playerSpotIm.color = spotColor;


        button.SetActive(false);

        foreach (Transform t in pathFromLastSpace){
            t.GetComponent<PathStep>().HideStep(0);
            //Image im = t.GetChild(0).GetComponent<Image>();
            //Color pathColor = im.color;
            //pathColor.a = 0;
            //im.color = pathColor;

            
            //Image im2 = t.GetChild(0).GetChild(0).GetComponent<Image>();
            //Color pathColor2 = im2.color;
            //pathColor2.a = 0;
            //im2.color = pathColor2;
        }
        pathFadeIndex = -1;
        StaticVariables.WaitTimeThenCallFunction(StaticVariables.sceneFadeDuration, FadeNextStepOfPath);
    }

    private void FadeNextStepOfPath(){
        pathFadeIndex ++;
        if (pathFadeIndex >= pathFromLastSpace.childCount){
            FadeInEnemy();
            return;
        }

        pathFromLastSpace.GetChild(pathFadeIndex).GetComponent<PathStep>().ShowStep(timeBetweenPathFadeSteps);
        StaticVariables.WaitTimeThenCallFunction(timeBetweenPathFadeSteps, FadeNextStepOfPath);

        //Image im = pathFromLastSpace.GetChild(pathFadeIndex).GetChild(0).GetComponent<Image>();
        //Color c = im.color;
        //c.a = 1;
        
        //Image im2 = pathFromLastSpace.GetChild(pathFadeIndex).GetChild(0).GetChild(0).GetComponent<Image>();
        //Color c2 = im2.color;
        //c2.a = 1;
        //im2.DOColor(c, timeBetweenPathFadeSteps);
        //im.DOColor(c, timeBetweenPathFadeSteps).OnComplete(FadeNextStepOfPath);
    }

    private void FadeInEnemy(){
        foreach (Image im in enemyImages)
            im.DOColor(Color.white, timeBetweenPathFadeSteps).OnComplete(TurnEnemyAniamtionsOn);


        overworldPlayerSpaceIcon.GetComponent<PathStep>().ShowStep(timeBetweenPathFadeSteps);

        //Image playerSpotIm = playerDestination.transform.GetChild(0).GetComponent<Image>();
        //Color spotColor = playerSpotIm.color;
        //spotColor.a = 1;
        //playerSpotIm.DOColor(spotColor, timeBetweenPathFadeSteps);
        //playerSpotIm.color = spotColor;

        //            Image im2 = pathFromLastSpace.GetChild(pathFadeIndex).GetChild(0).GetChild(0).GetComponent<Image>();
        //Color c2 = im2.color;
        //c2.a = 1;
        //im2.DOColor(c, timeBetweenPathFadeSteps);
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

