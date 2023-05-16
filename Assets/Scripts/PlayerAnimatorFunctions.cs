using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimatorFunctions : MonoBehaviour{

    [HideInInspector]
    public List<GameObject> attacksInProgress = new List<GameObject>();
    [HideInInspector]
    public List<float> pebblesInQueue = new List<float>();

    public BattleManager battleManager;
    public Animator animator;

    [Header("Attack Animation Objects")]
    public GameObject deathBubble;
    public GameObject basicFirePrefab;
    public GameObject powerFirePrefab;
    public GameObject basicWaterPrefab;
    public GameObject powerWaterPrefab;
    public GameObject basicDarkPrefab;
    public GameObject powerDarkPrefab;
    public GameObject basicEarthPrefab;
    public GameObject powerEarthPrefab;
    public GameObject basicLightningPrefab;
    public GameObject powerLightningPrefab;
    public GameObject powerHealPrefab;
    public GameObject powerEarthPebblePrefab;

    void Start(){
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
    }

    public void CreateAttackAnimation(BattleManager.PowerupTypes type, int strength, int powerupLevel){
        //strength = 6;
        //powerupLevel = StaticVariables.rand.Next(1,3);
        //powerupLevel = 1;
        //type = BattleManager.PowerupTypes.Water;

        GameObject o = basicFirePrefab;
        switch (type){
            case BattleManager.PowerupTypes.Fire:
                o = powerFirePrefab;
                break;
            case BattleManager.PowerupTypes.Water:
                o = powerWaterPrefab;
                break;
            case BattleManager.PowerupTypes.Heal:
                o = powerHealPrefab;
                break;
            case BattleManager.PowerupTypes.Dark:
                o = powerDarkPrefab;
                break;
            case BattleManager.PowerupTypes.Earth:
                o = powerEarthPrefab;
                break;
            case BattleManager.PowerupTypes.Lightning:
                o = powerLightningPrefab;
                break;
            case BattleManager.PowerupTypes.Pebble:
                o = powerEarthPebblePrefab;
                break;
            default:
                o = ChooseAnimationForPowerupTypeNone();
                break;
        }

        GameObject newAttack = Instantiate(o, battleManager.uiManager.playerAttackAnimationParent);
        newAttack.GetComponent<AttackAnimatorFunctions>().SetStats(type, strength, powerupLevel, battleManager);
        if (type == BattleManager.PowerupTypes.Pebble){
            newAttack.SetActive(true);
        }
        else{
            newAttack.SetActive(false);
            attacksInProgress.Add(newAttack);
        }
    }


    public void StartNextAttackAnimation(){
        GameObject go = attacksInProgress[0];
        go.SetActive(true);
        attacksInProgress.RemoveAt(0);
    }

    public GameObject ChooseAnimationForPowerupTypeNone(){
        int range = 5;
        int i = StaticVariables.rand.Next(0, range);
        switch (i){
            case 0:
                return basicFirePrefab;
            case 1:
                return basicWaterPrefab;
            case 2:
                return basicDarkPrefab;
            case 3:
                return basicEarthPrefab;
            default:
                return basicLightningPrefab;
        }
    }

    public void ShowDeathBubble(){
        deathBubble.SetActive(true);
        battleManager.uiManager.ShowDefeatPage();
    }

    public void AddPebblesToQueue(float multiplier, int count){
        for (int i =0; i<count; i++){
            pebblesInQueue.Add(multiplier);
            pebblesInQueue.Sort();
            pebblesInQueue.Reverse();
        }
    }

}
