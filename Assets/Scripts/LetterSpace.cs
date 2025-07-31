using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [HideInInspector]
    public bool isBurned = false;
    [HideInInspector]
    public bool isCharred = false;


    [Header("GameObjects")]
    public TextMeshProUGUI text;
    public GameObject selectedSignifier;
    public Animator selectedSignifierAnimator;
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
    public GameObject waterIcon;
    public Animator waterIconAnimator;
    public GameObject healingIcon;
    public Animator healingIconAnimator;
    public GameObject earthIcon;
    public Animator earthIconAnimator;
    public GameObject fireIcon;
    public Animator fireIconAnimator;
    public GameObject lightningIcon;
    public Animator lightningIconAnimator;
    public GameObject darkIcon;
    public Animator darkIconAnimator;
    public GameObject swordIcon;
    public Animator swordIconAnimator;

    [Header("Other Icons")]
    public Animator highlightAnimator;
    public Transform burnIcon;
    public Transform charredIcon;

    [Header("Colors")]
    public Color normalLetterColor;
    public Color powerupOrWordLetterColor;
    [Header("Fonts")]
    public TMP_FontAsset fontWithOutline;
    public TMP_FontAsset fontNoOutline;
    public TMP_FontAsset fontUsed;


    public void UpdateLetter(char letter) {
        this.letter = letter;
        text.text = "" + letter;
    }

    public void ShowAsPartOfWord(Color backgroundColor){
        selectedSignifier.SetActive(true);
        ShowDirectionsToNeighbors();
        HidePowerupIcons();
        UpdateBackgroundColors(backgroundColor);
        text.font = fontWithOutline;
        if (battleManager.inProgressWord.type == BattleManager.PowerupTypes.None){
            selectedSignifierAnimator.enabled = false;
            selectedSignifier.transform.localScale = Vector3.one;
        }
        else{
            selectedSignifierAnimator.enabled = true;
            battleManager.uiManager.SynchronizePulse(selectedSignifierAnimator, "SelectedLetterSignifier");
        }
    }

    public void ShowAsNotPartOfWord(){
        selectedSignifier.SetActive(false);
        HideAllDirectionLines();
        if (hasBeenUsedInAWordAlready){
            text.font = fontUsed;
            HidePowerupIcons();
        }
        else{
            text.font = fontNoOutline;
            ShowPowerup();
        }
    }

    private void HidePowerupIcons(){
        waterIcon.SetActive(false);
        healingIcon.SetActive(false);
        earthIcon.SetActive(false);
        fireIcon.SetActive(false);
        lightningIcon.SetActive(false);
        darkIcon.SetActive(false);
        swordIcon.SetActive(false);
    }

    private void ShowPowerupIcon(BattleManager.PowerupTypes powerupType) {
        GameObject icon = waterIcon;
        Animator animator = waterIconAnimator;
        switch (powerupType) {
            case BattleManager.PowerupTypes.Water:
                icon = waterIcon;
                animator = waterIconAnimator;
                break;
            case BattleManager.PowerupTypes.Heal:
                icon = healingIcon;
                animator = healingIconAnimator;
                break;
            case BattleManager.PowerupTypes.Earth:
                icon = earthIcon;
                animator = earthIconAnimator;
                break;
            case BattleManager.PowerupTypes.Fire:
                icon = fireIcon;
                animator = fireIconAnimator;
                break;
            case BattleManager.PowerupTypes.Lightning:
                icon = lightningIcon;
                animator = lightningIconAnimator;
                break;
            case BattleManager.PowerupTypes.Dark:
                icon = darkIcon;
                animator = darkIconAnimator;
                break;
            case BattleManager.PowerupTypes.Sword:
                icon = swordIcon;
                animator = swordIconAnimator;
                break;
        }
        waterIcon.SetActive(icon == waterIcon);
        healingIcon.SetActive(icon == healingIcon);
        earthIcon.SetActive(icon == earthIcon);
        fireIcon.SetActive(icon == fireIcon);
        lightningIcon.SetActive(icon == lightningIcon);
        darkIcon.SetActive(icon == darkIcon);
        swordIcon.SetActive(icon == swordIcon);
        battleManager.uiManager.SynchronizePulse(animator, "PowerupIcon");
    }

    private void UpdateBackgroundColors(Color c) {
        foreach (Image im in colorableBackgroundImages)
            im.color = c;
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
            HidePowerupIcons();
            text.font = fontNoOutline;
        }
        else{
            PowerupDisplayData d = battleManager.uiManager.GetPowerupDisplayDataWithType(powerupType);
            Color t = d.textColor;
            ShowPowerupIcon(powerupType);
            text.font = fontWithOutline;
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
        HidePowerupIcons();
        text.gameObject.SetActive(false);
        selectedSignifier.SetActive(false);
        HideAllDirectionLines();
    }
    public void DisableTouchDetection(){
        touchDetection1.SetActive(false);
        touchDetection2.SetActive(false);
        touchDetection3.SetActive(false);
    }


    public void ToggleTutorialSelector(bool value) {
        tutorialSelector.SetActive(value);
        if (value)
            battleManager.uiManager.SynchronizePulse(highlightAnimator, "TutorialHighlight_LetterSpace");

    }

    public void ApplyBurn() {
        isBurned = true;
        burnIcon.gameObject.SetActive(true);
        float originalScale = burnIcon.localScale.x;
        burnIcon.localScale = Vector2.zero;
        burnIcon.DOScale(originalScale, 0.2f);
        battleManager.puzzleGenerator.burnedLetters.Add(this);
    }
    
    public void RemoveBurn(){
        isBurned = false;
        burnIcon.gameObject.SetActive(false);
        battleManager.puzzleGenerator.burnedLetters.Remove(this);
    }
    
    public void ApplyChar() {
        isCharred = true;
        charredIcon.gameObject.SetActive(true);
        float originalScale = charredIcon.localScale.x;
        charredIcon.localScale = Vector2.zero;
        charredIcon.DOScale(originalScale, 0.2f);
        //battleManager.puzzleGenerator.burnedLetters.Add(this);
    }
    
    public void RemoveChar(){
        isCharred = false;
        charredIcon.gameObject.SetActive(false);
        //battleManager.puzzleGenerator.burnedLetters.Remove(this);
    }
}
