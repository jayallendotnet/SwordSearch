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
    public Text speakerNameTextBox;
    public Image screenDarkener;
    public Image playerChathead;
    public Image enemyChathead;
    public Text buttonText;
    [HideInInspector]
    public BattleData enemyBattleData;
    public RectTransform fakeButton1;
    public RectTransform fakeButton2;
    public RectTransform fakeButton3;
    public RectTransform fakeButton4;
    public RectTransform fakeEnemyName;
    public Text fakeEnemyNameText;

    [Header("Player Chatheads")]
    public Sprite playerChatheadNormal;
    public Sprite playerChatheadAngry;
    public Sprite playerChatheadDefeated;
    public Sprite playerChatheadHappy;
    public Sprite playerChatheadQuestioning;
    public Sprite playerChatheadWorried;

    [Header("Configurations")]
    public float transitionDuration = 0.5f;
    public bool isInOverworld = false;
    public bool isInBattle = false;
    public bool isInTutorial = false;

    [HideInInspector]
    public bool isInCutscene = false;
    [HideInInspector]
    public DialogueStep[] dialogueSteps;
    private int currentStep;
    private Image nameSeparator;
    private Color nameSeparatorColor;
    private Color screenDarkenerColor;
    private float chatheadStartingHeight;
    private RectTransform playerChatheadTransform;
    private RectTransform enemyChatheadTransform;
    private float fakeButtonStartingHeight;
    //private float fakeButton1Pos = 920f;
    private float fakeButton1Pos = 640;
    private float fakeButton2Pos = 360;
    private float fakeEnemyNamePos = 900;
    //private float fakeButton2Pos = 640f;
    //private float fakeButton3Pos = 360f;
    private EnemyData enemyData;
    [HideInInspector]
    public CutsceneManager cutsceneManager;
    [HideInInspector]
    public TutorialManager tutorialManager;
    private bool hasSetStartingValues = false;
    private Color dialogueTextColor;
    private Color nameTextColor;

    private bool showFakeTalkButton;

    void Start(){
        SetStartingValues();
    }

    public void Setup(DialogueStep[] ds, BattleData bd, bool startShown = false, bool showFakeTalkButton = false){
        gameObject.SetActive(true);
        fakeEnemyNameText.text = bd.enemyPrefab.GetComponent<EnemyData>().GetDisplayName().ToUpper();
        this.showFakeTalkButton = showFakeTalkButton;
        if (startShown){
            ShowFakeButtonsSlidingOut();
            overlay.anchoredPosition = Vector2.zero;
        }
        else{
            overlay.anchoredPosition = new Vector2(0, -overlay.rect.height);
        }
        StartDialogue(ds, bd);
    } 

    public void SetStartingValues(){
        if (hasSetStartingValues)
            return;
        
        gameObject.SetActive(false);
        hasSetStartingValues = true;
        nameSeparator = speakerNameTextBox.transform.GetChild(0).GetComponent<Image>();
        nameSeparatorColor = nameSeparator.color;
        screenDarkenerColor = screenDarkener.color;
        dialogueTextColor = dialogueTextBox.color;
        nameTextColor = speakerNameTextBox.color;
        playerChatheadTransform = playerChathead.GetComponent<RectTransform>();
        enemyChatheadTransform = enemyChathead.GetComponent<RectTransform>();
        chatheadStartingHeight = playerChatheadTransform.anchoredPosition.y;
        fakeButtonStartingHeight = buttonText.transform.parent.GetComponent<RectTransform>().anchoredPosition.y;

        dialogueTextBox.gameObject.SetActive(false);
        speakerNameTextBox.gameObject.SetActive(false);
        screenDarkener.gameObject.SetActive(false);
        playerChathead.gameObject.SetActive(false);
        enemyChathead.gameObject.SetActive(false);

        if (isInCutscene){
            gameObject.SetActive(true);
        }

        if (isInTutorial){
            gameObject.SetActive(true);
            //dialogueTextBox.gameObject.SetActive(false);
        }
    }

    private void StartDialogue(DialogueStep[] dialogueSteps, BattleData battleData){
        this.dialogueSteps = dialogueSteps;
        enemyBattleData = battleData;
        if (battleData != null)
            enemyData = battleData.enemyPrefab.GetComponent<EnemyData>();
        
        currentStep = 0;
        ShowCurrentTalkStage();
        TransitionToShowing();
    }

    public void TransitionToShowing(){
        overlay.DOAnchorPosY(0, transitionDuration);

        dialogueTextBox.gameObject.SetActive(true);
        speakerNameTextBox.gameObject.SetActive(true);
        screenDarkener.gameObject.SetActive(true);
        playerChathead.gameObject.SetActive(true);
        enemyChathead.gameObject.SetActive(true);

        HideChatheads();
        //float playerChatheadSize = playerChatheadTransform.sizeDelta.y * playerChatheadTransform.localScale.y;
        //float enemyChatheadSize = enemyChatheadTransform.sizeDelta.y * enemyChatheadTransform.localScale.y;
        //playerChatheadTransform.anchoredPosition = new Vector2(playerChatheadTransform.anchoredPosition.x, -playerChatheadSize);
        //enemyChatheadTransform.anchoredPosition = new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize);

        Color c = Color.white;
        c.a = 0;
        dialogueTextBox.color = c;
        speakerNameTextBox.color = c;
        nameSeparator.color = c;
        Color c2 = Color.black;
        c2.a = 0;
        screenDarkener.color = c2;
        dialogueTextBox.DOColor(dialogueTextColor, transitionDuration);
        speakerNameTextBox.DOColor(nameTextColor, transitionDuration);
        nameSeparator.DOColor(nameSeparatorColor, transitionDuration);
        screenDarkener.DOColor(screenDarkenerColor, transitionDuration);
        if (isInCutscene || isInTutorial)
            screenDarkener.gameObject.SetActive(false);
    }

    public void HideChatheads(float duration = 0f){
        float playerChatheadSize = playerChatheadTransform.sizeDelta.y * playerChatheadTransform.localScale.y;
        float enemyChatheadSize = enemyChatheadTransform.sizeDelta.y * enemyChatheadTransform.localScale.y;
        if (duration <= 0){
            playerChatheadTransform.anchoredPosition = new Vector2(playerChatheadTransform.anchoredPosition.x, -playerChatheadSize);
            enemyChatheadTransform.anchoredPosition = new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize);
        }
        playerChatheadTransform.DOAnchorPos(new Vector2(playerChatheadTransform.anchoredPosition.x, -playerChatheadSize), duration);
        enemyChatheadTransform.DOAnchorPos(new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize), duration);
    }
    public void HideEnemyChathead(float duration = 0f){
        float enemyChatheadSize = enemyChatheadTransform.sizeDelta.y * enemyChatheadTransform.localScale.y;
        if (duration <= 0){
            enemyChatheadTransform.anchoredPosition = new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize);
        }
        enemyChatheadTransform.DOAnchorPos(new Vector2(enemyChatheadTransform.anchoredPosition.x, -enemyChatheadSize), duration);

    }
    
    public void PressedButton(){
        if (isInCutscene)
            cutsceneManager.PressedNextButton();
        else if (isInTutorial)
            tutorialManager.PressedNextButton();
        else if (currentStep < dialogueSteps.Length)
            AdvanceTalkStage();
    }

    private void ShowCurrentTalkStage(){

        if (currentStep < dialogueSteps.Length){
            dialogueTextBox.text = dialogueSteps[currentStep].description;
            if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.PlayerTalking)
                ShowPlayerTalking(dialogueSteps[currentStep].emotion);
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.EnemyTalking)
                ShowEnemyTalking(enemyData, dialogueSteps[currentStep].emotion);
        }
        else{
            dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentStep;
            speakerNameTextBox.text = "WARNING";
        }
        if (currentStep == dialogueSteps.Length - 1)
            buttonText.text = "CONTINUE";
        else
            buttonText.text = "NEXT";

    }

    public void ShowPlayerTalking(DialogueStep.Emotion emotion){
        speakerNameTextBox.text = "PLAYER";
        speakerNameTextBox.alignment = TextAnchor.UpperLeft;
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
            case (DialogueStep.Emotion.Excited):
                sprite = playerChatheadAngry;
                break;
            case (DialogueStep.Emotion.Happy):
                sprite = playerChatheadHappy;
                break;
            case (DialogueStep.Emotion.Questioning):
                sprite = playerChatheadQuestioning;
                break;
            case (DialogueStep.Emotion.Worried):
                sprite = playerChatheadWorried;
                break;
            default:
                sprite = playerChatheadNormal;
                break;
        }
        playerChathead.sprite = sprite;
    }

    public void ShowNobodyTalking(){
        //might need some stuff here in the future, if we are going to have the narrator speak in between other dialogues
    }

    public void ShowEnemyTalking(EnemyData data, DialogueStep.Emotion emotion){
        if (data.nameOverride == "")
            speakerNameTextBox.text = data.name.ToUpper();
        else
            speakerNameTextBox.text = data.nameOverride.ToUpper();
        speakerNameTextBox.alignment = TextAnchor.UpperRight;
        enemyChatheadTransform.DOAnchorPosY(chatheadStartingHeight, transitionDuration);
        enemyChathead.DOColor(Color.white, transitionDuration);
        playerChathead.DOColor(Color.grey, transitionDuration);
        playerChatheadTransform.DOScale(new Vector2(35, 35), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(40, 40), transitionDuration);

        Sprite sprite;
        switch (emotion){
            case (DialogueStep.Emotion.Angry):
                sprite = data.angry;
                break;
            case (DialogueStep.Emotion.Defeated):
                sprite = data.defeated;
                break;
            case (DialogueStep.Emotion.Excited):
                sprite = data.excited;
                break;
            case (DialogueStep.Emotion.Happy):
                sprite = data.angry;
                break;
            case (DialogueStep.Emotion.Questioning):
                sprite = data.questioning;
                break;
            case (DialogueStep.Emotion.Worried):
                sprite = data.angry;
                break;
            default:
                sprite = data.normal;
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
        if (currentStep >= dialogueSteps.Length){
            EndTalk();
            return;
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
        speakerNameTextBox.DOColor(c, transitionDuration);
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
        else
            overlay.DOAnchorPosY(-overlay.rect.height, transitionDuration);
    }


    private void FinishedEndingTalk(){
        dialogueTextBox.gameObject.SetActive(false);
        speakerNameTextBox.gameObject.SetActive(false);
        screenDarkener.gameObject.SetActive(false);
        playerChathead.gameObject.SetActive(false);
        enemyChathead.gameObject.SetActive(false);
        if (isInOverworld)
            FindObjectOfType<OverworldSceneManager>().FinishedTalking();
    }

    private void ShowFakeButtonsSlidingIn(){
        float pos1 = fakeButton1Pos;
        float pos2 = fakeButton2Pos;
        float posE = fakeEnemyNamePos;
        //float pos3 = fakeButton3Pos;
        fakeButton1.gameObject.SetActive(true);
        fakeButton2.gameObject.SetActive(true);
        fakeButton3.gameObject.SetActive(true);
        fakeButton4.gameObject.SetActive(false);
        fakeEnemyName.gameObject.SetActive(true);
        fakeButton1.anchoredPosition = new Vector2(fakeButton1.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton2.anchoredPosition = new Vector2(fakeButton2.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton3.anchoredPosition = new Vector2(fakeButton3.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton4.anchoredPosition = new Vector2(fakeButton4.anchoredPosition.x, fakeButtonStartingHeight);
        fakeEnemyName.anchoredPosition = new Vector2(fakeEnemyName.anchoredPosition.x, fakeButtonStartingHeight);
        fakeButton1.DOAnchorPosY(pos1, transitionDuration);
        fakeButton2.DOAnchorPosY(pos2, transitionDuration);
        fakeButton3.DOAnchorPosY(pos2, transitionDuration);
        fakeButton4.DOAnchorPosY(pos2, transitionDuration);
        fakeEnemyName.DOAnchorPosY(posE, transitionDuration);

        if (showFakeTalkButton){
            fakeButton2.gameObject.SetActive(false);
            fakeButton3.gameObject.SetActive(false);
            fakeButton4.gameObject.SetActive(true);
        }

        StaticVariables.WaitTimeThenCallFunction(transitionDuration, HideFakeButtonsAndDisableSelf);
    }

    private void ShowFakeButtonsSlidingOut(){
        float pos1 = fakeButton1Pos;
        float pos2 = fakeButton2Pos;
        float posE = fakeEnemyNamePos;
        //float pos3 = fakeButton3Pos;
        fakeButton1.gameObject.SetActive(true);
        fakeButton2.gameObject.SetActive(true);
        fakeButton3.gameObject.SetActive(true);
        fakeButton4.gameObject.SetActive(false);
        fakeEnemyName.gameObject.SetActive(true);
        fakeButton1.anchoredPosition = new Vector2(fakeButton1.anchoredPosition.x, pos1);
        fakeButton2.anchoredPosition = new Vector2(fakeButton2.anchoredPosition.x, pos2);
        fakeButton3.anchoredPosition = new Vector2(fakeButton3.anchoredPosition.x, pos2);
        fakeButton4.anchoredPosition = new Vector2(fakeButton4.anchoredPosition.x, pos2);
        fakeEnemyName.anchoredPosition = new Vector2(fakeEnemyName.anchoredPosition.x, posE);
        fakeButton1.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeButton2.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeButton3.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeButton4.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);
        fakeEnemyName.DOAnchorPosY(fakeButtonStartingHeight, transitionDuration);

        if (showFakeTalkButton){
            fakeButton2.gameObject.SetActive(false);
            fakeButton3.gameObject.SetActive(false);
            fakeButton4.gameObject.SetActive(true);
        }

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

    public void ClearDialogue(){
        speakerNameTextBox.text = "";
        dialogueTextBox.text = "";
    }


}

