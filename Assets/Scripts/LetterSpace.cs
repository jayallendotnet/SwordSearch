using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LetterSpace : MonoBehaviour{

    [HideInInspector]
    public char letter;

    //when the letter space is part of the word, previousletterspace and nextletterspace link to other letters in the word
    [HideInInspector]
    public LetterSpace previousLetterSpace = null;
    [HideInInspector]
    public LetterSpace nextLetterSpace = null;    
    [HideInInspector]
    public Vector2 position = Vector2.zero;
    [HideInInspector]
    public BattleManager.PowerupTypes powerupType = BattleManager.PowerupTypes.None;
    [HideInInspector]
    public bool hasBeenUsedInAWordAlready = false;
    [HideInInspector]
    public BattleManager battleManager;
    [HideInInspector]
    public bool wasActiveBeforeFingerDown = false;
    [HideInInspector]
    public char nextLetter;
    [HideInInspector]
    public BattleManager.PowerupTypes nextPowerupType = BattleManager.PowerupTypes.None;


    [Header("GameObjects")]
    public Text text;
    public GameObject selectedSignifier;
    public Animator selectedSignifierAnimator;
    public GameObject powerupIcon;
    public Animator powerupIconAnimator;
    public Image[] colorableBackgroundImages = new Image[9];
    public GameObject tutorialSelector;
    public GameObject touchDetection1;
    public GameObject touchDetection2;
    public GameObject touchDetection3;

    [Header("Connectors")]
    public GameObject topConnector;
    public GameObject topRightConnector;
    public GameObject rightConnector;
    public GameObject bottomRightConnector;
    public GameObject bottomConnector;
    public GameObject bottomLeftConnector;
    public GameObject leftConnector;
    public GameObject topLeftConnector;

    [Header("Powerup Icons")]
    public Sprite waterIcon;
    public Sprite healingIcon;
    public Sprite earthIcon;
    public Sprite fireIcon;
    public Sprite lightningIcon;
    public Sprite darkIcon;
    public Sprite swordIcon;

    [Header("Colors")]
    public Color normalLetterColor;
    public Color powerupOrWordLetterColor;

    public void UpdateLetter(char letter) {
        this.letter = letter;
        text.text = "" + letter;
    }

    public void ShowAsPartOfWord(Color backgroundColor){
        selectedSignifier.SetActive(true);
        ShowDirectionsToNeighbors();
        HidePowerupIcon();
        //text.color = textColor;
        UpdateBackgroundColors(backgroundColor);
        ColorLetter(true);
        if (battleManager.powerupTypeForWord == BattleManager.PowerupTypes.None){
            selectedSignifierAnimator.enabled = false;
            selectedSignifier.transform.localScale = Vector3.one;
        }
        else{
            selectedSignifierAnimator.enabled = true;
            battleManager.uiManager.SynchronizePulse(selectedSignifierAnimator);
        }
    }

    public void ShowAsNotPartOfWord(){
        selectedSignifier.SetActive(false);
        HideAllDirectionLines();
        text.GetComponent<Outline>().enabled = false;
        if (hasBeenUsedInAWordAlready){
            text.color = battleManager.uiManager.usedLetterColor;
            HidePowerupIcon();
        }
        else{
            text.color = Color.black;
            ShowPowerup();
        }
    }

    private void HidePowerupIcon(){
        powerupIcon.SetActive(false);
    }

    private void UpdateBackgroundColors(Color c){
        foreach (Image im in colorableBackgroundImages){
            im.color = c;
        }
    }

    private void HideAllDirectionLines(){
        foreach (Transform t in topLeftConnector.transform.parent)
            t.gameObject.SetActive(false);
    }

    public void ShowDirectionsToNeighbors(){
        HideAllDirectionLines();
        ShowDirectionToNeighbor(previousLetterSpace);
        ShowDirectionToNeighbor(nextLetterSpace);
    }

    private void ShowDirectionToNeighbor(LetterSpace otherSpace){
        //if the selected letterspace is not adjacent, do not show anything
        if (otherSpace == null)
            return;
        Vector2 distance = GetDistanceToLetterSpace(otherSpace);
        if ((distance[0] == 1) && (distance[1] == 0)){
            topConnector.SetActive(true);
        }
        if ((distance[0] == 1) && (distance[1] == -1)){
            topRightConnector.SetActive(true);
        }
        if ((distance[0] == 0) && (distance[1] == -1)){
            rightConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == -1)){
            bottomRightConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == 0)){
            bottomConnector.SetActive(true);
        }
        if ((distance[0] == -1) && (distance[1] == 1)){
            bottomLeftConnector.SetActive(true);
        }
        if ((distance[0] == 0) && (distance[1] == 1)){
            leftConnector.SetActive(true);
        }
        if ((distance[0] == 1) && (distance[1] == 1)){
            topLeftConnector.SetActive(true);
        }
    }
    
    public Vector2 GetDistanceToLetterSpace(LetterSpace otherSpace){
        if (otherSpace == null)
            return Vector2.zero;

        Vector2 distance = (position - otherSpace.position);
        return distance;
    }

    public bool IsAdjacentToLetterSpace(LetterSpace otherSpace){
        Vector2 distance = GetDistanceToLetterSpace(otherSpace);
        if ((Mathf.Abs(distance[0]) < 2) && (Mathf.Abs(distance[1]) < 2))
            return true;
        return false;
    }

    public void ShowPowerup(){
        if (powerupType == BattleManager.PowerupTypes.None){
            powerupIcon.SetActive(false);
            ColorLetter(false);
        }
        else{
            powerupIcon.SetActive(true);
            PowerupDisplayData d = battleManager.uiManager.GetPowerupDisplayDataWithType(powerupType);
            Color t = d.textColor;
            Color b = d.backgroundColor;
            text.color = t;
            powerupIcon.GetComponent<Image>().color = b;
            battleManager.uiManager.SynchronizePulse(powerupIconAnimator);
            ColorLetter(true);

            Sprite icon = waterIcon;
            switch (powerupType){
                case BattleManager.PowerupTypes.Water:
                    icon = waterIcon;
                    break;
                case BattleManager.PowerupTypes.Heal:
                    icon = healingIcon;
                    break;
                case BattleManager.PowerupTypes.Earth:
                    icon = earthIcon;
                    break;
                case BattleManager.PowerupTypes.Fire:
                    icon = fireIcon;
                    break;
                case BattleManager.PowerupTypes.Lightning:
                    icon = lightningIcon;
                    break;
                case BattleManager.PowerupTypes.Dark:
                    icon = darkIcon;
                    break;
                case BattleManager.PowerupTypes.Sword:
                    icon = swordIcon;
                    break;

            }
            powerupIcon.GetComponent<Image>().sprite = icon;
        }

    }

    private void ColorLetter(bool isInPowerupOrWord){
        if (isInPowerupOrWord){
            text.color = powerupOrWordLetterColor;
            text.GetComponent<Outline>().enabled = true;
        }
        else {
            text.color = normalLetterColor;
            text.GetComponent<Outline>().enabled = false;
        }
    }

    public void SetPowerup(BattleManager.PowerupTypes type){
        powerupType = type;
        ShowPowerup();
    }

    public void SetNextPuzzleData(char nextLetter, BattleManager.PowerupTypes nextPowerupType){
        this.nextLetter = nextLetter;
        this.nextPowerupType = nextPowerupType;
    }

    public void ApplyNextPuzzleData(){
        SetPowerup(nextPowerupType);
        UpdateLetter(nextLetter);
    }

    public void HideVisuals(){
        HidePowerupIcon();
        text.gameObject.SetActive(false);
        selectedSignifier.SetActive(false);
        HideAllDirectionLines();
    }
    public void DisableTouchDetection(){
        touchDetection1.SetActive(false);
        touchDetection2.SetActive(false);
        touchDetection3.SetActive(false);
    }

    public void ToggleTutorialSelector(bool value){
        tutorialSelector.SetActive(value);
    }
}
