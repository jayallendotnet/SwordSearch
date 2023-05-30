using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class InteractOverlayManager : MonoBehaviour{

    
    [Header("Overworld Scene")]
    public OverworldSceneManager overworldSceneManager;

   
    [Header("Scene References")]
    public RectTransform interactOverlay;
    public RectTransform battleButton;
    public RectTransform talkButton;
    public RectTransform infoButton;
    public Text infoText;
    public RectTransform backButton;


    [Header("Configurations")]
    public float transitionDuration = 0.5f;
    public float minHeightAboveInteractOverlay = 200;


    [HideInInspector]
    public bool isInteractOverlayShowing = false;
    [HideInInspector]
    public bool isInfoShowing = false;
    private Vector2 talkButtonStartingPos;
    private Vector2 battleButtonStartingPos;
    private Vector2 infoButtonStartingPos;
    private Vector2 backButtonStartingPos;
    private Vector2 interactOverlayStartingSize;


    public void Setup(){
        StartInteractOverlayHidden();
        SetStartingValues();
    }
    
    private void StartInteractOverlayHidden(){
        Vector2 pos = new Vector2(0, -interactOverlay.rect.height);
        interactOverlay.anchoredPosition = pos;
    }    

    private void SetStartingValues(){
        talkButtonStartingPos = talkButton.localPosition;
        battleButtonStartingPos = battleButton.localPosition;
        infoButtonStartingPos = infoButton.localPosition;
        backButtonStartingPos = backButton.localPosition;
        interactOverlayStartingSize = interactOverlay.sizeDelta;
    }

    public void PressedBattleButton(){
        if (isInfoShowing)
            return;
        overworldSceneManager.StartBattle();
    }
   
    public void PressedTalkButton(){
        if (isInfoShowing)
            return;
        DialogueStep[] steps = overworldSceneManager.currentEnemyData.overworldDialogueSteps;
        BattleData bd = overworldSceneManager.currentPlayerSpace.battleData;
        overworldSceneManager.dialogueManager.Setup(steps, bd, true);
    }

    public void PressedInfoButton(){
        if (isInfoShowing)
            return;
        HideOtherButtonsBehindBack(transitionDuration);
        infoText.gameObject.SetActive(true);
        Color c = Color.white;
        c.a = 0;
        infoText.color = c;
        infoText.DOColor(Color.white, transitionDuration);
        InfoTextData infoTextData = GenerateEnemyInfoText(overworldSceneManager.currentEnemyData);
        infoText.text = infoTextData.text;
        AdjustHeightsForShowingInfo(infoTextData.lineCount);
        isInfoShowing = true;
    }

    private void MoveOverworldDownIfRequired(){
        //no idea how this function works lol
        interactOverlay.DOSizeDelta(interactOverlayStartingSize, transitionDuration);
        float diff = interactOverlayStartingSize.y - interactOverlay.sizeDelta.y;
        float temp = overworldSceneManager.overworldView.anchoredPosition.y + diff;
        if (temp < 0)
            temp = 0;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(temp, transitionDuration);
    }

    private void MoveOverworldUpIfRequired(){
        float playerHasToBeAbove = interactOverlay.rect.height + minHeightAboveInteractOverlay;
        float diff = overworldSceneManager.playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(-diff, transitionDuration);
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
        interactOverlay.DOSizeDelta(sd, transitionDuration);
        
        float playerHasToBeAbove = interactOverlay.rect.height + totalHeightAdded + minHeightAboveInteractOverlay;
        float diff = overworldSceneManager.playerParent.position.y - playerHasToBeAbove;
        if (diff < 0)
            overworldSceneManager.overworldView.DOAnchorPosY(overworldSceneManager.overworldView.anchoredPosition.y -diff, transitionDuration);
    }

    private InfoTextData GenerateEnemyInfoText(EnemyData enemy){
        //maybe move this into static variables at some point?
        //the infotextdata object definition would need to go somewhere else too
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

    public void PressedBackButton(){
        if (isInfoShowing){   
            ReturnButtonsToStartingPositions(transitionDuration);  
            StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedShowingInfo);
            Color c = Color.white;
            c.a = 0;
            infoText.DOColor(c, transitionDuration);
            MoveOverworldDownIfRequired();
            return;
        }
        if (isInteractOverlayShowing)
            HideInteractOverlay();
    }

    
    private void HideOtherButtonsBehindBack(float duration){
        talkButton.DOLocalMove(backButton.localPosition, duration);
        battleButton.DOLocalMove(backButton.localPosition, duration);
        infoButton.DOLocalMove(backButton.localPosition, duration);

    }
    private void ReturnButtonsToStartingPositions(float duration){
        talkButton.DOLocalMove(talkButtonStartingPos, duration);
        battleButton.DOLocalMove(battleButtonStartingPos, duration);
        infoButton.DOLocalMove(infoButtonStartingPos, duration);
        backButton.DOLocalMove(backButtonStartingPos, duration);
    }

    private void FinishedShowingInfo(){
        isInfoShowing = false;
        infoText.gameObject.SetActive(false);
    }

    private void HideInteractOverlay(){
        interactOverlay.DOAnchorPosY(-interactOverlay.rect.height, transitionDuration);
        overworldSceneManager.overworldView.DOAnchorPosY(0, transitionDuration).OnComplete(FinishedHidingInteractOverlay);
    }

    private void FinishedHidingInteractOverlay(){
        isInteractOverlayShowing = false;
    }

    public void ShowInteractOverlay(){
        interactOverlay.DOAnchorPosY(0, transitionDuration);
        isInteractOverlayShowing = true;
        MoveOverworldUpIfRequired();
    }
}


public class InfoTextData{
    public string text;
    public int lineCount;
}
