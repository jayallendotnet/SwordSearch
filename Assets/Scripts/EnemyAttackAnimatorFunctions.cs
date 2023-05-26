using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class EnemyAttackAnimatorFunctions : MonoBehaviour{

    public BattleManager battleManager;
    public EnemyData data;
    public List<int> burnDamageQueue = new List<int>();
    private int cyclesBetweenBurns = 4;
    private int cyclesLeftUntilBurn = 0;

    public void DoEnemyAttackEffect(){
        battleManager.DoEnemyAttackEffect(this);
    }

    public void QueueNextAttack(){
        if (battleManager.enemyData.isHorde){
            if (data == battleManager.firstEnemyInHorde)
                battleManager.QueueEnemyAttack();
        }
        else
            battleManager.QueueEnemyAttack();
    }

    public void ReturnedToIdle(){
        if (battleManager != null){ //allows the enemy to be used in non-battle scenes
            battleManager.EnemyReturnedToIdle();
            DecrementBurnCounter();
        }
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
            int burnDamage = burnDamageQueue[0];
            if (battleManager.enemyData.isHorde)
                burnDamage *= battleManager.currentHordeEnemyCount;
            battleManager.DamageEnemyHealth(burnDamage);
            burnDamageQueue.RemoveAt(0);
            battleManager.uiManager.ShowBurnCount();
            cyclesLeftUntilBurn = cyclesBetweenBurns;
        }
    }

    public void DeathAnimationFinished(){
        battleManager.EnemyDeathAnimationFinished();
    }
    
    public void StartDeathMovement(){
        Vector3 newPos = transform.parent.position + new Vector3(20,20,0);
        transform.parent.DOMove(newPos, 1f);
    }
    
}
