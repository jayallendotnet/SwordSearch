using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackAnimatorFunctions : MonoBehaviour{

    public void DoEnemyAttackEffect(){
        FindObjectOfType<BattleManager>().DoEnemyAttackEffect();
    }

}
