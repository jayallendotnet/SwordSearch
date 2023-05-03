using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationTriggers : MonoBehaviour{

    public WordDisplay wordDisplay;

    [Header("Animation Objects")]
    public GameObject DeathBubble;
    public GameObject basicFire;
    public GameObject powerFire;
    public GameObject basicWater;
    public GameObject powerWater;

    void Start(){
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    public void StartAttack(){
        BattleManager.PowerupTypes type = wordDisplay.powerupTypeForWord;
        GameObject o = basicFire;
        switch (type){
            case BattleManager.PowerupTypes.Fire:
                o = powerFire;
                break;
            case BattleManager.PowerupTypes.Water:
                o = powerWater;
                break;
            default:
                o = ChooseAnimationForPowerupTypeNone();
                break;
        }
        //o = powerFire;
        o.SetActive(true);
            
    }

    public GameObject ChooseAnimationForPowerupTypeNone(){
        int range = 2;
        int i = StaticVariables.rand.Next(0, range);
        switch (i){
            case 0:
                return basicFire;
            case 1:
                return basicWater;
        }
        return basicFire;
    }

    public void ShowDeathBubble(){
        DeathBubble.SetActive(true);
    }


}
