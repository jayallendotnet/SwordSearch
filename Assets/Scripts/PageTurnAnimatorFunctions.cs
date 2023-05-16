using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageTurnAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;
    public bool hidingLetters = false;

    public void PageTurnFrameBegan(int num){
        if (hidingLetters)
            battleManager.uiManager.HideLetterVisualsForSection(num);
            //battleManager.puzzleGenerator.HideLetterVisualsForSection(num);
        else
        battleManager.puzzleGenerator.UpdateLetterVisualsForSection(num);
    }

    public void PageTurnAnimationFinished(){
        battleManager.uiManager.PageTurnEnded();
        gameObject.SetActive(false);
    }
}
