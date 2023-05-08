using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("GameObjects")]
    public Image playerHealthOnes;
    public Image playerHealthTens;
    public Image enemyHealthOnes;
    public Image enemyHealthTens;
    public Transform playerAttackAnimationParent;


    [Header("Submit Word Button")]
    public Text wordDisplay;
    public Image submitWordButtonImage;
    public GameObject submitWordBorder;
    public Image countdownNumber;
    public GameObject countdownDivider;
    public Image wordStrengthImageSingle;
    public Image wordStrengthImageOnes;
    public Image wordStrengthImageTens;
    public GameObject wordStrengthDivider;
    public Image wordStrengthIcon;


    [Header("Colors")]
    public Color canRefreshPuzzleColor;
    public List<PowerupDisplayData> powerupDisplayDataList;
    [Header("Numbers")]
    public Sprite[] numberSprites;

    [Header("Player and Enemy")]
    public Animator playerAnimator;
    public Transform playerObject;
    public GameObject playerHealDoubleDigitPrefab;
    public GameObject playerHealSingleDigitPrefab;
    public GameObject playerDamageDoubleDigitPrefab;
    public GameObject playerDamageSingleDigitPrefab;
    public Animator enemyAnimator;
    public Transform enemyObject;
    public GameObject enemyHealDoubleDigitPrefab;
    public GameObject enemyHealSingleDigitPrefab;
    public GameObject enemyDamageDoubleDigitPrefab;
    public GameObject enemyDamageSingleDigitPrefab;

    [Header("Misc")]
    public BattleManager battleManager;

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
        Vector2 enemyHP = GetTensAndOnes(enemyHealth);
        Vector2 playerHP = GetTensAndOnes(playerHealth);
        playerHealthOnes.sprite = numberSprites[(int)playerHP[1]];
        playerHealthTens.sprite = numberSprites[(int)playerHP[0]];
        enemyHealthOnes.sprite = numberSprites[(int)enemyHP[1]];
        enemyHealthTens.sprite = numberSprites[(int)enemyHP[0]];
    }

    public void ShowPlayerTakingDamage(int amount, bool stillAlive){
        ShowNumbersAsChild(playerDamageSingleDigitPrefab, playerDamageDoubleDigitPrefab, playerObject, amount);
        if (stillAlive)
            playerAnimator.SetTrigger("TakeDamage");
        else
            playerAnimator.SetTrigger("Die");   
    }
    public void ShowPlayerGettingHealed(int amount){
        ShowNumbersAsChild(playerHealSingleDigitPrefab, playerHealDoubleDigitPrefab, playerObject, amount);
    }

    public void ShowEnemyTakingDamage(int amount, bool stillAlive){
        ShowNumbersAsChild(enemyDamageSingleDigitPrefab, enemyDamageDoubleDigitPrefab, enemyObject, amount);        
        if (stillAlive)
            enemyAnimator.SetTrigger("TakeDamage");
        else
            enemyAnimator.SetTrigger("Die");   
    }

    public void ShowEnemyGettingHealed(int amount){
        ShowNumbersAsChild(enemyHealSingleDigitPrefab, enemyHealDoubleDigitPrefab, enemyObject, amount);
        
    }

    private void ShowNumbersAsChild(GameObject singleDigitPrefab, GameObject doubleDigitPrefab, Transform parent, int amount){
        if (amount < 10){
            GameObject obj = Instantiate(singleDigitPrefab, parent);
            obj.transform.Find("Ones").GetComponent<Image>().sprite = numberSprites[amount];
        }
        else{
            Vector2 temp = GetTensAndOnes(amount);
            GameObject obj = Instantiate(doubleDigitPrefab, parent);
            obj.transform.Find("Ones").GetComponent<Image>().sprite = numberSprites[(int)temp[1]];
            obj.transform.Find("Tens").GetComponent<Image>().sprite = numberSprites[(int)temp[0]];
        }
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
            submitWordButtonImage.color = backgroundColorForWord;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
        else if ((countdown == 0) && (word.Length == 0)){
            wordDisplay.text = "NEW\nPUZZLE";
            wordDisplay.fontSize = 85;
            wordDisplay.color = validWordColor;
            submitWordButtonImage.color = canRefreshPuzzleColor;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            countdownDivider.SetActive(true);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
        else {
            wordDisplay.text = word;
            wordDisplay.fontSize = 150;
            wordDisplay.color = invalidWordColor;
            submitWordButtonImage.color = invalidButtonColor;
            submitWordBorder.SetActive(false);
            wordStrengthDivider.SetActive(false);
            countdownDivider.SetActive(false);
            UpdateWordStrengthDisplay(strength);
            UpdateCountdownDisplay(countdown);
        }
    }

    public void UpdateWordStrengthDisplay(int strength){
        if (strength < 10){
            wordStrengthImageSingle.sprite = numberSprites[strength];
            wordStrengthImageSingle.gameObject.SetActive(true);
            wordStrengthImageOnes.gameObject.SetActive(false);
            wordStrengthImageTens.gameObject.SetActive(false);
        }
        else{
            Vector2 str = GetTensAndOnes(strength);
            wordStrengthImageOnes.sprite = numberSprites[(int)str[1]];
            wordStrengthImageTens.sprite = numberSprites[(int)str[0]];
            wordStrengthImageSingle.gameObject.SetActive(false);
            wordStrengthImageOnes.gameObject.SetActive(true);
            wordStrengthImageTens.gameObject.SetActive(true);
        }
    }

    private Vector2 GetTensAndOnes(int val){
        int tens = (val / 10);
        int ones = val - (tens * 10);
        if (val > 99){
            ones = 9;
            tens = 9;
        }
        return new Vector2(tens, ones);
    }

    public void UpdateCountdownDisplay(int countdown){
        if (countdown > 9)
            countdown = 9;
        countdownNumber.sprite = numberSprites[countdown];
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
