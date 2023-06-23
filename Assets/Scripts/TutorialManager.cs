using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : BattleManager {

    private int tutorialStep = 0;
    private char[,] startingLayout = {{'W', 'K', 'M', 'O', 'S'}, {'L', 'E', 'V', 'Y', 'S'}, {'A', 'D', 'O', 'G', 'R'}, {'C', 'H', 'I', 'R', 'Z'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};
    public enum Cond{Click, SelectLetter, FinishWord, SubmitWord, EnemyTakesDamage, EnemyDies};
    private Cond advanceCondition;
    private char requiredLetter;
    private char[] requiredWord;
    private bool canEnemyTakeDamage = false;
    private bool canCountdown = false;

    
    [Header("Tutorial Stuff")]
    public Text tutorialTextBox;
    public GameObject highlightWord1;
    public GameObject highlightWord2;
    public GameObject highlightWord3;
    public GameObject highlightSubmit;

    public override void Start() {
        base.Start();
        powerupsPerPuzzle = 0;
        SetupDialogueManager();
        HideSubmitButtonExtras();
        HideHealthDisplays();
        //HidePowerups();
        AdvanceTutorialStep();
        ButtonText("CONTINUE");

    }

    public override void QueueEnemyAttack(){
        return;
    }


    public override void UpdateSubmitVisuals(){
        base.UpdateSubmitVisuals();
        HideSubmitButtonExtras();
    }

    private void AdvanceTutorialStep(){
        tutorialStep ++;

        switch (tutorialStep){
            case (1):
                DisplayText("Tap a letter to select it. Start by selecting the letter 'D'.");
                LoadCustomPuzzle();
                puzzleGenerator.letterSpaces[2,1].ToggleTutorialSelector(true);
                canEnemyTakeDamage = false;
                advanceCondition = Cond.SelectLetter;
                requiredLetter = 'D';
                break;
            case (2):
                DisplayText("Tap other nearby letters to make a word. Try to make the word 'DOG'.");
                advanceCondition = Cond.FinishWord;
                requiredWord = new char[] {'D', 'O', 'G'};
                puzzleGenerator.letterSpaces[2,1].ToggleTutorialSelector(false);
                highlightWord1.SetActive(true);
                break;
            case (3):
                DisplayText("Once you have completed a word, the submit button will glow. Tap it to send a magical attack at your opponent!");
                advanceCondition = Cond.SubmitWord;
                highlightWord1.SetActive(false);
                highlightSubmit.SetActive(true);
                break;
            case (4):
                DisplayText("");
                highlightSubmit.SetActive(false);
                advanceCondition = Cond.EnemyTakesDamage;
                break;
            case (5):
                DisplayPlayerTalking("Whoa! The book shot a blast of water at the goblin!", DialogueStep.Emotion.Happy);
                ToggleButton(true);
                advanceCondition = Cond.Click;
                break;
            case (6):
                DisplayEnemyTalking("Why am I wet??", enemyData, DialogueStep.Emotion.Angry);
                uiManager.enemyAnimator.GetComponent<RectTransform>().parent.localScale = new Vector3(-1, 1, 1);
                advanceCondition = Cond.Click;
                break;
            case (7):
                DisplayPlayerTalking("Oh no, the goblin noticed me!", DialogueStep.Emotion.Worried);
                advanceCondition = Cond.Click;
                break;
            case (8):
                DisplayText("You can also swipe to select multiple letters at once.");
                advanceCondition = Cond.Click;
                break;
            case (9):
                DisplayText("Try to make the word 'MOSSY' and attack the goblin.");
                ToggleButton(false);
                advanceCondition = Cond.FinishWord;
                requiredWord = new char[] {'M', 'O', 'S', 'S', 'Y'};
                highlightWord2.SetActive(true);
                break;
            case (10):
                advanceCondition = Cond.SubmitWord;
                highlightWord2.SetActive(false);
                highlightSubmit.SetActive(true);
                break;
            case (11):
                DisplayText("");
                highlightSubmit.SetActive(false);
                advanceCondition = Cond.EnemyTakesDamage;
                break;
            case (12):
                DisplayText("Letters don't have to be in all in one row to make a word!");
                advanceCondition = Cond.Click;
                break;
            case (13):
                DisplayText("As long as the letters are connected, you can make a word with them.");
                advanceCondition = Cond.Click;
                break;
            case (14):
                DisplayText("Try to make the word 'CHALK'.");
                ToggleButton(false);
                advanceCondition = Cond.FinishWord;
                requiredWord = new char[] {'C', 'H', 'A', 'L', 'K'};
                highlightWord3.SetActive(true);
                break;
            case (15):
                advanceCondition = Cond.SubmitWord;
                highlightSubmit.SetActive(true);
                highlightWord3.SetActive(false);
                break;
            case (16):
                DisplayText("");
                highlightSubmit.SetActive(false);
                advanceCondition = Cond.EnemyTakesDamage;
                break;
            case (17):
                uiManager.enemyAnimator.Play("Die");
                advanceCondition = Cond.EnemyDies;
                break;
            case (18):
                DisplayPlayerTalking("I managed to defeat one of the invading goblins!", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (19):
                DisplayPlayerTalking("This book has some powerful magic...", DialogueStep.Emotion.Normal);
                advanceCondition = Cond.Click;
                break;
            case (20):
                uiManager.EndDialogue();
                break;
        }

        switch (advanceCondition){
            case (Cond.Click):
                ToggleButton(true);
                break;
            case (Cond.SelectLetter):
                ToggleButton(false);
                break;
            case (Cond.FinishWord):
                ToggleButton(false);
                break;
            case (Cond.SubmitWord):
                ToggleButton(false);
                break;
        }
    }

    private void ToggleButton(bool value){
        uiManager.dialogueManager.buttonText.transform.parent.gameObject.SetActive(value);
    }

    private void SetupDialogueManager(){
        uiManager.dialogueManager.tutorialManager = this;
        
        uiManager.dialogueManager.ClearDialogue();
        uiManager.dialogueManager.SetStartingValues();
        uiManager.dialogueManager.TransitionToShowing();
    }

    private void HideSubmitButtonExtras(){
        uiManager.countdownDivider.SetActive(false);
        uiManager.countdownNumber.gameObject.SetActive(false);
        uiManager.countdownNumber.transform.parent.Find("Countdown Icon").gameObject.SetActive(false);
        uiManager.wordStrengthDivider.SetActive(false);
        uiManager.wordStrengthImageOnes.gameObject.SetActive(false);
        uiManager.wordStrengthImageTens.gameObject.SetActive(false);
        uiManager.wordStrengthImageSingle.gameObject.SetActive(false);
        uiManager.wordStrengthIcon.gameObject.SetActive(false);
    }

    private void HideHealthDisplays(){
        uiManager.playerHP1DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.playerHP2DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.playerHP3DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.enemyHP1DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.enemyHP2DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.enemyHP3DigitOnes.transform.parent.gameObject.SetActive(false);
    }

    private void HidePowerups(){
        foreach (LetterSpace ls in puzzleGenerator.letterSpaces){
            ls.powerupType = PowerupTypes.None;
            ls.ShowPowerup();
        }
    }

    private void HideChatheads(){
        uiManager.dialogueManager.HideChatheads(uiManager.dialogueManager.transitionDuration);
    }

    private void DisplayText(string s){
        tutorialTextBox.text = s;
        tutorialTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(false);
        HideChatheads();
    }

    private void DisplayPlayerTalking(string s, DialogueStep.Emotion emotion){
        uiManager.dialogueManager.dialogueTextBox.text = s;
        uiManager.dialogueManager.ShowPlayerTalking(emotion);
        //uiManager.dialogueManager.TransitionToShowing();
        tutorialTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(true);
    }

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion){
        uiManager.dialogueManager.dialogueTextBox.text = s;
        uiManager.dialogueManager.ShowEnemyTalking(enemyData, emotion);
        //uiManager.dialogueManager.TransitionToShowing();
        tutorialTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(true);

    }

    private void ButtonText(string s){
        uiManager.dialogueManager.buttonText.text = s;
    }

    public void PressedNextButton(){
        if (advanceCondition == Cond.Click)
            AdvanceTutorialStep();
    }

    private void LoadCustomPuzzle(){
        puzzleGenerator.FillPuzzleFromList(startingLayout);
    }

    public override bool CanAddLetter(LetterSpace letterSpace){
        if (!base.CanAddLetter(letterSpace))
            return false;
        switch (advanceCondition){
            case (Cond.SelectLetter):
                return (letterSpace.letter == requiredLetter);
            case (Cond.FinishWord):
                foreach (char c in requiredWord)
                    if (letterSpace.letter == c)
                        return true;
                return false;                        
        }

        return false;
    }

    public override bool CanRemoveLetter(LetterSpace letterSpace){
        if (!base.CanRemoveLetter(letterSpace))
            return false;
        switch (advanceCondition){
            case (Cond.SubmitWord):
                return false;
        }
        return true;
    }

    public override void AddLetter(LetterSpace ls){
        base.AddLetter(ls);

        switch (advanceCondition){
            case (Cond.SelectLetter):
                if (ls.letter == requiredLetter)
                    AdvanceTutorialStep();
                break;
            case (Cond.FinishWord):
                if (word == new string(requiredWord))
                    AdvanceTutorialStep();
                break;
        }
    }

    public override void PressSubmitWordButton(){        
        switch (advanceCondition){
            case (Cond.SubmitWord):
                base.PressSubmitWordButton();
                AdvanceTutorialStep();
                break;
        }
        if (!canCountdown)
            countdownToRefresh ++;
    }

    public override void DamageEnemyHealth(int amount){
        if (!canEnemyTakeDamage)
            enemyHealth += amount;
        base.DamageEnemyHealth(amount);
        if (!canEnemyTakeDamage)
            HideHealthDisplays();    

        switch (advanceCondition){
            case (Cond.EnemyTakesDamage):
                AdvanceTutorialStep();
                break;
        }
    }

    public override void EnemyDeathAnimationFinished(){
        switch (advanceCondition){
            case (Cond.EnemyDies):
                AdvanceTutorialStep();
                break;
        }
    }
}
 
