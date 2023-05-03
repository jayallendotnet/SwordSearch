using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnimationTriggers : MonoBehaviour{

    public WordDisplay wordDisplay;

    [Header("Animation Objects")]
    public GameObject DeathBubble;
    public GameObject fireBall;

    public void StartFireball(){
        //if (wordDisplay.powerupTypeForWord == BattleManager.PowerupTypes.Fire){

        //}
        //print("fire!");
        fireBall.SetActive(true);
        fireBall.GetComponent<Animator>().Play("Fire");
    }

    public void ShowDeathBubble(){
        DeathBubble.SetActive(true);
    }


}
