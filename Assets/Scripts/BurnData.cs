using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BurnData : MonoBehaviour{


    public int burnDamage = 10;
    public int burnsLeft = 10;
    public float timeBetweenBurns = 10f;
    public BattleManager battleManager;

    public void PauseBurnTimer(){
        DOTween.Pause(transform);
    }

    public void ResumeBurnTimer(){
        DOTween.Play(transform);
    }

    public void StartBurnTimer(){
        transform.DOLocalMove(transform.localPosition, timeBetweenBurns, false).OnComplete(DoBurnDamage);
    }

    private void DoBurnDamage(){
        if (!StaticVariables.IsAnimatorInIdle(battleManager.uiManager.enemyAnimator)){
            transform.DOLocalMove(transform.localPosition, 0.1f, false).OnComplete(DoBurnDamage);
            return;
        }
        print("burn happens! Damage:" + burnDamage + " Remaining: " + burnsLeft + " Time: " + timeBetweenBurns);
        battleManager.DamageEnemyHealth(burnDamage);
        burnsLeft --;
        battleManager.uiManager.ShowBurnCount();
        if (burnsLeft > 0)
            StartBurnTimer();
        else
            Destroy(this.gameObject);

    }

}
