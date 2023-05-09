using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttackAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;
    public List<int> burnDamageQueue = new List<int>();
    private int cyclesBetweenBurns = 4;
    private int cyclesLeftUntilBurn = 0;

    public void DoEnemyAttackEffect(){
        battleManager.DoEnemyAttackEffect();
    }

    public void QueueNextAttack(){
        battleManager.QueueEnemyAttack();
    }

    public void ReturnedToIdle(){
        battleManager.EnemyReturnedToIdle();
        DecrementBurnCounter();
    }

    public void AddBurnDamageToQueue(int damage, int count){
        if (burnDamageQueue.Count == 0)
            cyclesLeftUntilBurn = cyclesBetweenBurns;
        for (int i =0; i<count; i++){
            burnDamageQueue.Add(damage);
            burnDamageQueue.Sort();
            burnDamageQueue.Reverse();
        }
    }

    private void DecrementBurnCounter(){
        if (burnDamageQueue.Count == 0)
            return;
        //don't apply a burn if there is a player attack animation going on
        if (battleManager.uiManager.playerAttackAnimationParent.childCount > 0)
            return;
        cyclesLeftUntilBurn --;
        if (cyclesLeftUntilBurn == 0){
            battleManager.DamageEnemyHealth(burnDamageQueue[0]);
            burnDamageQueue.RemoveAt(0);
            battleManager.uiManager.ShowBurnCount();
            cyclesLeftUntilBurn = cyclesBetweenBurns;
        }
    }

}
