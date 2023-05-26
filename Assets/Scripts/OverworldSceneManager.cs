using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OverworldSceneManager : MonoBehaviour{

    public RectTransform playerParent;
    public Animator playerAnimator;
    public float playerWalkSpeed = 500f;
    public float minTimeToMove = 1f;
    public GeneralSceneManager generalSceneManager;
    public int thisWorldNum;

    [HideInInspector]
    public bool isPlayerMoving = false;
    [HideInInspector]
    public OverworldEnemySpace currentPlayerSpace = null;
    public OverworldEnemySpace[] overworldEnemySpaces;
    public RectTransform interactOverlay;
    public RectTransform overworldView;
    [HideInInspector]
    public bool isInteractOverlayShowing = false;
    public float interactOverlayMoveDuration = 3f;

    public RectTransform talkButton;
    public RectTransform battleButton;
    public RectTransform infoButton;
    public RectTransform backButton;

    [HideInInspector]
    public bool isInfoShowing = false;

    private Vector2 talkButtonStartingPos;
    private Vector2 battleButtonStartingPos;
    private Vector2 infoButtonStartingPos;
    private Vector2 backButtonStartingPos;

    public Text infoText;
    public float infoTransitionDuration = 0.5f;
    [HideInInspector]
    public bool isTalkShowing = false;
    public float talkTransitionDuration = 0.5f;

    


    void Start(){
        SetupOverworldEnemySpaces();
        ShowProgress();
        PlacePlayerAtPosition(StaticVariables.currentBattleLevel);
        AdvanceGameIfAppropriate();
        ClearCurrentBattleStats();
        StartInteractOverlayHidden();
        SetButtonStartingPositions();
        infoText.gameObject.SetActive(false);
    }

    private void SetButtonStartingPositions(){
        talkButtonStartingPos = talkButton.localPosition;
        battleButtonStartingPos = battleButton.localPosition;
        infoButtonStartingPos = infoButton.localPosition;
        backButtonStartingPos = backButton.localPosition;
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
    }

    private void EndPlayerWalk(){
        playerAnimator.SetTrigger("WalkEnd");
        isPlayerMoving = false;
        Vector3 s = playerParent.localScale;
        s.x = 1;
        playerParent.localScale = s;
        ShowInteractOverlay();
    }

    private void StartInteractOverlayHidden(){
        Vector2 pos = new Vector2(0, -interactOverlay.rect.height);
        interactOverlay.anchoredPosition = pos;
    }

    public void PressedBackInteractButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing){     
            talkButton.DOLocalMove(talkButtonStartingPos, infoTransitionDuration);
            battleButton.DOLocalMove(battleButtonStartingPos, infoTransitionDuration);
            infoButton.DOLocalMove(infoButtonStartingPos, infoTransitionDuration).OnComplete(FinishedShowingInfo);
            Color c = Color.white;
            c.a = 0;
            infoText.DOColor(c, infoTransitionDuration);
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    public void PressedTalkButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        isTalkShowing = true;
        talkButton.DOLocalMove(-infoButtonStartingPos, talkTransitionDuration);
        battleButton.DOLocalMove(-infoButtonStartingPos, talkTransitionDuration);
        infoButton.DOLocalMove(-infoButtonStartingPos, talkTransitionDuration);
        backButton.DOLocalMove(-infoButtonStartingPos, talkTransitionDuration);

        StaticVariables.WaitTimeThenCallFunction(5f, EndTalk);
    }

    private void EndTalk(){
        talkButton.DOLocalMove(talkButtonStartingPos, talkTransitionDuration);
        battleButton.DOLocalMove(battleButtonStartingPos, talkTransitionDuration);
        infoButton.DOLocalMove(infoButtonStartingPos, talkTransitionDuration);
        backButton.DOLocalMove(backButtonStartingPos, talkTransitionDuration).OnComplete(FinishedEndingTalk);
    }

    private void FinishedEndingTalk(){
        isTalkShowing = false;
    }

    private void FinishedShowingInfo(){
        isInfoShowing = false;
        infoText.gameObject.SetActive(false);
    }

    public void PressedBattleButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        LoadBattleWithData(currentPlayerSpace);
    }

    public void PressedInfoButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        talkButton.DOMove(backButton.position, infoTransitionDuration);
        battleButton.DOMove(backButton.position, infoTransitionDuration);
        infoButton.DOMove(backButton.position, infoTransitionDuration);
        infoText.gameObject.SetActive(true);
        Color c = Color.white;
        c.a = 0;
        infoText.color = c;
        infoText.DOColor(Color.white, infoTransitionDuration);
        infoText.text = GenerateEnemyInfoText(currentPlayerSpace.battleData.enemyPrefab.GetComponent<EnemyData>());
        isInfoShowing = true;
    }

    private string GenerateEnemyInfoText(EnemyData enemy){
        //to do: check for powerup unlocks before adding specific text
        string text = "";
        if (enemy.isHorde){
            text += "Horde enemies take more burn damage from fire spells.\n";
            text += "Horde enemies are stunned for less time from lightning spells.\n";
        }
        if (enemy.isDraconic){
            text += "This dragon takes more damage from the sword of slaying.\n";
        }
        if (enemy.isInHolyArea){
            text += "The holy ground increases healing spell effectiveness.\n";
            text += "The holy ground diminishes the power of darkness.\n";
        }
        if (text == ""){
            text = "This enemy has no particular weaknesses.";
        }
        return text; 
    }

    private void HideInteractOverlay(){
        Vector2 pos = new Vector2(0, -interactOverlay.rect.height);
        interactOverlay.DOAnchorPos(pos, interactOverlayMoveDuration);
        overworldView.DOAnchorPos(Vector2.zero, interactOverlayMoveDuration).OnComplete(FinishedHidingInteractOverlay);
    }

    private void FinishedHidingInteractOverlay(){
        isInteractOverlayShowing = false;
    }

    public void ShowInteractOverlay(){
        interactOverlay.DOAnchorPos(Vector2.zero, interactOverlayMoveDuration);
        isInteractOverlayShowing = true;

        float playerHasToBeAbove = interactOverlay.rect.height + 200;
        float diff = playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldView.DOAnchorPos(new Vector2(0, -diff), interactOverlayMoveDuration);
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
        int levelNum = 0;
        int i  = 0;
        while ((levelNum == 0) && (i < overworldEnemySpaces.Length)){
            if (space == overworldEnemySpaces[i])
                levelNum = i + 1;
            i++;
        }
        StaticVariables.currentBattleWorld = worldNum;
        StaticVariables.currentBattleLevel = levelNum;
        StaticVariables.beatCurrentBattle = false;
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

}
