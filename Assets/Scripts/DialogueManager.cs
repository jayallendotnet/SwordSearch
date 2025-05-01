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
    public GameObject realButton;
    public Text buttonText;
    [HideInInspector]
    public BattleData enemyBattleData;
    public RectTransform fakeBattleButton;
    public RectTransform fakeInfoButton;
    public RectTransform fakeReadButton;
    public RectTransform fakeTalkButton;
    public RectTransform fakeBackButton;
    public Text fakeBackButtonText;
    public Animator fakeInfoStar;
    public RectTransform fakeEnemyName;
    public Text fakeEnemyNameText;

    [Header("Player Chatheads")]
    public Sprite playerChatheadNormal;
    public Sprite playerChatheadAngry;
    public Sprite playerChatheadDefeated;
    public Sprite playerChatheadHappy;
    public Sprite playerChatheadQuestioning;
    public Sprite playerChatheadWorried;
    public Sprite playerChatheadSurprised;

    [Header("Player Emotion Flairs")]
    public GameObject playerAngryFlair;
    public GameObject playerHappyFlair;
    public GameObject playerSurprisedFlair;
    public GameObject playerQuestioningFlair;
    public GameObject playerWorriedFlair;
    public Image[] playerFlairImages;

    [Header("Configurations")]
    public float transitionDuration = 0.5f;
    public bool isInOverworld = false;
    public bool isInBattle = false;
    public bool isInTutorial = false;
    private float chatdeadSizePerPixel = 28f;

    [Header("Prefabs")]
    public GameObject magicFlashPrefab;

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
    private float fakeButtonTopHeight;
    private float fakeButtonMidHeight;
    private float fakeButtonBottomHeight;
    private float fakeEnemyNameHeight;
    private EnemyData enemyData;
    [HideInInspector]
    public CutsceneManager cutsceneManager;
    [HideInInspector]
    public TutorialManager tutorialManager;
    private bool hasSetStartingValues = false;
    private Color dialogueTextColor;
    private Color nameTextColor;


    void Start(){
        SetStartingValues();
    }

    public void Setup(DialogueStep[] ds, BattleData bd, bool startShown = false){
        //startshown is true means the interact overlay has already been pulled up before opening the dialoguemanager
        gameObject.SetActive(true);
        fakeEnemyNameText.text = bd.enemyPrefab.GetComponent<EnemyData>().GetDisplayName().ToUpper();
        //this.showFakeTalkButton = showFakeTalkButton;
        if (startShown){
            ShowFakeButtonsSlidingOut();
            MimicInfoAndReadButtons();
            //SynchronizeInfoIconAnimations();
            overlay.anchoredPosition = Vector2.zero;
        }
        else{
            overlay.anchoredPosition = new Vector2(0, -overlay.rect.height);
            HideFakeButtons();

        }
        SetStartingValues();
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
        fakeEnemyNameHeight = fakeEnemyName.anchoredPosition.y;
        fakeButtonTopHeight = fakeBattleButton.anchoredPosition.y;
        fakeButtonMidHeight = fakeInfoButton.anchoredPosition.y;
        fakeButtonBottomHeight = fakeBackButton.anchoredPosition.y;

        dialogueTextBox.gameObject.SetActive(false);
        speakerNameTextBox.gameObject.SetActive(false);
        screenDarkener.gameObject.SetActive(false);
        playerChathead.gameObject.SetActive(false);
        enemyChathead.gameObject.SetActive(false);
        //fakeEnemyName.gameObject.SetActive(false);

        if (isInCutscene){
            gameObject.SetActive(true);
        }

        if (isInTutorial)
            gameObject.SetActive(true);
    }

    private void MimicInfoAndReadButtons(){
        InteractOverlayManager iom = FindObjectOfType<InteractOverlayManager>();
        Transform infoButton = iom.infoButton.transform;
        Transform readButton = iom.readButton.transform;

        Transform fakeInfo = fakeInfoButton.transform;
        Transform fakeRead = fakeReadButton.transform;

        fakeInfo.GetChild(1).gameObject.SetActive(infoButton.GetChild(1).gameObject.activeSelf);
        fakeInfo.GetChild(2).gameObject.SetActive(infoButton.GetChild(2).gameObject.activeSelf);
        fakeInfo.GetChild(3).gameObject.SetActive(infoButton.GetChild(3).gameObject.activeSelf);

        
        fakeRead.GetChild(2).GetComponent<Image>().sprite = readButton.GetChild(2).GetComponent<Image>().sprite;
        fakeRead.GetChild(3).gameObject.SetActive(readButton.GetChild(3).gameObject.activeSelf);

        if (fakeInfo.GetChild(1).gameObject.activeSelf)
            SynchronizeInfoIconAnimations(iom.infoHighlight);
    }

    public void SynchronizeInfoIconAnimations(GameObject infoHighlight){
        fakeInfoStar.gameObject.SetActive(infoHighlight.activeSelf);
        if (infoHighlight.activeSelf)
            fakeInfoStar.Play("Info Icon", 0, infoHighlight.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime);
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

    private void EndDialogueEvent(){
        realButton.SetActive(true);
        AdvanceTalkStage();
    }

    private void ShowCurrentTalkStage(){

        if (currentStep < dialogueSteps.Length){
            dialogueTextBox.text = TextFormatter.FormatString(dialogueSteps[currentStep].description);
            if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.PlayerTalking)
                ShowPlayerTalking(dialogueSteps[currentStep].emotion);
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.EnemyTalking)
                ShowEnemyTalking(enemyData, dialogueSteps[currentStep].emotion);
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.EnemyTalkingNameOverride){
                ShowEnemyTalking(enemyData, dialogueSteps[currentStep].emotion, dialogueSteps[currentStep].name);
            }
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.OtherTalking)
                ShowEnemyTalking(dialogueSteps[currentStep].talker, dialogueSteps[currentStep].emotion);
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.OtherTalkingNameOverride) {
                ShowEnemyTalking(dialogueSteps[currentStep].talker, dialogueSteps[currentStep].emotion, dialogueSteps[currentStep].name);
            }
            else if (dialogueSteps[currentStep].type == DialogueStep.DialogueType.Event)
                if (dialogueSteps[currentStep].description == "Healing flash"){
                    ShowNobodyTalking();
                    dialogueTextBox.text = "";
                    realButton.SetActive(false);
                    MagicFlash flash = Instantiate(magicFlashPrefab, enemyChatheadTransform).GetComponent<MagicFlash>();
                    StaticVariables.WaitTimeThenCallFunction(flash.GetTotalTime(), EndDialogueEvent);
                    flash.StartProcess(StaticVariables.healingPowerupColor);
                    return;
                }
                /*
                else if (dialogueSteps[currentStep].description == "Screen Shake"){
                    
                    AdvanceTalkStage();
                    return;
                }
                */
        }
        else{
            dialogueTextBox.text = "No dialogue for this enemy, current talk step is " + currentStep;
            DisplaySpeakerName("WARNING");
        }
        if (currentStep == dialogueSteps.Length - 1)
            SetButtonText("CONTINUE");
        else
            SetButtonText("NEXT");

    }

    public void ShowPlayerTalking(DialogueStep.Emotion emotion){
        DisplaySpeakerName(StaticVariables.playerName.ToUpper());
        speakerNameTextBox.alignment = TextAnchor.UpperLeft;
        playerChatheadTransform.DOAnchorPosY(chatheadStartingHeight, transitionDuration);
        LightenPlayerChathead();
        enemyChathead.DOColor(Color.grey, transitionDuration);
        playerChatheadTransform.DOScale(new Vector2(1, 1), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(0.875f, 0.875f), transitionDuration);
        Sprite sprite = emotion switch {
            (DialogueStep.Emotion.Angry) => playerChatheadAngry,
            (DialogueStep.Emotion.Defeated) => playerChatheadDefeated,
            //(DialogueStep.Emotion.Excited) => playerChatheadAngry, //hopefully we can remove all instances of excited?
            (DialogueStep.Emotion.Happy) => playerChatheadHappy,
            (DialogueStep.Emotion.Questioning) => playerChatheadQuestioning,
            (DialogueStep.Emotion.Worried) => playerChatheadWorried,
            (DialogueStep.Emotion.Surprised) => playerChatheadSurprised,
            _ => playerChatheadNormal,
        };
        playerChathead.sprite = sprite;
        //print(sprite.rect.height);
        playerChatheadTransform.sizeDelta =new Vector2 (sprite.rect.width * chatdeadSizePerPixel, sprite.rect.height * chatdeadSizePerPixel);
        ShowPlayerEmotionFlair(emotion);
    }

    private void ShowPlayerEmotionFlair(DialogueStep.Emotion emotion){
        playerAngryFlair.SetActive(false);
        playerHappyFlair.SetActive(false);
        playerSurprisedFlair.SetActive(false);
        playerQuestioningFlair.SetActive(false);
        playerWorriedFlair.SetActive(false);

        GameObject flair = emotion switch {
            (DialogueStep.Emotion.Angry) => playerAngryFlair,
            //(DialogueStep.Emotion.Happy) => playerHappyFlair,
            //(DialogueStep.Emotion.Questioning) => playerQuestioningFlair,
            (DialogueStep.Emotion.Worried) => playerWorriedFlair,
            //(DialogueStep.Emotion.Surprised) => playerSurprisedFlair,
            _ => null,
        };

        if (flair != null)
            flair.SetActive(true);
    }

    public void DisplaySpeakerName(string name){
        speakerNameTextBox.text = TextFormatter.FormatString(name);
    }

    private void LightenPlayerChathead(){
        playerChathead.DOColor(Color.white, transitionDuration);
        foreach (Image im in playerFlairImages)
            im.DOColor(Color.white, transitionDuration);
    }

    private void DarkenPlayerChathead(){
        playerChathead.DOColor(Color.grey, transitionDuration);
        foreach (Image im in playerFlairImages)
            im.DOColor(Color.grey, transitionDuration);
    }

    public void ShowNobodyTalking(){
        //might need some stuff here in the future, if we are going to have the narrator speak in between other dialogues
        enemyChathead.DOColor(Color.grey, transitionDuration);
        DarkenPlayerChathead();
        DisplaySpeakerName("");
        dialogueTextBox.text = "";
        playerChatheadTransform.DOScale(new Vector2(0.875f, 0.875f), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(0.875f, 0.875f), transitionDuration);
    }

    public void ShowEnemyTalking(EnemyData data, DialogueStep.Emotion emotion, string alternateName = ""){
        if (alternateName != "")
            DisplaySpeakerName(alternateName.ToUpper());
        else if (data.nameOverride == "")
            DisplaySpeakerName(data.name.ToUpper());
        else
            DisplaySpeakerName(data.nameOverride.ToUpper());
        speakerNameTextBox.alignment = TextAnchor.UpperRight;
        enemyChatheadTransform.DOAnchorPosY(chatheadStartingHeight, transitionDuration);
        enemyChathead.DOColor(Color.white, transitionDuration);
        DarkenPlayerChathead();
        playerChatheadTransform.DOScale(new Vector2(0.875f, 0.875f), transitionDuration);
        enemyChatheadTransform.DOScale(new Vector2(1, 1), transitionDuration);

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
                sprite = data.happy;
                break;
            case (DialogueStep.Emotion.Questioning):
                sprite = data.questioning;
                break;
            case (DialogueStep.Emotion.Worried):
                sprite = data.worried;
                break;
            case (DialogueStep.Emotion.Surprised):
                sprite = data.surprised;
                break;
            case (DialogueStep.Emotion.Mystery):
                sprite = data.mystery;
                break;
            case (DialogueStep.Emotion.Custom1):
                sprite = data.custom1;
                break;
            case (DialogueStep.Emotion.Custom2):
                sprite = data.custom2;
                break;
            case (DialogueStep.Emotion.Custom3):
                sprite = data.custom3;
                break;
            default:
                sprite = data.normal;
                break;
        }
        enemyChathead.sprite = sprite;
        //int scaleFactor = 100;
        //if ((sprite.bounds.size.x * 100) >= 20)
        //    scaleFactor = 75;
            
        //print(sprite.rect.height);
        enemyChatheadTransform.sizeDelta = new Vector2 (sprite.rect.width * chatdeadSizePerPixel, sprite.rect.height * chatdeadSizePerPixel);
        //enemyChatheadTransform.sizeDelta = (new Vector2(sprite.bounds.size.x, sprite.bounds.size.y) * scaleFactor);
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
            SetButtonText("BACK");
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
        realButton.SetActive(false);

        fakeBattleButton.gameObject.SetActive(true);
        fakeInfoButton.gameObject.SetActive(true);
        fakeReadButton.gameObject.SetActive(true);
        fakeTalkButton.gameObject.SetActive(true);
        fakeBackButton.gameObject.SetActive(true);
        fakeEnemyName.gameObject.SetActive(true);
        fakeBattleButton.anchoredPosition = new Vector2(fakeBattleButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeInfoButton.anchoredPosition = new Vector2(fakeInfoButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeReadButton.anchoredPosition = new Vector2(fakeReadButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeTalkButton.anchoredPosition = new Vector2(fakeTalkButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeBackButton.anchoredPosition = new Vector2(fakeBackButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeEnemyName.anchoredPosition = new Vector2(fakeEnemyName.anchoredPosition.x, fakeButtonBottomHeight);
        fakeBattleButton.DOAnchorPosY(fakeButtonTopHeight, transitionDuration);
        fakeInfoButton.DOAnchorPosY(fakeButtonMidHeight, transitionDuration);
        fakeReadButton.DOAnchorPosY(fakeButtonMidHeight, transitionDuration);
        fakeTalkButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeBackButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeEnemyName.DOAnchorPosY(fakeEnemyNameHeight, transitionDuration);

        //also shrink the back button down to its normal size
        fakeBackButton.DOAnchorPosX(fakeReadButton.anchoredPosition.x, transitionDuration);
        fakeBackButton.DOSizeDelta(fakeReadButton.sizeDelta, 0.5f);
       
        //SynchronizeInfoIconAnimations();
        MimicInfoAndReadButtons();
        StaticVariables.WaitTimeThenCallFunction(transitionDuration, ShowRealButton);
        StaticVariables.WaitTimeThenCallFunction(transitionDuration, HideFakeButtonsAndDisableSelf);
    }

    private void ShowFakeButtonsSlidingOut(){
        realButton.SetActive(false);

        fakeBattleButton.gameObject.SetActive(true);
        fakeInfoButton.gameObject.SetActive(true);
        fakeReadButton.gameObject.SetActive(true);
        fakeTalkButton.gameObject.SetActive(true);
        fakeBackButton.gameObject.SetActive(true);
        fakeEnemyName.gameObject.SetActive(true);
        fakeBattleButton.anchoredPosition = new Vector2(fakeBattleButton.anchoredPosition.x, fakeButtonTopHeight);
        fakeInfoButton.anchoredPosition = new Vector2(fakeInfoButton.anchoredPosition.x, fakeButtonMidHeight);
        fakeReadButton.anchoredPosition = new Vector2(fakeReadButton.anchoredPosition.x, fakeButtonMidHeight);
        fakeTalkButton.anchoredPosition = new Vector2(fakeTalkButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeBackButton.anchoredPosition = new Vector2(fakeBackButton.anchoredPosition.x, fakeButtonBottomHeight);
        fakeEnemyName.anchoredPosition = new Vector2(fakeEnemyName.anchoredPosition.x, fakeEnemyNameHeight);
        fakeBattleButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeInfoButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeReadButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeTalkButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeBackButton.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);
        fakeEnemyName.DOAnchorPosY(fakeButtonBottomHeight, transitionDuration);

        //also expand the back button
        fakeBackButton.DOAnchorPosX(fakeBattleButton.anchoredPosition.x, transitionDuration);
        fakeBackButton.DOSizeDelta(fakeBattleButton.sizeDelta, 0.5f);

        StaticVariables.WaitTimeThenCallFunction(transitionDuration, HideFakeButtons);
        StaticVariables.WaitTimeThenCallFunction(transitionDuration, ShowRealButton);
    }

    public void HideFakeButtons(){
        fakeBattleButton.gameObject.SetActive(false);
        fakeInfoButton.gameObject.SetActive(false);
        fakeReadButton.gameObject.SetActive(false);
        fakeTalkButton.gameObject.SetActive(false);
        fakeBackButton.gameObject.SetActive(false);
        fakeEnemyName.gameObject.SetActive(false);
    }

    private void ShowRealButton(){
        realButton.SetActive(true);
    }

    private void HideFakeButtonsAndDisableSelf(){
        HideFakeButtons();
        gameObject.SetActive(false);
    }

    public void ClearDialogue(){
        DisplaySpeakerName("");
        dialogueTextBox.text = "";
    }

    public void SetButtonText(string t){
        //also sets the fake button text, helping transitions to/from the interact overlay to be more seamless
        buttonText.text = t;
        fakeBackButtonText.text = t;
    }


}

