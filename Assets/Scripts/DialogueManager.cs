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
    public RectTransform fakeButton1;
    public Text fakeButton1Text;
    public RectTransform fakeButton2;
    public Text fakeButton2Text;
    public RectTransform fakeButton3;
    public Text fakeButton3Text;

    [Header("Player Chatheads")]
    public Sprite playerChatheadNormal;
    public Sprite playerChatheadAngry;
    public Sprite playerChatheadDefeated;

    [Header("Configurations")]
    public float transitionDuration = 0.5f;
    public bool isInOverworld = false;
    public bool isInBattle = false;
    public bool isInCutscene = false;


    [HideInInspector]
    public DialogueStep[] dialogueSteps;
    [HideInInspector]
    public CutsceneStep[] cutsceneSteps;
    private int currentStep;
    private Image nameSeparator;
    private Color nameSeparatorColor;
    private Color screenDarkenerColor;
    //private int chatheadHiddenDepth = 450;
    private float chatheadStartingHeight;
    private RectTransform playerChatheadTransform;
    private RectTransform enemyChatheadTransform;
    private float fakeButtonStartingHeight;
    private float fakeButton1Pos = 920f;
    private float fakeButton2Pos = 640f;
    private float fakeButton3Pos = 360f;
    private EnemyData enemyData;
    private CutsceneData.AfterCutsceneDo afterCutsceneDo;


    void Start(){
        if (!isInCutscene)
            gameObject.SetActive(false);
        else
            buttonText.text = "NEXT";
        SetStartingValues();
    }

    public void Setup(DialogueStep[] ds, BattleData bd, bool startShown = false){
        gameObject.SetActive(true);
        if (startShown){
            ShowFakeButtonsSlidingOut();
            overlay.anchoredPosition = Vector2.zero;
        }
        else{
            overlay.anchoredPosition = new Vector2(0, -overlay.rect.height);
        }
        StartDialogue(ds, bd);
    } 

    public void Setup(CutsceneStep[] cs, CutsceneData.AfterCutsceneDo afterCutsceneDo){
        //for cutscenes, the dialogue box is always showing
        overlay.anchoredPosition = Vector2.zero;
        this.afterCutsceneDo = afterCutsceneDo;
        StartCutscene(cs);
    }

    private void SetStartingValues(){
        nameSeparator = speakerNameTetxBox.transform.GetChild(0).GetComponent<Image>();
        nameSeparatorColor = nameSeparator.color;
        screenDarkenerColor = screenDarkener.color;
        playerChatheadTransform = playerChathead.GetComponent<RectTransform>();
        enemyChatheadTransform = enemyChathead.GetComponent<RectTransform>();
        chatheadStartingHeight = playerChatheadTransform.anchoredPosition.y;
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
        enemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
        
        currentStep = 0;
        ShowCurrentTalkStage();
        TransitionToShowing();
    }

    private void StartCutscene(CutsceneStep[] cutsceneSteps){
        this.cutsceneSteps = cutsceneSteps;

        currentStep = 0;
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

        float playerChatheadSize = playerChatheadTransform.sizeDelta.y * playerChatheadTransform.localScale.y;
        float enemyChatheadSize = enemyChatheadTransform.sizeDelta.y * enemyChatheadTransform.localScale.y;
        playerChatheadTransform.anchoredPosition = new Vector2(playerChatheadTransform.anchoredPosition.x, -playerChatheadSize);
        enemyChatheadTransform.anchoredPosition = new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize);

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
        if (isInCutscene)
            screenDarkener.gameObject.SetActive(false);
    }

    
    public void PressedButton(){
        AdvanceTalkStage();
    }

    private void ShowCurrentTalkStage(){
        bool validStage = false;
        bool isLastStep = false;
        if (isInCutscene){
            if (currentStep < cutsceneSteps.Length){
                validStage = true;
                dialogueTextBox.text = cutsceneSteps[currentStep].description;
                if (cutsceneSteps[currentStep].isPlayerTalking)
                    ShowPlayerTalking(cutsceneSteps[currentStep].emotion);
                else
                    ShowEnemyTalking(cutsceneSteps[currentStep].emotion);
            }
            if (currentStep == cutsceneSteps.Length - 1)
                isLastStep = true;
        }
        else{
            if (currentStep < dialogueSteps.Length){
                validStage = true;
                dialogueTextBox.text = dialogueSteps[currentStep].description;
                if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.PlayerTalking)
                    ShowPlayerTalking(dialogueSteps[currentStep].emotion);
                else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.EnemyTalking)
                    ShowEnemyTalking(dialogueSteps[currentStep].emotion);
            }
            if (currentStep == dialogueSteps.Length - 1)
                isLastStep = true;
        }

        if (!validStage){
            dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentStep;
            speakerNameTetxBox.text = "WARNING";
        }
        if (isLastStep)
            buttonText.text = "CONTINUE";
        else
            buttonText.text = "NEXT";
    }

    private void ShowPlayerTalking(DialogueStep.Emotion emotion){
        speakerNameTetxBox.text = "PLAYER";
        speakerNameTetxBox.alignment = TextAnchor.UpperLeft;
        playerChatheadTransform.DOAnchorPosY(chatheadStartingHeight, transitionDuration);
        playerChathead.DOColor(Color.white, transitionDuration);
        enemyChathead.DOColor(Color.grey, transitionDuration);
        playerChatheadTransform.DOScale(new Vector2(40, 40), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(35, 35), transitionDuration);

        Sprite sprite;
        switch (emotion){
            case (DialogueStep.Emotion.Angry):
                sprite = playerChatheadAngry;
                break;
            case (DialogueStep.Emotion.Defeated):
                sprite = playerChatheadDefeated;
                break;
            default:
                sprite = playerChatheadNormal;
                break;
        }
        playerChathead.sprite = sprite;
    }

    private void ShowEnemyTalking(DialogueStep.Emotion emotion){
        if (isInCutscene)
            enemyData = cutsceneSteps[currentStep].characterTalking;
        speakerNameTetxBox.text = enemyData.name.ToUpper();
        speakerNameTetxBox.alignment = TextAnchor.UpperRight;
        enemyChatheadTransform.DOAnchorPosY(chatheadStartingHeight, transitionDuration);
        enemyChathead.DOColor(Color.white, transitionDuration);
        playerChathead.DOColor(Color.grey, transitionDuration);
        playerChatheadTransform.DOScale(new Vector2(35, 35), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(40, 40), transitionDuration);

        Sprite sprite;
        switch (emotion){
            case (DialogueStep.Emotion.Angry):
                sprite = enemyData.angry;
                break;
            case (DialogueStep.Emotion.Defeated):
                sprite = enemyData.defeated;
                break;
            default:
                sprite = enemyData.normal;
                break;
        }
        enemyChathead.sprite = sprite;
        int scaleFactor = 100;
        if ((sprite.bounds.size.x * 100) >= 20)
            scaleFactor = 75;
        enemyChatheadTransform.sizeDelta = (new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * scaleFactor);
    }

    private void AdvanceTalkStage(){
        currentStep ++;
        if (isInCutscene){
            if (currentStep >= cutsceneSteps.Length){
                EndTalk();
                return;
            }
        }
        else{
            if (currentStep >= dialogueSteps.Length){
                EndTalk();
                return;
            }
        }
        ShowCurrentTalkStage();
    }

    private void EndTalk(){
        if (isInBattle){
            FindObjectOfType<UIManager>().EndDialogue();
            return;
        }
        StaticVariables.WaitTimeThenCallFunction(transitionDuration, FinishedEndingTalk);

        Color c = Color.white;
        c.a = 0;
        dialogueTextBox.DOColor(c, transitionDuration);
        speakerNameTetxBox.DOColor(c, transitionDuration);
        nameSeparator.DOColor(c, transitionDuration);
        Color c2 = Color.black;
        c2.a = 0;
        screenDarkener.DOColor(c2, transitionDuration);


        float playerChatheadSize = playerChatheadTransform.sizeDelta.y * playerChatheadTransform.localScale.y;
        float enemyChatheadSize = enemyChatheadTransform.sizeDelta.y * enemyChatheadTransform.localScale.y;
        playerChatheadTransform.DOAnchorPosY(-playerChatheadSize, transitionDuration);
        enemyChatheadTransform.DOAnchorPosY(-enemyChatheadSize, transitionDuration);

        if (isInOverworld){
            buttonText.text = "BACK";
            ShowFakeButtonsSlidingIn();
        }
        else if (isInCutscene){
            if (afterCutsceneDo == CutsceneData.AfterCutsceneDo.GoToOverworld)
                StaticVariables.FadeOutThenLoadScene("World 1 - Grasslands");
            //else if (afterCutsceneDo == CutsceneManager.AfterCutsceneDo.GoToNextCutscene)
                //StaticVariables.FadeOutThenLoadScene(StaticVariables.GetCurrentWorldName());

        }
        else
            overlay.DOAnchorPosY(-overlay.rect.height, transitionDuration);
    }


    private void FinishedEndingTalk(){
        //print("finished ending talk");
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

