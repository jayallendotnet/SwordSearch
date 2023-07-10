using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomAnimationStart : MonoBehaviour{

    public string stateNameOverride = "";

    void Start(){
        Animator anim = GetComponent<Animator>();
        string statename = stateNameOverride;
        if (statename == "")
            statename = anim.runtimeAnimatorController.name;
        //print(anim.runtimeAnimatorController.name);
        anim.Play(statename, 0, (float)StaticVariables.rand.NextDouble());
        Destroy(this);
    }

}
