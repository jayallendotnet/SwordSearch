using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PageTurnAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;

    public BoxCollider2D[] revealAreas; //the area that letters should be changed within during each frame (frames 1-6)
    public GameObject[] victoryPageRevealAreas; //the objects that should be hidden on each subsequent frame to reveal the victory button
    public GameObject[] victoryPageHideAreas; //the objects that should be shown on each subsequent frame to hide the countdown, submit, and strength visuals

    public List<LetterSpace> letterSpacesNotYetChanged;

    public bool hidingLetters = false;

    public void SetUpLetterSpaces(){
        letterSpacesNotYetChanged = new();
        foreach (LetterSpace ls in battleManager.puzzleGenerator.letterSpaces)
            letterSpacesNotYetChanged.Add(ls);
        if (hidingLetters){
            foreach (GameObject go in victoryPageRevealAreas)
                go.SetActive(true);
            foreach (GameObject go in victoryPageHideAreas)
                go.SetActive(false);
        }
    }

    public void PageTurnFrameBegan(int num){
        int unchangedLettersAtStartOfFrame = letterSpacesNotYetChanged.Count;

        BoxCollider2D revealArea = revealAreas[num - 1];

        List<LetterSpace> letterSpacesToUpdate = new();

        foreach (LetterSpace ls in letterSpacesNotYetChanged){
            Collider2D[] colliders = Physics2D.OverlapPointAll(ls.transform.position);
            if (colliders.Contains(revealArea))
                letterSpacesToUpdate.Add(ls);
        }

        foreach (LetterSpace ls in letterSpacesToUpdate){
            if (hidingLetters){
                ls.HideVisuals();
                ls.DisableTouchDetection();
            }
            else
                battleManager.puzzleGenerator.UpdateLetterVisual(ls);
            letterSpacesNotYetChanged.Remove(ls);
        }

        if (hidingLetters)
            victoryPageRevealAreas[num - 1].SetActive(false);
        if (hidingLetters)
            victoryPageHideAreas[num - 1].SetActive(true);
    }

    public void PageTurnAnimationFinished(){
        battleManager.uiManager.PageTurnEnded();
        gameObject.SetActive(false);
    }
}
