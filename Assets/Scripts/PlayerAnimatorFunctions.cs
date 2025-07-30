using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimatorFunctions : MonoBehaviour{

    [HideInInspector]
    public AttackData attackInProgress = null;
    private List<GameObject> powerupTypeNoneOptions = new List<GameObject>();

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
    public GameObject powerSwordPrefab;

    void Start(){
        foreach (Transform t in transform)
            t.gameObject.SetActive(false);
        SetPowerupTypeNoneOptions();
    }

    public void StartAttackProjectile() {
        //creates the attackanimatorfunctions object and immediately launches the attack
        GameObject o = basicFirePrefab;
        o = attackInProgress.type switch  {
            BattleManager.PowerupTypes.Fire => powerFirePrefab,
            BattleManager.PowerupTypes.Water => powerWaterPrefab,
            //BattleManager.PowerupTypes.Heal => powerHealPrefab,
            BattleManager.PowerupTypes.Dark => powerDarkPrefab,
            BattleManager.PowerupTypes.Earth => powerEarthPrefab,
            BattleManager.PowerupTypes.Lightning => powerLightningPrefab,
            BattleManager.PowerupTypes.Pebble => powerEarthPebblePrefab,
            BattleManager.PowerupTypes.Sword => powerSwordPrefab,
            //none and heal
            _ => ChooseAnimationForPowerupTypeNone(),
        };
        GameObject newAttack = Instantiate(o, battleManager.uiManager.playerAttackAnimationParent);
        newAttack.GetComponent<AttackAnimatorFunctions>().SetStats(attackInProgress, battleManager);
        newAttack.SetActive(true);
    }

    private GameObject ChooseAnimationForPowerupTypeNone(){    
        int i = StaticVariables.rand.Next(0, powerupTypeNoneOptions.Count);
        return powerupTypeNoneOptions[i];
    }

    private void SetPowerupTypeNoneOptions(){
        powerupTypeNoneOptions = new List<GameObject>();
        if (StaticVariables.waterActive)
            powerupTypeNoneOptions.Add(basicWaterPrefab);
        if (StaticVariables.fireActive)
            powerupTypeNoneOptions.Add(basicFirePrefab);
        if (StaticVariables.earthActive)
            powerupTypeNoneOptions.Add(basicEarthPrefab);
        if (StaticVariables.lightningActive)
            powerupTypeNoneOptions.Add(basicLightningPrefab);
        if (StaticVariables.darkActive)
            powerupTypeNoneOptions.Add(basicDarkPrefab);
    }

    public void ShowDeathBubble(){
        deathBubble.SetActive(true);
        battleManager.uiManager.ShowDefeatPage();
    }
    
    public void ApplyHealToSelf(){
        if (attackInProgress.type == BattleManager.PowerupTypes.Heal)
            battleManager.ApplyHealToSelf(attackInProgress);
    }
}
