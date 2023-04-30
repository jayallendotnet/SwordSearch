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


    void Start(){
        enemyHealth = startingEnemyHealth;
        playerHealth = startingPlayerHealth;
        DisplayHealths();
    }
    
    private void DisplayHealths(){
        enemyHealthText.text = enemyHealth + "";
        playerHealthText.text = playerHealth + "";

    }

    public bool IsValidWord(){
        if (wordDisplay.text.text.Length > 3)
            return true;
        return false;

        //check the dictionary
        //have an adult setting for mature words?
        //some protection against the same (or similar?) words being used again
        //can't use more than 1 of the exact same letter in a future word (after 3 words you can get a new search)
        //no rule against submitting the same word more than once if you can make it
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
 
