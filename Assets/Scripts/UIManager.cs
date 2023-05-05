using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("GameObjects")]
    public Text playerHealthDisplay;
    public Text enemyHealthDisplay;


    [Header("Submit Word Button")]
    public Text wordDisplay;
    public Image submitWordButtonImage;
    public GameObject submitWordBorder;
    public Text refreshPuzzleCountdownText;
    public GameObject countdownDivider;
    public Text wordStrengthText;
    public GameObject wordStrengthDivider;
    public Image wordStrengthIcon;


    [Header("Colors")]
    public Color canRefreshPuzzleColor;
    public List<PowerupDisplayData> powerupDisplayDataList;


    [Header("Misc")]
    public BattleManager battleManager;
    public Animator playerAnimator;

    private Color validWordColor;
    private Color invalidWordColor;
    private Color validButtonColor;
    private Color invalidButtonColor;
    private Color textColorForWord = Color.black;
    private Color backgroundColorForWord = Color.grey;
    
    public void InitializeColors(){
        ColorUtility.TryParseHtmlString("#4B4B4B", out validWordColor);
        ColorUtility.TryParseHtmlString("#C8C8C8", out invalidWordColor);
        ColorUtility.TryParseHtmlString("#8DE1FF", out validButtonColor);
        ColorUtility.TryParseHtmlString("#B34A50", out invalidButtonColor);   
    }


    public void DisplayHealths(int playerHealth, int enemyHealth){
        enemyHealthDisplay.text = enemyHealth + "";
        playerHealthDisplay.text = playerHealth + "";

    }

    public void ShowPlayerTakingDamage(int amount, bool stillAlive){
        if (stillAlive)
            playerAnimator.SetTrigger("TakeDamage");
        else
            playerAnimator.SetTrigger("Die");   
    }

    public void StartPlayerCastAnimation(){
        playerAnimator.SetTrigger("StartCast");
    }

    public void StartPlayerHealAnimation(){
        playerAnimator.SetTrigger("StartHeal");
    }

    public void UpdateColorsForWord(string word, BattleManager.PowerupTypes type){
        if (word.Length == 0)
            return;
        foreach (PowerupDisplayData d in powerupDisplayDataList){
            if (d.type == type){
                textColorForWord = d.textColor;
                backgroundColorForWord = d.backgroundColor;
            }
        }

    }

    public void UpdatePowerupIcon(BattleManager.PowerupTypes type){
        PowerupDisplayData d =GetPowerupDisplayDataWithType(type);
        wordStrengthIcon.sprite = d.icon;
    }

    public void UpdateVisualsForLettersInWord(List<LetterSpace> letterSpacesForWord){
        foreach (LetterSpace ls2 in letterSpacesForWord)
            ls2.ShowAsPartOfWord(textColorForWord, backgroundColorForWord);
    }

    public void DisplayWord(string word, bool isValidWord, int countdown, int strength){
        if (isValidWord){
            wordDisplay.text = word;
            wordDisplay.fontSize = 150;
            wordDisplay.color = textColorForWord;
            wordStrengthText.color = textColorForWord;
            refreshPuzzleCountdownText.color = textColorForWord;
            submitWordButtonImage.color = backgroundColorForWord;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            refreshPuzzleCountdownText.text = "" + countdown;
            UpdateWordStrengthText(strength);
        }
        else if ((countdown == 0) && (word.Length == 0)){
            wordDisplay.text = "NEW\nPUZZLE";
            wordDisplay.fontSize = 85;
            wordDisplay.color = validWordColor;
            wordStrengthText.color = validWordColor;
            refreshPuzzleCountdownText.color = validWordColor;
            submitWordButtonImage.color = canRefreshPuzzleColor;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            refreshPuzzleCountdownText.text = "" + countdown;
            UpdateWordStrengthText(strength);
        }
        else {
            wordDisplay.text = word;
            wordDisplay.fontSize = 150;
            wordDisplay.color = invalidWordColor;
            wordStrengthText.color = invalidWordColor;
            refreshPuzzleCountdownText.color = invalidWordColor;
            submitWordButtonImage.color = invalidButtonColor;
            submitWordBorder.SetActive(false);
            wordStrengthDivider.SetActive(false);
            countdownDivider.SetActive(false);
            refreshPuzzleCountdownText.text = "" + countdown;
            UpdateWordStrengthText(strength);
        }
    }

    public void UpdateWordStrengthText(int strength){
        wordStrengthText.fontSize = 120;
        if (strength > 9)
            wordStrengthText.fontSize = 85;
        wordStrengthText.text = "" + strength;
    }


    public PowerupDisplayData GetPowerupDisplayDataWithType(BattleManager.PowerupTypes t){
        foreach (PowerupDisplayData d in powerupDisplayDataList){
            if (d.type == t)
                return d;
        }
        return null;
    }


}

[System.Serializable]
public class PowerupDisplayData{
    public BattleManager.PowerupTypes type;
    public Color textColor = Color.white;
    public Color backgroundColor = Color.white;
    public Sprite icon;

}
