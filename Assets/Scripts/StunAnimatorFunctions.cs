using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StunAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;

    public void EndExitAnimation(){
        gameObject.SetActive(false);
    }

}
