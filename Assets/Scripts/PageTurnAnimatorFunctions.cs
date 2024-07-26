using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PageTurnAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;

    public BoxCollider2D[] revealAreas; //the area that letters should be changed within during each frame (frames 1-6)

    public List<LetterSpace> letterSpacesNotYetChanged;

    public bool hidingLetters = false;

    public void SetUpLetterSpaces(){
        letterSpacesNotYetChanged = new();
        foreach (LetterSpace ls in battleManager.puzzleGenerator.letterSpaces)
            letterSpacesNotYetChanged.Add(ls);
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
            battleManager.puzzleGenerator.UpdateLetterVisual(ls);
            letterSpacesNotYetChanged.Remove(ls);
        }
    }

    public void PageTurnAnimationFinished(){
        battleManager.uiManager.PageTurnEnded();
        gameObject.SetActive(false);
    }
}
