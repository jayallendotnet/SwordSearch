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
    private List<GameObject> stepsToNextSpace;
    private bool changePlayerDirectionAtNextStep = false;


    void Start(){
        //print(StaticVariables.hasTalkedToNewestEnemy);
        //print("current progress - " + StaticVariables.currentBattleWorld + ":" + StaticVariables.currentBattleLevel);
        //print("highest progress - " + StaticVariables.highestUnlockedWorld + ":" + StaticVariables.highestUnlockedLevel);
        interactOverlayManager.gameObject.SetActive(true);
        dialogueManager.gameObject.SetActive(true);
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
            //space.playerDestination.transform.GetChild(0).gameObject.SetActive(false);
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

    public void StartMovingPlayerToSpace(OverworldSpace space){
        bool reverse = false;
        int currentSpaceIndex = 0;
        int destinationSpaceIndex = 0;
        for (int i = 0; i < overworldSpaces.Length; i++){
            if (overworldSpaces[i] == currentPlayerSpace)
                currentSpaceIndex = i;
            if (overworldSpaces[i] == space)
                destinationSpaceIndex = i;
        }
        int earlierIndex = currentSpaceIndex;
        int laterIndex = destinationSpaceIndex;
        if (destinationSpaceIndex < currentSpaceIndex){
            earlierIndex = destinationSpaceIndex;
            laterIndex = currentSpaceIndex;
            reverse = true;
        }

        //print("earlierIndex[" + earlierIndex + "], laterIndex[" + laterIndex + "]");

        stepsToNextSpace = new List<GameObject>();
        for (int i = 0; i < overworldSpaces.Length; i++){
            if (i == earlierIndex)
                stepsToNextSpace.Add(overworldSpaces[i].playerDestination);
            if ((i > earlierIndex) && (i <= laterIndex)){
                foreach (Transform t in overworldSpaces[i].pathFromLastSpace)
                    stepsToNextSpace.Add(t.gameObject);
                stepsToNextSpace.Add(overworldSpaces[i].playerDestination);
            }
        }

        if (reverse)
            stepsToNextSpace.Reverse();

        stepsToNextSpace.RemoveAt(0);

        playerAnimator.SetTrigger("WalkStart");
        isPlayerMoving = true;
        currentPlayerSpace.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        MovePlayerToNextStep();
        PointPlayerTowardNextEnemy();

        //print(stepsToNextSpace.Count);
    }

    private void MovePlayerToNextStep(){
        if (stepsToNextSpace.Count == 0){
            EndPlayerWalk();
            return;
        }
        GameObject space = stepsToNextSpace[0];
        stepsToNextSpace.RemoveAt(0);

        playerParent.DOMove(space.transform.position, 0.3f).OnComplete(MovePlayerToNextStep);

        if (changePlayerDirectionAtNextStep){
            PointPlayerTowardNextEnemy();
            changePlayerDirectionAtNextStep = false;
        }

        if (space.transform.parent.GetComponent<OverworldSpace>() != null)
            changePlayerDirectionAtNextStep = true;
    }

    private void PointPlayerTowardNextEnemy(){
        foreach (GameObject space in stepsToNextSpace){
            if (space.transform.parent.GetComponent<OverworldSpace>() != null){
                Vector3 s = playerParent.localScale;
                s.x = 1;
                if (space.transform.position.x < playerParent.position.x)
                    s.x = -1;
                playerParent.localScale = s;
                return;
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
            battleNum = 1;
        int index = battleNum -1;
        OverworldSpace space = overworldSpaces[index];
        GameObject newSpot = space.playerDestination;
        playerParent.transform.position = newSpot.transform.position;
        currentPlayerSpace = space;
        currentEnemyData = space.battleData.enemyPrefab.GetComponent<EnemyData>();
        currentPlayerSpace.transform.GetChild(2).Find("Overworld Player Space Icon").gameObject.SetActive(false);
    }

    private void EndPlayerWalk(){
        playerAnimator.SetTrigger("WalkEnd");
        isPlayerMoving = false;
        currentPlayerSpace.transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
        Vector3 s = playerParent.localScale;
        s.x = 1;
        playerParent.localScale = s;

        interactOverlayManager.ShowInteractOverlay();
        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Cutscene)
            return;    
        EnemyData ed = currentPlayerSpace.battleData.enemyPrefab.GetComponent<EnemyData>();
        interactOverlayManager.DisplayEnemyName(ed);
        interactOverlayManager.DisplayInfoHighlightIfAppropriate(ed);
        if ((IsCurrentEnemyNewestEnemy()) && (!StaticVariables.hasTalkedToNewestEnemy)){
            if ((currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Battle) || (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial))
                dialogueManager.Setup(currentEnemyData.overworldDialogueSteps, currentPlayerSpace.battleData, showFakeTalkButton: (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial));

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

