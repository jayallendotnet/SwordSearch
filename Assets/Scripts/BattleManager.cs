using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    public WordDisplay wordDisplay;
    public Text enemyHealthText;
    public Text playerHealthText;

    public int startingEnemyHealth = 30;
    public int startingPlayerHealth = 30;
    public int maxHealth = 99; //for display purposes

    private int enemyHealth = 0;
    private int playerHealth = 0;
    public PuzzleGenerator puzzleGenerator;
    public TextAsset wordLibraryForCheckingFile; //all words that can be considered valid, even if they are not in the generating list
    private string[] wordLibraryForChecking;
    public int minCheckingWordLength = 3;
    public Text refreshPuzzleCountdownText;

    public int maxPuzzleCountdown = 3;
    public int currentPuzzleCountdown;
    public Color canRefreshPuzzleColor;
    public Color cannotRefreshPuzzleColor;

    public enum PowerupTypes{None, Water, Fire, Heal, Dark, Earth, Lightning};
    [HideInInspector]
    public System.Array powerupArray = PowerupTypes.GetValues(typeof(BattleManager.PowerupTypes));

    public Animator playerAnimator;

    private int currentAttackDamage = 0;
    private PowerupTypes currentAttackType;
    private int currentAttackStrength = 0;

    void Start(){
        wordLibraryForChecking = wordLibraryForCheckingFile.text.Split("\r\n");
        currentPuzzleCountdown = maxPuzzleCountdown;
        enemyHealth = startingEnemyHealth;
        playerHealth = startingPlayerHealth;
        ColorUtility.TryParseHtmlString("#8DE1FF", out canRefreshPuzzleColor);
        ColorUtility.TryParseHtmlString("#E7BD86", out cannotRefreshPuzzleColor);
        DisplayHealths();
    }
    
    private void DisplayHealths(){
        enemyHealthText.text = enemyHealth + "";
        playerHealthText.text = playerHealth + "";

    }

    private void ShowPlayerTakingDamage(int amount){
        if (amount > 0){
            if (playerHealth == 0){
                playerAnimator.SetTrigger("Die");
            }
            else
                playerAnimator.SetTrigger("TakeDamage");
        }
    }


    public bool IsValidWord(){
        string w = wordDisplay.word;
        if (w.Length < minCheckingWordLength)
            return false;
        if (SearchLibraryForWord(w))
            return true;
        return false;
    }

    private bool SearchLibraryForWord(string word){
        //returns true if the library contains the word
        int result = System.Array.BinarySearch<string>(wordLibraryForChecking, word);
        return (result > -1);
    }

    public int calculateDamage(){
        int val =  Mathf.FloorToInt(Mathf.Pow((wordDisplay.word.Length - 2), 2));
        if (wordDisplay.word.Length < 2)
            val = 0;
        return val;
    }

    private void DamageEnemyHealth(int amount){
        //print("damaging health by: " + amount);
        enemyHealth -= amount;
        if (enemyHealth < 0)
            enemyHealth = 0;
        DisplayHealths();
    }

    private void DamagePlayerHealth(int amount){
        playerHealth -= amount;
        if (playerHealth < 0)
            playerHealth = 0;
        DisplayHealths();
        ShowPlayerTakingDamage(amount);
    }

    public void PressSubmitWordButton(){
        if (IsValidWord()){
            if (wordDisplay.powerupTypeForWord == PowerupTypes.Heal)
                playerAnimator.SetTrigger("StartHeal");
            //if (wordDisplay.powerupTypeForWord != PowerupTypes.None)
            //    print("your word has a powerup! type[" + wordDisplay.powerupTypeForWord + "] strength[" + wordDisplay.powerupStrength + "]");
            else
                playerAnimator.SetTrigger("StartCast");
            SetCurrentAttackData();
            //DamageEnemyHealth(calculateDamage());
            //DamagePlayerHealth(20);
            DecrementRefreshPuzzleCountdown();
            wordDisplay.ClearWord();
        }
        else{
            if (wordDisplay.word.Length == 0){
                if (currentPuzzleCountdown == 0)
                    PressRefreshPuzzleButton();
            }
                

        }

    }

    private void SetCurrentAttackData(){
        currentAttackDamage = calculateDamage();
        currentAttackType = wordDisplay.powerupTypeForWord;
        currentAttackStrength = wordDisplay.powerupStrength;
    }

    public void ApplyAttackToEnemy(){
        print("attack hits enemy! damage [" + currentAttackDamage + "] type [" + currentAttackType + "] strength [" + currentAttackStrength +"]");
        DamageEnemyHealth(currentAttackDamage);
    }

    public void ApplyHealToSelf(){
        int healAmount = currentAttackDamage * 3;
        print("heal hits self! amount [" + healAmount + "] type [" + currentAttackType + "] strength [" + currentAttackStrength +"]");
        HealPlayerHealth(healAmount * 3);
    }

    private void HealPlayerHealth(int amount){
        playerHealth += amount;
        if (playerHealth > 99)
            playerHealth = 99;
        DisplayHealths();
    }

    private void DecrementRefreshPuzzleCountdown(){
        currentPuzzleCountdown --;
        if (currentPuzzleCountdown < 0)
            currentPuzzleCountdown = 0;
        UpdateRefreshPuzzleButton();
    }

    public void PressRefreshPuzzleButton(){
        if (currentPuzzleCountdown == 0){
            puzzleGenerator.GenerateNewPuzzle();
            currentPuzzleCountdown = maxPuzzleCountdown;
            wordDisplay.ClearWord();
            UpdateRefreshPuzzleButton();
        }
    }

    private void UpdateRefreshPuzzleButton(){
        refreshPuzzleCountdownText.text = "" + currentPuzzleCountdown;
    }


}
 
