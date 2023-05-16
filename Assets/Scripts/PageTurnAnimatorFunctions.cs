using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageTurnAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;

    public void PageTurnFrameBegan(int num){
        battleManager.puzzleGenerator.UpdateLetterVisualsForSection(num);
    }

    public void PageTurnAnimationFinished(){
        gameObject.SetActive(false);
    }
}
