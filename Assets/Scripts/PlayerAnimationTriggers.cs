using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationTriggers : MonoBehaviour{

    public BattleManager battleManager;
    public Animator animator;

    [Header("Animation Objects")]
    public GameObject DeathBubble;
    public GameObject basicFire;
    public GameObject powerFire;
    public GameObject basicWater;
    public GameObject powerWater;
    public GameObject basicDark;
    public GameObject powerDark;
    public GameObject basicEarth;
    public GameObject powerEarth;
    public GameObject basicLightning;
    public GameObject powerLightning;
    public GameObject powerHeal;
    

    void Start(){
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    public void StartAttack(){
        BattleManager.PowerupTypes type = battleManager.powerupTypeForWord;
        GameObject o = basicFire;
        switch (type){
            case BattleManager.PowerupTypes.Fire:
                o = powerFire;
                break;
            case BattleManager.PowerupTypes.Water:
                o = powerWater;
                break;
            case BattleManager.PowerupTypes.Heal:
                o = powerHeal;
                //animator.SetTrigger("StartHeal");
                break;
            case BattleManager.PowerupTypes.Dark:
                o = powerDark;
                break;
            case BattleManager.PowerupTypes.Earth:
                o = powerEarth;
                break;
            case BattleManager.PowerupTypes.Lightning:
                o = powerLightning;
                break;
            default:
                o = ChooseAnimationForPowerupTypeNone();
                break;
        }
        //o = powerLightning;
        o.SetActive(true);
            
    }

    public GameObject ChooseAnimationForPowerupTypeNone(){
        int range = 5;
        int i = StaticVariables.rand.Next(0, range);
        switch (i){
            case 0:
                return basicFire;
            case 1:
                return basicWater;
            case 2:
                return basicDark;
            case 3:
                return basicEarth;
            case 4:
                return basicLightning;
        }
        return basicFire;
    }

    public void ShowDeathBubble(){
        DeathBubble.SetActive(true);
    }

    
    public void HealHitsSelf(){
        FindObjectOfType<BattleManager>().ApplyHealToSelf();

    }


}
