using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;

    public void DoEnemyAttackEffect(){
        battleManager.DoEnemyAttackEffect();
    }

    public void QueueNextAttack(){
        battleManager.QueueEnemyAttack();
    }

    public void ReturnedToIdle(){
        battleManager.EnemyReturnedToIdle();
    }

}
