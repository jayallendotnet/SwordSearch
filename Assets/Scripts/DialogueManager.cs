using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DialogueManager : MonoBehaviour{

   
    [Header("Scene References")]
    public RectTransform overlay;
    public Text dialogueTextBox;
    public Text speakerNameTetxBox;
    public Image screenDarkener;
    public Image playerChathead;
    public Image enemyChathead;
    public Text buttonText;
    [HideInInspector]
    public BattleData enemyBattleData;


    [Header("Configurations")]
    public float transitionDuration = 0.5f;



    [HideInInspector]
    public DialogueStep[] dialogueSteps;
    private int currentTalkStep;
    private Image nameSeparator;
    private Color nameSeparatorColor;
    private Color screenDarkenerColor;
    private float chatheadHiddenDepth = 200f;
    private float cahtheadStartingHeight;
    private RectTransform playerChatheadTransform;
    private RectTransform enemyChatheadTransform;
    public bool isInOverworld = false;

    public RectTransform fakeButton1;
    public Text fakeButton1Text;
    public RectTransform fakeButton2;
    public Text fakeButton2Text;
    public RectTransform fakeButton3;
    public Text fakeButton3Text;

    private float fakeButtonStartingHeight;
    private float fakeButton1Pos = 920f;
    private float fakeButton2Pos = 640f;
    private float fakeButton3Pos = 360f;


    void Start(){
        gameObject.SetActive(false);
        SetStartingValues();
    }

    public void Setup(DialogueStep[] ds, BattleData bd, bool startShown = false){
        gameObject.SetActive(true);
        if (startShown){
            ShowFakeButtonsSlidingOut();
        }
        else{
            overlay.anchoredPosition = new Vector2(0, -overlay.rect.height);
        }
        StartDialogue(ds, bd);
    } 

    private void SetStartingValues(){
        nameSeparator = speakerNameTetxBox.transform.GetChild(0).GetComponent<Image>();
        nameSeparatorColor = nameSeparator.color;
        screenDarkenerColor = screenDarkener.color;
        playerChatheadTransform = playerChathead.GetComponent<RectTransform>();
        enemyChatheadTransform = enemyChathead.GetComponent<RectTransform>();
        cahtheadStartingHeight = playerChatheadTransform.anchoredPosition.y;
        fakeButtonStartingHeight = buttonText.transform.parent.GetComponent<RectTransform>().anchoredPosition.y;

        dialogueTextBox.gameObject.SetActive(false);
        speakerNameTetxBox.gameObject.SetActive(false);
        screenDarkener.gameObject.SetActive(false);
        playerChathead.gameObject.SetActive(false);
        enemyChathead.gameObject.SetActive(false);
    }

    private void StartDialogue(DialogueStep[] dialogueSteps, BattleData battleData){
        this.dialogueSteps = dialogueSteps;
        enemyBattleData = battleData;
        
        currentTalkStep = 0;
        ShowCurrentTalkStage();
        TransitionToShowing();
    }

    private void TransitionToShowing(){
        overlay.DOAnchorPosY(0, transitionDuration);

        dialogueTextBox.gameObject.SetActive(true);
        speakerNameTetxBox.gameObject.SetActive(true);
        screenDarkener.gameObject.SetActive(true);
        playerChathead.gameObject.SetActive(true);
        enemyChathead.gameObject.SetActive(true);

        playerChatheadTransform.anchoredPosition = new Vector2(playerChatheadTransform.anchoredPosition.x, -chatheadHiddenDepth);
        enemyChatheadTransform.anchoredPosition = new Vector2(enemyChatheadTransform.anchoredPosition.x, -chatheadHiddenDepth);

        Color c = Color.white;
        c.a = 0;
        dialogueTextBox.color = c;
        speakerNameTetxBox.color = c;
        nameSeparator.color = c;
        Color c2 = Color.black;
        c2.a = 0;
        screenDarkener.color = c2;
        dialogueTextBox.DOColor(Color.white, transitionDuration);
        speakerNameTetxBox.DOColor(Color.white, transitionDuration);
        nameSeparator.DOColor(nameSeparatorColor, transitionDuration);
        screenDarkener.DOColor(screenDarkenerColor, transitionDuration);
    }

    
    public void PressedButton(){
        AdvanceTalkStage();
    }

    private void ShowCurrentTalkStage(){
        if (currentTalkStep < dialogueSteps.Length){
            dialogueTextBox.text = dialogueSteps[currentTalkStep].description;
            if (dialogueSteps[currentTalkStep].type == DialogueStep.DialogueType.PlayerTalking)
                ShowPlayerTalking();
            else if (dialogueSteps[currentTalkStep].type == DialogueStep.DialogueType.EnemyTalking)
                ShowEnemyTalking();
        }
        else{
            dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentTalkStep;
            speakerNameTetxBox.text = "WARNING";
        }
        if (currentTalkStep == dialogueSteps.Length - 1)
            buttonText.text = "CONTINUE";
        else
            buttonText.text = "NEXT";
    }

    private void ShowPlayerTalking(){
        speakerNameTetxBox.text = "PLAYER";
        speakerNameTetxBox.alignment = TextAnchor.UpperLeft;
        playerChatheadTransform.DOAnchorPosY(cahtheadStartingHeight, transitionDuration);
        playerChathead.DOColor(Color.white, transitionDuration);
        enemyChathead.DOColor(Color.grey, transitionDuration);
    }

    private void ShowEnemyTalking(){
        speakerNameTetxBox.text = enemyBattleData.enemyPrefab.name.ToUpper();
        speakerNameTetxBox.alignment = TextAnchor.UpperRight;
        enemyChatheadTransform.DOAnchorPosY(cahtheadStartingHeight, transitionDuration);
        enemyChathead.DOColor(Color.white, transitionDuration);
        playerChathead.DOColor(Color.grey, transitionDuration);
    }

    private void AdvanceTalkStage(){
        currentTalkStep ++;
        if (currentTalkStep >= dialogueSteps.Length){
            EndTalk();
            return;
        }
        ShowCurrentTalkStage();
    }

    private void EndTalk(){
        StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedEndingTalk);

        Color c = Color.white;
        c.a = 0;
        dialogueTextBox.DOColor(c, transitionDuration);
        speakerNameTetxBox.DOColor(c, transitionDuration);
        nameSeparator.DOColor(c, transitionDuration);
        Color c2 = Color.black;
        c2.a = 0;
        screenDarkener.DOColor(c2, transitionDuration);

        playerChatheadTransform.DOAnchorPosY(-chatheadHiddenDepth, transitionDuration);
        enemyChatheadTransform.DOAnchorPosY(-chatheadHiddenDepth, transitionDuration);

        if (isInOverworld){
            buttonText.text = "BACK";
            ShowFakeButtonsSlidingIn();
        }
        else
            overlay.DOAnchorPosY(-overlay.rect.height, transitionDuration);
    }


    private void FinishedEndingTalk(){
        dialogueTextBox.gameObject.SetActive(false);
        speakerNameTetxBox.gameObject.SetActive(false);
        screenDarkener.gameObject.SetActive(false);
        playerChathead.gameObject.SetActive(false);
        enemyChathead.gameObject.SetActive(false);
    }

    private void ShowFakeButtonsSlidingIn(){
        float pos1 = fakeButton1Pos;
        float pos2 = fakeButton2Pos;
        float pos3 = fakeButton3Pos;
        fakeButton1.gameObject.SetActive(true);
        fakeButton2.gameObject.SetActive(true);
        fakeButton3.gameObject.SetActive(true);
        fakeButton1.anchoredPosition = new Vector2(fakeButton1.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton2.anchoredPosition = new Vector2(fakeButton2.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton3.anchoredPosition = new Vector2(fakeButton3.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton1.DOAnchorPosY(pos1, transitionDuration);
        fakeButton2.DOAnchorPosY(pos2, transitionDuration);
        fakeButton3.DOAnchorPosY(pos3, transitionDuration);

        StaticVariables.WaitTimeThenCallFunction(transitionDuration, HideFakeButtonsAndDisableSelf);
    }

    private void ShowFakeButtonsSlidingOut(){
        float pos1 = fakeButton1Pos;
        float pos2 = fakeButton2Pos;
        float pos3 = fakeButton3Pos;
        fakeButton1.gameObject.SetActive(true);
        fakeButton2.gameObject.SetActive(true);
        fakeButton3.gameObject.SetActive(true);
        fakeButton1.anchoredPosition = new Vector2(fakeButton1.anchoredPosition.x, pos1);
        fakeButton2.anchoredPosition = new Vector2(fakeButton2.anchoredPosition.x, pos2);
        fakeButton3.anchoredPosition = new Vector2(fakeButton3.anchoredPosition.x, pos3);
        fakeButton1.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeButton2.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeButton3.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);

        StaticVariables.WaitTimeThenCallFunction(transitionDuration, HideFakeButtons);
    }

    private void HideFakeButtons(){
        fakeButton1.gameObject.SetActive(false);
        fakeButton2.gameObject.SetActive(false);
        fakeButton3.gameObject.SetActive(false);
    }

    private void HideFakeButtonsAndDisableSelf(){
        HideFakeButtons();
        gameObject.SetActive(false);
    }


}

