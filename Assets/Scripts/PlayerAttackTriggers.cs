using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttackTriggers : MonoBehaviour{

    public void DisableSelf(){
        //print("disable");
        this.gameObject.SetActive(false);
    }

    public void AttackHitsEnemy(){
        FindObjectOfType<BattleManager>().ApplyAttackToEnemy();
    }
}
