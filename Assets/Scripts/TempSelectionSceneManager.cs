using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TempSelectionSceneManager : MonoBehaviour{

    public GameObject waterOffCheck;
    public GameObject waterOnCheck;
    public GameObject waterPlusCheck;
    public GameObject healOffCheck;
    public GameObject healOnCheck;
    public GameObject healPlusCheck;
    public GameObject fireOffCheck;
    public GameObject fireOnCheck;
    public GameObject firePlusCheck;
    public GameObject earthOffCheck;
    public GameObject earthOnCheck;
    public GameObject earthPlusCheck;
    public GameObject lightningOffCheck;
    public GameObject lightningOnCheck;
    public GameObject lightningPlusCheck;
    public GameObject darkOffCheck;
    public GameObject darkOnCheck;
    public GameObject darkPlusCheck;
    public GameObject swordOffCheck;
    public GameObject swordOnCheck;
    public GameObject swordPlusCheck;

    public GameObject powerupInfoBox;

    void Start(){
        powerupInfoBox.SetActive(false);
        ShowPowerupSelections();
    }

    public void PushedOffButton(GameObject button){
        string name = button.transform.parent.name.ToUpper();
        switch (name){
            case ("WATER"):
                StaticVariables.waterActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Water);
                break;
            case ("HEAL"):
                StaticVariables.healActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Heal);
                break;
            case ("FIRE"):
                StaticVariables.fireActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Fire);
                break;
            case ("EARTH"):
                StaticVariables.earthActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Earth);
                break;
            case ("LIGHTNING"):
                StaticVariables.lightningActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Lightning);
                break;
            case ("DARK"):
                StaticVariables.darkActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Dark);
                break;
            case ("SWORD"):
                StaticVariables.swordActive = false;
                RemovePlusFromType(BattleManager.PowerupTypes.Sword);
                break;
        }
        ShowPowerupSelections();
    }

    public void PushedOnButton(GameObject button){
        string name = button.transform.parent.name.ToUpper();
        switch (name){
            case ("WATER"):
                StaticVariables.waterActive = true;
                break;
            case ("HEAL"):
                StaticVariables.healActive = true;
                break;
            case ("FIRE"):
                StaticVariables.fireActive = true;
                break;
            case ("EARTH"):
                StaticVariables.earthActive = true;
                break;
            case ("LIGHTNING"):
                StaticVariables.lightningActive = true;
                break;
            case ("DARK"):
                StaticVariables.darkActive = true;
                break;
            case ("SWORD"):
                StaticVariables.swordActive = true;
                break;
        }
        ShowPowerupSelections();
    }

    public void PushedPlusButton(GameObject button){
        PushedOnButton(button);
        string name = button.transform.parent.name.ToUpper();
        switch (name){
            case ("WATER"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Water;
                break;
            case ("HEAL"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Heal;
                break;
            case ("FIRE"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Fire;
                break;
            case ("EARTH"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Earth;
                break;
            case ("LIGHTNING"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Lightning;
                break;
            case ("DARK"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Dark;
                break;
            case ("SWORD"):
                StaticVariables.buffedType = BattleManager.PowerupTypes.Sword;
                break;
        }
        ShowPowerupSelections();
    }    

    private void ShowPowerupSelections(){
        
        waterOffCheck.SetActive(true);
        waterOnCheck.SetActive(false);
        waterPlusCheck.SetActive(false);
        healOffCheck.SetActive(true);
        healOnCheck.SetActive(false);
        healPlusCheck.SetActive(false);
        fireOffCheck.SetActive(true);
        fireOnCheck.SetActive(false);
        firePlusCheck.SetActive(false);
        earthOffCheck.SetActive(true);
        earthOnCheck.SetActive(false);
        earthPlusCheck.SetActive(false);
        lightningOffCheck.SetActive(true);
        lightningOnCheck.SetActive(false);
        lightningPlusCheck.SetActive(false);
        darkOffCheck.SetActive(true);
        darkOnCheck.SetActive(false);
        darkPlusCheck.SetActive(false);
        swordOffCheck.SetActive(true);
        swordOnCheck.SetActive(false);
        swordPlusCheck.SetActive(false);
        
        if (StaticVariables.waterActive){
            waterOffCheck.SetActive(false);
            waterOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Water)
            waterPlusCheck.SetActive(true);

        if (StaticVariables.healActive){
            healOffCheck.SetActive(false);
            healOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Heal)
            healPlusCheck.SetActive(true);

        if (StaticVariables.fireActive){
            fireOffCheck.SetActive(false);
            fireOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Fire)
            firePlusCheck.SetActive(true);

        if (StaticVariables.earthActive){
            earthOffCheck.SetActive(false);
            earthOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Earth)
            earthPlusCheck.SetActive(true);

        if (StaticVariables.lightningActive){
            lightningOffCheck.SetActive(false);
            lightningOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Lightning)
            lightningPlusCheck.SetActive(true);

        if (StaticVariables.darkActive){
            darkOffCheck.SetActive(false);
            darkOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Dark)
            darkPlusCheck.SetActive(true);

        if (StaticVariables.swordActive){
            swordOffCheck.SetActive(false);
            swordOnCheck.SetActive(true);
        }
        if (StaticVariables.buffedType == BattleManager.PowerupTypes.Sword)
            swordPlusCheck.SetActive(true);
    }

    private void RemovePlusFromType(BattleManager.PowerupTypes type){
        if (type == StaticVariables.buffedType)
            StaticVariables.buffedType = BattleManager.PowerupTypes.None;
    }

    public void ShowPowerupInfo(){
        powerupInfoBox.SetActive(true);
    }

    public void HidePowerupInfo(){
        powerupInfoBox.SetActive(false);
    }

}
