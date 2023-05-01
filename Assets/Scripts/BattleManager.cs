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


    void Start(){
        wordLibraryForChecking = wordLibraryForCheckingFile.text.Split("\r\n");
        enemyHealth = startingEnemyHealth;
        playerHealth = startingPlayerHealth;
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
            DamageEnemyHealth(calculateDamage());
            foreach (LetterSpace ls in wordDisplay.letterSpacesForWord)
                ls.hasBeenUsedInAWordAlready = true;
            wordDisplay.ClearWord();
        }


        //make the word fly toward the enemy and hit them?

    }


}
 
