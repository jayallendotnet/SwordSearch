using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WordDisplay : MonoBehaviour {

    [HideInInspector]
    public string word = "";
    public Text text;
    [HideInInspector]
    public List<LetterSpace> letterSpacesForWord = new List<LetterSpace>(){};

    private LetterSpace lastLetterSpace = null;
    private LetterSpace secondToLastLetterSpace = null;

    public BattleManager battleManager;
    private Color validWordColor;
    private Color invalidWordColor;
    private Color validButtonColor;
    private Color invalidButtonColor;

    public Image submitWordButtonImage;
    public GameObject submitWordBorder;
    public GameObject wordStrengthDivider;
    public Text wordStrengthText;
    public GameObject wordTypeDivider;

    public List<PowerupDisplayData> powerupDisplayDataList;

    private Color textColorForWord = Color.black;
    private Color backgroundColorForWord = Color.grey;

    [HideInInspector]
    public BattleManager.PowerupTypes powerupTypeForWord;
    [HideInInspector]
    public int powerupStrength;


    void Start() {
        text.text = "";
        ColorUtility.TryParseHtmlString("#4B4B4B", out validWordColor);
        ColorUtility.TryParseHtmlString("#C8C8C8", out invalidWordColor);
        ColorUtility.TryParseHtmlString("#8DE1FF", out validButtonColor);
        ColorUtility.TryParseHtmlString("#B34A50", out invalidButtonColor);           
        
        UpdateWordDisplay();
    }
    public void AddLetter(LetterSpace ls){
        word += ls.letter;
        //text.text += ls.letter;
        letterSpacesForWord.Add(ls);
        if (lastLetterSpace != null){
            lastLetterSpace.nextLetterSpace = ls;
            ls.previousLetterSpace = lastLetterSpace;
        }
        SetLastTwoLetterSpaces();
        UpdatePowerupTypeAndStrengthForWord();
        UpdateColorsForWord();
        UpdateVisualsForLettersInWord();
        UpdateWordDisplay();
    }

    public void RemoveLetter(LetterSpace ls){
        //assumes the last letter in the list is the provided letter
        if (word.Length < 2)
            word = "";
        else
            word = word.Substring(0, (word.Length - 1));
        letterSpacesForWord.Remove(ls);
        ls.ShowAsNotPartOfWord();
        if (secondToLastLetterSpace != null){
            secondToLastLetterSpace.nextLetterSpace = null;
            lastLetterSpace.previousLetterSpace = null;
        }
        SetLastTwoLetterSpaces();
        UpdatePowerupTypeAndStrengthForWord();
        UpdateColorsForWord();
        UpdateVisualsForLettersInWord();
        UpdateWordDisplay();
    }

    private void UpdatePowerupTypeAndStrengthForWord(){
        if (letterSpacesForWord.Count == 0)
            return;
        powerupTypeForWord = BattleManager.PowerupTypes.None;
        powerupStrength = 0;
        foreach (LetterSpace ls in letterSpacesForWord){
            if (ls.powerupType != BattleManager.PowerupTypes.None){
                powerupStrength++;
                if (powerupStrength == 1)
                    powerupTypeForWord = ls.powerupType;
            }

        }
    }

    private void UpdateColorsForWord(){
        if (letterSpacesForWord.Count == 0)
            return;
        foreach (PowerupDisplayData d in powerupDisplayDataList){
            if (d.type == powerupTypeForWord){
                textColorForWord = d.textColor;
                backgroundColorForWord = d.backgroundColor;
            }
        }

    }

    private void UpdateVisualsForLettersInWord(){
        foreach (LetterSpace ls2 in letterSpacesForWord)
            ls2.ShowAsPartOfWord(textColorForWord, backgroundColorForWord);
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
        UpdateWordDisplay();
    }

    private void UpdateWordDisplay(){
        if (battleManager.IsValidWord()){
            text.text = word;
            text.color = textColorForWord;
            wordStrengthText.color = textColorForWord;
            submitWordButtonImage.color = backgroundColorForWord;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            wordTypeDivider.SetActive(true);
            wordStrengthText.gameObject.SetActive(true);
            UpdateWordStrengthText();
        }
        else if ((battleManager.currentPuzzleCountdown == 0) && (word.Length == 0)){
            text.text = "NEW PUZZLE";
            text.color = Color.black;
            wordStrengthText.color = invalidWordColor;
            submitWordButtonImage.color = battleManager.canRefreshPuzzleColor;
            submitWordBorder.SetActive(true);
            wordStrengthDivider.SetActive(true);
            wordTypeDivider.SetActive(true);
            wordStrengthText.gameObject.SetActive(true);
            UpdateWordStrengthText();
        }
        else {
            text.text = word;
            text.color = invalidWordColor;
            wordStrengthText.color = invalidWordColor;
            submitWordButtonImage.color = invalidButtonColor;
            submitWordBorder.SetActive(false);
            wordStrengthDivider.SetActive(false);
            wordTypeDivider.SetActive(false);
            wordStrengthText.gameObject.SetActive(false);
        }
    }

    private void UpdateWordStrengthText(){
        int strength = battleManager.calculateDamage();
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

}
