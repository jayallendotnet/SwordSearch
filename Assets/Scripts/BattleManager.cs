using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour {

    private string word = "";
    
    [HideInInspector]
    public List<LetterSpace> letterSpacesForWord = new List<LetterSpace>(){};

    private LetterSpace lastLetterSpace = null;
    private LetterSpace secondToLastLetterSpace = null;

    [HideInInspector]
    public BattleManager.PowerupTypes powerupTypeForWord;
    [HideInInspector]
    public int powerupLevel;
    private int enemyHealth = 0;
    private int playerHealth = 0;
    private string[] wordLibraryForChecking;
    private int countdownToRefresh;
    public enum PowerupTypes{None, Water, Fire, Heal, Dark, Earth, Lightning};
    [HideInInspector]
    public System.Array powerupArray = PowerupTypes.GetValues(typeof(BattleManager.PowerupTypes));
    private int inProgressAttackStrength = 0;
    private PowerupTypes inProgressAttackType;
    private int inProgressAttackPowerupLevel = 0;

    private bool isValidWord = false;
    private int wordStrength = 0;


    [Header("Game Variables")]
    public int startingEnemyHealth = 30;
    public int startingPlayerHealth = 30;
    public int maxHealth = 99; //for display purposes
    public int minCheckingWordLength = 3;
    public int maxPuzzleCountdown = 3;

    [Header("Scripts")]
    public UIManager uiManager;
    public PuzzleGenerator puzzleGenerator;

    [Header("Libraries")]
    public TextAsset wordLibraryForGenerationFile; //all words that can be used to generate the puzzle
    public TextAsset wordLibraryForCheckingFile; //all words that can be considered valid, even if they are not in the generating list
    public TextAsset randomLetterPoolFile;


    void Start(){
        wordLibraryForChecking = wordLibraryForCheckingFile.text.Split("\r\n");
        countdownToRefresh = maxPuzzleCountdown;
        enemyHealth = startingEnemyHealth;
        playerHealth = startingPlayerHealth;

        uiManager.InitializeColors();
        uiManager.DisplayHealths(playerHealth, enemyHealth);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
    }
    




    public void SetIsValidWord(){
        if (word.Length < minCheckingWordLength)
            isValidWord = false;
        else
            isValidWord = SearchLibraryForWord(word);
    }

    private bool SearchLibraryForWord(string word){
        //returns true if the library contains the word
        int result = System.Array.BinarySearch<string>(wordLibraryForChecking, word);
        return (result > -1);
    }

    public void CalcWordStrength(){
        if (word.Length < 2)
            wordStrength = 0;
        else
            wordStrength =  Mathf.FloorToInt(Mathf.Pow((word.Length - 2), 2));
    }



    private void DamageEnemyHealth(int amount){
        //print("damaging health by: " + amount);
        enemyHealth -= amount;
        if (enemyHealth < 0)
            enemyHealth = 0;
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    private void DamagePlayerHealth(int amount){
        playerHealth -= amount;
        if (playerHealth < 0)
            playerHealth = 0;
        uiManager.ShowPlayerTakingDamage(amount, playerHealth > 0);
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    public void PressSubmitWordButton(){
        if (isValidWord){
            if (powerupTypeForWord == PowerupTypes.Heal)
                uiManager.StartPlayerHealAnimation();
            else
                uiManager.StartPlayerCastAnimation();
            SetCurrentAttackData();
            DecrementRefreshPuzzleCountdown();
            ClearWord();
        }
        else if ((word.Length == 0) && (countdownToRefresh == 0)){
            puzzleGenerator.GenerateNewPuzzle();
            countdownToRefresh = maxPuzzleCountdown;
            ClearWord();           
        }
    }

    private void SetCurrentAttackData(){
        inProgressAttackStrength = wordStrength;
        inProgressAttackType = powerupTypeForWord;
        inProgressAttackPowerupLevel = powerupLevel;
    }

    public void ApplyAttackToEnemy(){
        //print("attack hits enemy! damage [" + currentAttackDamage + "] type [" + currentAttackType + "] strength [" + currentAttackStrength +"]");
        DamageEnemyHealth(inProgressAttackStrength);
    }

    public void ApplyHealToSelf(){
        int healAmount = inProgressAttackStrength * 3;
        //print("heal hits self! amount [" + healAmount + "] type [" + currentAttackType + "] strength [" + currentAttackStrength +"]");
        HealPlayerHealth(healAmount * 3);
    }

    private void HealPlayerHealth(int amount){
        playerHealth += amount;
        if (playerHealth > 99)
            playerHealth = 99;
        uiManager.DisplayHealths(playerHealth, enemyHealth);
    }

    private void DecrementRefreshPuzzleCountdown(){
        countdownToRefresh --;
        if (countdownToRefresh < 0)
            countdownToRefresh = 0;
    }

    public void AddLetter(LetterSpace ls){
        word += ls.letter;
        SetIsValidWord();
        CalcWordStrength();
        //text.text += ls.letter;
        letterSpacesForWord.Add(ls);
        if (lastLetterSpace != null){
            lastLetterSpace.nextLetterSpace = ls;
            ls.previousLetterSpace = lastLetterSpace;
        }
        SetLastTwoLetterSpaces();
        UpdatePowerupTypeAndStrengthForWord();
        uiManager.UpdateColorsForWord(word, powerupTypeForWord);
        uiManager.UpdatePowerupIcon(powerupTypeForWord);
        uiManager.UpdateVisualsForLettersInWord(letterSpacesForWord);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
    }

    public void RemoveLetter(LetterSpace ls){
        //assumes the last letter in the list is the provided letter
        if (word.Length < 2)
            word = "";
        else
            word = word.Substring(0, (word.Length - 1));
        SetIsValidWord();
        CalcWordStrength();
        letterSpacesForWord.Remove(ls);
        ls.ShowAsNotPartOfWord();
        if (secondToLastLetterSpace != null){
            secondToLastLetterSpace.nextLetterSpace = null;
            lastLetterSpace.previousLetterSpace = null;
        }
        SetLastTwoLetterSpaces();
        UpdatePowerupTypeAndStrengthForWord();
        uiManager.UpdateColorsForWord(word, powerupTypeForWord);
        uiManager.UpdatePowerupIcon(powerupTypeForWord);
        uiManager.UpdateVisualsForLettersInWord(letterSpacesForWord);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
    }

    private void UpdatePowerupTypeAndStrengthForWord(){
        if (letterSpacesForWord.Count == 0)
            return;
        powerupTypeForWord = BattleManager.PowerupTypes.None;
        powerupLevel = 0;
        foreach (LetterSpace ls in letterSpacesForWord){
            if (ls.powerupType != BattleManager.PowerupTypes.None){
                powerupLevel++;
                if (powerupLevel == 1)
                    powerupTypeForWord = ls.powerupType;
            }

        }
    }


        private void SetLastTwoLetterSpaces(){
        lastLetterSpace = null;
        secondToLastLetterSpace = null;
        if (letterSpacesForWord.Count > 0)
            lastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 1];
        if (letterSpacesForWord.Count > 1)
            secondToLastLetterSpace = letterSpacesForWord[letterSpacesForWord.Count - 2];

    }



    public bool CanAddLetter(LetterSpace letterSpace){
        if (letterSpace.hasBeenUsedInAWordAlready)
            return false;
        if (letterSpacesForWord.Contains(letterSpace))
            return false;
        if (letterSpacesForWord.Count == 0)
            return true;
        if (letterSpacesForWord.Count > 8) //decide on some limit, based on screen / text size?
            return false;
        if (letterSpace.IsAdjacentToLetterSpace(lastLetterSpace))
            return true;
        return false;
    }

    public bool CanRemoveLetter(LetterSpace letterSpace){
        if (letterSpacesForWord.Count == 0)
            return false;
        return (lastLetterSpace == letterSpace);
    }

    public void ClearWord(){
        foreach (LetterSpace ls in letterSpacesForWord){
            ls.previousLetterSpace = null;
            ls.nextLetterSpace = null;
            ls.hasBeenUsedInAWordAlready = true;
            ls.ShowAsNotPartOfWord();
        }
        letterSpacesForWord = new List<LetterSpace>();
        SetLastTwoLetterSpaces();
        word = "";
        isValidWord = false;
        wordStrength = 0;
        uiManager.UpdatePowerupIcon(PowerupTypes.None);
        uiManager.DisplayWord(word, isValidWord, countdownToRefresh, wordStrength);
    }


}
 
