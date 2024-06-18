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
    public RectTransform sceneHeader;


    [Header("Timing Configurations")]
    public float playerWalkSpeed = 500f;
    public float minTimeToMove = 1f;

    [Header("Overworld Settings")]
    public int thisWorldNum; // 0 means this is the atlas/map scene
    public int powerupsPerPuzzle = 3;
    public bool healActive = false;
    public bool waterActive = false;
    public bool fireActive = false;
    public bool earthActive = false;
    public bool lightningActive = false;
    public bool darkActive = false;
    public bool swordActive = false;
    

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
        interactOverlayManager.gameObject.SetActive(true);
        dialogueManager.gameObject.SetActive(true);
        SetPowerupAvailability();
        SetupOverworldSpaces();
        if (thisWorldNum == 0){
            ShowAllWorldsProgress();
            PlacePlayerAtPosition(StaticVariables.lastVisitedStage.world);
        }
        else{
            ShowPartialWorldProgress();
            PlacePlayerAtPosition(StaticVariables.lastVisitedStage.stage);
            CheckIfCompletedStage();
        }
        interactOverlayManager.Setup();
    }

    private void CheckIfCompletedStage(){
        if (StaticVariables.lastVisitedStage == StaticVariables.highestBeatenStage.nextStage){
            if (StaticVariables.hasCompletedStage){
                StaticVariables.highestBeatenStage = StaticVariables.highestBeatenStage.nextStage;
                StaticVariables.hasTalkedToNewestEnemy = false;
                if (StaticVariables.highestBeatenStage.nextStage.world != thisWorldNum)
                    ImmediatelyStartNextWorld();
                else
                    UnlockNextEnemy();
            }
        }
        StaticVariables.hasCompletedStage = false;
    }

    private void ImmediatelyStartNextWorld(){
        StaticVariables.lastVisitedStage = StaticVariables.highestBeatenStage.nextStage;
        SceneManager.LoadScene(StaticVariables.lastVisitedStage.worldName);
    }

    private void SetPowerupAvailability(){
        StaticVariables.healActive = healActive;
        StaticVariables.waterActive = waterActive;
        StaticVariables.fireActive = fireActive;
        StaticVariables.earthActive = earthActive;
        StaticVariables.lightningActive = lightningActive;
        StaticVariables.darkActive = darkActive;
        StaticVariables.swordActive = swordActive;
        StaticVariables.powerupsPerPuzzle = powerupsPerPuzzle;
    }

    private void SetupOverworldSpaces(){
        for (int i = 0; i < overworldSpaces.Length; i++){
            OverworldSpace space = overworldSpaces[i];
            space.overworldSceneManager = this;
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
        //if (stepsToNextSpace.Count > 0) //do this part later
        playerAnimator.SetTrigger("WalkStart");
        isPlayerMoving = true;
        currentPlayerSpace.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        MovePlayerToNextStep();
        PointPlayerTowardNextEnemy();
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


    private void PlacePlayerAtPosition(int stageNum){
        if (stageNum == 0)
            stageNum = 1;
        int index = stageNum -1;
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

        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Atlas){
            StaticVariables.lastVisitedStage = StaticVariables.GetStage(currentPlayerSpace.worldNumber, 1);
            StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
            return;

            
        //StaticVariables.lastVisitedStage = StaticVariables.GetStage(worldNum, 1);
        //StaticVariables.FadeOutThenLoadScene(StaticVariables.lastVisitedStage.worldName);
        }

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
        if (thisWorldNum == StaticVariables.highestBeatenStage.nextStage.world){
            if (GetStageNumOfSpace(currentPlayerSpace) == StaticVariables.highestBeatenStage.nextStage.stage)
                return true;
        }
        return false;
    }

    public void LoadBattleWithData(OverworldSpace space){
        StaticVariables.battleData = space.battleData;
        SetLastWorldStageVisited(space);
        StaticVariables.FadeOutThenLoadScene(StaticVariables.battleSceneName);
    }

    private void ShowPartialWorldProgress(){
        if (thisWorldNum < StaticVariables.highestBeatenStage.nextStage.world)
            return;
        for (int i = StaticVariables.highestBeatenStage.nextStage.stage; i < overworldSpaces.Length; i++)
            overworldSpaces[i].gameObject.SetActive(false);
    }

    private void ShowAllWorldsProgress(){
        //only used in the atlas/map scene to show full world progress
        for (int i = StaticVariables.highestBeatenStage.nextStage.world; i < overworldSpaces.Length; i++)
            overworldSpaces[i].gameObject.SetActive(false);

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

    private void SetLastWorldStageVisited(OverworldSpace space){
        StaticVariables.lastVisitedStage = StaticVariables.GetStage(thisWorldNum, GetStageNumOfSpace(space));
    }

    private int GetStageNumOfSpace(OverworldSpace space){
        int i = 0;
        foreach (OverworldSpace s in overworldSpaces){
            i++;
            if (s == space)
                return i;
        }
        return -1;
    }

    public void StartBattle(){
        if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Battle)
            LoadBattleWithData(currentPlayerSpace);
        else if (currentPlayerSpace.type == OverworldSpace.OverworldSpaceType.Tutorial){
            StaticVariables.battleData = currentPlayerSpace.battleData;
            SetLastWorldStageVisited(currentPlayerSpace);
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
        StaticVariables.FadeOutThenLoadScene(StaticVariables.mapName);
    }

    public void BackToHomepage(){
        StaticVariables.FadeOutThenLoadScene(StaticVariables.mainMenuName);
    }

    public void FinishedTalking(){
        if (IsCurrentEnemyNewestEnemy())
            StaticVariables.hasTalkedToNewestEnemy = true;
    }

    public void HideSceneHeader(float duration){
        sceneHeader.DOAnchorPos((sceneHeader.anchoredPosition + new Vector2(0, 400)), duration).OnComplete(DisableSceneHeader);
    }

    public void ShowSceneHeader(float duration){
        sceneHeader.gameObject.SetActive(true);
        sceneHeader.DOAnchorPos((sceneHeader.anchoredPosition + new Vector2(0, -400)), duration);
    }

    private void DisableSceneHeader(){
        sceneHeader.gameObject.SetActive(false);
    }
}

