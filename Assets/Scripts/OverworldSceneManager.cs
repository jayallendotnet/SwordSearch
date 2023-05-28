using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class OverworldSceneManager : MonoBehaviour{

    [Header("Scene References")]
    public GeneralSceneManager generalSceneManager;
    public RectTransform interactOverlay;
    public RectTransform overworldView;
    public RectTransform playerParent;
    public Animator playerAnimator;
    public RectTransform battleButton;
    public RectTransform talkButton;
    public Text talkText;
    public Text talkTextSpeakerName;
    public Image talkDarkenedBackground;
    public Image talkPlayerChathead;
    public Image talkEnemyChathead;
    public RectTransform infoButton;
    public Text infoText;
    public RectTransform backButton;
    public Text backButtonText;
    public OverworldEnemySpace[] overworldEnemySpaces;


    [Header("Timing Configurations")]
    public float playerWalkSpeed = 500f;
    public float minTimeToMove = 1f;
    public float interactOverlayMoveDuration = 3f;
    public float infoTransitionDuration = 0.5f;
    public float talkTransitionDuration = 0.5f;

    [Header("Other")]
    public int thisWorldNum;
    public float minHeightAboveInteractOverlay = 200;



    [HideInInspector]
    public bool isPlayerMoving = false;
    [HideInInspector]
    public OverworldEnemySpace currentPlayerSpace = null;
    [HideInInspector]
    public bool isInteractOverlayShowing = false;
    [HideInInspector]
    public bool isInfoShowing = false;
    private Vector2 talkButtonStartingPos;
    private Vector2 battleButtonStartingPos;
    private Vector2 infoButtonStartingPos;
    private Vector2 backButtonStartingPos;
    [HideInInspector]
    public bool isTalkShowing = false;
    private Vector2 interactOverlayStartingSize;
    private int currentTalkStep;
    private Image textSeparator;
    private Color textSeparatorColor;
    private Color talkDarkenedBackgroundColor;
    private float chatheadHiddenDepth = 200f;
    private float cahtheadStartingHeight;
    private RectTransform talkPlayerChatheadTransform;
    private RectTransform talkEnemyChatheadTransform;


    void Start(){
        SetupOverworldEnemySpaces();
        ShowProgress();
        PlacePlayerAtPosition(StaticVariables.currentBattleLevel);
        AdvanceGameIfAppropriate();
        ClearCurrentBattleStats();
        StartInteractOverlayHidden();
        SetOverlayStartingDimensions();
        infoText.gameObject.SetActive(false);
        talkText.gameObject.SetActive(false);
        talkTextSpeakerName.gameObject.SetActive(false);
        textSeparator = talkTextSpeakerName.transform.GetChild(0).GetComponent<Image>();
        textSeparatorColor = textSeparator.color;
        talkDarkenedBackground.gameObject.SetActive(false);
        talkPlayerChathead.gameObject.SetActive(false);
        talkEnemyChathead.gameObject.SetActive(false);
        talkDarkenedBackgroundColor = talkDarkenedBackground.color;
        talkPlayerChatheadTransform = talkPlayerChathead.GetComponent<RectTransform>();
        talkEnemyChatheadTransform = talkEnemyChathead.GetComponent<RectTransform>();
        cahtheadStartingHeight = talkPlayerChatheadTransform.anchoredPosition.y;
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
    
    public void PressedBattleButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        LoadBattleWithData(currentPlayerSpace);
    }
   
    public void PressedTalkButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        isTalkShowing = true;
        HideOtherButtonsBehindBack(talkTransitionDuration);
        backButtonText.text = "NEXT";

        TransitionToTalk();

        currentTalkStep = 0;
        ShowCurrentTalkStage();
    }

    private void TransitionToTalk(){
        talkText.gameObject.SetActive(true);
        talkTextSpeakerName.gameObject.SetActive(true);
        talkDarkenedBackground.gameObject.SetActive(true);
        talkPlayerChathead.gameObject.SetActive(true);
        talkEnemyChathead.gameObject.SetActive(true);

        talkPlayerChatheadTransform.anchoredPosition = new Vector2(talkPlayerChatheadTransform.anchoredPosition.x, -chatheadHiddenDepth);
        talkEnemyChatheadTransform.anchoredPosition = new Vector2(talkEnemyChatheadTransform.anchoredPosition.x, -chatheadHiddenDepth);

        Color c = Color.white;
        c.a = 0;
        talkText.color = c;
        talkTextSpeakerName.color = c;
        textSeparator.color = c;
        talkDarkenedBackground.color = c;
        talkText.DOColor(Color.white, talkTransitionDuration);
        talkTextSpeakerName.DOColor(Color.white, talkTransitionDuration);
        textSeparator.DOColor(textSeparatorColor, talkTransitionDuration);
        talkDarkenedBackground.DOColor(talkDarkenedBackgroundColor, talkTransitionDuration);
    }

    public void PressedInfoButton(){
        if (isTalkShowing)
            return;
        if (isInfoShowing)
            return;
        HideOtherButtonsBehindBack(infoTransitionDuration);
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

    public void PressedBackInteractButton(){
        if (isTalkShowing){
            AdvanceTalkStage();
            return;
        }
        if (isInfoShowing){   
            ReturnButtonsToStartingPositions(infoTransitionDuration);  
            StaticVariables.WaitTimeThenCallFunction(infoTransitionDuration, FinishedShowingInfo);
            Color c = Color.white;
            c.a = 0;
            infoText.DOColor(c, infoTransitionDuration);
            AdjustHeightsForHidingInfo();
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    private void ShowCurrentTalkStage(){
        if (currentTalkStep < currentPlayerSpace.dialogueSteps.Length){
            talkText.text = currentPlayerSpace.dialogueSteps[currentTalkStep].description;
            if (currentPlayerSpace.dialogueSteps[currentTalkStep].type == DialogueStep.DialogueType.PlayerTalking)
                ShowPlayerTalking();
            else if (currentPlayerSpace.dialogueSteps[currentTalkStep].type == DialogueStep.DialogueType.EnemyTalking)
                ShowEnemyTalking();
        }
        else{
            talkText.text = "No dialogue for this enemy, current talk step is " + currentTalkStep;
            talkTextSpeakerName.text = "WARNING";
        }
        if (currentTalkStep == currentPlayerSpace.dialogueSteps.Length - 1)
            backButtonText.text = "END";
        else
            backButtonText.text = "NEXT";
    }

    private void ShowPlayerTalking(){
        talkTextSpeakerName.text = "PLAYER";
        talkTextSpeakerName.alignment = TextAnchor.UpperLeft;
        talkPlayerChatheadTransform.DOAnchorPosY(cahtheadStartingHeight, talkTransitionDuration);
        talkPlayerChathead.DOColor(Color.white, talkTransitionDuration);
        talkEnemyChathead.DOColor(Color.grey, talkTransitionDuration);

    }

    private void ShowEnemyTalking(){
        talkTextSpeakerName.text = currentPlayerSpace.battleData.enemyPrefab.name.ToUpper();
        talkTextSpeakerName.alignment = TextAnchor.UpperRight;
        talkEnemyChatheadTransform.DOAnchorPosY(cahtheadStartingHeight, talkTransitionDuration);
        talkEnemyChathead.DOColor(Color.white, talkTransitionDuration);
        talkPlayerChathead.DOColor(Color.grey, talkTransitionDuration);
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

    private void AdvanceTalkStage(){
        currentTalkStep ++;
        if (currentTalkStep >= currentPlayerSpace.dialogueSteps.Length){
            EndTalk();
            return;
        }
        ShowCurrentTalkStage();
    }

    private void EndTalk(){
        ReturnButtonsToStartingPositions(talkTransitionDuration);
        StaticVariables.WaitTimeThenCallFunction(talkTransitionDuration, FinishedEndingTalk);

        backButtonText.text = "BACK";
        Color c = Color.white;
        c.a = 0;
        talkText.DOColor(c, talkTransitionDuration);
        talkTextSpeakerName.DOColor(c, talkTransitionDuration);
        textSeparator.DOColor(c, talkTransitionDuration);
        talkDarkenedBackground.DOColor(c, talkTransitionDuration);

        talkPlayerChatheadTransform.DOAnchorPosY(-chatheadHiddenDepth, talkTransitionDuration);
        talkEnemyChatheadTransform.DOAnchorPosY(-chatheadHiddenDepth, talkTransitionDuration);
    }

    private void HideOtherButtonsBehindBack(float duration){
        talkButton.DOMove(backButton.position, duration);
        battleButton.DOMove(backButton.position, duration);
        infoButton.DOMove(backButton.position, duration);

    }
    private void ReturnButtonsToStartingPositions(float duration){
        talkButton.DOLocalMove(talkButtonStartingPos, duration);
        battleButton.DOLocalMove(battleButtonStartingPos, duration);
        infoButton.DOLocalMove(infoButtonStartingPos, duration);
        backButton.DOLocalMove(backButtonStartingPos, duration);
    }

    private void FinishedEndingTalk(){
        isTalkShowing = false;
        talkText.gameObject.SetActive(false);
        talkTextSpeakerName.gameObject.SetActive(false);
        talkDarkenedBackground.gameObject.SetActive(false);
        talkPlayerChathead.gameObject.SetActive(false);
        talkEnemyChathead.gameObject.SetActive(false);
    }

    private void FinishedShowingInfo(){
        isInfoShowing = false;
        infoText.gameObject.SetActive(false);
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
        if (enemy.isHoly){
            if (StaticVariables.healActive){
                text += "This creature's holy aura amplifies healing magic.\n\n";
                lineCount ++;
            }
            if (StaticVariables.darkActive){
                text += "This creature's holy aura diminishes the power of darkness.\n\n";
                lineCount ++;
            }
        }        
        if (enemy.isDark){
            if (StaticVariables.healActive){
                text += "This creature's dark aura dampens healing magic.\n\n";
                lineCount ++;
            }
            if (StaticVariables.darkActive){
                text += "This creature's dark aura bolsters the power of darkness.\n\n";
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
