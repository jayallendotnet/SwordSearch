using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour{

    public GameObject DeathBubble;

    public void ShowDeathBubble(){
        DeathBubble.SetActive(true);
    }


}
