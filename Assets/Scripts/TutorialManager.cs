using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TutorialManager : BattleManager {

    private int tutorialStep = 0;
    private char[,] startingLayout1 = {{'W', 'K', 'B', 'U', 'M'}, {'L', 'I', 'V', 'Y', 'P'}, {'A', 'D', 'O', 'G', 'E'}, {'C', 'H', 'I', 'R', 'Z'}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}};
    private char[,] startingLayout2 = {{'S', 'P', 'U', 'N', 'K'}, {'N', 'E', 'W', 'Y', 'S'}, {'T', 'E', 'A', 'G', 'S'}, {'L', 'O', 'M', 'I', 'T'}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}};
    private char[,] startingLayout3 = {{'O', 'G', 'E', 'N', 'N'}, {'F', 'R', 'I', 'L', 'U'}, {'E', 'A', 'O', 'Z', 'P'}, {'T', 'N', 'E', 'I', 'M'}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}};
    private char[,] startingLayout4 = {{'C', 'A', 'L', 'D', 'E'}, {'E', 'S', 'I', 'N', 'M'}, {'T', 'A', 'E', 'B', 'R'}, {'T', 'K', 'C', 'O', 'L'}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}};
    private char[,] startingLayout5 = {{'B', 'R', 'X', 'A', 'F'}, {'A', 'N', 'E', 'D', 'U'}, {'L', 'I', 'A', 'L', 'T'}, {'G', 'O', 'W', 'E', 'I'}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}, {' ', ' ', ' ', ' ', ' '}};

    private char[,] startingPowerups1 = {{'-', '-', '-', '-', '-'}, {'-', 'H', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', 'W', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};
    private char[,] startingPowerups2 = {{'-', 'E', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', 'E', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}, {'-', '-', '-', '-', '-'}};


    private enum Cond{Click, CompleteWord, ReleaseFinger, SubmitWord, EnemyTakesDamage, EnemyDies, SubmitAnyWord, EnemyAttacks, PlayerTakesDamage, TurnPage, TurnPageEnds, NormalBattle, SubmitHealingWord, PlayerGetsHealed, SubmitWaterWord, WaterFillsPage, WaterDrains, SubmitEarthWord};
    private Cond advanceCondition;
    private char[] requiredWord;
    private bool canEnemyTakeDamage = false;
    private bool canCountdown = false;
    private bool canQueueAttack = false;
    private bool canShowStrength = false;
    private bool canShowCountdown = false;
    private bool canShowPlayerHealth = false;
    private bool canShowEnemyHealth = false;
    private bool canEnemyDie = true;

    private float originalFloatDuration;

    
    [Header("Tutorial Stuff")]
    public int tutorialNumber;
    public Text tutorialTextBox;
    public GameObject highlightWord1;
    public GameObject highlightWord2;
    public GameObject highlightWord3;
    public GameObject highlightWordArea;
    public GameObject highlightEnemyHealth;
    public GameObject highlightAttackStrength;
    public GameObject highlightEnemyTimer;
    public GameObject highlightPlayerHealth;
    public GameObject highlightCountdown;
    public Image tutorialShadow;
    public Image tutorialShadowForLetters;
    private float enemyTimerBarRemainder = 0f;
    private Color tutorialShadowOriginalColor;
    //private Color tutorialShadowTransparentColor;


    public override void Start() {
        puzzleGenerator.wordCount = 4;
        puzzleGenerator.useSmallerLayout = true;

        

        tutorialShadowOriginalColor = tutorialShadow.color;
        //print(tutorialShadowOriginalColor.a);
        //tutorialShadowTransparentColor = tutorialShadow.color;
        //tutorialShadowTransparentColor.a = 0;
        //print(tutorialShadowTransparentColor.a);
        //print(tutorialShadowOriginalColor.a);

        base.Start();
        SetTutorialNumber();
        switch (tutorialNumber) {
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

        uiManager.pauseButton.gameObject.SetActive(false);
    }

    private void SetTutorialNumber() {
    string name = enemyData.name;
    string[] split = name.Split(" ");
    string n = split[1];
    tutorialNumber = int.Parse(n);
    //print(tutorialNumber);
    if ((tutorialNumber > 4) || (tutorialNumber < 1))
        print("tutorial number is wrong! number is " + tutorialNumber + ", retrieved from enemy name " + name);
}

    private void SetupTutorial1(){
        StaticVariables.powerupsPerPuzzle = 0;
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
        StaticVariables.powerupsPerPuzzle = 0;
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
        StaticVariables.powerupsPerPuzzle = 0;
        canEnemyTakeDamage = true;
        canCountdown = true;
        canQueueAttack = false;
        canShowStrength = true;
        canShowCountdown = true;
        canShowPlayerHealth = true;
        canShowEnemyHealth = true;
        canEnemyDie = false;
    }

    private void SetupTutorial4(){
        StaticVariables.powerupsPerPuzzle = 0;
        StaticVariables.waterActive = false;
        StaticVariables.healActive = false;
        puzzleGenerator.SetPowerupTypeList();
        canEnemyTakeDamage = true;
        canCountdown = true;
        canQueueAttack = false;
        canShowStrength = true;
        canShowCountdown = true;
        canShowPlayerHealth = true;
        canShowEnemyHealth = true;
        canEnemyDie = false;
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
        switch (tutorialNumber){
            case (1):
                AdvanceTutorial1();
                break;
            case (2):
                AdvanceTutorial2();
                break;
            case (3):
                AdvanceTutorial3();
                break;
            case (4):
                AdvanceTutorial4();
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
    


    private void AdvanceTutorial1(){
        tutorialStep ++;
        int i = 0;
        if (++i == tutorialStep){
            DisplayText("Swipe some letters to select a word. Start by spelling the word 'DOG'.");//Tap a letter to select it. Start by selecting the letter 'D'.");
            LoadCustomPuzzle(startingLayout1);
            highlightWord1.SetActive(true);
            advanceCondition = Cond.CompleteWord;
            requiredWord = new char[] {'D', 'O', 'G'};

        }
        else if (++i == tutorialStep){
            DisplayText("Once you have completed a word, you can let go. Lift your finger to send a magical attack at your opponent!");
            advanceCondition = Cond.ReleaseFinger;
            highlightWord1.SetActive(false);
        }
        else if (++i == tutorialStep){                
            DisplayText("");
            advanceCondition = Cond.EnemyTakesDamage;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayPlayerTalking("Whoa! The book shot a blast of water at the goblin!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayEnemyTalking("Why am I wet??", enemyData, DialogueStep.Emotion.Angry);
            uiManager.enemyAnimator.GetComponent<RectTransform>().parent.localScale = new Vector3(-1, 1, 1);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("Oh no, the goblin noticed me!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Try to make the word 'BUMPY' and attack the goblin.");
            advanceCondition = Cond.SubmitWord;
            requiredWord = new char[] {'B', 'U', 'M', 'P', 'Y'};
            highlightWord2.SetActive(true);
        }
        else if (++i == tutorialStep){
            DisplayText("");
            highlightWord2.SetActive(false);
            advanceCondition = Cond.EnemyTakesDamage;
        }        
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("Letters don't have to be in all in one row to make a word!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("As long as the letters are connected, you can make a word with them.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Try to make the word 'CHALK'.");
            advanceCondition = Cond.SubmitWord;
            requiredWord = new char[] {'C', 'H', 'A', 'L', 'K'};
            highlightWord3.SetActive(true);
        }
        else if (++i == tutorialStep){
            DisplayText("");
            highlightWord3.SetActive(false);
            advanceCondition = Cond.EnemyTakesDamage;
        }
        else if (++i == tutorialStep){
            uiManager.enemyAnimator.Play("Die");
            advanceCondition = Cond.EnemyDies;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayPlayerTalking("I managed to defeat one of the invading goblins!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("This book has some powerful magic...", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            uiManager.EndDialogue();
        }
    }

     private void AdvanceTutorial2(){
        tutorialStep ++;
        int i = 0;
        if (++i == tutorialStep){
            ShowTutorialShadow(true);
            DisplayText("An enemy's health is shown above, in red.");
            LoadCustomPuzzle(startingLayout2);
            highlightEnemyHealth.SetActive(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Find a word and attack with it to damage the enemy's health!");
            highlightEnemyHealth.SetActive(false);
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){   
            DisplayText("");
            advanceCondition = Cond.EnemyTakesDamage;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("A word has to be at least 3 letters long to be used for an attack.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("While you are making a word, you can see how much damage the attack will do, shown under the enemy's health.");
            highlightAttackStrength.SetActive(true);
            canShowStrength = true;
            UpdateSubmitVisuals();
            uiManager.wordStrengthIconGameObjects[0].SetActive(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The longer a word is, the more damage you will deal!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Keep making attacks to damage the goblin!");
            highlightAttackStrength.SetActive(false);
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            advanceCondition = Cond.EnemyTakesDamage;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("While you're making words, the enemies can attack you too!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Your health is also shown above, in blue.");
            highlightPlayerHealth.SetActive(true);
            canShowPlayerHealth = true;
            uiManager.DisplayHealths(playerHealth, enemyHealth);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The enemy's attacks will damage your health.");
            highlightPlayerHealth.SetActive(false);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Between your health and the enemy's health is a brown bar. This bar is a timer.");
            highlightEnemyTimer.SetActive(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("When the bar is empty, the enemy will attack!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The goblin will attack soon. Keep an eye on the attack timer!");
            highlightEnemyTimer.SetActive(true);
            canQueueAttack = true;
            QueueEnemyAttack();
            canQueueAttack = false;
            advanceCondition = Cond.EnemyAttacks;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("");
            highlightEnemyTimer.SetActive(false);
            advanceCondition = Cond.PlayerTakesDamage;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayPlayerTalking("Ouch! That hurt!", DialogueStep.Emotion.Angry);
            highlightEnemyTimer.SetActive(false);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){                
            DisplayText("If you run out of health, you'll lose and have to try the battle again!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Try to get the enemy's health to 0 before your health gets depleted.");
            TurnToPredefinedPage(startingLayout3);
            canQueueAttack = true;
            QueueEnemyAttack();
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){
            advanceCondition = Cond.SubmitAnyWord;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("It looks like you are running out of letters!");
            advanceCondition = Cond.Click;
            canQueueAttack = false;
            DOTween.Clear(uiManager.enemyTimerBar);
            enemyTimerBarRemainder = uiManager.enemyTimerBar.localScale.x;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Tapping the big space at the top of the page will turn to another page in the book.");
            highlightWordArea.SetActive(true);
            countdownToRefresh = 0;
            canCountdown = true;
            UpdateSubmitVisuals();
            HideCountdown();
            advanceCondition = Cond.TurnPage;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            UpdateSubmitVisuals();
            highlightWordArea.SetActive(false);
            advanceCondition = Cond.TurnPageEnds;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("This new page has a brand new set of letters, with the potential to make completely different words!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("After you turn a page, you have to wait before another page becomes available.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Up above on the left is a countdown.");
            highlightCountdown.SetActive(true);
            canShowCountdown = true;
            ShowCountdown();
            UpdateSubmitVisuals();
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Every time you make an attack or get attacked, the countdown number will go down.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("When the number hits 0, you can turn the page again and get new letters.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Keep battling the goblin, and don't forget to turn the page when you need to.");
            highlightCountdown.SetActive(false);
            canEnemyDie = true;
            canQueueAttack = true;
            //resume the attack where it was paused
            float remainingTime = enemyTimerBarRemainder * enemyData.attackOrder.Value[0].attackSpeed;
            uiManager.enemyTimerBar.DOScale(new Vector3(0,1,1), remainingTime).SetEase(Ease.Linear).OnComplete(TriggerEnemyAttack);
            advanceCondition = Cond.NormalBattle;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            advanceCondition = Cond.EnemyDies;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayEnemyTalking("Human, I don't like your weird magic!", enemyData, DialogueStep.Emotion.Defeated);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("I love it! This book is incredible!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            uiManager.EndDialogue();
        }
    }


    private void AdvanceTutorial3(){
        tutorialStep ++;
        int i = 0;
        if (++i == tutorialStep){
            ShowTutorialShadowForLetters(true);
            DisplayText("The true magic within the book has awakened!");
            LoadCustomPuzzle(startingLayout4);
            puzzleGenerator.SetCustomPowerups(startingPowerups1);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Some letters are now glowing with special powerups.");
            puzzleGenerator.letterSpaces[1,1].ToggleTutorialSelector(true);
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("First is the <healing>power of healing<>, which has a light green color.");
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(false);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("If you make a word with the <healing>power of healing<>, your health will be restored.");
            advanceCondition = Cond.Click;
        }
        //else if (++i == tutorialStep){
        //    DisplayText("The powerup does not have to be in the first letter of the word to take effect.");
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == tutorialStep){
            HideTutorialShadowForLetters();
            DisplayText("Try making a word that includes the <healing>power of healing<>.");
            advanceCondition = Cond.SubmitHealingWord;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            puzzleGenerator.letterSpaces[1,1].ToggleTutorialSelector(false);
            advanceCondition = Cond.PlayerGetsHealed;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadowForLetters();
            DisplayPlayerTalking("Wow! I feel so refreshed!", DialogueStep.Emotion.Happy);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The <healing>power of healing<> heals you for " + StaticVariables.healMultiplier + " times the strength of the attack.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("This means the <healing>power of healing<> is more effective with longer words!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The <water>power of water<> is better for dealing more damage.");
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("The <water>power of water<> is blue.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadowForLetters();
            DisplayText("Try making a word that includes the <water>power of water<>.");
            advanceCondition = Cond.SubmitWaterWord;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(false);
            advanceCondition = Cond.WaterFillsPage;
            originalFloatDuration = uiManager.waterFloatDuration;
            uiManager.waterFloatDuration = -1;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("After making an attack with the <water>power of water<>, the book's pages are <water>flooded<>!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("For the next " + StaticVariables.waterFloodDuration + " seconds, every attack you make does an extra <damage>+" + StaticVariables.waterFloodDamageBonus + " damage<>!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("To make the most of the <water>power of water<>, make as many words as you can in the next " + StaticVariables.waterFloodDuration + " seconds.");
            ButtonText("READY!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            DisplayText("Attack!!");
            uiManager.StartDrainingWaterSmallerPage();
            advanceCondition = Cond.WaterDrains;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("From now on, both <healing>healing<> and <water>water<> powerups will appear!");
            ButtonText("CONTINUE");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Keep using powerups to defeat the goblin!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            canEnemyDie = true;
            StaticVariables.powerupsPerPuzzle = 3;
            TurnToSmallerPage();
            UpdateSubmitVisuals();
            canQueueAttack = true;
            QueueEnemyAttack();
            uiManager.waterFloatDuration = originalFloatDuration;
            advanceCondition = Cond.NormalBattle;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            advanceCondition = Cond.EnemyDies;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayPlayerTalking("Look, Fido is alive, over there!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayEnemyTalking("He is??", enemyData, DialogueStep.Emotion.Defeated);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("Yes! Now go take care of your little dog, and get out of my way!", DialogueStep.Emotion.Angry);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            uiManager.EndDialogue();
        }
    }


    private void AdvanceTutorial4(){
        tutorialStep ++;
        int i = 0;
        if (++i == tutorialStep){
            ShowTutorialShadowForLetters(true);
            DisplayPlayerTalking("I really shouldn't use the <water>power of water<> here.", DialogueStep.Emotion.Normal);
            LoadCustomPuzzle(startingLayout5);
            puzzleGenerator.SetCustomPowerups(startingPowerups2);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("Who knows what would happen if I flooded all of these burrows!", DialogueStep.Emotion.Worried);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayPlayerTalking("Okay, book. Let's see what this <earth>power of earth<> is all about!", DialogueStep.Emotion.Normal);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("From now on, <earth>earth powerups<> will appear!");
            puzzleGenerator.letterSpaces[0,1].ToggleTutorialSelector(true);
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadowForLetters();
            DisplayText("Try making an attack with one of the <earth>earth powerups<>!");
            puzzleGenerator.letterSpaces[0,1].ToggleTutorialSelector(false);
            puzzleGenerator.letterSpaces[3,3].ToggleTutorialSelector(false);
            advanceCondition = Cond.SubmitEarthWord;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            advanceCondition = Cond.EnemyTakesDamage;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayText("After using the <earth>power of earth<>, magic crystals surround you.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("These magical crystals glow purple and appear above your health.");
            highlightPlayerHealth.SetActive(true);
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Currently, there are 3 crystals around you.");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("When you make another attack, a crystal will strike the enemy and deal <damage>+33% extra damage<>!");
            highlightPlayerHealth.SetActive(false);
            advanceCondition = Cond.Click;
        }
        //else if (++i == tutorialStep){
        //    DisplayText("When you make another attack, a crystal will strike the enemy and deal <damage>extra damage<>!");
        //    highlightPlayerHealth.SetActive(false);
        //    advanceCondition = Cond.Click;
        //}
        //else if (++i == tutorialStep){
            //DisplayText("The crystal damage is a fixed portion of your attack's damage.");
        //    DisplayText("The crystal's damage is <damage>one third<> of your attack's damage.");
        //    advanceCondition = Cond.Click;
        //}
        else if (++i == tutorialStep){
            DisplayText("This means you should make longer words to get the most damage from each crystal!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayText("Use the <earth>power of earth<> and the magical crystals to defeat the rabbits!");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            HideTutorialShadow();
            canEnemyDie = true;
            StaticVariables.powerupsPerPuzzle = 3;
            StaticVariables.healActive = true;
            puzzleGenerator.SetPowerupTypeList();
            TurnToSmallerPage();
            UpdateSubmitVisuals();
            canQueueAttack = true;
            QueueEnemyAttack();
            advanceCondition = Cond.NormalBattle;
        }
        else if (++i == tutorialStep){
            DisplayText("");
            advanceCondition = Cond.EnemyDies;
        }
        else if (++i == tutorialStep){
            ShowTutorialShadow();
            DisplayEnemyTalking("Is she using <earth>earth magic<>?", enemyData, DialogueStep.Emotion.Custom1, "Hopsy");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayEnemyTalking("Oh, no! What will <earth>earth magic<> do to my carrots??", enemyData, DialogueStep.Emotion.Custom2, "Fopsy");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayEnemyTalking("Don't you mean OUR carrots?", enemyData, DialogueStep.Emotion.Custom3, "Hopsy");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            DisplayEnemyTalking("Oh, get over yourself, Hopsy!", enemyData, DialogueStep.Emotion.Custom2, "Fopsy");
            advanceCondition = Cond.Click;
        }
        else if (++i == tutorialStep){
            uiManager.EndDialogue();
        }
    }


    public override void TurnPageEnded(){
        switch(advanceCondition){
            case (Cond.TurnPageEnds):
                AdvanceTutorialStep();
                break;
        }
    }

    public override void WaterReachedTopOfPage(){
        switch(advanceCondition){
            case (Cond.WaterFillsPage):
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
        ClearWord(true); 
        puzzleGenerator.GenerateNewPuzzle();
        countdownToRefresh = maxPuzzleCountdown; 
        uiManager.ShowPageTurn();  

    }
    
    private void ToggleButton(bool value){
        uiManager.dialogueManager.realButton.SetActive(value);
    }

    private void SetupDialogueManager(){
        print("setting up dialogue manager");
        uiManager.dialogueManager.tutorialManager = this;
        uiManager.dialogueManager.HideFakeButtons();
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
        foreach (GameObject go in uiManager.wordStrengthIconGameObjects)
            go.SetActive(false);
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
        //print("displaying text: " + s);
        s = TextFormatter.FormatString(s);
        tutorialTextBox.text = s;
        tutorialTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(false);
        HideChatheads();
    }

    private void DisplayPlayerTalking(string s, DialogueStep.Emotion emotion){
        s = TextFormatter.FormatString(s);
        uiManager.dialogueManager.dialogueTextBox.text = s;
        uiManager.dialogueManager.ShowPlayerTalking(emotion);
        tutorialTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(true);
    }

    private void DisplayEnemyTalking(string s, EnemyData enemyData, DialogueStep.Emotion emotion, string alternateName = ""){
        s = TextFormatter.FormatString(s);
        uiManager.dialogueManager.dialogueTextBox.text = s;
        uiManager.dialogueManager.ShowEnemyTalking(enemyData, emotion, alternateName);
        tutorialTextBox.gameObject.SetActive(false);
        uiManager.dialogueManager.dialogueTextBox.gameObject.SetActive(true);
        uiManager.dialogueManager.speakerNameTextBox.gameObject.SetActive(true);

    }

    private void ButtonText(string s){
        uiManager.dialogueManager.SetButtonText(s);
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
            case (Cond.CompleteWord):
                foreach (char c in requiredWord)
                    if (letterSpace.letter == c)
                        return true;
                return false;
            case (Cond.SubmitWord):
                foreach (char c in requiredWord)
                    if (letterSpace.letter == c)
                        return true;
                return false;    
            case (Cond.SubmitAnyWord):
                return true;  
            case (Cond.NormalBattle):
                return true;        
            case (Cond.SubmitHealingWord):
                return true;          
            case (Cond.SubmitWaterWord):
                return true;            
            case (Cond.SubmitEarthWord):
                return true;        
            case (Cond.WaterDrains):
                return true;              
        }
        return false;
    }

    public override bool CanRemoveLetter(LetterSpace letterSpace){
        if (!base.CanRemoveLetter(letterSpace))
            return false;
        switch (advanceCondition){
            case (Cond.SubmitWord):
                if (inProgressWord.word == new string(requiredWord))
                    return false;
                return true;
            case (Cond.ReleaseFinger):
                return false;
        }
        return true;
    }

    public override void AddLetter(LetterSpace ls){
        base.AddLetter(ls);

        switch (advanceCondition){
            case (Cond.CompleteWord):
                if (inProgressWord.word == new string(requiredWord))
                    AdvanceTutorialStep();
                break;
        }
    }

    public override void ProcessFingerRelease(){
        switch (advanceCondition){
            case (Cond.ReleaseFinger):
                base.ProcessFingerRelease();
                AdvanceTutorialStep();
                break;
            case (Cond.SubmitWord):
                if (inProgressWord.word == new string(requiredWord)){
                    base.ProcessFingerRelease();
                    AdvanceTutorialStep();
                }
                else
                    ClearWord(false);
                break;
            case (Cond.CompleteWord):
                ClearWord(false);
                break;
            case (Cond.SubmitAnyWord):
                if (inProgressWord.isValidWord){
                    base.ProcessFingerRelease();
                    AdvanceTutorialStep();
                }
                else
                    base.ProcessFingerRelease();
                break;
            case (Cond.NormalBattle):
                    base.ProcessFingerRelease();
                break;   
            case (Cond.SubmitHealingWord):
                if((inProgressWord.isValidWord) && (inProgressWord.type == BattleManager.PowerupTypes.Heal)){
                    base.ProcessFingerRelease();
                    AdvanceTutorialStep();
                }
                else
                    ClearWord(false);
                break;   
            case (Cond.SubmitWaterWord):
                if((inProgressWord.isValidWord) && (inProgressWord.type == BattleManager.PowerupTypes.Water)){
                    base.ProcessFingerRelease();
                    AdvanceTutorialStep();
                }
                else
                    ClearWord(false);
                break;  
            case (Cond.SubmitEarthWord):
                if((inProgressWord.isValidWord) && (inProgressWord.type == BattleManager.PowerupTypes.Earth)){
                    base.ProcessFingerRelease();
                    AdvanceTutorialStep();
                }
                else
                    ClearWord(false);
                break;      
            default:
                base.ProcessFingerRelease();
                break;
        }
        if (!canCountdown)
            countdownToRefresh ++;
    }

    public override void PressWordArea(){   
        switch (advanceCondition){
            case (Cond.TurnPage):  
                TurnToSmallerPage();
                AdvanceTutorialStep();
                break;
            case (Cond.NormalBattle):  
                base.PressWordArea();
                break;
            case (Cond.WaterDrains):  
                base.PressWordArea();
                break;
        }
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
        if (amount >= playerHealth)
            amount = playerHealth - 1;
        playerHealth -= amount;
        uiManager.ShowPlayerTakingDamage(amount, playerHealth > 0, showZeroDamage: amount == 0);
        uiManager.DisplayHealths(playerHealth, enemyHealth);

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
        if (!canCountdown)
            countdownToRefresh ++;
    }

    public override void ApplyHealToSelf(AttackData attackData){
        base.ApplyHealToSelf(attackData);
        switch (advanceCondition){
            case (Cond.PlayerGetsHealed):
                AdvanceTutorialStep();
                break;
        }
    }

    public override void WaterDrainComplete(){
        base.WaterDrainComplete();
        switch (advanceCondition){
            case (Cond.WaterDrains):
                AdvanceTutorialStep();
                break;
        }
    }
    
    private void ShowTutorialShadow(bool immediately = false){
        tutorialShadow.gameObject.SetActive(true);
        if (immediately)
            return;
        Color c = tutorialShadowOriginalColor;
        c.a = 0;
        tutorialShadow.color = c;
        tutorialShadow.DOFade(tutorialShadowOriginalColor.a, 0.5f);
    }
    
    private void HideTutorialShadow(){
        tutorialShadow.color = tutorialShadowOriginalColor;
        //tutorialShadow.DOFade(0, 0.5f).OnComplete(DisableTutorialShadow);
        tutorialShadow.DOFade(0, 0.5f);
    }

    //private void DisableTutorialShadow(){
    //    if (tutorialShadow.color.a == 0)
    //        tutorialShadow.gameObject.SetActive(false);
    //}
    private void ShowTutorialShadowForLetters(bool immediately = false){
        tutorialShadowForLetters.gameObject.SetActive(true);
        if (immediately)
            return;
        Color c = tutorialShadowOriginalColor;
        c.a = 0;
        tutorialShadow.color = c;
        tutorialShadowForLetters.DOFade(tutorialShadowOriginalColor.a, 0.5f);
        
    }
    
    private void HideTutorialShadowForLetters(){
        tutorialShadowForLetters.color = tutorialShadowOriginalColor;
        tutorialShadowForLetters.DOFade(0, 0.5f);
    }

    //private void DisableTutorialShadowForLetters(){
    //    if (tutorialShadowForLetters.color == tutorialShadowTransparentColor)
    //        tutorialShadowForLetters.gameObject.SetActive(false);
    //}
    //private void ShowTutorialShadowForLetters()
    //{
    //    tutorialShadowForLetters.SetActive(true);

    //}
    
    //private void HideTutorialShadowForLetters(){
    //    tutorialShadowForLetters.SetActive(false);
        
    //}
}
 
