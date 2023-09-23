using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackAnimatorFunctions : MonoBehaviour{

    [HideInInspector]
    public BattleManager.PowerupTypes type;
    private int strength;
    private int powerupLevel;
    private BattleManager battleManager;

    public void SetStats(BattleManager.PowerupTypes type, int strength, int powerupLevel, BattleManager battleManager){
        this.type = type;
        this.strength = strength;
        this.powerupLevel = powerupLevel;
        this.battleManager = battleManager;
    }

    public void DestroySelf(){
        battleManager.PlayerAttackAnimationFinished(this.gameObject);
    }

    public void DoAttackEffect(){
        print("do attack effect, strength " + strength);
        print("type " + type);
        battleManager.DoAttackEffect(type, strength, powerupLevel);
    }

    public void DamagePlayerForDarkAttack(){
        battleManager.DamagePlayerForDarkAttack();
    }

    public void ThrowPebble(){
        battleManager.ThrowPebbleIfPossible(strength);
    }

}
