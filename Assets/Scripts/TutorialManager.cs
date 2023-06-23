using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : BattleManager {

    private int tutorialStep = 0;
    private char[,] startingLayout = {{'-', '-', '-', '-', 'A'}, {'O', 'R', '-', 'L', 'V'}, {'W', 'S', 'D', 'S', 'I'}, {'-', 'A', 'R', 'R', 'U'}, {'-', '-', '-', 'P', 'Y'}, {'-', 'L', 'A', 'T', '-'}, {'P', '-', '-', '-', '-'}};


    public override void Start() {
        base.Start();
        powerupsPerPuzzle = 0;
        SetupDialogueManager();
        HideSubmitButtonExtras();
        HideHealthDisplays();
        //HidePowerups();
        AdvanceTutorialStep();
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
                ButtonText("...");
                LoadCustomPuzzle();
                break;
        }
    }

    private void SetupDialogueManager(){
        uiManager.dialogueManager.tutorialManager = this;
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

    private void DisplayText(string s){
        uiManager.dialogueManager.dialogueTextBox.text = s;
    }

    private void ButtonText(string s){
        uiManager.dialogueManager.buttonText.text = s;
    }

    public void PressedNextButton(){
        print("next");
    }

    private void LoadCustomPuzzle(){
        puzzleGenerator.FillPuzzleFromList(startingLayout);
    }
}
 
