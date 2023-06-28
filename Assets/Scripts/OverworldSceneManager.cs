using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OverworldSceneManager : MonoBehaviour{

    [Header("Scene References")]
    public RectTransform overworldView;
    public RectTransform playerParent;
    public Animator playerAnimator;
    public OverworldSpace[] overworldSpaces;


    [Header("Timing Configurations")]
    public float playerWalkSpeed = 500f;
    public float minTimeToMove = 1f;

    [Header("Other")]
    public int thisWorldNum;
    

    [HideInInspector]
    public bool isPlayerMoving = false;
    [HideInInspector]
    public OverworldSpace currentPlayerSpace = null;
    [HideInInspector]
    public EnemyData currentEnemyData;
    public InteractOverlayManager interactOverlayManager;
    public DialogueManager dialogueManager;


    void Start(){
        //print(StaticVariables.hasTalkedToNewestEnemy);
        //print("current progress - " + StaticVariables.currentBattleWorld + ":" + StaticVariables.currentBattleLevel);
        //print("highest progress - " + StaticVariables.highestUnlockedWorld + ":" + StaticVariables.highestUnlockedLevel);
        SetupOverworldSpaces();
        ShowProgress();
        PlacePlayerAtPosition(StaticVariables.currentBattleLevel);
        AdvanceGameIfAppropriate();
        ClearCurrentBattleStats();
        interactOverlayManager.Setup();
    }

    private void SetupOverworldSpaces(){
        for (int i = 0; i < overworldSpaces.Length; i++){
            OverworldSpace space = overworldSpaces[i];
            space.overworldSceneManager = this;
            space.playerDestination.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void AdvanceGameIfAppropriate(){
        if ((thisWorldNum == StaticVariables.currentBattleWorld) && (thisWorldNum == StaticVariables.highestUnlockedWorld)){
            if (StaticVariables.highestUnlockedLevel == StaticVariables.currentBattleLevel){
                if (StaticVariables.beatCurrentBattle){
                    StaticVariables.hasTalkedToNewestEnemy = false;
                    UnlockNextEnemy();
                    AdvanceGameProgress();
                }
            }
        }
    }

    public void MovePlayerToPosition(GameObject destination){
        Vector2 vectorToMove = destination.transform.position - playerParent.position;
        float distanceToMove = vectorToMove.magnitude;
        float timeToMove = distanceToMove / playerWalkSpeed;
        if (timeToMove < minTimeToMove)
            timeToMove = minTimeToMove;

        playerParent.DOMove(destination.transform.position, timeToMove).OnComplete(EndPlayerWalk);
        playerAnimator.SetTrigger("WalkStart");
        isPlayerMoving = true;
        if (destination.transform.position.x < playerParent.position.x){
            Vector3 s = playerParent.localScale;
            s.x = -1;
            playerParent.localScale = s;
        }
    }


    private void PlacePlayerAtPosition(int battleNum){
        if (battleNum == 0)
            return;
        int index = battleNum -1;
        OverworldSpace space = overworldSpaces[index];
        GameObject newSpot = space.playerDestination;
        playerParent.transform.position = newSpot.transform.position;
        currentPlayerSpace = space;
        currentEnemyData = space.battleData.enemyPrefab.GetComponent<EnemyData>();
    }

    private void EndPlayerWalk(){
        playerAnimator.SetTrigger("WalkEnd");
        isPlayerMoving = false;
        Vector3 s = playerParent.localScale;
        s.x = 1;
        playerParent.localScale = s;

        interactOverlayManager.ShowInteractOverlay();
        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Cutscene)
            return;
        if ((IsCurrentEnemyNewestEnemy()) && (!StaticVariables.hasTalkedToNewestEnemy)){
            if ((currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Battle) || (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial))
                dialogueManager.Setup(currentEnemyData.overworldDialogueSteps, currentPlayerSpace.battleData);

        }
    }

    private bool IsCurrentEnemyNewestEnemy(){
        if (thisWorldNum == StaticVariables.highestUnlockedWorld){
            if (StaticVariables.highestUnlockedLevel == GetLevelNumOfSpace(currentPlayerSpace)){
                return true;
            }
        }
        return false;
    }

    public void LoadBattleWithData(OverworldSpace space){
        StaticVariables.battleData = space.battleData;
        SetCurrentBattleData(space);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.battleSceneName);
    }

    private void ShowProgress(){
        if (thisWorldNum < StaticVariables.highestUnlockedWorld)
            return; 
        for (int i = StaticVariables.highestUnlockedLevel; i < overworldSpaces.Length; i++){
            overworldSpaces[i].gameObject.SetActive(false);
        }
    }

    private void UnlockNextEnemy(){
        OverworldSpace nextSpace = GetFirstLockedEnemySpace();
        if (nextSpace != null){
            nextSpace.gameObject.SetActive(true);
            nextSpace.FadeInVisuals();
        }
    }

    private OverworldSpace GetFirstLockedEnemySpace(){
        for (int i = 0; i < overworldSpaces.Length; i++){
            OverworldSpace space = overworldSpaces[i];
            if (!space.gameObject.activeSelf)
                return space;
        }
        return null;
    }

    private void SetCurrentBattleData(OverworldSpace space){
        int worldNum = thisWorldNum;
        int levelNum = GetLevelNumOfSpace(space);
        StaticVariables.currentBattleWorld = worldNum;
        StaticVariables.currentBattleLevel = levelNum;
        StaticVariables.beatCurrentBattle = false;
    }

    private int GetLevelNumOfSpace(OverworldSpace space){
        int i = 0;
        foreach (OverworldSpace s in overworldSpaces){
            i++;
            if (s == space)
                return i;
        }
        return -1;
    }

    private void AdvanceGameProgress(){
        StaticVariables.highestUnlockedLevel ++;
        if (StaticVariables.highestUnlockedLevel > overworldSpaces.Length){
            StaticVariables.highestUnlockedWorld ++;
            StaticVariables.highestUnlockedLevel = 1;
            if (StaticVariables.highestUnlockedWorld > 6)
                StaticVariables.highestUnlockedWorld = 6;
        }

        //print(StaticVariables.hasTalkedToNewestEnemy);
        //print("current progress - " + StaticVariables.currentBattleWorld + ":" + StaticVariables.currentBattleLevel);
        //print("highest progress - " + StaticVariables.highestUnlockedWorld + ":" + StaticVariables.highestUnlockedLevel);
    }

    private void ClearCurrentBattleStats(){
        StaticVariables.currentBattleLevel = 0;
        StaticVariables.currentBattleWorld = 0;
        StaticVariables.beatCurrentBattle = false;
    }

    public void StartBattle(){
        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Battle)
            LoadBattleWithData(currentPlayerSpace);
        else if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial){
        StaticVariables.battleData = currentPlayerSpace.battleData;
            SetCurrentBattleData(currentPlayerSpace);
            StaticVariables.FadeOutThenLoadScene("Tutorial");
        }
    }

    public void StartCutscene(){
        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Cutscene){
            StaticVariables.cutsceneID = currentPlayerSpace.cutsceneID;
            StaticVariables.FadeOutThenLoadScene("Cutscene");
        }
    }

    public void BackToMap(){
        StaticVariables.FadeOutThenLoadScene("Map Scene");
    }

    public void FinishedTalking(){
        if (IsCurrentEnemyNewestEnemy())
            StaticVariables.hasTalkedToNewestEnemy = true;
    }

}

