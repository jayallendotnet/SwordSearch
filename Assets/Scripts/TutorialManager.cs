using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : BattleManager {

    private int tutorialStep = 0;
    private char[,] startingLayout1 = {{'W', 'K', 'B', 'U', 'M'}, {'L', 'I', 'V', 'Y', 'P'}, {'A', 'D', 'O', 'G', 'E'}, {'C', 'H', 'I', 'R', 'Z'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};
    private char[,] startingLayout2 = {{'S', 'P', 'U', 'N', 'K'}, {'N', 'E', 'W', 'Y', 'S'}, {'T', 'E', 'A', 'G', 'S'}, {'L', 'O', 'M', 'I', 'T'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};
    private char[,] startingLayout3 = {{'O', 'G', 'E', 'N', 'N'}, {'F', 'R', 'I', 'L', 'U'}, {'E', 'A', 'O', 'Z', 'P'}, {'T', 'N', 'E', 'I', 'M'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};

    public enum Cond{Click, SelectLetter, FinishWord, SubmitWord, EnemyTakesDamage, EnemyDies, SubmitAnyWord, EnemyAttacks, PlayerTakesDamage, TurnPage, TurnPageEnds, NormalBattle};
    private Cond advanceCondition;
    private char requiredLetter;
    private char[] requiredWord;
    private bool canEnemyTakeDamage = false;
    private bool canCountdown = false;
    private bool canQueueAttack = false;
    private bool canShowStrength = false;
    private bool canShowCountdown = false;
    private bool canShowPlayerHealth = false;
    private bool canShowEnemyHealth = false;
    private bool canEnemyDie = true;

    
    [Header("Tutorial Stuff")]
    public int tutorialNumber;
    public Text tutorialTextBox;
    public GameObject highlightWord1;
    public GameObject highlightWord2;
    public GameObject highlightWord3;
    public GameObject highlightSubmit;
    public GameObject highlightEnemyHealth;
    public GameObject highlightAttackStrength;
    public GameObject highlightEnemyTimer;
    public GameObject highlightPlayerHealth;
    public GameObject highlightCountdown;
    public TextAsset wordLibraryForSmallerPuzzles;
    

    public override void Start() {
        wordLibraryForGenerationFile = wordLibraryForSmallerPuzzles;
        puzzleGenerator.wordCount = 4;
        puzzleGenerator.useSmallerLayout = true;
        
        //puzzleGenerator.wordLibraryForGeneration = wordLibraryForGenerationFile.text.Split("\r\n");
        base.Start();
        SetTutorialNumber();
        switch (tutorialNumber){
            case (1):
                SetupTutorial1();
                break;
            case (2):
                SetupTutorial2();
                break;
            case (3):
                SetupTutorial3();
                break;
            case (4):
                SetupTutorial4();
                break;
        }
        SetupDialogueManager();
        UpdateSubmitVisuals();
        ButtonText("CONTINUE");
        AdvanceTutorialStep();
    }

    private void SetTutorialNumber(){
        string name = enemyData.name;
        string[] split = name.Split(" ");
        string n = split[1];
        tutorialNumber = int.Parse(n);
        //print(tutorialNumber);
        if ((tutorialNumber > 4) || (tutorialNumber < 1))
            print("tutorial number is wrong! number is " + tutorialNumber + ", retrieved from enemy name " + name);
    }

    private void SetupTutorial1(){
        powerupsPerPuzzle = 0;
        HideHealthDisplays();
        canEnemyTakeDamage = false;
        canCountdown = false;
        canQueueAttack = false;
        canShowStrength = false;
        canShowCountdown = false;
        canShowPlayerHealth = false;
        canShowEnemyHealth = false;
        canEnemyDie = true;
    }
    
    private void SetupTutorial2(){
        powerupsPerPuzzle = 0;
        HidePlayerHealth();
        canEnemyTakeDamage = true;
        canCountdown = false;
        canQueueAttack = false;
        canShowStrength = false;
        canShowCountdown = false;
        canShowPlayerHealth = false;
        canShowEnemyHealth = true;
        canEnemyDie = false;
    }
    
    private void SetupTutorial3(){
        
    }
    
    private void SetupTutorial4(){
        
    }

    public override void QueueEnemyAttack(){
        if (canQueueAttack)
            base.QueueEnemyAttack();
    }


    public override void UpdateSubmitVisuals(){
        base.UpdateSubmitVisuals();
        if (!canShowCountdown)
            HideCountdown();
        if (!canShowStrength)
            HideStrength();
    }

    private void AdvanceTutorialStep(){
        tutorialStep ++;
        switch (tutorialNumber){
            case (1):
                DoTutorial1Step();
                break;
            case (2):
                DoTutorial2Step();
                break;
            case (3):
                DoTutorial3Step();
                break;
            case (4):
                DoTutorial4Step();
                break;
        }
        switch (advanceCondition){
            case (Cond.Click):
                ToggleButton(true);
                break;
            default:
                ToggleButton(false);
                break;
        }
    }

    private void DoTutorial1Step(){        
        switch (tutorialStep){
            case (1):
                DisplayText("Tap a letter to select it. Start by selecting the letter 'D'.");
                LoadCustomPuzzle(startingLayout1);
                puzzleGenerator.letterSpaces[2,1].ToggleTutorialSelector(true);
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
                DisplayText("Try to make the word 'BUMPY' and attack the goblin.");
                advanceCondition = Cond.FinishWord;
                requiredWord = new char[] {'B', 'U', 'M', 'P', 'Y'};
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
    }
    private void DoTutorial2Step(){
        switch (tutorialStep){
            case (1):
                DisplayText("Enemies have varying amounts of health, shown above the submit button.");
                LoadCustomPuzzle(startingLayout2);
                highlightEnemyHealth.SetActive(true);
                advanceCondition = Cond.Click;
                break;
            case (2):
                DisplayText("Find a word and attack with it to damage the enemy's health!");
                highlightEnemyHealth.SetActive(false);
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (3):
                DisplayText("");
                advanceCondition = Cond.EnemyTakesDamage;
                break;
            case (4):
                DisplayText("A word has to be at least 3 letters long to be used for an attack.");
                advanceCondition = Cond.Click;
                break;
            case (5):
                DisplayText("While you are making a word, the attack's damage is shown on the submit button.");
                highlightAttackStrength.SetActive(true);
                canShowStrength = true;
                UpdateSubmitVisuals();
                uiManager.wordStrengthIcon.gameObject.SetActive(true);
                advanceCondition = Cond.Click;
                break;
            case (6):
                DisplayText("The longer a word is, the more damage the attack will do!");
                advanceCondition = Cond.Click;
                break;
            case (7):
                DisplayText("Keep making attacks to damage the goblin!");
                highlightAttackStrength.SetActive(false);
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (8):
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (9):
                DisplayText("");
                advanceCondition = Cond.EnemyTakesDamage;
                break;
            case (10):
                DisplayText("While you are making words, your enemies can attack you!");
                advanceCondition = Cond.Click;
                break;
            case (11):
                DisplayText("The bar next to the enemy's health is a timer.");
                highlightEnemyTimer.SetActive(true);
                advanceCondition = Cond.Click;
                break;
            case (12):
                DisplayText("When the bar is empty, the enemy will attack!");
                advanceCondition = Cond.Click;
                break;
            case (13):
                DisplayText("Your health is shown on the other side of the enemy attack timer.");
                highlightEnemyTimer.SetActive(false);
                highlightPlayerHealth.SetActive(true);
                canShowPlayerHealth = true;
                uiManager.DisplayHealths(playerHealth, enemyHealth);
                advanceCondition = Cond.Click;
                break;
            case (14):
                DisplayText("The enemy's attacks will damage your health.");
                advanceCondition = Cond.Click;
                break;
            case (15):
                DisplayText("The goblin will attack soon. Keep an eye on the attack timer!");
                highlightEnemyTimer.SetActive(true);
                highlightPlayerHealth.SetActive(false);
                canQueueAttack = true;
                QueueEnemyAttack();
                canQueueAttack = false;
                advanceCondition = Cond.EnemyAttacks;
                break;
            case (16):
                DisplayText("");
                highlightEnemyTimer.SetActive(false);
                advanceCondition = Cond.PlayerTakesDamage;
                break;
            case (17):
                DisplayPlayerTalking("Ouch! That hurt!", DialogueStep.Emotion.Angry);
                highlightEnemyTimer.SetActive(false);
                advanceCondition = Cond.Click;
                break;
            case (18):
                DisplayText("If you run out of health, you'll lose and have to try the battle again!");
                advanceCondition = Cond.Click;
                break;
            case (19):
                DisplayText("Try to get the enemy's health to 0 before your health gets depleted.");
                TurnToPredefinedPage(startingLayout3);
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (20):
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (21):
                advanceCondition = Cond.SubmitAnyWord;
                break;
            case (22):
                DisplayText("It looks like you are running out of letters!");
                advanceCondition = Cond.Click;
                break;
            case (23):
                DisplayText("You can click the submit button to turn to another page in the book.");
                highlightSubmit.SetActive(true);
                countdownToRefresh = 0;
                canCountdown = true;
                UpdateSubmitVisuals();
                HideCountdown();
                advanceCondition = Cond.TurnPage;
                break;
            case (24):
                DisplayText("");
                UpdateSubmitVisuals();
                highlightSubmit.SetActive(false);
                advanceCondition = Cond.TurnPageEnds;
                break;
            case (25):
                DisplayText("After you turn a page, you can't turn the page again immediately.");
                advanceCondition = Cond.Click;
                break;
            case (26):
                DisplayText("On the left side of the submit button is a countdown.");
                highlightCountdown.SetActive(true);
                canShowCountdown = true;
                ShowCountdown();
                UpdateSubmitVisuals();
                advanceCondition = Cond.Click;
                break;
            case (27):
                DisplayText("Every time you make an attack or get attacked, the number will go down.");
                advanceCondition = Cond.Click;
                break;
            case (28):
                DisplayText("When the number hits 0, you can turn the page again and get new letters.");
                advanceCondition = Cond.Click;
                break;
            case (29):
                DisplayText("Keep battling the goblin, and don't forget to turn the page when you need to.");
                highlightCountdown.SetActive(false);
                canEnemyDie = true;
                canQueueAttack = true;
                QueueEnemyAttack();
                advanceCondition = Cond.NormalBattle;
                break;
            case (30):
                DisplayText("");
                advanceCondition = Cond.EnemyDies;
                break;
            case (31):
                DisplayEnemyTalking("I can't stand up to your strange magics!!", enemyData, DialogueStep.Emotion.Defeated);
                advanceCondition = Cond.Click;
                break;
            case (32):
                DisplayPlayerTalking("I beat another one! That was a tough fight!", DialogueStep.Emotion.Happy);
                advanceCondition = Cond.Click;
                break;
            case (33):
                uiManager.EndDialogue();
                break;
        }
    }
    private void DoTutorial3Step(){
        
    }
    private void DoTutorial4Step(){
        
    }

    public override void TurnPageEnded(){
        switch(advanceCondition){
            case (Cond.TurnPageEnds):
                AdvanceTutorialStep();
                break;
        }
    }

    
    private void TurnToPredefinedPage(char[,] layout){
        puzzleGenerator.GenerateNextPuzzleForTutorial(layout);
        countdownToRefresh = maxPuzzleCountdown; 
        ClearWord(true); 
        uiManager.ShowPageTurn();  
    }
    
    
    private void TurnToSmallerPage(){
        puzzleGenerator.GenerateNewPuzzle();
        countdownToRefresh = maxPuzzleCountdown; 
        ClearWord(true); 
        uiManager.ShowPageTurn();  

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

    private void HideCountdown(){
        uiManager.countdownDivider.SetActive(false);
        uiManager.countdownNumber.gameObject.SetActive(false);
        uiManager.countdownNumber.transform.parent.Find("Countdown Icon").gameObject.SetActive(false);
    }

    private void ShowCountdown(){
        uiManager.countdownDivider.SetActive(true);
        uiManager.countdownNumber.gameObject.SetActive(true);
        uiManager.countdownNumber.transform.parent.Find("Countdown Icon").gameObject.SetActive(true);
    }

    private void HideStrength(){
        uiManager.wordStrengthDivider.SetActive(false);
        uiManager.wordStrengthImageOnes.gameObject.SetActive(false);
        uiManager.wordStrengthImageTens.gameObject.SetActive(false);
        uiManager.wordStrengthImageSingle.gameObject.SetActive(false);
        uiManager.wordStrengthIcon.gameObject.SetActive(false);
    }

    private void HideHealthDisplays(){
        HidePlayerHealth();
        HideEnemyHealth();
    }

    private void HidePlayerHealth(){
        uiManager.playerHP1DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.playerHP2DigitOnes.transform.parent.gameObject.SetActive(false);
        uiManager.playerHP3DigitOnes.transform.parent.gameObject.SetActive(false);
    }

    private void HideEnemyHealth(){
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

    private void LoadCustomPuzzle(char[,] layout){
        puzzleGenerator.FillPuzzleFromList(layout);
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
            case (Cond.SubmitAnyWord):
                return true;  
            case (Cond.NormalBattle):
                return true;                  
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
            case (Cond.SubmitAnyWord):
                if (isValidWord){
                    base.PressSubmitWordButton();
                    AdvanceTutorialStep();
                }
                break;
            case (Cond.TurnPage):
                //TurnToPredefinedPage(startingLayout2);   
                TurnToSmallerPage();
                AdvanceTutorialStep();
                break;
            case (Cond.NormalBattle):
                base.PressSubmitWordButton();
                break;
        }
        if (!canCountdown)
            countdownToRefresh ++;
    }

    public override void DamageEnemyHealth(int amount){
        if (!canEnemyTakeDamage)
            enemyHealth += amount;
        if (!canEnemyDie && amount >= enemyHealth)
            amount = enemyHealth -1;
        base.DamageEnemyHealth(amount);
        if (!canEnemyTakeDamage)
            HideHealthDisplays();    
        if (!canShowEnemyHealth)
            HideEnemyHealth();
        if (!canShowPlayerHealth)
            HidePlayerHealth();

        switch (advanceCondition){
            case (Cond.EnemyTakesDamage):
                AdvanceTutorialStep();
                break;
            case (Cond.NormalBattle):
                if (enemyHealth <= 0)
                    AdvanceTutorialStep();
                break;
        }
    }

    public override void DamagePlayerHealth(int amount){
        base.DamagePlayerHealth(amount);
        switch (advanceCondition){
            case (Cond.PlayerTakesDamage):
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

    public override void TriggerEnemyAttack(){
        base.TriggerEnemyAttack();
        switch (advanceCondition){
            case (Cond.EnemyAttacks):
                AdvanceTutorialStep();
                break;
        }
        
    }
}
 
