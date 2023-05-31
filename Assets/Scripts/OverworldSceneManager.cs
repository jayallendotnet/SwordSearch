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
    public OverworldEnemySpace[] overworldEnemySpaces;


    [Header("Timing Configurations")]
    public float playerWalkSpeed = 500f;
    public float minTimeToMove = 1f;

    [Header("Other")]
    public int thisWorldNum;
    

    [HideInInspector]
    public bool isPlayerMoving = false;
    [HideInInspector]
    public OverworldEnemySpace currentPlayerSpace = null;
    [HideInInspector]
    public EnemyData currentEnemyData;
    public InteractOverlayManager interactOverlayManager;
    public DialogueManager dialogueManager;


    void Start(){
        SetupOverworldEnemySpaces();
        ShowProgress();
        PlacePlayerAtPosition(StaticVariables.currentBattleLevel);
        AdvanceGameIfAppropriate();
        ClearCurrentBattleStats();
        interactOverlayManager.Setup();
    }

    private void SetupOverworldEnemySpaces(){
        for (int i = 0; i < overworldEnemySpaces.Length; i++){
            OverworldEnemySpace space = overworldEnemySpaces[i];
            space.overworldSceneManager = this;
            space.playerDestination.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    private void AdvanceGameIfAppropriate(){
        if ((thisWorldNum == StaticVariables.currentBattleWorld) && (thisWorldNum == StaticVariables.highestUnlockedWorld)){
            if (StaticVariables.highestUnlockedLevel == StaticVariables.currentBattleLevel){
                if (StaticVariables.beatCurrentBattle){
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
        OverworldEnemySpace space = overworldEnemySpaces[index];
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
        if ((IsCurrentEnemyNewestEnemy()) && (!StaticVariables.hasTalkedToNewestEnemy))
            dialogueManager.Setup(currentEnemyData.overworldDialogueSteps, currentPlayerSpace.battleData);
    }

    private bool IsCurrentEnemyNewestEnemy(){
        if (thisWorldNum == StaticVariables.highestUnlockedWorld){
            if (StaticVariables.highestUnlockedLevel == GetLevelNumOfSpace(currentPlayerSpace)){
                return true;
            }
        }
        return false;
    }

    public void LoadBattleWithData(OverworldEnemySpace space){
        StaticVariables.battleData = space.battleData;
        SetCurrentBattleData(space);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.battleSceneName);
    }

    private void ShowProgress(){
        if (thisWorldNum < StaticVariables.highestUnlockedWorld)
            return; 
        for (int i = StaticVariables.highestUnlockedLevel; i < overworldEnemySpaces.Length; i++){
            overworldEnemySpaces[i].gameObject.SetActive(false);
        }
    }

    private void UnlockNextEnemy(){
        OverworldEnemySpace nextSpace = GetFirstLockedEnemySpace();
        if (nextSpace != null){
            nextSpace.gameObject.SetActive(true);
            nextSpace.FadeInVisuals();
        }
    }

    private OverworldEnemySpace GetFirstLockedEnemySpace(){
        for (int i = 0; i < overworldEnemySpaces.Length; i++){
            OverworldEnemySpace space = overworldEnemySpaces[i];
            if (!space.gameObject.activeSelf)
                return space;
        }
        return null;
    }

    private void SetCurrentBattleData(OverworldEnemySpace space){
        int worldNum = thisWorldNum;
        int levelNum = GetLevelNumOfSpace(space);
        StaticVariables.currentBattleWorld = worldNum;
        StaticVariables.currentBattleLevel = levelNum;
        StaticVariables.beatCurrentBattle = false;
    }

    private int GetLevelNumOfSpace(OverworldEnemySpace space){
        int i = 0;
        foreach (OverworldEnemySpace s in overworldEnemySpaces){
            i++;
            if (s == space)
                return i;
        }
        return -1;
    }

    private void AdvanceGameProgress(){
        StaticVariables.highestUnlockedLevel ++;
        if (StaticVariables.highestUnlockedLevel > overworldEnemySpaces.Length){
            StaticVariables.highestUnlockedWorld ++;
            StaticVariables.highestUnlockedLevel = 1;
            if (StaticVariables.highestUnlockedWorld > 6)
                StaticVariables.highestUnlockedWorld = 6;
        }
    }

    private void ClearCurrentBattleStats(){
        StaticVariables.currentBattleLevel = 0;
        StaticVariables.currentBattleWorld = 0;
        StaticVariables.beatCurrentBattle = false;
    }

    public void StartBattle(){
        LoadBattleWithData(currentPlayerSpace);
    }

}

