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
    private Vector2 interactOverlayStartingSize;

    public float minHeightAboveInteractOverlay = 200;

    


    void Start(){
        SetupOverworldEnemySpaces();
        ShowProgress();
        PlacePlayerAtPosition(StaticVariables.currentBattleLevel);
        AdvanceGameIfAppropriate();
        ClearCurrentBattleStats();
        StartInteractOverlayHidden();
        SetOverlayStartingDimensions();
        infoText.gameObject.SetActive(false);
    }

    private void SetOverlayStartingDimensions(){
        talkButtonStartingPos = talkButton.localPosition;
        battleButtonStartingPos = battleButton.localPosition;
        infoButtonStartingPos = infoButton.localPosition;
        backButtonStartingPos = backButton.localPosition;
        interactOverlayStartingSize = interactOverlay.sizeDelta;
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
            AdjustHeightsForHidingInfo();
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    private void AdjustHeightsForHidingInfo(){
        //no idea how this function works lol
        interactOverlay.DOSizeDelta(interactOverlayStartingSize, interactOverlayMoveDuration);
        float diff = interactOverlayStartingSize.y - interactOverlay.sizeDelta.y;
        float temp = overworldView.anchoredPosition.y + diff;
        if (temp < 0)
            temp = 0;
        if (diff < 0)
            overworldView.DOAnchorPosY(temp, interactOverlayMoveDuration);
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
        InfoTextData infoTextData = GenerateEnemyInfoText(currentPlayerSpace.battleData.enemyPrefab.GetComponent<EnemyData>());
        infoText.text = infoTextData.text;
        AdjustHeightsForShowingInfo(infoTextData.lineCount);
        isInfoShowing = true;
    }

    private void AdjustHeightsForShowingInfo(int lineCount){
        if (lineCount < 3)
            return;
        int extraAmount = lineCount - 2;
        if (extraAmount > 3){
            extraAmount = 3;
            //change font size to fit more spaces?
        }
        float heightPerAmount = 450f;
        float totalHeightAdded = extraAmount * heightPerAmount;
        Vector2 sd = new Vector2(interactOverlay.sizeDelta.x, interactOverlay.sizeDelta.y + totalHeightAdded);
        interactOverlay.DOSizeDelta(sd, interactOverlayMoveDuration);
        
        float playerHasToBeAbove = interactOverlay.rect.height + totalHeightAdded + minHeightAboveInteractOverlay;
        float diff = playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldView.DOAnchorPosY(overworldView.anchoredPosition.y -diff, interactOverlayMoveDuration);
    }

    private InfoTextData GenerateEnemyInfoText(EnemyData enemy){
        InfoTextData infoText = new InfoTextData();
        int lineCount = 0;
        string text = "";
        if (enemy.isHorde){
            if (StaticVariables.fireActive){
                text += "Horde enemies take more burn damage from fire spells.\n\n";
                lineCount ++;
            }
            if (StaticVariables.lightningActive){
                text += "Horde enemies are stunned for less time from lightning spells.\n\n";
                lineCount ++;
            }
        }
        if (enemy.isDraconic){
            if (StaticVariables.swordActive){
                text += "This dragon takes more damage from the sword of slaying.\n\n";
                lineCount ++;
            }
        }
        if (enemy.isInHolyArea){
            if (StaticVariables.healActive){
                text += "The holy ground increases healing spell effectiveness.\n\n";
                lineCount ++;
            }
            if (StaticVariables.darkActive){
                text += "The holy ground diminishes the power of darkness.\n\n";
                lineCount ++;
            }
        }
        if (text == ""){
            text = "This enemy has no particular weaknesses.";
            lineCount ++;
        }
        infoText.text = text;
        infoText.lineCount = lineCount;
        return infoText; 
    }

    private void HideInteractOverlay(){
        //Vector2 pos = new Vector2(0, -interactOverlay.rect.height);
        interactOverlay.DOAnchorPosY(-interactOverlay.rect.height, interactOverlayMoveDuration);
        overworldView.DOAnchorPosY(0, interactOverlayMoveDuration).OnComplete(FinishedHidingInteractOverlay);
    }

    private void FinishedHidingInteractOverlay(){
        isInteractOverlayShowing = false;
    }

    public void ShowInteractOverlay(){
        interactOverlay.DOAnchorPosY(0, interactOverlayMoveDuration);
        isInteractOverlayShowing = true;

        float playerHasToBeAbove = interactOverlay.rect.height + minHeightAboveInteractOverlay;
        float diff = playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldView.DOAnchorPosY(-diff, interactOverlayMoveDuration);
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

public class InfoTextData{
    public string text;
    public int lineCount;
}
