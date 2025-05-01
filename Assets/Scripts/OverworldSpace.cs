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
    [HideInInspector]
    public int originalSiblingIndex;
    [Header("Scene references")]
    public GameObject playerDestination;
    public Transform pathFromLastSpace;
    public GameObject button;
    public GameObject overworldPlayerSpaceIcon;
    


    public enum OverworldSpaceType{Battle, Cutscene, Tutorial, Atlas}
    [Header("Gameplay Stuff")]
    public OverworldSpaceType type = OverworldSpaceType.Battle;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Battle, OverworldSpaceType.Tutorial)]
    public BattleData battleData;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public CutsceneManager.Cutscene cutsceneID;
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public string cutsceneTitle = "";
    [ConditionalField(nameof(type), false, OverworldSpaceType.Cutscene)]
    public string cutsceneDescription = "";
    [ConditionalField(nameof(type), false, OverworldSpaceType.Atlas)]
    public int worldNumber = 1;

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
        float angle = Vector2.Angle(Vector2.down, diff);
        if (diff.x < 0)
            angle = 360 - angle;
        arrow.rotation = Quaternion.Euler(0,0, angle);
    }

    public void MovePlayerToThisSpace(){
        overworldSceneManager.StartMovingPlayerToSpace(this);
        overworldSceneManager.currentPlayerSpace = this;
        overworldSceneManager.currentEnemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
    }

    public void PlayerIsAlreadyAtThisSpace(){
        //overworldSceneManager.StartMovingPlayerToSpace(this);
        overworldSceneManager.currentPlayerSpace = this;
        overworldSceneManager.currentEnemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
        overworldSceneManager.PlayerArrivedAtDestination();
    }

    public void ClickedSpace(){
        if (overworldSceneManager.interactOverlayManager.isInteractOverlayShowing)
            return;        
        if (overworldSceneManager.isPlayerMoving)
            return;
        if (overworldSceneManager.currentPlayerSpace != this)
            MovePlayerToThisSpace();
        else{
            PlayerIsAlreadyAtThisSpace(); //if the player is already on the space. "moving" them a distance 0 should just start the interact menu popup
        }
    }

    public void FadeInVisuals(){
        if ((type == OverworldSpaceType.Battle) || (type == OverworldSpaceType.Tutorial)){
            if (battleData.enemyPrefab.GetComponent<EnemyData>().isHorde){
                foreach (Transform t in transform.GetChild(0).GetChild(0))
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

        }        
        if (type == OverworldSpaceType.Cutscene){
            foreach (Transform t in transform.GetChild(0).GetChild(0))
                enemyImages.Add(t.GetComponent<Image>());
                
            foreach (Image im in enemyImages){
                Color enemyImageColor = im.color;
                enemyImageColor.a = 0;
                im.color = enemyImageColor;
            }
        }

        overworldPlayerSpaceIcon.GetComponent<PathStep>().HideStep(0);


        button.SetActive(false);

        foreach (Transform t in pathFromLastSpace)
            t.GetComponent<PathStep>().HideStep(0);
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
    }

    private void FadeInEnemy(){
        print("fading in enemy");
        foreach (Image im in enemyImages){
            Color c = im.color;
            c.a = 1;
            im.DOColor(c, timeBetweenPathFadeSteps).OnComplete(TurnEnemyAniamtionsOn);
        }
        overworldPlayerSpaceIcon.GetComponent<PathStep>().ShowStep(timeBetweenPathFadeSteps);
    }

    private void TurnEnemyAniamtionsOn(){
        button.SetActive(true);
        foreach (Image im in enemyImages)
            im.GetComponent<Animator>().enabled = true;
    }
}


[System.Serializable]
public class BattleData{
    public GameObject enemyPrefab;
    public GameObject backgroundPrefab;
}

