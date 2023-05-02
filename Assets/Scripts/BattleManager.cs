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

    public Image refreshPuzzleButtonImage;
    public Text refreshPuzzleCountdownText;

    public int maxPuzzleCountdown = 3;
    private int currentPuzzleCountdown;
    private Color canRefreshPuzzleColor;
    private Color cannotRefreshPuzzleColor;

    public enum PowerupTypes{None, Ice, Fire, Heal};
    [HideInInspector]
    public System.Array powerupArray = BattleManager.PowerupTypes.GetValues(typeof(BattleManager.PowerupTypes));


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

    public bool IsValidWord(){
        string w = wordDisplay.text.text;
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

    private int calculateDamage(){
        return Mathf.FloorToInt(Mathf.Pow((wordDisplay.text.text.Length - 2), 2));
    }

    private void DamageEnemyHealth(int amount){
        print("damaging health by: " + amount);
        enemyHealth -= amount;
        if (enemyHealth < 0)
            enemyHealth = 0;
        DisplayHealths();
    }

    public void PressSubmitWordButton(){
        if (IsValidWord()){
            if (wordDisplay.powerupTypeForWord != PowerupTypes.None)
                print("your word has a powerup! type[" + wordDisplay.powerupTypeForWord + "] strength[" + wordDisplay.powerupStrength + "]");
            DamageEnemyHealth(calculateDamage());
            wordDisplay.ClearWord();
            DecrementRefreshPuzzleCountdown();
        }


        //make the word fly toward the enemy and hit them?

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
            UpdateRefreshPuzzleButton();
        }
    }

    private void UpdateRefreshPuzzleButton(){
        refreshPuzzleCountdownText.text = "" + currentPuzzleCountdown;
        if (currentPuzzleCountdown > 0)
            refreshPuzzleButtonImage.color = cannotRefreshPuzzleColor;
        else
            refreshPuzzleButtonImage.color = canRefreshPuzzleColor;
        
    }


}
 
