using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackAnimatorFunctions : MonoBehaviour{
    private BattleManager battleManager;
    private AttackData attackData;

    public void SetStats(AttackData attackData, BattleManager battleManager) {
        this.attackData = attackData;
        this.battleManager = battleManager;
    }

    public void DestroySelf() {
        battleManager.PlayerAttackAnimationFinished(this.gameObject);
    }

    public void DoAttackEffect() {
        battleManager.AttackHitsEnemy(attackData);
    }

    public void DamagePlayerForDarkAttack() {
        battleManager.DamagePlayerForDarkAttack();
    }
}
